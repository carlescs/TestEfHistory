using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestEfHistory.DataAccess.Model;
using TestEfHistory.DataAccess.Model.People;

namespace TestEfHistory.Pages.People
{
    public class PersonDetailModel(ApplicationDbContext context) : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        [Required, MaxLength(100), BindProperty]
        public string Name { get; set; } = null!;

        public void OnGet()
        {
            if (Id == null) return;
            var person = context.People.Find(Id);
            if (person == null)
                RedirectToPage("/People/Index");
            else
            {
                Name=person.Name;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
                return Page();
            if (Id == null)
            {
                var person = new Person { Name = Name };
                context.People.Add(person);
                await context.SaveChangesAsync();
                return RedirectToPage("/People/Index");
            }
            else
            {
                var person = await context.People.FindAsync(Id);
                if (person == null)
                    return RedirectToPage("/People/Index");
                else
                {
                    person.Name = Name;
                    await context.SaveChangesAsync();
                    return RedirectToPage("/People/Index");
                }
            }
        }
    }
}

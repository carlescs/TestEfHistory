using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<PersonHistory> PersonHistory { get; set; } = [];

        public async Task<IActionResult> OnGet()
        {
            if (Id == null) RedirectToPage("/People/Index");
            var person =await  context.People
                .Include(t=>t.PersonHistory)
                .FirstOrDefaultAsync(t=>t.Id==Id);
            if (person == null)
                return RedirectToPage("/People/Index");
            Name=person.Name;
            PersonHistory= person.PersonHistory.OrderByDescending(t=>t.ModifiedOn);
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if(!ModelState.IsValid)
                return Page();
            if (Id == null)
            {
                var addedEntry = context.People.Add(new Person { Name = Name });
                await context.SaveChangesAsync();
                return RedirectToPage("/People/PersonDetail", new { addedEntry.Entity.Id });
            }
            else
            {
                var person = await context.People.FindAsync(Id);
                if (person == null)
                    return RedirectToPage("/People/Index");
                person.Name = Name;
                await context.SaveChangesAsync();
            }
            return RedirectToPage();
        }
    }
}

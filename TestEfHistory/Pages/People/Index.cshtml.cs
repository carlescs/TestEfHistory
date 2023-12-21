using Microsoft.AspNetCore.Mvc.RazorPages;
using TestEfHistory.DataAccess.Model;

namespace TestEfHistory.Pages.People
{
    public class IndexModel(ApplicationDbContext context) : PageModel
    {
        public IEnumerable<PersonDto> People { get; set; } = null!;

        public void OnGet()
        {
            People = context.People.Select(t => new PersonDto
            {
                Id = t.Id,
                Name = t.Name,
                CreatedOn = t.CreatedOn
            });
        }
    }

    public class PersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}

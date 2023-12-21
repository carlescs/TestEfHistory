using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestEfHistory.DataAccess.Model.People;

public class Person
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = null!;

    [Required] public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

    public virtual ICollection<PersonHistory> PersonHistories { get; set; } = null!;
}
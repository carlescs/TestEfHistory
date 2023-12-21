using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TestEfHistory.DataAccess.Interceptors.Attributes;

namespace TestEfHistory.DataAccess.Model.People;

[History(typeof(PersonHistory))]
public class Person
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity), HistoryField(nameof(People.PersonHistory.Id))]
    public int Id { get; set; }

    [Required, MaxLength(100),HistoryField(nameof(People.PersonHistory.Name))]
    public string Name { get; set; } = null!;

    [Required] public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [HistoryField(nameof(People.PersonHistory.ModifiedOn), true)]
    public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

    [HistoryCollection]
    public virtual ICollection<PersonHistory> PersonHistory { get; set; } = null!;
} 
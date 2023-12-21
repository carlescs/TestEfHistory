using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TestEfHistory.DataAccess.Model.People;

public class PersonHistory
{
    [Key]
    public int Id { get; set; }

    [Key]
    public DateTime ModifiedOn { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey(nameof(Id))]
    public virtual Person Person { get; set; } = null!;

}
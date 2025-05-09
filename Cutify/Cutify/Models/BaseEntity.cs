using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cutify.Models
{
    public abstract class BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }  
        public bool? SoftDelete { get; set; } = false;
        public DateTime DateTime { get; set; } = DateTime.UtcNow;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserAPI.Models
{

        public class Role
        {
            [Key, ForeignKey("User")]
            public Guid UserId { get; set; }

            [Required]
            public string Name { get; set; }

            [JsonIgnore]
            public User User { get; set; } = null!;
        }
    
}

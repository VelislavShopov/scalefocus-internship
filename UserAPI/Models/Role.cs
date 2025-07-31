using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace UserAPI.Models
{

        public class Role
        {
            [Key]
            public int Id{ get; set; }

            [Required]
            public string Name { get; set; }

            public List<User> Users{ get; } = [];
        }
    
}

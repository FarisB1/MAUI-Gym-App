using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTFGym.Models
{
    public class Trener
    {
        [PrimaryKey] // Ensure consistency
        public int Id { get; set; }

        [Indexed] // Improve search performance
        public string Ime { get; set; }

        [Indexed] // Ensure unique emails
        public string? Email { get; set; }

        public string Specijalnost { get; set; }

        public string UserId { get; set; } // Ensure it's stored correctly


        public Trener()
        {
        }
    }
}

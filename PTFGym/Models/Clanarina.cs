using SQLite;
using System.ComponentModel.DataAnnotations.Schema;

namespace PTFGym.Models
{
    public class Clanarina
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed] // Optimize lookups
        public int ClanId { get; set; }

        public DateTime DatumPocetka { get; set; }
        public DateTime DatumZavrsetka { get; set; }
        public float Iznos { get; set; }

        // Navigation Property
        public Clanarina()
        {
        }
    }
}

using SQLite;

namespace PTFGym.Models
{
    public class Napredak
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed] // Optimize lookups
        public int ClanId { get; set; }

        [Indexed]
        public int TrenerId { get; set; }

        [Indexed] // Improve performance for time-based queries
        public DateTime DatumUnosa { get; set; }

        public float Tezina { get; set; }
        public string Biljeske { get; set; }

        public Napredak()
        {
        }
    }
}

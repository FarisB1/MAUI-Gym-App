using SQLite;

namespace PTFGym.Models
{
    public class Rezervacija
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int ClanId { get; set; }

        [Indexed]
        public int TrenerId { get; set; }

        [Indexed] // Improve query performance
        public DateTime DatumRezervacije { get; set; }

        public Rezervacija()
        {
        }
    }
}

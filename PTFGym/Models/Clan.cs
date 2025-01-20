using SQLite;

namespace PTFGym.Models
{
    public class Clan
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        [Indexed] 
        public string Ime { get; set; }

        public string Email { get; set; } // Removed `[Unique]` because SQLite-net-pcl doesn't support it.
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public DateTime DatumPocetkaClanstva { get; set; }
        public DateTime DatumKrajaClanstva { get; set; }


        public Clan() { }
    }
}

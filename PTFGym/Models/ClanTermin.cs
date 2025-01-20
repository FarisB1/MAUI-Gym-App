using SQLite;

namespace PTFGym.Models
{
    public class ClanTermin
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ClanId { get; set; }

        public int TerminId { get; set; }

    }
}

using SQLite;

namespace PTFGym.Models
{
    public class Termin
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime DatumVrijeme { get; set; }
        public string VrstaTreninga { get; set; } // Personalni, Grupni, itd.
        public int MaksimalniBrojClanova { get; set; }
        public int TrenerId { get; set; }

        [Ignore] // This ensures it will not be persisted in the database
        public int TrenutnoClanova { get; set; }


        [Ignore]
        public int SlobodnaMjesta => MaksimalniBrojClanova - TrenutnoClanova;

        // Many-to-Many relationship requires a junction table
        public Termin()
        {
        }
    }
}

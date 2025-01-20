using SQLite;
using PTFGym.Models;

namespace PTFGym.Services
{
    public class LocalDbService
    {
        private const string DB_NAME = "ptfgym.db3";
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
        }

        // Initialize Database and Create Tables
        public async Task InitAsync()
        {
            await _connection.EnableWriteAheadLoggingAsync(); // Improve performance

            await _connection.CreateTableAsync<Clan>();
            await _connection.CreateTableAsync<Trener>();
            await _connection.CreateTableAsync<Termin>();
            await _connection.CreateTableAsync<Clanarina>();
            await _connection.CreateTableAsync<Napredak>();
            await _connection.CreateTableAsync<Rezervacija>();
            await _connection.CreateTableAsync<ClanTermin>(); // Junction Table
        }

        // ---------------- CRUD OPERATIONS ----------------

        // INSERT or UPDATE (Upsert)
        public async Task<int> SaveItemAsync<T>(T item) where T : new()
        {
            return await _connection.InsertOrReplaceAsync(item);
        }

        public async Task<int> SaveClan<T>(T item) where T : new()
        {
            if (item is Clan clan)
            {
                if (clan.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(clan);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(clan);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Clan");
            }
        }

        public async Task<int> SaveClanarina<T>(T item) where T : new()
        {
            if (item is Clanarina clanarina)
            {
                if (clanarina.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(clanarina);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(clanarina);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Clanarina");
            }
        }

        public async Task<int> SaveNapredak<T>(T item) where T : new()
        {
            if (item is Napredak napredak)
            {
                if (napredak.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(napredak);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(napredak);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Napredak");
            }
        }
        public async Task<int> SaveRezervacija<T>(T item) where T : new()
        {
            if (item is Rezervacija rezervacija)
            {
                if (rezervacija.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(rezervacija);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(rezervacija);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Rezervacija");
            }
        }

        public async Task<int> SaveTermin<T>(T item) where T : new()
        {
            if (item is Termin termin)
            {
                if (termin.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(termin);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(termin);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Termin");
            }
        }
        
        public async Task<int> SaveClanTermin<T>(T item) where T : new()
        {
            if (item is ClanTermin clanTermin)
            {
                if (clanTermin.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(clanTermin);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(clanTermin);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Termin");
            }
        }


        public async Task<int> SaveTrener<T>(T item) where T : new()
        {
            if (item is Trener trener)
            {
                if (trener.Id == 0)
                {
                    // Insert new Clan if Id is 0 (new record)
                    return await _connection.InsertAsync(trener);
                }
                else
                {
                    // Update existing Clan if Id is not 0 (existing record)
                    return await _connection.UpdateAsync(trener);
                }
            }
            else
            {
                throw new InvalidOperationException("Item must be of type Termin");
            }
        }

        // GET ALL ITEMS (Generic)
        public async Task<List<T>> GetItemsAsync<T>() where T : new()
        {
            return await _connection.Table<T>().ToListAsync();
        }

        // GET SINGLE ITEM BY ID
        public async Task<T> GetItemByIdAsync<T>(int id) where T : new()
        {
            return await _connection.FindAsync<T>(id);
        }

        // DELETE ITEM
        public async Task<int> DeleteItemAsync<T>(T item) where T : new()
        {
            return await _connection.DeleteAsync(item);
        }

        public async Task<int> DeleteRezervacijaAsync(int id)
        {
            try
            {
                // Fetch the rezervacija by its ID
                var rezervacija = await _connection.Table<Rezervacija>().FirstOrDefaultAsync(r => r.Id == id);

                // Ensure the rezervacija was found
                if (rezervacija == null)
                {
                    throw new ArgumentException($"No rezervacija found with ID {id}", nameof(id));
                }

                // Delete the rezervacija
                return await _connection.DeleteAsync(rezervacija);
            }
            catch (Exception ex)
            {
                // Handle any errors, such as connection issues or invalid data
                Console.WriteLine($"Error during deletion: {ex.Message}");
                throw;  // Re-throw the exception if necessary
            }
        }


        public async Task<Clan?> GetClanByEmailAsync(string email)
        {
            return await _connection.Table<Clan>().FirstOrDefaultAsync(c => c.Email == email);
        }

        internal async Task<IEnumerable<object>> GetAllClansAsync()
        {
            throw new NotImplementedException();
        }
    }
}

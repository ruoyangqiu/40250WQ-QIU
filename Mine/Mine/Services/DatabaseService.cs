using Mine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Linq;

namespace Mine.Services
{
    public class DatabaseService : IDataStore<ItemModel>
    {

        // ...

        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }

        public Task<List<ItemModel>> IndexAsync(bool forceRefresh = false)
        {
            return Database.Table<ItemModel>().ToListAsync();
        }

        public Task<bool> CreateAsync(ItemModel item)
        {
            Database.InsertAsync(item);
            return Task.FromResult(true);
        }

        public Task<ItemModel> ReadAsync(string id)
        {
            return Database.Table<ItemModel>().Where(i => i.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public Task<bool> UpdateAsync(ItemModel item)
        {
            var data = ReadAsync(item.Id).GetAwaiter().GetResult();

            if(data == null)
            {
                return Task.FromResult(false);
            }

            var result = Database.UpdateAsync(item).GetAwaiter().GetResult();
            return Task.FromResult((result == 1));
        }

        public Task<bool> DeleteAsync(string id)
        {
            var data = ReadAsync(id).GetAwaiter().GetResult();

            if (data == null)
            {
                return Task.FromResult(false);
            }

            var result = Database.DeleteAsync(data).GetAwaiter().GetResult();
            return Task.FromResult((result == 1));
        }

        public void WipeDataList()
        {
            Database.DropTableAsync<ItemModel>().GetAwaiter().GetResult();
            Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false).GetAwaiter().GetResult();
        }

    }
}

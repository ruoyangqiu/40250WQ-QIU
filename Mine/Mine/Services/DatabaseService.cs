﻿using Mine.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Linq;

namespace Mine.Services
{
    public class DatabaseService
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

        public Task<List<ItemModel>> IndexAsync()
        {
            return Database.Table<ItemModel>().ToListAsync();
        }

        public Task<int> CreateAsync(ItemModel item)
        {
            return Database.InsertAsync(item);
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

        public Task<bool> DeleteAsync(ItemModel item)
        {
            var data = ReadAsync(item.Id).GetAwaiter().GetResult();

            if (data == null)
            {
                return Task.FromResult(false);
            }

            var result = Database.DeleteAsync(item).GetAwaiter().GetResult();
            return Task.FromResult((result == 1));
        }

    }
}

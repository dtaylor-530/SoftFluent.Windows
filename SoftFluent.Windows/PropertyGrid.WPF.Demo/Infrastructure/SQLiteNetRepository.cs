//using Jellyfish.DependencyInjection;
//using Microsoft.VisualBasic;
//using Newtonsoft.Json;
//using SQLite;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PropertyGrid.WPF.Demo.Infrastructure
//{

//    public class Relationship
//    {
//        public Guid Parent { get; set; }
//        public Guid Child { get; set; }
//    }




//    public class KeyValue
//    {
//        [PrimaryKey]
//        public Guid Id { get; set; }
//        public string Key { get; set; }
//        public string Value { get; set; }
//    }

 

//        public class SQLiteNetRepository
//    {
//        protected readonly SQLiteAsyncConnection connection;

//        public SQLiteNetRepository(string? dbDirectory)
//        {
//            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"));
//            connection.CreateTableAsync<KeyValue>();
//        }

//        public async Task Update<T>(string key, T value)
//        {
//            var table = connection.Table<KeyValue>();
//            var x = await table.Where(v => v.Key.Equals(key)).ToListAsync();
//            if (x.Count == 0)
//            {
//                await connection.InsertAsync(new KeyValue { Key = key, Value = JsonConvert.SerializeObject(value) });
//            }
//            else if (x.Count == 1)
//            {
//                await connection.InsertOrReplaceAsync(new KeyValue { Id = x.Single().Id, Key = key, Value = JsonConvert.SerializeObject(value) });
//            }
//            else
//            {
//                throw new Exception("ere 4323");
//            }
//        }


//        public async Task<T?> Find<T>(string key)
//        {
//            var x = await connection.Table<KeyValue>().Where(v => v.Key.Equals(key)).ToListAsync();
//            if (x.Count == 0)
//            {
//                return default;
//            }
//            else if (x.Count == 1)
//            {
//                return await Task.Run(()=> JsonConvert.DeserializeObject<T>(x.Single().Value));
//            }
//            else
//            {
//                throw new Exception("e222  re 4323");
//            }
//        }


//    }
//}
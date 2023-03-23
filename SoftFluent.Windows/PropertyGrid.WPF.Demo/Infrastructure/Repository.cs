using Jellyfish.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace PropertyGrid.WPF.Demo.Infrastructure
{
    public class Table
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public Guid? Parent { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }
    }

    public class Type
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Assembly { get; set; }
        public string? Namespace { get; set; }
        public string Name { get; set; }
    }


    public class Property
    {
        [PrimaryKey]
        public Guid Guid { get; set; }
        public DateTime Added { get; set; }
        public DateTime? Removed { get; set; }
        public string Value { get; set; }
    }




    public class Repository
    {
        protected readonly SQLiteAsyncConnection connection;
        private Task initialisationTask;

        public Repository(string? dbDirectory)
        {
            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"));
            Initialise();
        }

        async void Initialise()
        {
            initialisationTask = Task.WhenAll(
             new[]{
                 connection.CreateTableAsync<Table>(),
                 connection.CreateTableAsync<Type>()
             });

        }


        public async Task Update(Guid key, object value)
        {
            await initialisationTask;

            var tables = await connection.Table<Table>().Where(v => v.Guid.Equals(key)).ToListAsync();

            if (tables.Count == 0)
            {
                throw new Exception("22v ere 4323");
            }
            else if (tables.Count == 1)
            {
                var tableName = "T" + tables.Single().Id;

                var lastWithSameValue = await connection.ExecuteScalarAsync<int>($"Select RowId from '{tableName}' where RowId = (SELECT MAX(RowId) from '{tableName}') And Value = '{value}'");
                if (lastWithSameValue == 0)
                    await connection.RunInTransactionAsync(c =>
                    {
                        c.Execute($"Update '{tableName}' Set Removed = '{DateTime.Now}' where Removed == null");
                        //await connection.InsertAsync(new Property { Guid = Guid.NewGuid(), Added = DateTime.Now, Value = JsonConvert.SerializeObject(value) });
                        c.Execute($"INSERT INTO '{tableName}' (Guid,Added,Removed, Value) VALUES('{Guid.NewGuid()}','{DateTime.Now}',null,'{JsonConvert.SerializeObject(value)}')");
                    });
            }
            else
            {
                throw new Exception("676 ere 4323");
            }
        }


        public async Task<Guid> FindOrCreateKey(Guid guid, string name, System.Type type)
        {
            await initialisationTask;
            var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Guid = '{guid}' AND Name = '{name}'");
            if (tables.Count == 0)
            {
                var types = await connection.QueryAsync<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                //throw new Exception("2241!43 ere 4323");
                //var max = await connection.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM Table");
                await connection.RunInTransactionAsync(c =>
                {

                    int typeId;
                    if (types.Count == 0)
                    {
                        c.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
                        typeId = c.ExecuteScalar<int>("Select Max(Id) from 'Type'");
                    }
                    else if (types.Count == 1)
                    {
                        typeId = types.Single().Id;
                    }
                    else
                        throw new Exception("f 434 4");

                    var i = c.Insert(new Table { Guid = guid, Name = name, Parent = null, Type = typeId });

                    var max = c.ExecuteScalar<int>("Select Max(Id) from 'Table'");
                    var tableName = "T" + max;
                    c.Execute($"Create Table {tableName} (Guid GUID PRIMARY KEY, Added DateTime, Removed DateTime, Value Text)");
                });
                return guid;
            }
            else if (tables.Count == 1)
            {
                return tables.Single().Guid;
            }
            else
            {
                throw new Exception("e909re 4323");
            }
        }


        public async Task<Guid> FindOrCreateKeyByParent(Guid parent, string name, System.Type type)
        {
            await initialisationTask;
  

            var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parent}' AND Name = '{name}'");
            if (tables.Count == 0)
            {
                var types = await connection.QueryAsync<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
                //throw new Exception("2241!43 ere 4323");
                var guid = Guid.NewGuid();
                //var max = await connection.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM Table");
                await connection.RunInTransactionAsync(c =>
                {
                    int typeId;
                    if (types.Count == 0)
                    {
                        c.Insert(new Type { Assembly = type.Assembly.FullName, Namespace = type.Namespace, Name = type.Name });
                        typeId = c.ExecuteScalar<int>("Select Max(Id) from 'Type'");
                    }
                    else if (types.Count == 1)
                    {
                        typeId = types.Single().Id;
                    }
                    else
                        throw new Exception("f 434 4");

                    var i = c.Insert(new Table { Guid = guid, Name = name, Parent = parent, Type = typeId });

                    var max = c.ExecuteScalar<int>("Select Max(Id) from 'Table'");
                    var tableName = "T" + max;
                    c.Execute($"Create Table {tableName} (Guid GUID PRIMARY KEY, Added DateTime, Removed DateTime, Value Text)");
                });
                return guid;
            }
            else if (tables.Count == 1)
            {
                return tables.Single().Guid;
            }
            else
            {
                throw new Exception("e909re 4323");
            }

        }


        public async Task<List<object>> Find(Guid key)
        {
            await initialisationTask;
            var tables = await connection.Table<Table>().Where(v => v.Guid.Equals(key)).ToListAsync();

            if (tables.Count == 0)
            {
                throw new Exception("!43 ere 4323");
            }
            else if (tables.Count == 1)
            {
                var table = tables.Single();
                var type = await connection.Table<Type>().Where(v => v.Id.Equals(table.Type)).FirstAsync();
                string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
                var _type = System.Type.GetType(assemblyQualifiedName);
                //await connection.InsertOrReplaceAsync(new KeyValue { Id = x.Single().Id, Key = key, Value = JsonConvert.SerializeObject(value) });
                var properties = await connection.QueryAsync<Property>($"Select * from 'T{tables.Single().Id}' where Removed is null order by Added asc");
                //if (typeof(T).IsAssignableFrom(typeof(IEnumerable)) == false)
                {
                    List<object> list = new();
                    foreach (var property in properties)
                        list.Add(JsonConvert.DeserializeObject(property.Value, _type));
                    return list;
                }
                // return JsonConvert.DeserializeObject<T>(properties.Single().Value);
            }
            else
            {
                throw new Exception("ere 4323");
            }


        }

    }
}

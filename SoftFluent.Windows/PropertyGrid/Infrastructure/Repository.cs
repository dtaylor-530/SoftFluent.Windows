
using SoftFluent.Windows;
using SQLite;
using System.Globalization;
using System.Reflection;
using System.Text.Json;

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

            if (tables.Count == 1)
            {
                var tableName = "T" + tables.Single().Id;
                var count = await connection.ExecuteScalarAsync<int>($"SELECT RowId FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'");

                await connection.RunInTransactionAsync(c =>
                {
                    if (count == 0)
                    {
                        c.Execute($"Create Table {tableName} (Guid GUID PRIMARY KEY, Added DateTime, Removed DateTime, Value Text)");
                    }

                    var lastWithSameValue = c.ExecuteScalar<int>($"Select RowId from '{tableName}' where RowId = (SELECT MAX(RowId) from '{tableName}') And Value = '{value}'");
                    if (lastWithSameValue == 0)
                    {
                        c.Execute($"Update '{tableName}' Set Removed = '{DateTime.Now}' where Removed == null");
                        c.Execute($"INSERT INTO '{tableName}' (Guid,Added,Removed, Value) VALUES('{Guid.NewGuid()}','{DateTime.Now}',null,'{value}')");
                    }
                });
            }
            else if (tables.Count == 0)
            {
                throw new Exception("22v ere 4323");
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
                        throw new Exception("56f 34 4");

                    var i = c.Insert(new Table { Guid = guid, Name = name, Parent = null, Type = typeId });

                });
                return guid;
            }
            else if (tables.Count == 1)
            {
                return tables.Single().Guid;
            }
            else
            {
                throw new Exception("e99re 4323");
            }
        }


        public async Task<Guid> FindOrCreateKeyByParent(Guid parent, string name, System.Type type)
        {
            await initialisationTask;


            var tables = await connection.QueryAsync<Table>($"Select * from 'Table' where Parent = '{parent}' AND Name = '{name}'");
            if (tables.Count == 0)
            {

                //throw new Exception("2241!43 ere 4323");
                var guid = Guid.NewGuid();
                //var max = await connection.ExecuteScalarAsync<int>("SELECT MAX(Id) FROM Table");
                await connection.RunInTransactionAsync(c =>
                {
                    var tables = c.Query<Table>($"Select * from 'Table' where Parent = '{parent}' AND Name = '{name}'");
                    if (tables.Count != 0)
                        return;
                    var types = c.Query<Type>($"Select * from 'Type' where Assembly = '{type.Assembly.FullName}' AND Namespace = '{type.Namespace}' AND Name = '{type.Name}'");
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
                });
                return guid;
            }
            if (tables.Count == 1)
            {
                return tables.Single().Guid;
            }
            else
            {
                throw new Exception("3e909re 4323");
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
                var tableName = $"T{tables.Single().Id}";
                var count = await connection.ExecuteScalarAsync<int>($"SELECT RowId FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'");

                if (count == 0)
                {
                    return new();
                }

                var type = await connection.Table<Type>().Where(v => v.Id.Equals(table.Type)).FirstAsync();
                string assemblyQualifiedName = Assembly.CreateQualifiedName(type.Assembly, $"{type.Namespace}.{type.Name}");
                var _type = System.Type.GetType(assemblyQualifiedName);
                //await connection.InsertOrReplaceAsync(new KeyValue { Id = x.Single().Id, Key = key, Value = JsonConvert.SerializeObject(value) });
                var properties = await connection.QueryAsync<Property>($"Select * from '{tableName}' where Removed is null order by Added asc");
                {
                    List<object> list = new();
                    foreach (var property in properties)
                    {
                        if (ConversionHelper.TryChangeType(property.Value, _type, CultureInfo.CurrentCulture, out var value))
                            list.Add(value);
                        else
                            throw new Exception("332 b64ere 4323");

                    }
                    return list;
                }
            }
            else
            {
                throw new Exception("ere 4323");
            }
        }
    }
}

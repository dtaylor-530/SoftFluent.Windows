
using Abstractions;
using PropertyGrid.Abstractions;
using PropertyGrid.Infrastructure;
using SoftFluent.Windows;
using SQLite;
using System.ComponentModel.Design;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Utility.Trees;

namespace PropertyGrid.WPF.Demo.Infrastructure
{




    public class Repository2 : IRepository
    {
        protected readonly SQLiteAsyncConnection connection;
        private Task initialisationTask;

        Dictionary<Guid, IKey> Dictionary = new();

        public Repository2(string? dbDirectory)
        {
            connection = new SQLiteAsyncConnection(Path.Combine(dbDirectory, "data" + "." + "sqlite"));
            //Initialise();
        }

        //async void Initialise()
        //{
        //    initialisationTask = Task.WhenAll(
        //     new[]{
        //         connection.CreateTableAsync<Table>(),
        //         connection.CreateTableAsync<Type>()
        //     });

        //}

        Tree tree = new();

        public async Task UpdateValue(IKey key, object value)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }
            tree[key].Add(value);
            Dictionary[Guid.NewGuid()] = key;
        }




        public Task<IKey> FindKeyByParent(IKey key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            return Task.Run(() =>
            {
                ITree? parent = tree[_key];

                if (parent == default)
                {
                    tree.Add(_key);
                }

                IKey? childKey = ((tree[_key] ?? throw new Exception("vdf 44gfgdf"))
                .Items
                .Select(a =>
                {
                    if (a.Data is Key { Guid: var guid, Name: var name, Type: var type })
                    {
                        if (name == _key.Name && type == _key.Type)
                        {
                            return (IKey)new Key(guid, name, type);
                        }
                    }

                    return null;
                }).SingleOrDefault(a => a != null));

                if (childKey == default)
                {
                    childKey = new Key(guid, name, type);
                    (tree[_key] ?? throw new Exception("88df 44gfgdf")).Add(childKey);
                }

                return childKey;
            });
        }


        public Task<object> FindValue(IKey key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            return Task.Run(() => (object)(tree[_key] ?? throw new Exception("82228df 44gfgdf"))?.Items.LastOrDefault());


        }
    }
}

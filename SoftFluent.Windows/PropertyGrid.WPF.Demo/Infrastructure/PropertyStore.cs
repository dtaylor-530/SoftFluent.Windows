//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reactive.Threading.Tasks;
//using System.Threading.Tasks;
//using Abstractions;
//using Newtonsoft.Json.Linq;

//namespace PropertyGrid.WPF.Demo.Infrastructure
//{
//    public class PropertyStore : IPropertyStore
//    {
//        Dictionary<IKey, IObserver> dictionary = new();
//        Dictionary<IKey, object> store = new();
//        Dictionary<Guid, Dictionary<string, Guid>> guids = new();
//        private readonly DirectoryInfo directory;
//        SQLiteNetRepository sQLiteNetRepository;
//        private PropertyStore()
//        {
//            directory = Directory.CreateDirectory("../../../Data");
//            sQLiteNetRepository = new(directory.FullName);
//        }

//        public T GetValue<T>(IKey key)
//        {
//            sQLiteNetRepository.Find<T>(key.ToString()).ToObservable().Subscribe(a =>
//            {

//            });

//            return store.ContainsKey(key) ? (T?)store[key] : default;
//        }

//        public async void SetValue<T>(IKey key, T value)
//        {
//            store[key] = value;
//            await sQLiteNetRepository.Update(key.ToString(), value);
//        }

//        public void Subscribe(IObserver observer)
//        {
//            dictionary[observer] = observer;
//        }

//        public string Validate(string memberName)
//        {
//            return string.Empty;
//        }

//        public Task<Guid> GetGuidByParent(Guid guid, string? name, System.Type type)
//        {
//            if (guids.ContainsKey(guid) == false)
//            {
//                guids.Add(guid, new());
//            }
//            if (name == null)
//            {
//                name = guids[guid].Count.ToString();
//            }

//            if (guids[guid].ContainsKey(name) == false)
//            {


//                guids[guid].Add(name, Guid.NewGuid());
//            }
//            return Task.FromResult(guids[guid][name]);
//        }

//        public static PropertyStore Instance { get; } = new();
//    }
//}

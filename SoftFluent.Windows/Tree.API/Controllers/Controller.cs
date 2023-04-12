using Abstractions;
using Microsoft.AspNetCore.Mvc;
using PropertyGrid.Abstractions;
using System.Collections.Specialized;
using Utility.Trees;


namespace PropertyTree.API.Controllers
{

    public class KeyValue
    {
        public KeyValue(Guid key, object value)
        {
            Key = key;
            Value = value;
        }

        public Guid Key { get; set; }
        public object Value { get; set; }
    }


    [ApiController]
    [Route("[controller]/[action]")]
    public class Controller : ControllerBase
    {
        Kaos.Collections.RankedDictionary<Guid, Guid> Dictionary = new();


        Tree tree = new();

        [HttpPost()]
        public async Task UpdateValue(Guid key, object value)
        {
            tree[key].Add(value);
            Dictionary[Guid.NewGuid()] = key;
        }

        [HttpGet]
        public List<KeyValue> GetChanges(Guid? guid)
        {
            List<KeyValue> keyValueList = new List<KeyValue>();
            foreach (var keyValue in Dictionary.ElementsBetween(guid ?? Dictionary.MinKey, Dictionary.MaxKey))
            {
                var key = keyValue.Value;
                keyValueList.Add(new KeyValue(key, tree[key].Data));
            }
            return keyValueList;
        }

        [HttpGet]
        public Task<Guid> GetKeyByParent(Guid key, string name)
        {

            return Task.Run(() =>
            {
                ITree? parent = tree[key];

                if (parent == default)
                {
                    tree.Add(key);
                }

                var childTree = (tree[key] ?? throw new Exception("vdf 44gfgdf"))
                .Items
               .SingleOrDefault(a => a.Data.Equals(name));

                if (childTree == default)
                {
                    var guid = Guid.NewGuid();
                    tree[key].Add(new Tree(name) { Key = guid });
                    return guid;
                }
                else
                {
                    return childTree.Key;

                }
            });
        }

        [HttpGet]
        public Task<object> GetValue(Guid key)
        {
            return Task.Run(() => (object)(tree[key] ?? throw new Exception("82228df 44gfgdf"))?.Items.LastOrDefault());
        }
    }
}

using Abstractions;
using Microsoft.AspNetCore.Mvc;
using PropertyGrid.Abstractions;
using System.Collections.Specialized;
using Utility.Trees;


namespace PropertyTree.API.Controllers
{
    public class KeyValue
    {
        public KeyValue(Guid key, string value)
        {
            Key = key;
            Value = value;
        }

        public Guid Key { get; set; }
        public string Value { get; set; }
    }

    public class Repository
    {
        public Kaos.Collections.RankedDictionary<Guid, Guid> Dictionary { get; } = new();

        public Tree Tree { get; } = new();

        public static Repository Instance { get; } = new Repository();
    }



    [ApiController]
    [Route("[controller]/[action]")]
    public class Controller : ControllerBase
    {
        private Tree tree => Repository.Instance.Tree;
        private Kaos.Collections.RankedDictionary<Guid, Guid> Dictionary => Repository.Instance.Dictionary;

        [HttpPost()]
        public void PostValue(Guid key, string value)
        {
            var parent = tree[key];
            parent.Add(new Tree(value) { Key = Guid.NewGuid(), Parent = parent });
            Dictionary[Guid.NewGuid()] = key;
        }

        [HttpGet]
        public KeyValue[] GetChanges(Guid? guid)
        {

            List<KeyValue> keyValueList = new List<KeyValue>();
            foreach (var keyValue in Dictionary.ElementsBetween(guid ?? Dictionary.MinKey, Dictionary.MaxKey))
            {
                var key = keyValue.Value;
                keyValueList.Add(new KeyValue(key, tree[key].Items.Last().ToString()));
            }
            return keyValueList.ToArray();
        }

        [HttpGet]
        public Guid GetKeyByParent(Guid key, string name)
        {
            lock (tree)
            {
                ITree? parent = tree[key];

                if (parent == default)
                {
                    parent = new Tree() { Key = key, Parent = tree };
                    tree.Add(parent);
                    var guid = Guid.NewGuid();
                    var childTree = new Tree(name) { Key = guid, Parent = parent };
                    parent.Add(childTree);
                    return guid;
                }
                else
                {
                    var childTree = (tree[key] ?? throw new Exception("vdf 44gfgdf"))
                    .Items
                   .SingleOrDefault(a => a.Data.Equals(name));

                    if (childTree == default)
                    {
                        var guid = Guid.NewGuid();
                        parent.Add(new Tree(name) { Key = guid, Parent = parent });
                        return guid;
                    }
                    else
                    {
                        return childTree.Key;

                    }
                }
            }
        }

        [HttpGet]
        public string GetValue(Guid key)
        {
            var data = (tree[key] ?? throw new Exception("82228df 44gfgdf"))?.Items.LastOrDefault()?.Data;
            return (string)data;
        }
    }


}

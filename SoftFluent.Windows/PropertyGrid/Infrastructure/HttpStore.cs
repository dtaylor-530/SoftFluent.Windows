using Abstractions;
using PropertyGrid.Abstractions;
using PropertyGrid.Demo.Model;
using PropertyGrid.WPF.Demo.Infrastructure;
using System;
using System.Text.Json;
using System.Web;

namespace PropertyGrid.Infrastructure
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

    public class HttpStore : IRepository
    {
        private readonly HttpClient client;

        public HttpStore()
        {
            client = new HttpClient { BaseAddress = new Uri("https://localhost:7272/api/") };

        }

        public async Task<IKey> FindKeyByParent(IKey key)
        {

            if (key is not Key { Guid: var guid, Name:var name, Type:var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var query = HttpUtility.ParseQueryString("GetKeyByParent");
            query["key"] = guid.ToString();
            query["name"] = name;
            string queryString = query.ToString();

            HttpResponseMessage response = await client.GetAsync(query.ToString());
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<Guid>(jsonResponseBody);
            if (model == null)
                throw new Exception("sdfklj3 fsdfsd3433");

            return new Key( model, name,  type);
        }

        public Task<object> FindValue(IKey key)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var query = HttpUtility.ParseQueryString("GetValue");
            query["key"] = guid.ToString();
            query["name"] = name;
            string queryString = query.ToString();

            HttpResponseMessage response = await client.GetAsync(query.ToString());
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<Guid>(jsonResponseBody);
            if (model == null)
                throw new Exception("sdfklj3 fsdfsd3433");

            return new Key(model, name, type);

        }

        public Task UpdateValue(IKey key, object value)
        {
            if (key is not Key { Guid: var guid, Name: var name, Type: var type } _key)
            {
                throw new Exception("reg 43cs ");
            }

            var query = HttpUtility.ParseQueryString("UpdateValue");
            query["key"] = guid.ToString();
            query["value"] = name;
            string queryString = query.ToString();

            HttpResponseMessage response = await client.GetAsync(query.ToString());
            response.EnsureSuccessStatusCode();
            string jsonResponseBody = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<Guid>(jsonResponseBody);
            if (model == null)
                throw new Exception("sdfklj3 fsdfsd3433");

            return new Key(model, name, type);

        }
    }
}

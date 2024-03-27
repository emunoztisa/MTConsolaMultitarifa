using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace TestMdfEntityFramework.Utils
{
    public class Api<T>
    {
        public T RequestPOST(string URL, string method, object parameters, Type classType)
        {
            HttpClient client = new HttpClient();
            string jsonString = JsonConvert.SerializeObject(parameters);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(URL + method, content).Result;
            string responseJson = response.Content.ReadAsStringAsync().Result;
            T result = JsonConvert.DeserializeObject<T>(responseJson);
            return result;
        } // FUNCIONO CON LOGIN  

        public T RequestGet_withToken(string URL, string method, Dictionary<string, string> headers, Type classOff)
        {
            HttpClient client = new HttpClient();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.MissingMemberHandling = MissingMemberHandling.Ignore;

            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
            }

            HttpResponseMessage response = client.GetAsync(method).Result;
            response.EnsureSuccessStatusCode();
            string responseJson = response.Content.ReadAsStringAsync().Result;
            T fromJson = JsonConvert.DeserializeObject<T>(responseJson, settings);
            return fromJson;
        } // FUNCIONA

        public T RequestPost_withToken(string URL, string method, object parameters, Dictionary<string, string> headers, Type classType)
        {
            HttpClient client = new HttpClient();
            string jsonString = JsonConvert.SerializeObject(parameters);
            StringContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.Add(header.Key, header.Value.ToString());
            }
            HttpResponseMessage response = client.PostAsync(URL + method, content).Result;
            string responseJson = response.Content.ReadAsStringAsync().Result;
            T result = JsonConvert.DeserializeObject<T>(responseJson);
            return result;
        } //FUNCIONA

        public T RequestGet(string URL, string method, Type classType)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = client.GetAsync(URL + method).Result;
            string responseJson = response.Content.ReadAsStringAsync().Result;
            T result = JsonConvert.DeserializeObject<T>(responseJson);
            return result;
        }
    }
}

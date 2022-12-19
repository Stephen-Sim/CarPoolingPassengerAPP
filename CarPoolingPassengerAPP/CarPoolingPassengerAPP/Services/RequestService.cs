using CarPoolingPassengerAPP.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CarPoolingPassengerAPP.Services
{
    public class RequestService
    {
        private string url = $"{APIConstant.APIURL}request/";
        public HttpClient client { get; set; }

        public RequestService()
        {
            client = new HttpClient();
            var token = App.Current.Properties["token"] as string;
            var authHeader = new AuthenticationHeaderValue("bearer", token);
            client.DefaultRequestHeaders.Authorization = authHeader;
            client.DefaultRequestHeaders.Add("token", token);
        }

        public async Task<List<RequestRequest>> GetRequests()
        {
            try
            {
                string url = $"{this.url}GetRequests";
                var res = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<List<RequestRequest>>(res);
                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
        }

        public async Task<bool> CreateRequest(RequestRequest request)
        {
            try
            {
                string url = $"{this.url}Create";
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return false;
            }
        }
    }
}

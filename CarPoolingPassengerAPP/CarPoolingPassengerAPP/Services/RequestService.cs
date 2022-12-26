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

        public async Task<RequestRequest> GetRequest(int id)
        {
            try
            {
                string url = $"{this.url}GetRequest?requestId={id}";
                var res = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<RequestRequest>(res);
                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
        }

        public async Task<bool> CancelTrip(string id)
        {
            try
            {
                string url = $"{this.url}CancelRequest";

                var dict = new Dictionary<string, string>
                {
                    { "requestId", id }
                };

                var content = new FormUrlEncodedContent(dict);
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

        public async Task<CompletedTripInfo> GetCompleteRequestInfo(int id)
        {
            try
            {
                string url = $"{this.url}GetCompleteRequestInfo?requestId={id}";
                var res = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<CompletedTripInfo>(res);
                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
        }

        public async Task<bool> RateTrip(string id, int rating)
        {
            try
            {
                string url = $"{this.url}RateTrip";

                var dict = new Dictionary<string, string>
                {
                    { "requestId", id },
                    { "rating", rating.ToString()}
                };

                var content = new FormUrlEncodedContent(dict);
                var res = await client.PutAsync(url, content);

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

        public async Task<AcceptedTripInfo> GetAcceptedTripInfo(string id)
        {
            try
            {
                string url = $"{this.url}GetAcceptedTripInfo?requestId={id}";
                var res = await client.GetAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    var result = await res.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<AcceptedTripInfo>(result);
                    return data;
                }

                return null;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
        }
    }
}

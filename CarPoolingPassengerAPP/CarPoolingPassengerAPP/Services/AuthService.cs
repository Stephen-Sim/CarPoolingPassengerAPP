using CarPoolingPassengerAPP.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CarPoolingPassengerAPP.Services
{
    public class AuthService
    {
        private string url = "http://10.105.13.82:45455/api/auth/";
        public HttpClient client { get; set; }

        public AuthService()
        {
            client = new HttpClient();
        }

        public async Task<bool> Login(LoginRequest request)
        {
            try
            {
                string url = $"{this.url}Login";
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    Application.Current.Properties["token"] = data;
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

        public async Task<bool> EditProfile(string idToken, User request)
        {
            try
            {
                string url = $"{this.url}EditProfile";
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("idToken", idToken);

                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    Application.Current.Properties["token"] = data;
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

        public async Task<bool> Register(RegisterRequest request)
        {
            try
            {
                string url = $"{this.url}Register";
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    Application.Current.Properties["token"] = data;
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

        public async Task<bool> RefreshToken(string idToken)
        {
            try
            {
                string url = $"{this.url}RefreshToken";

                var values = new Dictionary<string, string>
                {
                    { "idToken", idToken }
                };

                var content = new FormUrlEncodedContent(values);
                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    Application.Current.Properties["token"] = data;
                    return true;
                }

                Console.WriteLine(res.StatusCode);
                return false;

            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return false;
            }
        }

        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", idToken);
        public async Task<User> GetUserByToken(string idToken)
        {
            try
            {
                string url = $"{this.url}GetUserByToken?idToken={idToken}";
                var res = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<User>(res);
                return result;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return null;
            }
        }

        public async Task<bool> ChangePassword(string idToken, string oldPassword, string newPassword)
        {
            try
            {
                string url = $"{this.url}ChangePassword";

                var values = new Dictionary<string, string>
                {
                    { "idToken", idToken },
                    { "oldPassword", oldPassword },
                    { "newPassword", newPassword}
                };

                var content = new FormUrlEncodedContent(values);
                var res = await client.PostAsync(url, content);

                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    return true;
                }

                Console.WriteLine(res.StatusCode);
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

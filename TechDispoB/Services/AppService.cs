using Newtonsoft.Json;
using System.Text;
using TechDispo.Models;
using TechDispoB.Model;
using TechDispoB.Models;


namespace TechDispoB.Services
{
    public class AppService : IAppService   
    {
        private string _baseUrl = "https://techdispoweb.azurewebsites.net";

        private HttpClientHandler CreateHttpClientHandler() // pour passer la certification
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            return handler;
        }
        public async Task<string> AuthenticateUser(LoginModel loginModel)
        {
            using (var client = new HttpClient(CreateHttpClientHandler()))
            {
                var url = $"{_baseUrl}{Apis.AuthenticateUser}";
                var serializedString = JsonConvert.SerializeObject(loginModel);

                try
                {
                    var response = await client.PostAsync(url, new StringContent(serializedString, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        return responseString; // Vous pouvez ajuster cela pour retourner un message ou une structure plus appropriée
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                return null;
            }
        }

        //public async Task<List<Mission>> GetMissions()
        //{
        //    using var client = new HttpClient(CreateHttpClientHandler());
        //    var response = await client.GetAsync($"{_baseUrl}/api/Mission/getmissions");
        //    return response.IsSuccessStatusCode
        //        ? await response.Content.ReadFromJsonAsync<List<Mission>>()
        //        : null;
        //}

        //public async Task<List<Mission>> GetMissions2()
        //{
        //    using var client = new HttpClient(CreateHttpClientHandler());
        //    var response = await client.GetAsync($"{_baseUrl}/api/Mission/getmissions");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        return JsonConvert.DeserializeObject<List<Mission>>(content);
        //    }
        //    else
        //    {
        //        // handle the error case
        //        return null;
        //    }
        //}

        public async Task<List<Mission>> GetMissions()
        {
            using (var client = new HttpClient(CreateHttpClientHandler()))
            {
                var url = $"{_baseUrl}{Apis.ListMissions}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Mission>>(content);
                }
                else
                {
                    // handle the error case
                    return null;
                }
            }
        }

        public async Task<Mission> GetMissionById(int missionId)
        {
            using (var client = new HttpClient(CreateHttpClientHandler()))
            {
                var url = $"{_baseUrl}/api/Mission/mission/{missionId}";
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Mission>(content);
                }
                else
                {
                    // handle the error case
                    return null;
                }
            }
        }
        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                using (var client = new HttpClient(CreateHttpClientHandler()))
                {
                    var url = $"{_baseUrl}/api/login"; 
                    var response = await client.GetAsync(url);
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }








    }
}

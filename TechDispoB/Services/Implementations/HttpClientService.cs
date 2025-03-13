namespace TechDispoB.Services.Implementations
{
    public static class HttpClientService
    {
        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(" https://11f7-2a01-e0a-1d4-b530-9bd-6465-dba0-6203.ngrok-free.app/api/")
            };
        }
        //public static async Task<bool> SendFCMTokenAsync(string userId, string fcmToken)
        //{
        //    try
        //    {
        //        var client = CreateHttpClient();

        //        var data = new Dictionary<string, string>
        //{
        //    { "userId", userId },
        //    { "fcmToken", fcmToken }
        //};

        //        var content = new FormUrlEncodedContent(data);
        //        var response = await client.PostAsync("Account/UpdateFCMToken", content);

        //        return response.IsSuccessStatusCode;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Erreur lors de l'envoi du token FCM: {ex.Message}");
        //        return false;
        //    }
        //}

    }
}

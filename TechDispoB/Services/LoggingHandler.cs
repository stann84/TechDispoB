
namespace TechDispoB.Services
{
    public class LoggingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 📡 Log de la requête HTTP
            Console.WriteLine("📤 HTTP Request:");
            Console.WriteLine($"Method: {request.Method}");
            Console.WriteLine($"Request URI: {request.RequestUri}");
            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                Console.WriteLine($"Request Body: {content}");
            }

            // Envoyer la requête au serveur
            var response = await base.SendAsync(request, cancellationToken);

            // 📥 Log de la réponse HTTP
            Console.WriteLine("📥 HTTP Response:");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            if (response.Content != null)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Body: {responseBody}");
            }

            return response;
        }
    }

}

using System.Text.Json.Serialization;

namespace TechDispoB.Models
{
    public class MissionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = "Nom";

        [JsonPropertyName("ville")]
        public string Ville { get; set; } = "Ville";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "Description";

        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [JsonPropertyName("clientName")]
        public string ClientName { get; set; } = "Nom du client";

        [JsonPropertyName("missionStatus")]
        public MissionStatus Status { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDispoB.Models
{
    public class MissionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ville { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public string ClientName { get; set; } // Données adaptées pour l'API
    }
}

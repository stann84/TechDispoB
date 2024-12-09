using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDispoB.Model
{
    internal class Apis
    {
        public const string Login = "/api/account/login";
        public const string ListMissions = "/api/Mission/getmissions";
        public const string GetMissionById = "api/Mission/mission/{id}";
       // public const string GetMissionById = "api/Mission/mission/{0}";

    }
}

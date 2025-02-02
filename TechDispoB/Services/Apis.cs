namespace TechDispoB.Services
{
    internal class Apis
    {
        //users
        public const string Login = "user/login";
        public const string CheckDatabaseConnection = "user/connectdatabase";
        public const string UpdateUserLocation = "user/location";
        public const string GetUserById = "user";

        //missions

        public const string ListMissions = "mission/get-missions";
        public const string GetMissionById = "mission/{id}";
        public const string GetMissionsForUser = "mission/user";


    }
}

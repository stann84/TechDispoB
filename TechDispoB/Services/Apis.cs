namespace TechDispoB.Services
{
    internal class Apis
    {
        public const string Login = "user/login";
        public const string ListMissions = "mission/get-missions";
        public const string GetMissionById = "mission/{id}";
        public const string GetMissionsForUser = "mission/user/{userId}";
        public const string CheckDatabaseConnection = "user/connectdatabase";
    }
}

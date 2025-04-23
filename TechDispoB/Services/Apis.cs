namespace TechDispoB.Services
{
    internal class Apis
    {
        //users
        public const string Login = "user/login";
        public const string CheckDatabaseConnection = "user/connectdatabase";
        public const string UpdateUserLocation = "user/location";
        public const string GetUserById = "user";
        public const string UpdateFCMToken = "user/updatefcmtoken";

        //missions

        public const string ListMissions = "mission/get-missions";
        public const string GetMissionById = "mission/{id}";
        public const string GetMissionsForUser = "mission/user";
        public const string AcceptMission = "mission/{0}/accept";
        public const string RefuseMission = "mission/{0}/refuse";

    }
}

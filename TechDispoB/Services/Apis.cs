namespace TechDispoB.Services
{
    internal class Apis
    {
        //users
        public const string Login = "user/login";
        public const string CheckDatabaseConnection = "user/connectdatabase";
        public const string UpdateUserLocation = "user/location";
        public const string GetUserById = "user/{0}";
        public const string UpdateFCMToken = "user/updatefcmtoken";

        //missions

        public const string ListMissions = "mission/get-missions";
        public const string GetMissionById = "mission/{0}";
        public const string GetMissionsForUser = "mission/user";
        public const string AcceptMission = "mission/{0}/accepter";
        public const string RefuseMission = "mission/{0}/refuser";
        public const string CommencerMission = "mission/{0}/commencer";
        public const string CloturerMission = "mission/{0}/cloturer";

    }
}

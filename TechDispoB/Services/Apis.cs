namespace TechDispoB.Services
{
    public static class Apis
    {
        // 📂 Routes pour les utilisateurs
        public static class Users
        {
            public const string Base = "users"; // ex: api/users
            public const string Login = "users/login";
            public const string CheckDatabaseConnection = "users/connectdatabase";
            public const string UpdateLocation = "users"; // Tu ajouteras /{userId}/location à la main
            public const string GetById = "users"; // Tu ajouteras /{userId} aussi
            public const string UpdateFCMToken = "users/updatefcmtoken";
            public const string ChangerMotDePasse = "users/changermotdepasse";
        }

        // 📂 Routes pour les missions
        public static class Missions
        {
            public const string Base = "mission"; 
            public const string List = "mission/list";
            public const string GetByUser = "mission/user";
            public const string Accepter = "mission/{0}/accepter";
            public const string Refuser = "mission/{0}/refuser";
            public const string Commencer = "mission/{0}/commencer";
            public const string Cloturer = "mission/{0}/cloturer";
        }
    }
}

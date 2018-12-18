namespace GPUpdate.Services
{
    public static class Configuration
    {
        public const string APP_NAME = "GPUpdate";

        public const string API_KEY = "AIzaSyDy1XysUTeLisHONirSrFS0Pt-Fz0gaXD8";

        public const string clientID = "1661716203865817";

        public const string clientSecret = null;

        public const string scope = "email"; //"https://www.googleapis.com/auth/userinfo.email"

        public const string authorizeUrl = "https://www.facebook.com/dialog/oauth";

        public const string
            redirectUrl =
                "https://www.facebook.com/connect/login_success.html"; //"https://gpupdate-11ba7.firebaseapp.com/__/auth/handler";

        public const string accessTokenUrl = null;

        public struct FirebaseAuthProperties
        {
            private string ExpiresIn;
            private string FirebaseToken;
            private string RefreshToken;
            private string UserDisplayName;
            private string UserEmail;
            private string UserFederatedId;
            private string UserFirstName;
            private string UserLastName;
            private string UserLocalId;
            private string UserPhotoUrl;
        }
    }
}
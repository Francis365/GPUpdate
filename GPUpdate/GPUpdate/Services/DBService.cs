using System.Collections.Generic;
using GPUpdate.Models;
using Realms;
using Xamarin.Forms.FirebaseWrapper.Auth;
using Xamarin.Forms.FirebaseWrapper.Database;

namespace GPUpdate.Services
{
    internal class DBService : IDBService
    {
        public static NotificationCallbackDelegate<LevelModel> LevelChangeListener;

        public static NotificationCallbackDelegate<CourseModel> CourseChangeListener;

        public static NotificationCallbackDelegate<UserModel> UserDataChangeDelegate;


        private static string uid;

        private LevelModel _levelModel;

        private FirebaseAuth firebaseAuth;

        private FirebaseClient firebaseClient;

        private readonly FirebaseDBService firebaseDBService;

        private readonly RealmDBService realmDBService;

        protected Realm realmInstance;


        public DBService()
        {
            //realmInstance = Realm.GetInstance();

            realmDBService = new RealmDBService(this);

            firebaseDBService = new FirebaseDBService(this);
        }

        public static string UID
        {
            get => uid;
            set => uid = value == null ? "Anonymous" : value;
        }

        public List<LevelModel> RetrieveLevel()
        {
            //var uid = (FirebaseDBService.isSigned) ? "a" : "b";

            return realmDBService.RetrieveLevel(UID);
        }

        //public List<CourseModel> RetrieveCoursesById(string level) {

        //    return realmInstance.All<LevelModel>().FirstOrDefault(x => x.Level == level).CourseList.ToList();
        //}

        public LevelModel RetrieveLevelById(string level)
        {
            return realmDBService.RetrieveLevelById(level);
        }

        public bool SaveCourse(string level, string courseTitle, string grade, string score, int creditUnit)
        {
            return realmDBService.SaveCourse(level, courseTitle, grade, score, creditUnit);
        }

        public void RemoveLevel(LevelModel level)
        {
            realmDBService.RemoveLevel(level);
        }

        public void RemoveCourse(string level, CourseModel removeItem)
        {
            realmDBService.RemoveCourse(level, removeItem);
        }

        public void FirebaseAuthSave(FirebaseAuthModel authLocal)
        {
            realmDBService.FirebaseAuthSave(authLocal);
        }

        public FirebaseAuthModel GetFirebaseAuthLocal()
        {
            return realmDBService.GetCurrentUserData().FirebaseAuthModel;
        }

        public void SignInWithFacebook()
        {
            firebaseDBService.SignInWithFacebookAsync();
        }

        public List<UserModel> GetAllUsers()
        {
            return realmDBService.GetAllUsers();
        }


        public async void Upload()
        {
            //// Email/Password Auth
            //var authProvider = new FirebaseAuthProvider(new FirebaseConfig("<google.firebase.com API Key>"));

            //var auth = await authProvider.CreateUserWithEmailAndPasswordAsync("email@email.com", "password");

            //// The auth Object will contain auth.User and the Authentication Token from the request
            //System.Diagnostics.Debug.WriteLine(auth.FirebaseToken);


            //// Facebook Auth
            //var authProvider2 = new FirebaseAuthProvider(new FirebaseConfig("<google.firebase.com API Key>"));
            //var facebookAccessToken = "<login with facebook and get oauth access token>";

            //var auth2 = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, facebookAccessToken);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using GPUpdate.Models;
using Xamarin.Auth;
using Xamarin.Auth.Presenters;
using Xamarin.Forms.FirebaseWrapper.Auth;
using Xamarin.Forms.FirebaseWrapper.Database;

namespace GPUpdate.Services
{
    internal class FirebaseDBService
    {
        private Account account;
        private FirebaseAuthProvider authProvider;
        private readonly IDBService dBService;
        private OAuth2Authenticator facebookAuthenticator;
        private string facebookToken;
        public FirebaseAuth firebaseAuth;
        private FirebaseClient firebaseClient;

        //OAuth2Authenticator googleAuthenticator;

        private List<LevelModel> level;
        private OAuthLoginPresenter presenter;

        public FirebaseDBService(IDBService dBService)
        {
            this.dBService = dBService;

            //Initialize Firebase
            InitFirebase();
        }

        public static bool isSigned { get; set; }

        public static string UID { get; set; }

        private void InitFirebase()
        {
            firebaseClient = new FirebaseClient("https://gpupdate-11ba7.firebaseio.com");


            facebookAuthenticator = new OAuth2Authenticator(
                Configuration.clientID,
                Configuration.scope,
                new Uri(Configuration.authorizeUrl),
                new Uri(Configuration.redirectUrl),
                null,
                true
            )
            {
                AllowCancel = false
            };


            authProvider = new FirebaseAuthProvider(new FirebaseConfig(Configuration.API_KEY));

            presenter = new OAuthLoginPresenter();
            //{
            //    AllowCancel = true
            //};

            //Login events { Completed and Error }
            facebookAuthenticator.Completed += FacebookAuth_Completed;

            facebookAuthenticator.Error += FacebookAuth_Error;

            //get authentication from device "If any"
            firebaseAuth = GetAuthLocal();

            facebookToken = firebaseAuth?.FirebaseToken;


            account = AccountStore.Create().FindAccountsForService(Configuration.APP_NAME).FirstOrDefault();


            DBService.UID = firebaseAuth?.User.LocalId;

            App.isSignedIn =
                firebaseAuth != null ? true : false; // check whether to Load Sign in page from App.cs class

            isSigned = firebaseAuth.User.FederatedId == "Anonymous"
                ? false
                : true; // check whether to Load Sign in page from App.cs class
        }

        private void FacebookAuth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            //var presenter = new OAuthLoginPresenter();
        }

        private async void FacebookAuth_Completed(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated)
            {
                facebookToken = e.Account.Properties["access_token"];

                //SaveAuth(firebaseAuth);

                //account = new Account
                //{
                //    Username = e.Account.Username
                //};

                //AccountStore.Create().Save(account, Configuration.APP_NAME);

                try
                {
                    firebaseAuth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, facebookToken);

                    firebaseAuth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Twitter, facebookToken);

                    isSigned = true;

                    SaveAuth(firebaseAuth);

                    await App.DisplayAction("Facebook Login Successful");
                }
                catch
                {
                    await App.DisplayAction("Error signing in");
                }
            }
            else
            {
                await App.DisplayAction("Authentication failed");
            }
        }

        public async void CreateUser(string username, string password)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(Configuration.API_KEY));


            firebaseAuth = await authProvider.CreateUserWithEmailAndPasswordAsync(username, password);

            //auth.

            //var auth = await authProvider.CreateUserWithEmailAndPasswordAsync("adaanyeji@gmail.com", "estherada");

            firebaseAuth.User.FirstName = firebaseAuth.User.FirstName == "" ? "Anonymous" : firebaseAuth.User.FirstName;

            account = new Account
            {
                Username = facebookToken
            };
            AccountStore.Create().Save(account, Configuration.APP_NAME);

            SaveAuth(firebaseAuth);

            //var observable = firebaseClient
            //    .Child(firebaseAuth.User.FirstName)
            //    .WithAuth(firebaseAuth.FirebaseToken)
            //    .PutAsync(level);


            //var auth = await authProvider.CreateUserWithEmailAndPasswordAsync("", "");
        }

        public void SignInWithFacebookAsync()
        {
            //presenter.Completed += async (obj, e) =>
            //{


            //};

            presenter.Login(facebookAuthenticator);

            //else    // Try Login with saved Details if user has been signed in before
            //{
            //    try
            //    {
            //        firebaseAuth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, firebaseAuth.FirebaseToken);

            //        await App.DisplayAction("\n\nLogin Successful");

            //    }
            //    catch (Exception e)
            //    {
            //        //Firebase Token expired(Refresh token) or no network
            //        await App.DisplayAction(e.Message);
            //    }
            //}
        }

        private void SaveAuth(FirebaseAuth auth)
        {
            //account.Properties.Add(FirebaseAuthProperties.ExpiresIn, auth.ExpiresIn.ToString());

            //account.Properties.Add(FirebaseAuthProperties.FirebaseToken, auth.FirebaseToken);

            //account.Properties.Add(FirebaseAuthProperties.RefreshToken, auth.RefreshToken);

            //account.Properties.Add(FirebaseAuthProperties.UserDisplayName, auth.User.DisplayName);

            //account.Properties.Add(FirebaseAuthProperties.UserEmail, auth.User.Email);

            //account.Properties.Add(FirebaseAuthProperties.UserFederatedId, auth.User.FederatedId);

            //account.Properties.Add(FirebaseAuthProperties.UserFirstName, auth.User.FirstName);

            //account.Properties.Add(FirebaseAuthProperties.UserLastName, auth.User.LastName);

            //account.Properties.Add(FirebaseAuthProperties.UserLocalId, auth.User.LocalId);

            //account.Properties.Add(FirebaseAuthProperties.UserPhotoUrl, auth.User.PhotoUrl);


            //AccountStore.Create().Save(account, Configuration.APP_NAME);

            var firebaseAuth = new FirebaseAuthModel
            {
                ExpiresIn = auth.ExpiresIn,
                FirebaseToken = auth.FirebaseToken,
                RefreshToken = auth.RefreshToken,
                UserDisplayName = auth.User.DisplayName,
                UserEmail = auth.User.Email,
                UserFederatedId = auth.User.FederatedId,
                UserFirstName = auth.User.FirstName,
                UserLastName = auth.User.LastName,
                UserLocalId = auth.User.LocalId,
                UserPhotoUrl = auth.User.PhotoUrl
            };

            dBService.FirebaseAuthSave(firebaseAuth);
        }

        private void SaveUserID(FirebaseAuth auth)
        {
            account.Properties.Add(FirebaseAuthProperties.UserLocalId, auth.User.LocalId);

            AccountStore.Create().Save(account, Configuration.APP_NAME);

            var x = GetUserID();

            //x.Properties.All< FirebaseAuthProperties>((abc)=> {
            //    abc
            //});
        }


        private string GetUserID()
        {
            return AccountStore.Create().FindAccountsForService(Configuration.APP_NAME).FirstOrDefault()
                .Properties[FirebaseAuthProperties.UserLocalId];
        }

        private FirebaseAuth GetAuthLocal()
        {
            FirebaseAuth _firebaseAuth = null;
            try
            {
                var auth = dBService.GetFirebaseAuthLocal();

                _firebaseAuth = new FirebaseAuth
                {
                    ExpiresIn = auth.ExpiresIn,
                    FirebaseToken = auth.FirebaseToken,
                    RefreshToken = auth.RefreshToken,
                    User = new User
                    {
                        DisplayName = auth.UserDisplayName,
                        Email = auth.UserEmail,
                        FederatedId = auth.UserFederatedId,
                        FirstName = auth.UserFirstName,
                        LastName = auth.UserLastName,
                        LocalId = auth.UserLocalId,
                        PhotoUrl = auth.UserPhotoUrl
                    }
                };
            }
            catch (Exception e)
            {
                App.DisplayAction(e.Message);
            }

            return _firebaseAuth;
        }

        //void SaveAuth(FirebaseAuth auth)
        //{

        //    var authLocal = new FirebaseAuthModel
        //    {
        //        ExpiresIn = auth.ExpiresIn,
        //        FirebaseToken = auth.FirebaseToken,
        //        RefreshToken = auth.RefreshToken,
        //        UserDisplayName = auth.User.DisplayName,
        //        UserEmail = auth.User.Email,
        //        UserFederatedId = auth.User.FederatedId,
        //        UserFirstName = auth.User.FirstName,
        //        UserLastName = auth.User.LastName,
        //        UserLocalId = auth.User.LocalId,
        //        UserPhotoUrl = auth.User.PhotoUrl
        //    };

        //    realmDBService.FirebaseAuthSave(authLocal);

        //}

        //FirebaseAuth GetAuthLocal()
        //{
        //    FirebaseAuth _firebaseAuth = null;
        //    try
        //    {
        //        var auth = realmDBService.GetFirebaseAuthLocal();

        //        _firebaseAuth = new FirebaseAuth
        //        {
        //            ExpiresIn = auth.ExpiresIn,
        //            FirebaseToken = auth.FirebaseToken,
        //            RefreshToken = auth.RefreshToken,
        //            User = new User
        //            {
        //                DisplayName = auth.UserDisplayName,
        //                Email = auth.UserEmail,
        //                FederatedId = auth.UserFederatedId,
        //                FirstName = auth.UserFirstName,
        //                LastName = auth.UserLastName,
        //                LocalId = auth.UserLocalId,
        //                PhotoUrl = auth.UserPhotoUrl
        //            }
        //        };
        //    }
        //    catch(Exception e)
        //    {
        //        App.DisplayAction(e.Message);
        //    }


        //    return _firebaseAuth;

        //}

        private static class FirebaseAuthProperties
        {
            public static string ExpiresIn = "ExpiresIn";
            public static string FirebaseToken = "FirebaseToken";
            public static string RefreshToken = "RefreshToken";
            public static string UserDisplayName = "UserDisplayName";
            public static string UserEmail = "UserEmail";
            public static string UserFederatedId = "UserFederatedId";
            public static string UserFirstName = "UserFirstName";
            public static string UserLastName = "UserLastName";
            public static readonly string UserLocalId = "UserLocalId";
            public static string UserPhotoUrl = "UserPhotoUrl";
        }
    }
}
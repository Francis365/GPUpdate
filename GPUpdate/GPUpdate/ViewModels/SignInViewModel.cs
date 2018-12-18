using System.ComponentModel;
using System.Runtime.CompilerServices;
using GPUpdate.Services;
using Xamarin.Forms;

namespace GPUpdate.ViewModels
{
    internal class SignInViewModel : INotifyPropertyChanged
    {
        private readonly IDBService DBService;

        private string signInState;

        private string signInwithFacebook;

        public SignInViewModel(IDBService dbService)
        {
            DBService = dbService;
        }

        public string SignInState
        {
            get => signInState;
            set => signInState = App.isSignedIn ? "Sign out" : "Sign in";
        }

        public Command SignInwithFacebook
        {
            get
            {
                return new Command(async () =>
                {
                    if (FirebaseDBService.isSigned)
                    {
                        if (await App.DisplayAction("Login As Another User"))
                            DBService.SignInWithFacebook();
                    }
                    else
                    {
                        DBService.SignInWithFacebook();
                    }
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
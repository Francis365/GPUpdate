using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GPUpdate.Models;
using GPUpdate.Services;

namespace GPUpdate.ViewModels
{
    internal class UsersViewModel : INotifyPropertyChanged
    {
        private readonly IDBService dbService;

        private UserModel displaySelectedUserData;

        private ObservableCollection<UserModel> usersList;

        public UsersViewModel(IDBService dbService)
        {
            this.dbService = dbService;
        }

        public ObservableCollection<UserModel> UsersList
        {
            get => new ObservableCollection<UserModel>(dbService.GetAllUsers());
            set
            {
                usersList = value;

                OnPropertyChanged();
            }
        }

        public UserModel DisplaySelectedUserData
        {
            get => displaySelectedUserData;
            set
            {
                displaySelectedUserData = value;

                var userDetail = displaySelectedUserData.FirebaseAuthModel;

                DBService.UID = userDetail?.UserFederatedId;
                dbService.RetrieveLevel();

                App.DisplayAction(userDetail?.UserFederatedId + " Signed in");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
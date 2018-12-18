using Realms;
using System.ComponentModel;

namespace GPUpdate.Models
{
    public class FirebaseAuthModel : RealmObject, INotifyPropertyChanged
    {
        public int ExpiresIn { get; set; }

        public string FirebaseToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserDisplayName { get; set; }

        public string UserEmail { get; set; }

        public string UserFederatedId { get; set; }

        public string UserFirstName { get; set; }

        public string UserLastName { get; set; }

        public string UserLocalId { get; set; }

        public string UserPhotoUrl { get; set; }
    }
}
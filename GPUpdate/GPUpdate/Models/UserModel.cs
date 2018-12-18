using Realms;
using System.Collections.Generic;

namespace GPUpdate.Models
{
    internal class UserModel : RealmObject
    {
        [PrimaryKey] public string UID { get; set; }

        public bool IsActive { get; set; }

        public FirebaseAuthModel FirebaseAuthModel { get; set; }

        public IList<LevelModel> LevelCollection { get; }
    }
}
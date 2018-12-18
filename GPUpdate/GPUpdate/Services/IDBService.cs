using System.Collections.Generic;
using GPUpdate.Models;

namespace GPUpdate.Services
{
    internal interface IDBService
    {
        bool SaveCourse(string level, string courseTitle, string grade, string score, int creditUnit);

        List<LevelModel> RetrieveLevel();

        LevelModel RetrieveLevelById(string level);

        void RemoveLevel(LevelModel level);

        void RemoveCourse(string level, CourseModel removeItem);

        void SignInWithFacebook();

        FirebaseAuthModel GetFirebaseAuthLocal();

        void FirebaseAuthSave(FirebaseAuthModel authLocal);
        List<UserModel> GetAllUsers();
    }
}
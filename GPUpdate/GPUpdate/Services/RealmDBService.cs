using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using GPUpdate.Models;
using Realms;
using static Realms.Realm;

namespace GPUpdate.Services
{
    internal class RealmDBService
    {
        private IDisposable coursetoken;
        private readonly IDBService dBService;
        private IDisposable leveltoken;

        protected Realm realmInstance;
        private IDisposable usertoken;

        public RealmDBService(IDBService dBService)
        {
            this.dBService = dBService;

            //Enable database schema migration, All previous data will be lost
            var realmConfig = new RealmConfiguration
            {
                ShouldDeleteIfMigrationNeeded = true
            };

            //Get realm Instance
            realmInstance = Realm.GetInstance(realmConfig);


            //Assign the active UID value
            DBService.UID = realmInstance.All<UserModel>().Where(user => user.IsActive == true).FirstOrDefault()?.UID;


            //Subscribe to realm events
            realmInstance.RealmChanged += RealmInstance_RealmChanged;

            usertoken = realmInstance.All<UserModel>().SubscribeForNotifications(UserDataChangeListener);


            //try
            //{
            //    leveltoken = GetCurrentUserData().LevelCollection.AsRealmCollection<LevelModel>().SubscribeForNotifications(LevelChangeListener);


            //    coursetoken = realmInstance.All<CourseModel>().SubscribeForNotifications(CourseChangeListener);

            //    //var query = from x in GetCurrentUserData().LevelCollection
            //    //            select new { x.Courses };

            //    //query.ToList().SubscribeForNotifications(CourseChangeListener);

            //    //GetCurrentUserData().LevelCollection.OfType<CourseModel>().GetEnumerator().Current.PropertyChanged += Current_PropertyChanged;

            //}
            //catch
            //{

            //    leveltoken = realmInstance.All<LevelModel>().SubscribeForNotifications(LevelChangeListener);

            //    coursetoken = realmInstance.All<CourseModel>().SubscribeForNotifications(CourseChangeListener);


            //}
        }

        public static RealmChangedEventHandler CourseChangedDelegate { get; set; }


        private void CourseChangeListener(IRealmCollection<object> sender, ChangeSet changes, Exception error)
        {
            var x = sender;
        }

        private void Current_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var x = sender;
        }

        private void RealmDBService_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var x = sender;
        }

        public List<LevelModel> RetrieveLevel(string uid)
        {
            //realmInstance.Write(() =>
            //{
            //    realmInstance.Add(new UserModel
            //    {
            //        UID = uid
            //    }, update: true);
            //});

            //var xy = realmInstance.All<UserModel>().ToList().Where(x => x.UID == uid).ToList();

            //return xy.Select(xyz => xyz.Level).FirstOrDefault().ToList();

            try
            {
                return GetCurrentUserData().LevelCollection.ToList();
            }
            catch (Exception)
            {
                realmInstance.Write(() =>
                {
                    realmInstance.Add(new UserModel
                    {
                        UID = uid,
                        FirebaseAuthModel = null,
                        IsActive = true
                    }, update: true);
                });
                return GetCurrentUserData().LevelCollection.ToList();
            }
        }

        public UserModel GetCurrentUserData()
        {
            var user = realmInstance.Find<UserModel>(DBService.UID);

            user = user == null
                ? new UserModel
                {
                    UID = DBService.UID,
                    FirebaseAuthModel = new FirebaseAuthModel
                    {
                        UserFederatedId = DBService.UID
                    },
                    IsActive = true
                }
                : user;

            return user;
        }

        public List<UserModel> GetAllUsers()
        {
            return realmInstance.All<UserModel>().ToList();
        }

        public LevelModel RetrieveLevelById(string level)
        {
            return dBService.RetrieveLevel().Find(x => x.Level == level);
        }

        public bool SaveCourse(string level, string courseTitle, string grade, string score, int creditUnit)
        {
            realmInstance?.Write(() =>
            {
                //realmInstance.Add(new UserModel
                //{
                //    UID = "b"
                //}, update: true);

                var user = GetCurrentUserData();

                var _level = user?.LevelCollection?.ToList().Find(x => x.Level == level);

                //Add Level when database Id is null
                if (_level == null)
                    _level = new LevelModel
                    {
                        Level = level,
                        GPA = 0.0f,
                        TotalCourses = "0",
                        TotalCreditUnit = 0
                    };
                else user.LevelCollection?.Remove(_level);


                if (courseTitle != null && courseTitle != "")
                {
                    //Add new Course
                    var _newCourse = new CourseModel
                    {
                        CourseTitle = courseTitle,
                        Grade = grade,
                        Score = score,
                        CreditUnit = creditUnit
                    };

                    //Check if course already exist for update
                    if (_level.Courses.ToList().Exists(x =>
                        x.CourseTitle.ToLower().Replace(" ", "") == courseTitle.ToLower().Replace(" ", "")))
                    {
                        //Fetch the data to be deleted and updated
                        _newCourse = _level.Courses.FirstOrDefault(x =>
                            x.CourseTitle.ToLower().Replace(" ", "") == courseTitle.ToLower().Replace(" ", ""));

                        //Delete old entry
                        var pointGrade = PointGrade(_newCourse.Grade, _newCourse.CreditUnit);
                        _level.GPA = (_level.GPA * _level.TotalCreditUnit - pointGrade) /
                                     (_level.TotalCreditUnit -
                                      _newCourse.CreditUnit); //Using formula: GPA = pointGrade/totalCreditUnit
                        _level.TotalCreditUnit -= _newCourse.CreditUnit;
                        _level.Courses.Remove(_newCourse);

                        //Update with recent entry
                        _newCourse.CourseTitle = courseTitle;
                        _newCourse.Grade = grade;
                        _newCourse.Score = score;
                        _newCourse.CreditUnit = creditUnit;
                    }

                    //Add the updated/new course data
                    _level.Courses.Add(_newCourse);

                    //Add The count of total course
                    _level.TotalCourses = _level.Courses.Count().ToString();

                    //Calculate GPA and update Level GPA
                    _level.GPA = CalculateNewGPA(_level.GPA, _level.TotalCreditUnit, _newCourse.CreditUnit,
                        _newCourse.Grade, out var totalCreditUnit);

                    //Add total credit unit
                    _level.TotalCreditUnit = totalCreditUnit;
                }

                //Add to realm database

                user?.LevelCollection?.Add(_level);

                //user.LevelCollection.AsRealmCollection().CollectionChanged


                realmInstance.Add(user, update: true);


                App.DisplayAction.Invoke("Success");
            });
            return true;
        }


        public void FirebaseAuthSave(FirebaseAuthModel authLocal)
        {
            using (var transaction = realmInstance.BeginWrite())
            {
                try
                {
                    //Set last user data and set as inactive
                    var user = GetCurrentUserData();
                    user.IsActive = false;

                    realmInstance.Add(user, update: true);


                    //set new user as active
                    DBService.UID = authLocal.UserLocalId;

                    user = realmInstance.Find<UserModel>(DBService.UID);

                    user = user == null
                        ? new UserModel
                        {
                            UID = authLocal.UserLocalId,
                            FirebaseAuthModel = authLocal,
                            IsActive = true
                        }
                        : user;


                    //leveltoken.Dispose();

                    //coursetoken.Dispose();


                    //leveltoken = user.LevelCollection.AsRealmCollection<LevelModel>().SubscribeForNotifications(LevelChangeListener);

                    //coursetoken = realmInstance.All<CourseModel>().SubscribeForNotifications(CourseChangeListener);

                    //leveltoken.Dispose();
                    //coursetoken.Dispose();

                    //leveltoken = realmInstance.All<LevelModel>().SubscribeForNotifications(LevelChangeListener);

                    //coursetoken = realmInstance.All<CourseModel>().SubscribeForNotifications(CourseChangeListener);


                    usertoken.Dispose();

                    usertoken = realmInstance.All<UserModel>().SubscribeForNotifications(UserDataChangeListener);


                    realmInstance.Add(user, update: true);
                }
                catch (Exception e)
                {
                    App.DisplayAction.Invoke("Firebase Save: " + e.Message);
                }

                transaction.Commit();
            }
        }

        public void RemoveCourse(string level, CourseModel removeItem)
        {
            using (var transaction = realmInstance.BeginWrite())
            {
                var _level = RetrieveLevelById(level);

                //Delete old entry
                var pointGrade = PointGrade(removeItem.Grade, removeItem.CreditUnit);

                _level.GPA = _level.TotalCreditUnit != removeItem.CreditUnit
                    ? (_level.GPA * _level.TotalCreditUnit - pointGrade) /
                      (_level.TotalCreditUnit - removeItem.CreditUnit)
                    : 0.0f; //Using formula: GPA = pointGrade/totalCreditUnit
                _level.TotalCreditUnit -= removeItem.CreditUnit;

                _level.Courses.Remove(removeItem);

                _level.TotalCourses = _level.Courses.Count().ToString();

                transaction.Commit();
            }
        }

        public void RemoveLevel(LevelModel level)
        {
            using (var transaction = realmInstance.BeginWrite())
            {
                realmInstance.Remove(level);

                transaction.Commit();
            }
        }

        private int PointGrade(string grade, int creditUnit)
        {
            switch (grade)
            {
                case "A":
                    return 5 * creditUnit;
                case "B":
                    return 4 * creditUnit;
                case "C":
                    return 3 * creditUnit;
                case "D":
                    return 2 * creditUnit;
                case "E":
                    return 1 * creditUnit;
                default:
                    return 0;
            }
        }

        private float CalculateNewGPA(float GPA, int totalCreditUnit, int creditUnit, string grade,
            out int outTotalCreditUnit)
        {
            var _totalPointGrade = GPA * totalCreditUnit;

            totalCreditUnit += creditUnit;
            outTotalCreditUnit = totalCreditUnit;

            // choose point grade from grade
            var pointGrade = PointGrade(grade, creditUnit);


            if (totalCreditUnit > 0) GPA = (pointGrade + _totalPointGrade) / totalCreditUnit;

            return GPA;
        }

        protected virtual void UserDataChangeListener(IRealmCollection<UserModel> sender, ChangeSet changes,
            Exception error)
        {
            DBService.UserDataChangeDelegate?.Invoke(sender, changes, error);
        }

        protected virtual void LevelChangeListener(IRealmCollection<LevelModel> sender, ChangeSet changes,
            Exception error)
        {
            DBService.LevelChangeListener?.Invoke(sender, changes, error);
        }


        private void CourseChangeListener(IRealmCollection<CourseModel> sender, ChangeSet changes, Exception error)
        {
            DBService.CourseChangeListener?.Invoke(sender, changes, error);
        }

        private void RealmInstance_RealmChanged(object sender, EventArgs e)
        {
            //App.DisplayAction((sender as UserModel).LevelCollection.FirstOrDefault()?.Level.ToString());
        }
    }
}
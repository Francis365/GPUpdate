using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using GPUpdate.Models;
using GPUpdate.Services;
using Realms;
using Xamarin.Forms;

namespace GPUpdate.ViewModels
{
    internal class CourseViewModel : INotifyPropertyChanged
    {
        private CourseModel _selectedCourseItem;

        private ObservableCollection<CourseModel> courseList;

        private string courseTitle;

        private int creditUnit;

        private readonly IDBService dbService;

        private string grade;


        private bool isSlide;


        private string level;

        private string score;

        public CourseViewModel(IDBService dbService)
        {
            DefaultHeight = App.screenSize.Height;

            this.dbService = dbService;

            DBService.UserDataChangeDelegate += UserDataChangeHandler;

            SaveCourse = new Command(() =>
            {
                this.dbService.SaveCourse(level, courseTitle, grade, score, creditUnit);

                //CourseList = new ObservableCollection<CourseModel>(dbService.RetrieveLevelById(level).Courses.OrderBy(levelCourses => levelCourses.CourseTitle));
            }, () => courseTitle != null && grade != null);
        }

        public string Level
        {
            get => level;
            set
            {
                level = value;
                OnPropertyChanged();
            }
        }

        public string CourseTitle
        {
            get => courseTitle;
            set
            {
                courseTitle = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public string Grade
        {
            get => grade;
            set
            {
                grade = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public string Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public int CreditUnit
        {
            get => creditUnit;
            set
            {
                creditUnit = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public CourseModel SelectedCourseItem
        {
            get => _selectedCourseItem;
            set
            {
                _selectedCourseItem = value;


                if (_selectedCourseItem != null)
                {
                    SlideOpenClose.Execute(null);

                    //Keep
                    if (_selectedCourseItem.CourseTitle != courseTitle) IsSlide = true;

                    //Populate the Course details Entry in Slide up Window
                    CourseTitle = _selectedCourseItem.CourseTitle;
                    Grade = _selectedCourseItem.Grade;
                    Score = _selectedCourseItem.Score;
                    CreditUnit = _selectedCourseItem.CreditUnit;

                    SelectedCourseItem = null;
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<CourseModel> CourseList
        {
            get
            {
                return new ObservableCollection<CourseModel>(dbService.RetrieveLevelById(level).Courses
                    .OrderBy(levelCourses => levelCourses.CourseTitle));
            }
            set
            {
                courseList = value;
                OnPropertyChanged();
            }
        }

        public Command SlideOpenClose
        {
            get
            {
                return new Command(() =>
                {
                    if (IsSlide)
                        IsSlide = false;
                    else
                        IsSlide = true;
                });
            }
        }

        public Command RemoveCourse
        {
            get
            {
                return new Command((object sender) =>
                {
                    var removeItem = sender as CourseModel;
                    dbService.RemoveCourse(level, removeItem);
                });
            }
        }

        public bool IsSlide
        {
            get => isSlide;
            set
            {
                isSlide = value;
                OnPropertyChanged();
            }
        }

        public Command SwipeGesture
        {
            get
            {
                return new Command(direction =>
                {
                    switch (direction)
                    {
                        case "up":
                            IsSlide = true;
                            break;
                        case "down":
                            IsSlide = false;
                            break;
                    }
                });
            }
        }

        public Command SaveCourse { get; }

        public double DefaultHeight { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        public void Init(string level)
        {
            Level = level;
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        protected virtual void UserDataChangeHandler(IRealmCollection<UserModel> sender, ChangeSet changes,
            Exception error)
        {
            //If User available, update changes from the active user
            if (sender.Count() > 0)
            {
                var courseCollection = sender?.Where(user => user.UID == DBService.UID)?.FirstOrDefault()
                    ?.LevelCollection?.Where(x => x.Level == level)?.Select(level => level.Courses).FirstOrDefault();

                if (courseCollection != null)
                    CourseList = new ObservableCollection<CourseModel>(courseCollection);
            }
        }
    }
}
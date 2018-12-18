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
    internal class LevelViewModel : INotifyPropertyChanged
    {
        private string courseTitle;

        private readonly IDBService DbService;

        private string gp_Unit;

        private string grade;


        private bool isSlide;


        private ObservableCollection<LevelModel> levelList;

        private bool saveSuccessful;

        private string score;

        private string selectedLevel;

        private string totalCourses;

        public LevelViewModel(IDBService dbService)
        {
            DefaultHeight = App.screenSize.Height;

            //UserModel.LevelPropertyChanged += OnLevelPropertyChanged;

            DbService = dbService;

            DBService.UserDataChangeDelegate += UserDataChangeHandler;

            //LevelList = new ObservableCollection<LevelModel> (DbService.RetrieveLevel ());
            SaveCourse = new Command(() => { SaveSuccessful = DbService.SaveCourse(selectedLevel, "", "", "", 0); },
                () => selectedLevel != null);

            //LevelList = new ObservableCollection<LevelModel>(DbService.RetrieveLevel().OrderBy(x => x.Level));
        }


        public double DefaultHeight { get; set; }

        public bool SaveSuccessful
        {
            get => saveSuccessful;
            set
            {
                saveSuccessful = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public string SelectedLevel
        {
            get => selectedLevel;
            set
            {
                selectedLevel = value;
                OnPropertyChanged();
                SaveCourse.ChangeCanExecute();
            }
        }

        public string TotalCourses
        {
            get => totalCourses;
            set
            {
                totalCourses = value;
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
            }
        }

        public string Grade
        {
            get => grade;
            set
            {
                grade = value;
                OnPropertyChanged();
            }
        }

        public string Score
        {
            get => score;
            set
            {
                score = value;
                OnPropertyChanged();
            }
        }

        public string GP_Unit
        {
            get => gp_Unit;
            set
            {
                gp_Unit = value;
                OnPropertyChanged();
            }
        }

        public LevelModel SelectedLevelItem { get; set; }

        public ObservableCollection<LevelModel> LevelList
        {
            get => levelList;
            set
            {
                levelList = value;
                OnPropertyChanged();
            }
        }

        public Command SaveCourse { get; }

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

        public Command RemoveLevel
        {
            get
            {
                return new Command((object sender) =>
                {
                    var removeItem = sender as LevelModel;

                    if (removeItem != null) DbService.RemoveLevel(removeItem);
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

        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void LevelViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            App.DisplayAction(e?.PropertyName);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;

            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void LevelChangeHandler(IRealmCollection<LevelModel> sender, ChangeSet changes,
            Exception error)
        {
            LevelList = new ObservableCollection<LevelModel>(sender.ToList().OrderBy(x => x.Level));
        }

        protected virtual void UserDataChangeHandler(IRealmCollection<UserModel> sender, ChangeSet changes,
            Exception error)
        {
            if (sender.Count() > 0)
                LevelList = new ObservableCollection<LevelModel>(sender?.Where(user => user.UID == DBService.UID)
                    ?.FirstOrDefault()?.LevelCollection);
        }
    }
}
using Realms;
using System.Collections.Generic;

namespace GPUpdate.Models
{
    internal class LevelModel : RealmObject
    {
        public string Level { get; set; }

        public string TotalCourses { get; set; }

        public float GPA { get; set; }

        public int TotalCreditUnit { get; set; }

        public IList<CourseModel> Courses { get; }

        //public static event PropertyChangedEventHandler mPropertyChanged;

        //public static event PropertyChangedEventHandler CoursePropertyChanged;

        //protected override void OnPropertyChanged(string propertyName)
        //{
        //    base.OnPropertyChanged(propertyName);

        //    //var propName = propertyName;
        //    if(propertyName == nameof(Courses))
        //        CoursePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //public List<CourseModel> CourseList => Courses.ToList();

        //public bool good { get; set; }
    }
}
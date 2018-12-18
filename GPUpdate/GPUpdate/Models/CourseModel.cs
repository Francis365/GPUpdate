using Realms;

namespace GPUpdate.Models
{
    internal class CourseModel : RealmObject
    {
        public string CourseTitle { get; set; }

        public string Grade { get; set; }

        public string Score { get; set; }

        public int CreditUnit { get; set; }
    }
}
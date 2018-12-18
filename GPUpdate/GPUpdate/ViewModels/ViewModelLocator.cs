using GPUpdate.Services;

namespace GPUpdate.ViewModels
{
    internal class ViewModelLocator
    {
        public ViewModelLocator()
        {
            var dbService = new DBService();

            levelViewModel = new LevelViewModel(dbService);

            courseViewModel = new CourseViewModel(dbService);

            signInViewModel = new SignInViewModel(dbService);

            usersViewModel = new UsersViewModel(dbService);
        }

        public LevelViewModel levelViewModel { get; set; }

        public CourseViewModel courseViewModel { get; set; }

        public SignInViewModel signInViewModel { get; set; }

        public UsersViewModel usersViewModel { get; set; }
    }
}
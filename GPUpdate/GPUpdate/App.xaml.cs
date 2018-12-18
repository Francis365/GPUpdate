using GPUpdate.Services;
using GPUpdate.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GPUpdate
{
	public partial class App : Application
	{
        public static Size screenSize;

        public static Func<string, Task<bool>> DisplayAction;

        public static bool isSignedIn;

        public App ()
		{
			InitializeComponent();

            MainPage = FirebaseDBService.isSigned
                ? new NavigationPage(new LevelView())
                : new NavigationPage(new LevelView());
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

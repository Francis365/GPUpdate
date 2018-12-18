using System;
using System.Threading.Tasks;
using GPUpdate.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPUpdate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LevelView : ContentPage
    {
        private string level;

        public LevelView()
        {
            InitializeComponent();

            App.DisplayAction = LevelViewDisplayAction;
        }

        private void Edit_OnClicked(object sender, EventArgs e)
        {
            var editItem = sender as MenuItem;

            if (editItem != null)
            {
                level = editItem.CommandParameter as string;
                CoursePageNavigation();
            }
        }

        private void Remove_OnClicked(object sender, EventArgs e)
        {
            //var removeItem = sender as MenuItem;

            //if (removeItem != null)
            //{
            //    level = removeItem.CommandParameter as string;
            //    //(BindingContext as LevelViewModel).
            //}
        }

        private async void CoursePageNavigation()
        {
            await Navigation.PushAsync(new CourseView(level));
        }

        private void levelList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var clickedItem = e.Item as LevelModel;

            if (clickedItem != null)
            {
                level = clickedItem.Level;
                CoursePageNavigation();
            }
        }


        private async void ProfileMenu_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfileView());
        }

        private async void SignInMenu_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignInView());
        }

        private async Task<bool> LevelViewDisplayAction(string message)
        {
            return await DisplayAlert("Alert", message, "Ok", "Cancel");
        }
    }
}
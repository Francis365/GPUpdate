using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPUpdate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UsersView : ContentPage
    {
        public UsersView()
        {
            InitializeComponent();

            App.DisplayAction = UsersViewDisplayAction;
        }

        private async Task<bool> UsersViewDisplayAction(string message)
        {
            return await DisplayAlert("Alert", message, "Ok", "Cancel");
        }
    }
}
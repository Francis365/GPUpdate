using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPUpdate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignInView : ContentPage
    {
        public SignInView()
        {
            InitializeComponent();

            App.DisplayAction = LevelViewDisplayAction;
        }

        private async Task<bool> LevelViewDisplayAction(string message)
        {
            return await DisplayAlert("Alert", message, "Ok", "Cancel");
        }
    }
}
using System.Threading.Tasks;
using GPUpdate.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPUpdate.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseView : ContentPage
    {
        private readonly string Level;

        public CourseView(string level)
        {
            Level = level;
            InitializeComponent();

            App.DisplayAction = CourseViewDisplayAction;
        }

        public CourseView()
        {
            InitializeComponent();
        }

        private async Task<bool> CourseViewDisplayAction(string message)
        {
            return await DisplayAlert("Alert", message, "Ok", "Cancel");
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext != null)
                (BindingContext as CourseViewModel).Init(Level);
        }
    }
}
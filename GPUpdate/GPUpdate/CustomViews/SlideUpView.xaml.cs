using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GPUpdate.CustomViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SlideUpView : ContentView
    {
        public static readonly BindableProperty DefaultHeightProperty = BindableProperty.Create("DefaultHeight",
            typeof(double), typeof(SlideUpView), 0.0, propertyChanged: DefaultHeightChanged);


        public static readonly BindableProperty IsSlideOpenProperty = BindableProperty.Create("IsSlideOpen",
            typeof(bool), typeof(SlideUpView), false, propertyChanged: SlideOpenClose);

        public SlideUpView()
        {
            InitializeComponent();
        }


        public double DefaultHeight
        {
            get => (double) GetValue(DefaultHeightProperty);
            set => SetValue(DefaultHeightProperty, value);
        }

        public bool IsSlideOpen
        {
            get => (bool) GetValue(IsSlideOpenProperty);
            set => SetValue(IsSlideOpenProperty, value);
        }

        private static void DefaultHeightChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as SlideUpView).IsVisible = false;
            (bindable as SlideUpView).TranslationY = (double) newValue;
        }

        private static async void SlideOpenClose(BindableObject bindable, object oldValue, object newValue)
        {
            if ((bool) newValue)
            {
                (bindable as SlideUpView).IsVisible = true;
                await (bindable as SlideUpView).TranslateTo(0, 0, 250, Easing.SinInOut);
                newValue = false;
            }
            else
            {
                await (bindable as SlideUpView).TranslateTo(0, App.screenSize.Height, 250, Easing.SinInOut);
                (bindable as SlideUpView).IsVisible = false;
                newValue = true;
            }
        }
    }
}
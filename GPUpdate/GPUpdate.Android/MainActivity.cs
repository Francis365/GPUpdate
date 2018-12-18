using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace GPUpdate.Droid
{
    [Activity(Label = "GPUpdate", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            App.screenSize =
                new Xamarin.Forms.Size((int)(Resources.DisplayMetrics.WidthPixels / Resources.DisplayMetrics.Density),
                    (int)(Resources.DisplayMetrics.HeightPixels / Resources.DisplayMetrics.Density));

            Xamarin.Auth.Presenters.XamarinAndroid.AuthenticationConfiguration.Init(this, bundle);

            Xamarin.Auth.CustomTabsConfiguration.CustomTabsClosingMessage = null;


            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}


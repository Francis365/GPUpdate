using System;
using Android.App;
using Android.Content;
using Android.OS;

namespace GPUpdate.Droid
{
    [Activity(Label = "GoogleAuthInterceptor")]
    [
        IntentFilter
        (
            new[] {Intent.ActionView},
            Categories = new[]
            {
                Intent.CategoryDefault,
                Intent.CategoryBrowsable
            },
            DataSchemes = new[]
            {
                // First part of the redirect url (Package name)
                "com.francis.gpupdate"
            },
            DataPaths = new[]
            {
                // Second part of the redirect url (Path)
                "/oauth2redirect"
            }
        )
    ]
    public class GoogleAuthInterceptor : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var uri_android = Intent.Data;

            // Convert iOS NSUrl to C#/netxf/BCL System.Uri - common API
            var uri_netfx = new Uri(uri_android.ToString());

            // Send the URI to the Authenticator for continuation

            //MainActivity.Auth?.OnPageLoading(uri_netfx);

            Finish();
        }
    }
}
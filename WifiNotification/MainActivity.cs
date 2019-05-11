using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Xamarin.Essentials;
using Android.Content;
using Android.Support.V4.App;
using System.Linq;
using System;
using Android.Content.Res;

namespace WifiNotification
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        static readonly int NOTIFICATION_ID = 45674567;
        static readonly string CHANNEL_ID = "wifinotif";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            if (!(Build.VERSION.SdkInt < BuildVersionCodes.O))
                CreateNotificationChannel();

            if (!Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
                CreateNotification();

            Connectivity.ConnectivityChanged += Connectivity_ConnectivityChanged;

            var exitButton = FindViewById<Button>(Resource.Id.exitBtn);
            exitButton.Click += ExitButtonClick;
        }

        void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var name = Resources.GetString(Resource.String.channel_name);
            var description = GetString(Resource.String.channel_description);
            var channel = new NotificationChannel(CHANNEL_ID, name, NotificationImportance.Default)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        void CreateNotification()
        {
            var intent = new Intent(this, typeof(MainActivity));

            // Build the notification:
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(this, CHANNEL_ID)
                          .SetContentTitle("Wifi is off!!") // Set the title
                          .SetSmallIcon(Resource.Drawable.notification_icon_background) // This is the icon to display
                          .SetOngoing(true)
                          .SetContentIntent(PendingIntent.GetActivity(this, NOTIFICATION_ID, intent, PendingIntentFlags.CancelCurrent));

            // Finally, publish the notification:
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());
        }

        void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            var access = e.NetworkAccess;
            var profiles = e.ConnectionProfiles;

            if (e.ConnectionProfiles.Contains(ConnectionProfile.WiFi))
            {
                var notificationManager = NotificationManagerCompat.From(this);
                notificationManager.Cancel(NOTIFICATION_ID);
            }
            else
            {
                CreateNotification();
            }
        }

        void ExitButtonClick(object sender, EventArgs eventArgs)
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}
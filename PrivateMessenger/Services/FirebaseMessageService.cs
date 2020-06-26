using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Media;
using Android.Net;
using Android.OS;
using Android.Support.V4.App;
using Firebase.Database;
using Firebase.Messaging;
using PrivateMessenger.Abstractions;
using PrivateMessenger.Activities;
using PrivateMessenger.Extensions;
using PrivateMessenger.JavaHelpers;

namespace PrivateMessenger.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT", "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseMessageService : FirebaseMessagingService
    {
        public const string TAG = "FirebaseMessageService";
        private const string CHANNEL_ID = "private_messenger_notification_channel";
        private const int NOTIFICATION_ID = 100;

        private static readonly long[] _vibrationPattern = new long[] { 0, 400, 100, 250 };
        private static readonly bool _useNotificationChannel = Build.VERSION.SdkInt >= BuildVersionCodes.O;
        private static readonly Uri _ringtone = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

        private NotificationManager _notificationManager;
        private Settings _settings;
        private Bitmap _largeIcon;

        public override void OnCreate()
        {
            base.OnCreate();
            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _settings = new Settings(this);

            if (_useNotificationChannel)
            {
                var notificationChannel = new NotificationChannel(CHANNEL_ID, "Private Messenger Notification Channel", NotificationImportance.Default)
                {
                    Description = "Private Messenger Messages",
                    LightColor = Color.Green
                };

                notificationChannel.EnableLights(true);
                notificationChannel.SetVibrationPattern(_vibrationPattern);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetSound(_ringtone, null);
                _notificationManager.CreateNotificationChannel(notificationChannel);
                _largeIcon = BitmapFactory.DecodeResource(Resources, Resource.Drawable.send);
            }
        }

        public override async void OnNewToken(string token)
        {
            base.OnNewToken(token);
            _settings.FirebaseToken = token;
            await FirebaseDatabase.Instance.GetReference("tokens").PutAsync(JavaConvert.ToJavaObject(new { Token = token }));
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var kvp in message.Data)
            {
                intent.PutExtra(kvp.Key, kvp.Value);
            }

            var pendingIntent = PendingIntent.GetActivity(this, NOTIFICATION_ID, intent, PendingIntentFlags.OneShot);
            var messageNotification = message.GetNotification();

            Notification notification;
            if (_useNotificationChannel)
            {

                notification = new Notification.Builder(this, CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.send_no_background)
                                      .SetLargeIcon(_largeIcon)
                                      .SetContentTitle(messageNotification.Title)
                                      .SetContentText(messageNotification.Body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .Build();
            }
            else
            {
                notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.send_no_background)
                                      .SetLargeIcon(_largeIcon)
                                      .SetContentTitle(messageNotification.Title)
                                      .SetContentText(messageNotification.Body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .SetLights(Color.Green, 100, 100)
                                      .SetSound(_ringtone)
                                      .SetVibrate(_vibrationPattern)
                                      .Build();
            }

            _notificationManager.Notify(NOTIFICATION_ID, notification);
        }
    }
}
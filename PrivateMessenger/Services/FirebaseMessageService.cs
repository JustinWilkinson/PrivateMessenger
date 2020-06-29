using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Firebase.Database;
using Firebase.Messaging;
using PrivateMessenger.Abstractions;
using PrivateMessenger.Activities;
using PrivateMessenger.Extensions;
using static PrivateMessenger.Abstractions.MessageNotification;

namespace PrivateMessenger.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT", "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseMessageService : FirebaseMessagingService
    {
        public const string TAG = "FirebaseMessageService";
        private const string CHANNEL_ID = "private_messenger_notification_channel";
        private const int NOTIFICATION_ID = 100;

        private static readonly bool _useNotificationChannel = Build.VERSION.SdkInt >= BuildVersionCodes.O;

        private NotificationManager _notificationManager;

        public override void OnCreate()
        {
            base.OnCreate();
            _notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (_useNotificationChannel)
            {
                var notificationChannel = new NotificationChannel(CHANNEL_ID, "Private Messenger Notification Channel", NotificationImportance.Default)
                {
                    Description = "Private Messenger Messages",
                    LightColor = LightColour
                };

                notificationChannel.EnableLights(true);
                notificationChannel.EnableVibration(true);
                notificationChannel.SetVibrationPattern(VibrationPattern);
                notificationChannel.SetSound(Ringtone, null);
                _notificationManager.CreateNotificationChannel(notificationChannel);
            }
        }

        public override async void OnNewToken(string token)
        {
            base.OnNewToken(token);
            App.Settings.FirebaseToken = token;
            await FirebaseDatabase.Instance.GetReference("tokens").PutAsync(new { Token = token });
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);

            var pendingIntent = PendingIntent.GetActivity(this, NOTIFICATION_ID, intent, PendingIntentFlags.OneShot);
            var messageNotification = new MessageNotification(message);

            Notification notification;
            if (_useNotificationChannel)
            {

                notification = new Notification.Builder(this, CHANNEL_ID)
                                      .SetSmallIcon(SmallIcon)
                                      .SetLargeIcon(LargeIcon)
                                      .SetContentTitle(messageNotification.Title)
                                      .SetContentText(messageNotification.Body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .Build();
            }
            else
            {
                notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                                      .SetSmallIcon(SmallIcon)
                                      .SetLargeIcon(LargeIcon)
                                      .SetContentTitle(messageNotification.Title)
                                      .SetContentText(messageNotification.Body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent)
                                      .SetLights(LightColour, LightOnMs, LightOffMs)
                                      .SetSound(Ringtone)
                                      .SetVibrate(VibrationPattern)
                                      .Build();
            }

            _notificationManager.Notify(NOTIFICATION_ID, notification);
        }
    }
}
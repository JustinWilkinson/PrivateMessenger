using Android.Graphics;
using Android.Media;
using Android.Net;
using Firebase.Messaging;

namespace PrivateMessenger.Abstractions
{
    public class MessageNotification
    {
        public string Title { get; set; }

        public string Body { get; set; }

        public MessageNotification(RemoteMessage remoteMessage)
        {
            Title = remoteMessage.Data.TryGetValue("title", out var title) ? title : "Private Messenger: New Message";
            Body = remoteMessage.Data.TryGetValue("body", out var body) ? body : "";
        }

        #region Static
        public static Uri Ringtone { get; set; } = RingtoneManager.GetDefaultUri(RingtoneType.Notification);

        public static int SmallIcon { get; set; } = Resource.Drawable.send_no_background;

        public static Bitmap LargeIcon { get; set; } = BitmapFactory.DecodeResource(App.AppResources, Resource.Drawable.send);

        public static long[] VibrationPattern { get; set; } = new long[] { 0, 400, 100, 250 };

        public static int LightColour { get; set; } = Color.Green;

        public static int LightOnMs { get; set; } = 100;

        public static int LightOffMs { get; set; } = 100;
        #endregion
    }
}
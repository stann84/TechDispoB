//using Android.App;
//using Android.Util;
//using Firebase.Messaging;
//using Resource = Microsoft.Maui.Controls.Resource;

//[Service(Exported = true)]
//[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
//public class MyFirebaseMessagingService : FirebaseMessagingService
//{
//    const string TAG = "FCM";

//    public override void OnMessageReceived(RemoteMessage message)
//    {
//        base.OnMessageReceived(message);

//        Log.Debug(TAG, "Message reçu !");
//        if (message.Data.Count > 0)
//        {
//            Log.Debug(TAG, "Data: " + string.Join(", ", message.Data));
//        }

//        if (message.GetNotification() != null)
//        {
//            ShowNotification(message.GetNotification().Title, message.GetNotification().Body);
//        }
//    }

//    private void ShowNotification(string title, string body)
//    {
//        var notificationBuilder = new Notification.Builder(this)
//            .SetContentTitle(title/*)*/
//            .SetContentText(body)
//            .SetSmallIcon(Resource.Drawable.notification_bg);

//        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
//        notificationManager.Notify(0, notificationBuilder.Build());
//    }
//}

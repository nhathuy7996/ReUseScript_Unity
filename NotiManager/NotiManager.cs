using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Notifications.Android;
using System.Linq;

public class NotiManager : MonoBehaviour
{
    private bool inForeground;

    public IGameNotificationsPlatform Platform { get; private set; }
    public bool Initialized { get; private set; }
    public List<PendingNotification> PendingNotifications { get; private set; }
    public IPendingNotificationsSerializer Serializer { get; set; }
    public event Action<PendingNotification> LocalNotificationDelivered;

    private const string DefaultFilename = "notifications.bin";

    public const string ChannelId = "game_channel0";
    public const string ReminderChannelId = "reminder_channel1";
    public const string NewsChannelId = "news_channel2";

    private bool updatePendingNotifications;

   

    // Start is called before the first frame update
    void Start()
    {
        var c1 = new GameNotificationChannel(ChannelId, "Default Game Channel", "Generic notifications");
        var c2 = new GameNotificationChannel(NewsChannelId, "News Channel", "News feed notifications");
        var c3 = new GameNotificationChannel(ReminderChannelId, "Reminder Channel", "Reminder notifications");

        this.Initialize(c1, c2, c3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Initialize the notifications manager.
    /// </summary>
    /// <param name="channels">An optional collection of channels to register, for Android</param>
    /// <exception cref="InvalidOperationException"><see cref="Initialize"/> has already been called.</exception>
    public void Initialize(params GameNotificationChannel[] channels)
    {
        if (Initialized)
        {
            throw new InvalidOperationException("NotificationsManager already initialized.");
        }

        Initialized = true;

#if UNITY_ANDROID
            Platform = new AndroidNotificationsPlatform();

            // Register the notification channels
            var doneDefault = false;
            foreach (GameNotificationChannel notificationChannel in channels)
            {
                if (!doneDefault)
                {
                    doneDefault = true;
                    ((AndroidNotificationsPlatform)Platform).DefaultChannelId = notificationChannel.Id;
                }

                long[] vibrationPattern = null;
                if (notificationChannel.VibrationPattern != null)
                    vibrationPattern = notificationChannel.VibrationPattern.Select(v => (long)v).ToArray();

                // Wrap channel in Android object
                var androidChannel = new AndroidNotificationChannel(notificationChannel.Id, notificationChannel.Name,
                    notificationChannel.Description,
                    (Importance)notificationChannel.Style)
                {
                    CanBypassDnd = notificationChannel.HighPriority,
                    CanShowBadge = notificationChannel.ShowsBadge,
                    EnableLights = notificationChannel.ShowLights,
                    EnableVibration = notificationChannel.Vibrates,
                    LockScreenVisibility = (LockScreenVisibility)notificationChannel.Privacy,
                    VibrationPattern = vibrationPattern
                };

                AndroidNotificationCenter.RegisterNotificationChannel(androidChannel);
            }
#elif UNITY_IOS
            Platform = new iOSNotificationsPlatform();
#endif

        if (Platform == null)
        {
            return;
        }

        PendingNotifications = new List<PendingNotification>();
        Platform.NotificationReceived += OnNotificationReceived;

        // Check serializer
        if (Serializer == null)
        {
            Serializer = new DefaultSerializer(Path.Combine(Application.persistentDataPath, DefaultFilename));
        }

        OnForegrounding();
    }


    public void SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null,
           bool reschedule = false, string channelId = null,
           string smallIcon = null, string largeIcon = null)
    {
        IGameNotification notification = Platform?.CreateNotification();

        if (notification == null)
        {
            return;
        }

        notification.Title = title;
        notification.Body = body;
        notification.Group = !string.IsNullOrEmpty(channelId) ? channelId : ChannelId;
        notification.DeliveryTime = deliveryTime;
        notification.SmallIcon = smallIcon;
        notification.LargeIcon = largeIcon;
        if (badgeNumber != null)
        {
            notification.BadgeNumber = badgeNumber;
        }

        PendingNotification notificationToDisplay = this.ScheduleNotification(notification);
        notificationToDisplay.Reschedule = reschedule;
        updatePendingNotifications = true;

        
    }

    /// <summary>
    /// Schedules a notification to be delivered.
    /// </summary>
    /// <param name="notification">The notification to deliver.</param>
    public PendingNotification ScheduleNotification(IGameNotification notification)
    {
        if (!Initialized)
        {
            throw new InvalidOperationException("Must call Initialize() first.");
        }

        if (notification == null || Platform == null)
        {
            return null;
        }

        Platform.ScheduleNotification(notification);

        // Register pending notification
        var result = new PendingNotification(notification);
        PendingNotifications.Add(result);

        return result;
    }


    /// <summary>
    /// Event fired by <see cref="Platform"/> when a notification is received.
    /// </summary>
    private void OnNotificationReceived(IGameNotification deliveredNotification)
    {
        // Ignore for background messages (this happens on Android sometimes)
        if (!inForeground)
        {
            return;
        }

        // Find in pending list
        int deliveredIndex =
            PendingNotifications.FindIndex(scheduledNotification =>
                scheduledNotification.Notification.Id == deliveredNotification.Id);
        if (deliveredIndex >= 0)
        {
            LocalNotificationDelivered?.Invoke(PendingNotifications[deliveredIndex]);

            PendingNotifications.RemoveAt(deliveredIndex);
        }
    }
    // Clear foreground notifications and reschedule stuff from a file
    private void OnForegrounding()
    {
        PendingNotifications.Clear();

        Platform.OnForeground();

        // Deserialize saved items
        IList<IGameNotification> loaded = Serializer?.Deserialize(Platform);

        if (loaded == null)
        {
            return;
        }

        foreach (IGameNotification savedNotification in loaded)
        {
            if (savedNotification.DeliveryTime > DateTime.Now)
            {
                PendingNotifications.Add(new PendingNotification(savedNotification));
            }
        }
    }

}

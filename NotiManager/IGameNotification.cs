using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_IOS
using Unity.Notifications.iOS;
#else
using Unity.Notifications.Android;
#endif
using UnityEngine.Assertions;

/// <summary>
/// Represents a notification that will be delivered for this application.
/// </summary>
public interface IGameNotification
    {
        /// <summary>
        /// Gets or sets a unique identifier for this notification.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If null, will be generated automatically once the notification is delivered, and then
        /// can be retrieved afterwards.
        /// </para>
        /// <para>On some platforms, this might be converted to a string identifier internally.</para>
        /// </remarks>
        /// <value>A unique integer identifier for this notification, or null (on some platforms) if not explicitly set.</value>
        int? Id { get; set; }

        /// <summary>
        /// Gets or sets the notification's title.
        /// </summary>
        /// <value>The title message for the notification.</value>
        string Title { get; set; }

        /// <summary>
        /// Gets or sets the body text of the notification.
        /// </summary>
        /// <value>The body message for the notification.</value>
        string Body { get; set; }

        /// <summary>
        /// Gets or sets a subtitle for the notification.
        /// </summary>
        /// <value>The subtitle message for the notification.</value>
        string Subtitle { get; set; }

        /// <summary>
        /// Gets or sets optional arbitrary data for the notification.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Gets or sets group to which this notification belongs.
        /// </summary>
        /// <value>A platform specific string identifier for the notification's group.</value>
        string Group { get; set; }

        /// <summary>
        /// Gets or sets the badge number for this notification. No badge number will be shown if null.
        /// </summary>
        /// <value>The number displayed on the app badge.</value>
        int? BadgeNumber { get; set; }

        /// <summary>
        /// Gets or sets if this notification will be dismissed automatically when the user taps it.
        /// Only available on Android.
        /// </summary>
        bool ShouldAutoCancel { get; set; }

        /// <summary>
        /// Gets or sets time to deliver the notification.
        /// </summary>
        /// <value>The time of delivery in local time.</value>
        DateTime? DeliveryTime { get; set; }

        /// <summary>
        /// Gets whether this notification has been scheduled.
        /// </summary>
        /// <value>True if the notification has been scheduled with the underlying operating system.</value>
        bool Scheduled { get; }

        /// <summary>
        /// Notification small icon.
        /// </summary>
        string SmallIcon { get; set; }

        /// <summary>
        /// Notification large icon.
        /// </summary>
        string LargeIcon { get; set; }
    }
public interface IGameNotificationsPlatform
{
    /// <summary>
    /// Fired when a notification is received.
    /// </summary>
    event Action<IGameNotification> NotificationReceived;

    /// <summary>
    /// Create a new instance of a <see cref="IGameNotification"/> for this platform.
    /// </summary>
    /// <returns>A new platform-appropriate notification object.</returns>
    IGameNotification CreateNotification();

    /// <summary>
    /// Schedules a notification to be delivered.
    /// </summary>
    /// <param name="gameNotification">The notification to deliver.</param>
    /// <exception cref="ArgumentNullException"><paramref name="gameNotification"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="gameNotification"/> isn't of the correct type.</exception>
    void ScheduleNotification(IGameNotification gameNotification);

    /// <summary>
    /// Cancels a scheduled notification.
    /// </summary>
    /// <param name="notificationId">The ID of a previously scheduled notification.</param>
    void CancelNotification(int notificationId);

    /// <summary>
    /// Dismiss a displayed notification.
    /// </summary>
    /// <param name="notificationId">The ID of a previously scheduled notification that is being displayed to the user.</param>
    void DismissNotification(int notificationId);

    /// <summary>
    /// Cancels all scheduled notifications.
    /// </summary>
    void CancelAllScheduledNotifications();

    /// <summary>
    /// Dismisses all displayed notifications.
    /// </summary>
    void DismissAllDisplayedNotifications();

    /// <summary>
    /// Use this to retrieve the last local or remote notification received by the app.
    /// </summary>
    /// <remarks>
    /// On Android the last notification is not cleared until the application is explicitly quit.
    /// </remarks>
    /// <returns>
    /// Returns the last local or remote notification used to open the app or clicked on by the user. If no
    /// notification is available it returns null.
    /// </returns>
    IGameNotification GetLastNotification();

    /// <summary>
    /// Performs any initialization or processing necessary on foregrounding the application.
    /// </summary>
    void OnForeground();

    /// <summary>
    /// Performs any processing necessary on backgrounding or closing the application.
    /// </summary>
    void OnBackground();
}

/// <summary>
/// Any type that handles notifications for a specific game platform.
/// </summary>
/// <remarks>Has a concrete notification type</remarks>
/// <typeparam name="TNotificationType">The type of notification returned by this platform.</typeparam>
public interface IGameNotificationsPlatform<TNotificationType> : IGameNotificationsPlatform
    where TNotificationType : IGameNotification
{
    /// <summary>
    /// Create an instance of <typeparamref name="TNotificationType"/>.
    /// </summary>
    /// <returns>A new platform-appropriate notification object.</returns>
    new TNotificationType CreateNotification();

    /// <summary>
    /// Schedule a notification to be delivered.
    /// </summary>
    /// <param name="notification">The notification to deliver.</param>
    /// <exception cref="ArgumentNullException"><paramref name="notification"/> is null.</exception>
    void ScheduleNotification(TNotificationType notification);

    /// <summary>
    /// Use this to retrieve the last local or remote notification of <typeparamref name="TNotificationType"/>
    /// received by the app.
    /// </summary>
    /// <remarks>
    /// On Android the last notification is not cleared until the application is explicitly quit.
    /// </remarks>
    /// <returns>
    /// Returns new platform-appropriate notification object for the last local or remote notification used to open
    /// the app or clicked on by the user. If no notification is available it returns null.
    /// </returns>
    new TNotificationType GetLastNotification();
}

public struct GameNotificationChannel
{
    /// <summary>
    /// The style of notification shown for this channel. Corresponds to the Importance setting of
    /// an Android notification, and do nothing on iOS.
    /// </summary>
    public enum NotificationStyle
    {
        /// <summary>
        /// Notification does not appear in the status bar.
        /// </summary>
        None = 0,
        /// <summary>
        /// Notification makes no sound.
        /// </summary>
        NoSound = 2,
        /// <summary>
        /// Notification plays sound.
        /// </summary>
        Default = 3,
        /// <summary>
        /// Notification also displays a heads-up popup.
        /// </summary>
        Popup = 4
    }

    /// <summary>
    /// Controls how notifications display on the device lock screen.
    /// </summary>
    public enum PrivacyMode
    {
        /// <summary>
        /// Notifications aren't shown on secure lock screens.
        /// </summary>
        Secret = -1,
        /// <summary>
        /// Notifications display an icon, but content is concealed on secure lock screens.
        /// </summary>
        Private = 0,
        /// <summary>
        /// Notifications display on all lock screens.
        /// </summary>
        Public
    }

    /// <summary>
    /// The identifier for the channel.
    /// </summary>
    public readonly string Id;

    /// <summary>
    /// The name of the channel as displayed to the user.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// The description of the channel as displayed to the user.
    /// </summary>
    public readonly string Description;

    /// <summary>
    /// A flag determining whether messages on this channel can show a badge. Defaults to true.
    /// </summary>
    public readonly bool ShowsBadge;

    /// <summary>
    /// A flag determining whether messages on this channel cause the device light to flash. Defaults to false.
    /// </summary>
    public readonly bool ShowLights;

    /// <summary>
    /// A flag determining whether messages on this channel cause the device to vibrate. Defaults to true.
    /// </summary>
    public readonly bool Vibrates;

    /// <summary>
    /// A flag determining whether messages on this channel bypass do not disturb settings. Defaults to false.
    /// </summary>
    public readonly bool HighPriority;

    /// <summary>
    /// The display style for this notification. Defaults to <see cref="NotificationStyle.Popup"/>.
    /// </summary>
    public readonly NotificationStyle Style;

    /// <summary>
    /// The privacy setting for this notification. Defaults to <see cref="PrivacyMode.Public"/>.
    /// </summary>
    public readonly PrivacyMode Privacy;

    /// <summary>
    /// The custom vibration pattern for this channel. Set to null to use the default.
    /// </summary>
    public readonly int[] VibrationPattern;

    /// <summary>
    /// Initialize a new instance of <see cref="GameNotificationChannel"/> with
    /// optional fields set to their default values.
    /// </summary>
    public GameNotificationChannel(string id, string name, string description) : this()
    {
        Id = id;
        Name = name;
        Description = description;

        ShowsBadge = true;
        ShowLights = false;
        Vibrates = true;
        HighPriority = false;
        Style = NotificationStyle.Popup;
        Privacy = PrivacyMode.Public;
        VibrationPattern = null;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="GameNotificationChannel"/>, providing the notification style
    /// and optionally all other settings.
    /// </summary>
    public GameNotificationChannel(string id, string name, string description, NotificationStyle style, bool showsBadge = true, bool showLights = false, bool vibrates = true, bool highPriority = false, PrivacyMode privacy = PrivacyMode.Public, long[] vibrationPattern = null)
    {
        Id = id;
        Name = name;
        Description = description;
        ShowsBadge = showsBadge;
        ShowLights = showLights;
        Vibrates = vibrates;
        HighPriority = highPriority;
        Style = style;
        Privacy = privacy;
        if (vibrationPattern != null)
            VibrationPattern = vibrationPattern.Select(v => (int)v).ToArray();
        else
            VibrationPattern = null;
    }
}

public interface IPendingNotificationsSerializer
{
    /// <summary>
    /// Save a list of pending notifications.
    /// </summary>
    /// <param name="notifications">The collection notifications to save.</param>
    void Serialize(IList<PendingNotification> notifications);

    /// <summary>
    /// Retrieve a saved list of pending notifications.
    /// </summary>
    /// <returns>The deserialized collection of pending notifications, or null if the file did not exist.</returns>
    IList<IGameNotification> Deserialize(IGameNotificationsPlatform platform);
}

public class PendingNotification
{
    /// <summary>
    /// Whether to reschedule this event if it hasn't displayed once the app is foregrounded again.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Only valid if the <see cref="GameNotificationsManager"/>'s <see cref="GameNotificationsManager.Mode"/>
    /// flag is set to <see cref="GameNotificationsManager.OperatingMode.RescheduleAfterClearing"/>.
    /// </para>
    /// <para>
    /// Will not function for any notifications that are using a delivery scheduling method that isn't time
    /// based, such as iOS location notifications.
    /// </para>
    /// </remarks>
    public bool Reschedule;

    /// <summary>
    /// The scheduled notification.
    /// </summary>
    public readonly IGameNotification Notification;

    /// <summary>
    /// Instantiate a new instance of <see cref="PendingNotification"/> from a <see cref="IGameNotification"/>.
    /// </summary>
    /// <param name="notification">The notification to create from.</param>
    public PendingNotification(IGameNotification notification)
    {
        Notification = notification ?? throw new ArgumentNullException(nameof(notification));
    }
}

public class DefaultSerializer : IPendingNotificationsSerializer
{
    private const byte Version = 1;

    private readonly string filename;

    /// <summary>
    /// Instantiate a new instance of <see cref="DefaultSerializer"/>.
    /// </summary>
    /// <param name="filename">The filename to save to. This should be an absolute path.</param>
    public DefaultSerializer(string filename)
    {
        this.filename = filename;
    }

    /// <inheritdoc />
    public void Serialize(IList<PendingNotification> notifications)
    {
        using (var file = new FileStream(filename, FileMode.Create))
        {
            using (var writer = new BinaryWriter(file))
            {
                // Write version number
                writer.Write(Version);

                // Write list length
                writer.Write(notifications.Count);

                // Write each item
                foreach (PendingNotification notificationToSave in notifications)
                {
                    IGameNotification notification = notificationToSave.Notification;

                    // ID
                    writer.Write(notification.Id.HasValue);
                    if (notification.Id.HasValue)
                    {
                        writer.Write(notification.Id.Value);
                    }

                    // Title
                    writer.Write(notification.Title ?? "");

                    // Body
                    writer.Write(notification.Body ?? "");

                    // Subtitle
                    writer.Write(notification.Subtitle ?? "");

                    // Group
                    writer.Write(notification.Group ?? "");

                    // Data
                    writer.Write(notification.Data ?? "");

                    // Badge
                    writer.Write(notification.BadgeNumber.HasValue);
                    if (notification.BadgeNumber.HasValue)
                    {
                        writer.Write(notification.BadgeNumber.Value);
                    }

                    // Time (must have a value)
                    writer.Write(notification.DeliveryTime.Value.Ticks);
                }
            }
        }
    }

    /// <inheritdoc />
    public IList<IGameNotification> Deserialize(IGameNotificationsPlatform platform)
    {
        if (!File.Exists(filename))
        {
            return null;
        }

        using (var file = new FileStream(filename, FileMode.Open))
        {
            using (var reader = new BinaryReader(file))
            {
                // Version
                var version = reader.ReadByte();

                // Length
                int numElements = reader.ReadInt32();

                var result = new List<IGameNotification>(numElements);
                for (var i = 0; i < numElements; ++i)
                {
                    IGameNotification notification = platform.CreateNotification();
                    bool hasValue;

                    // ID
                    hasValue = reader.ReadBoolean();
                    if (hasValue)
                    {
                        notification.Id = reader.ReadInt32();
                    }

                    // Title
                    notification.Title = reader.ReadString();

                    // Body
                    notification.Body = reader.ReadString();

                    // Body
                    notification.Subtitle = reader.ReadString();

                    // Group
                    notification.Group = reader.ReadString();

                    // Data, introduced in version 1
                    if (version > 0)
                        notification.Data = reader.ReadString();

                    // Badge
                    hasValue = reader.ReadBoolean();
                    if (hasValue)
                    {
                        notification.BadgeNumber = reader.ReadInt32();
                    }

                    // Time
                    notification.DeliveryTime = new DateTime(reader.ReadInt64(), DateTimeKind.Local);

                    result.Add(notification);
                }

                return result;
            }
        }
    }
}


#if UNITY_IOS
public class iOSGameNotification : IGameNotification
{
    private readonly iOSNotification internalNotification;

    /// <summary>
    /// Gets the internal notification object used by the mobile notifications system.
    /// </summary>
    public iOSNotification InternalNotification => internalNotification;

    /// <inheritdoc />
    /// <remarks>
    /// Internally stored as a string. Gets parsed to an integer when retrieving.
    /// </remarks>
    /// <value>The identifier as an integer, or null if the identifier couldn't be parsed as a number.</value>
    public int? Id
    {
        get
        {
            if (!int.TryParse(internalNotification.Identifier, out int value))
            {
                Debug.LogWarning("Internal iOS notification's identifier isn't a number.");
                return null;
            }

            return value;
        }
        set
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            internalNotification.Identifier = value.Value.ToString();
        }
    }

    /// <inheritdoc />
    public string Title { get => internalNotification.Title; set => internalNotification.Title = value; }

    /// <inheritdoc />
    public string Body { get => internalNotification.Body; set => internalNotification.Body = value; }

    /// <inheritdoc />
    public string Subtitle { get => internalNotification.Subtitle; set => internalNotification.Subtitle = value; }

    /// <inheritdoc />
    public string Data { get => internalNotification.Data; set => internalNotification.Data = value; }

    /// <inheritdoc />
    /// <remarks>
    /// On iOS, this represents the notification's Category Identifier.
    /// </remarks>
    /// <value>The value of <see cref="CategoryIdentifier"/>.</value>
    public string Group { get => CategoryIdentifier; set => CategoryIdentifier = value; }

    /// <inheritdoc />
    public int? BadgeNumber
    {
        get => internalNotification.Badge != -1 ? internalNotification.Badge : (int?)null;
        set => internalNotification.Badge = value ?? -1;
    }

    /// <inheritdoc />
    public bool ShouldAutoCancel { get; set; }

    /// <inheritdoc />
    public bool Scheduled { get; private set; }

    /// <inheritdoc />
    /// <remarks>
    /// <para>On iOS, setting this causes the notification to be delivered on a calendar time.</para>
    /// <para>If it has previously been manually set to a different type of trigger, or has not been set before,
    /// this returns null.</para>
    /// <para>The millisecond component of the provided DateTime is ignored.</para>
    /// </remarks>
    /// <value>A <see cref="DateTime"/> representing the delivery time of this message, or null if
    /// not set or the trigger isn't a <see cref="iOSNotificationCalendarTrigger"/>.</value>
    public DateTime? DeliveryTime
    {
        get
        {
            if (!(internalNotification.Trigger is iOSNotificationCalendarTrigger calendarTrigger))
            {
                return null;
            }

            DateTime now = DateTime.Now;
            var result = new DateTime
                (
                calendarTrigger.Year ?? now.Year,
                calendarTrigger.Month ?? now.Month,
                calendarTrigger.Day ?? now.Day,
                calendarTrigger.Hour ?? now.Hour,
                calendarTrigger.Minute ?? now.Minute,
                calendarTrigger.Second ?? now.Second,
                DateTimeKind.Local
                );

            return result;
        }
        set
        {
            if (!value.HasValue)
            {
                return;
            }

            DateTime date = value.Value.ToLocalTime();

            internalNotification.Trigger = new iOSNotificationCalendarTrigger
            {
                Year = date.Year,
                Month = date.Month,
                Day = date.Day,
                Hour = date.Hour,
                Minute = date.Minute,
                Second = date.Second
            };
        }
    }

    /// <summary>
    /// The category identifier for this notification.
    /// </summary>
    public string CategoryIdentifier
    {
        get => internalNotification.CategoryIdentifier;
        set => internalNotification.CategoryIdentifier = value;
    }

    /// <summary>
    /// Does nothing on iOS.
    /// </summary>
    public string SmallIcon { get => null; set { } }

    /// <summary>
    /// Does nothing on iOS.
    /// </summary>
    public string LargeIcon { get => null; set { } }

    /// <summary>
    /// Instantiate a new instance of <see cref="iOSGameNotification"/>.
    /// </summary>
    public iOSGameNotification()
    {
        internalNotification = new iOSNotification
        {
            ShowInForeground = true // Deliver in foreground by default
        };
    }

    /// <summary>
    /// Instantiate a new instance of <see cref="iOSGameNotification"/> from a delivered notification.
    /// </summary>
    /// <param name="internalNotification">The delivered notification.</param>
    internal iOSGameNotification(iOSNotification internalNotification)
    {
        this.internalNotification = internalNotification;
    }

    /// <summary>
    /// Mark this notifications scheduled flag.
    /// </summary>
    internal void OnScheduled()
    {
        Assert.IsFalse(Scheduled);
        Scheduled = true;
    }
}

/// <summary>
/// iOS implementation of <see cref="IGameNotificationsPlatform"/>.
/// </summary>
public class iOSNotificationsPlatform : IGameNotificationsPlatform<iOSGameNotification>,
    IDisposable
{
    /// <inheritdoc />
    public event Action<IGameNotification> NotificationReceived;

    /// <summary>
    /// Instantiate a new instance of <see cref="iOSNotificationsPlatform"/>.
    /// </summary>
    public iOSNotificationsPlatform()
    {
        iOSNotificationCenter.OnNotificationReceived += OnLocalNotificationReceived;
    }

    /// <inheritdoc />
    public void ScheduleNotification(IGameNotification gameNotification)
    {
        if (gameNotification == null)
        {
            throw new ArgumentNullException(nameof(gameNotification));
        }

        if (!(gameNotification is iOSGameNotification notification))
        {
            throw new InvalidOperationException(
                "Notification provided to ScheduleNotification isn't an iOSGameNotification.");
        }

        ScheduleNotification(notification);
    }

    /// <inheritdoc />
    public void ScheduleNotification(iOSGameNotification notification)
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        iOSNotificationCenter.ScheduleNotification(notification.InternalNotification);
        notification.OnScheduled();
    }

    /// <inheritdoc />
    /// <summary>
    /// Create a new <see cref="T:NotificationSamples.Android.AndroidNotification" />.
    /// </summary>
    IGameNotification IGameNotificationsPlatform.CreateNotification()
    {
        return CreateNotification();
    }

    /// <inheritdoc />
    /// <summary>
    /// Create a new <see cref="T:NotificationSamples.Android.AndroidNotification" />.
    /// </summary>
    public iOSGameNotification CreateNotification()
    {
        return new iOSGameNotification();
    }

    /// <inheritdoc />
    public void CancelNotification(int notificationId)
    {
        iOSNotificationCenter.RemoveScheduledNotification(notificationId.ToString());
    }

    /// <inheritdoc />
    public void DismissNotification(int notificationId)
    {
        iOSNotificationCenter.RemoveDeliveredNotification(notificationId.ToString());
    }

    /// <inheritdoc />
    public void CancelAllScheduledNotifications()
    {
        iOSNotificationCenter.RemoveAllScheduledNotifications();
    }

    /// <inheritdoc />
    public void DismissAllDisplayedNotifications()
    {
        iOSNotificationCenter.RemoveAllDeliveredNotifications();
    }

    /// <inheritdoc />
    IGameNotification IGameNotificationsPlatform.GetLastNotification()
    {
        return GetLastNotification();
    }

    /// <inheritdoc />
    public iOSGameNotification GetLastNotification()
    {
        var notification = iOSNotificationCenter.GetLastRespondedNotification();

        if (notification != null)
        {
            return new iOSGameNotification(notification);
        }

        return null;
    }

    /// <summary>
    /// Clears badge count.
    /// </summary>
    public void OnForeground()
    {
        iOSNotificationCenter.ApplicationBadge = 0;
    }

    /// <summary>
    /// Does nothing on iOS.
    /// </summary>
    public void OnBackground() { }

    /// <summary>
    /// Unregister delegates.
    /// </summary>
    public void Dispose()
    {
        iOSNotificationCenter.OnNotificationReceived -= OnLocalNotificationReceived;
    }

    // Event handler for receiving local notifications.
    private void OnLocalNotificationReceived(iOSNotification notification)
    {
        // Create a new AndroidGameNotification out of the delivered notification, but only
        // if the event is registered
        NotificationReceived?.Invoke(new iOSGameNotification(notification));
    }
}
#else
public class AndroidNotificationsPlatform : IGameNotificationsPlatform<AndroidGameNotification>,
       IDisposable
{
    /// <inheritdoc />
    public event Action<IGameNotification> NotificationReceived;

    /// <summary>
    /// Gets or sets the default channel ID for notifications.
    /// </summary>
    /// <value>The default channel ID for new notifications, or null.</value>
    public string DefaultChannelId { get; set; }

    /// <summary>
    /// Instantiate a new instance of <see cref="AndroidNotificationsPlatform"/>.
    /// </summary>
    public AndroidNotificationsPlatform()
    {
        AndroidNotificationCenter.OnNotificationReceived += OnLocalNotificationReceived;
    }

    /// <inheritdoc />
    /// <remarks>
    /// Will set the <see cref="AndroidGameNotification.Id"/> field of <paramref name="gameNotification"/>.
    /// </remarks>
    public void ScheduleNotification(AndroidGameNotification gameNotification)
    {
        if (gameNotification == null)
        {
            throw new ArgumentNullException(nameof(gameNotification));
        }

        if (gameNotification.Id.HasValue)
        {
            AndroidNotificationCenter.SendNotificationWithExplicitID(gameNotification.InternalNotification,
                gameNotification.DeliveredChannel,
                gameNotification.Id.Value);
        }
        else
        {
            int notificationId = AndroidNotificationCenter.SendNotification(gameNotification.InternalNotification,
                gameNotification.DeliveredChannel);
            gameNotification.Id = notificationId;
        }

        gameNotification.OnScheduled();
    }

    /// <inheritdoc />
    /// <remarks>
    /// Will set the <see cref="AndroidGameNotification.Id"/> field of <paramref name="gameNotification"/>.
    /// </remarks>
    public void ScheduleNotification(IGameNotification gameNotification)
    {
        if (gameNotification == null)
        {
            throw new ArgumentNullException(nameof(gameNotification));
        }

        if (!(gameNotification is AndroidGameNotification androidNotification))
        {
            throw new InvalidOperationException(
                "Notification provided to ScheduleNotification isn't an AndroidGameNotification.");
        }

        ScheduleNotification(androidNotification);
    }

    /// <inheritdoc />
    /// <summary>
    /// Create a new <see cref="AndroidGameNotification" />.
    /// </summary>
    public AndroidGameNotification CreateNotification()
    {
        var notification = new AndroidGameNotification()
        {
            DeliveredChannel = DefaultChannelId
        };

        return notification;
    }

    /// <inheritdoc />
    /// <summary>
    /// Create a new <see cref="AndroidGameNotification" />.
    /// </summary>
    IGameNotification IGameNotificationsPlatform.CreateNotification()
    {
        return CreateNotification();
    }

    /// <inheritdoc />
    public void CancelNotification(int notificationId)
    {
        AndroidNotificationCenter.CancelScheduledNotification(notificationId);
    }

    /// <inheritdoc />
    /// <summary>
    /// Not currently implemented on Android
    /// </summary>
    public void DismissNotification(int notificationId)
    {
        AndroidNotificationCenter.CancelDisplayedNotification(notificationId);
    }

    /// <inheritdoc />
    public void CancelAllScheduledNotifications()
    {
        AndroidNotificationCenter.CancelAllScheduledNotifications();
    }

    /// <inheritdoc />
    public void DismissAllDisplayedNotifications()
    {
        AndroidNotificationCenter.CancelAllDisplayedNotifications();
    }

    /// <inheritdoc />
    IGameNotification IGameNotificationsPlatform.GetLastNotification()
    {
        return GetLastNotification();
    }

    /// <inheritdoc />
    public AndroidGameNotification GetLastNotification()
    {
        var data = AndroidNotificationCenter.GetLastNotificationIntent();

        if (data != null)
        {
            return new AndroidGameNotification(data.Notification, data.Id, data.Channel);
        }

        return null;
    }

    /// <summary>
    /// Does nothing on Android.
    /// </summary>
    public void OnForeground() { }

    /// <summary>
    /// Does nothing on Android.
    /// </summary>
    public void OnBackground() { }

    /// <summary>
    /// Unregister delegates.
    /// </summary>
    public void Dispose()
    {
        AndroidNotificationCenter.OnNotificationReceived -= OnLocalNotificationReceived;
    }

    // Event handler for receiving local notifications.
    private void OnLocalNotificationReceived(AndroidNotificationIntentData data)
    {
        // Create a new AndroidGameNotification out of the delivered notification, but only
        // if the event is registered
        NotificationReceived?.Invoke(new AndroidGameNotification(data.Notification, data.Id, data.Channel));
    }
}

public class AndroidGameNotification : IGameNotification
{
    private AndroidNotification internalNotification;

    /// <summary>
    /// Gets the internal notification object used by the mobile notifications system.
    /// </summary>
    public AndroidNotification InternalNotification => internalNotification;

    /// <inheritdoc />
    /// <summary>
    /// On Android, if the ID isn't explicitly set, it will be generated after it has been scheduled.
    /// </summary>
    public int? Id { get; set; }

    /// <inheritdoc />
    public string Title { get => InternalNotification.Title; set => internalNotification.Title = value; }

    /// <inheritdoc />
    public string Body { get => InternalNotification.Text; set => internalNotification.Text = value; }

    /// <summary>
    /// Does nothing on Android.
    /// </summary>
    public string Subtitle { get => null; set { } }

    /// <inheritdoc />
    public string Data { get => InternalNotification.IntentData; set => internalNotification.IntentData = value; }

    /// <inheritdoc />
    /// <remarks>
    /// On Android, this represents the notification's channel, and is required. Will be configured automatically by
    /// <see cref="AndroidNotificationsPlatform"/> if <see cref="AndroidNotificationsPlatform.DefaultChannelId"/> is set
    /// </remarks>
    /// <value>The value of <see cref="DeliveredChannel"/>.</value>
    public string Group { get => DeliveredChannel; set => DeliveredChannel = value; }

    /// <inheritdoc />
    public int? BadgeNumber
    {
        get => internalNotification.Number != -1 ? internalNotification.Number : (int?)null;
        set => internalNotification.Number = value ?? -1;
    }

    /// <inheritdoc />
    public bool ShouldAutoCancel
    {
        get => InternalNotification.ShouldAutoCancel;
        set => internalNotification.ShouldAutoCancel = value;
    }

    /// <inheritdoc />
    public DateTime? DeliveryTime
    {
        get => InternalNotification.FireTime;
        set => internalNotification.FireTime = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets or sets the channel for this notification.
    /// </summary>
    public string DeliveredChannel { get; set; }

    /// <inheritdoc />
    public bool Scheduled { get; private set; }

    /// <inheritdoc />
    public string SmallIcon { get => InternalNotification.SmallIcon; set => internalNotification.SmallIcon = value; }

    /// <inheritdoc />
    public string LargeIcon { get => InternalNotification.LargeIcon; set => internalNotification.LargeIcon = value; }

    /// <summary>
    /// Instantiate a new instance of <see cref="AndroidGameNotification"/>.
    /// </summary>
    public AndroidGameNotification()
    {
        internalNotification = new AndroidNotification();
    }

    /// <summary>
    /// Instantiate a new instance of <see cref="AndroidGameNotification"/> from a delivered notification
    /// </summary>
    /// <param name="deliveredNotification">The notification that has been delivered.</param>
    /// <param name="deliveredId">The ID of the delivered notification.</param>
    /// <param name="deliveredChannel">The channel the notification was delivered to.</param>
    internal AndroidGameNotification(AndroidNotification deliveredNotification, int deliveredId,
                                     string deliveredChannel)
    {
        internalNotification = deliveredNotification;
        Id = deliveredId;
        DeliveredChannel = deliveredChannel;
    }

    /// <summary>
    /// Set the scheduled flag.
    /// </summary>
    internal void OnScheduled()
    {
        Assert.IsFalse(Scheduled);
        Scheduled = true;
    }
}


#endif

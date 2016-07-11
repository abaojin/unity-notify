using UnityEngine;

#if UNITY_IPHONE
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif

public class NotificationScript : MonoBehaviour
{
    /// <summary>
    /// 本地推送
    /// </summary>
    /// <param name="message"></param>
    /// <param name="hour"></param>
    /// <param name="isRepeatDay"></param>
    public static void NotificationMessage(string message, int hour, bool isRepeatDay)
    {
        int year = System.DateTime.Now.Year;
        int month = System.DateTime.Now.Month;
        int day = System.DateTime.Now.Day;
        System.DateTime newDate = new System.DateTime(year, month, day, hour, 0, 0);
        NotificationMessage(message, newDate, isRepeatDay);
    }

    /// <summary>
    /// 本地推送-传固定时间
    /// </summary>
    /// <param name="message"></param>
    /// <param name="newDate"></param>
    /// <param name="isRepeatDay"></param>
    public static void NotificationMessage(string message, System.DateTime newDate, bool isRepeatDay)
    {

#if UNITY_IPHONE
		// 推送时间需要大于当前时间
		if(newDate > System.DateTime.Now)
		{
			UnityEngine.iOS.LocalNotification localNotification = new UnityEngine.iOS.LocalNotification();
			localNotification.fireDate =newDate;	
			localNotification.alertBody = message;
			localNotification.applicationIconBadgeNumber = 1;
			localNotification.hasAction = true;
			localNotification.alertAction = "这是notificationtest的标题";
			if(isRepeatDay)
			{
				// 是否每天定期循环
				localNotification.repeatCalendar = UnityEngine.iOS.CalendarIdentifier.ChineseCalendar;
				localNotification.repeatInterval = UnityEngine.iOS.CalendarUnit.Day;
			}
			localNotification.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
			UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(localNotification);
		}
#endif

#if UNITY_ANDROID
		if (newDate > System.DateTime.Now) {
            LocalNotification.SendNotification(1, 10, "这是notificationtest的标题", "这是notificationtest的消息", new Color32(0xff, 0x44, 0x44, 255));
			if (System.DateTime.Now.Hour >= 12) {
				long delay = 24 * 60 * 60 - ((System.DateTime.Now.Hour - 12)* 60 * 60 + System.DateTime.Now.Minute * 60 + System.DateTime.Now.Second);
                LocalNotification.SendRepeatingNotification(2, delay, 24 * 60 * 60, "这是notificationtest的标题", "每天中午12点推送", new Color32(0xff, 0x44, 0x44, 255));
			} else {
				long delay = (12 - System.DateTime.Now.Hour)* 60 * 60 - System.DateTime.Now.Minute * 60 - System.DateTime.Now.Second;
				LocalNotification.SendRepeatingNotification(2,delay,24 * 60 * 60 ,"这是notificationtest的标题","每天中午12点推送",new Color32(0xff, 0x44, 0x44, 255));	
			}
		}
#endif
    }

    void Awake()
    {
#if UNITY_IPHONE
		UnityEngine.iOS.NotificationServices.RegisterForNotifications (NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
#endif

        CleanNotification();
    }

    void OnApplicationPause(bool paused)
    {
        // 程序进入后台时
        if (paused) {
            // 10秒后发送
            NotificationMessage("这是notificationtest的推送正文信息", System.DateTime.Now.AddSeconds(10), false);
            // 每天中午12点推送
            NotificationMessage("每天中午12点推送", 12, true);
        } else {
            // 程序从后台进入前台时
            CleanNotification();
        }
    }

    //清空所有本地消息
    void CleanNotification()
    {
#if UNITY_IPHONE
		UnityEngine.iOS.LocalNotification l = new UnityEngine.iOS.LocalNotification (); 
		l.applicationIconBadgeNumber = -1; 
		UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow (l); 
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications (); 
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications (); 
#endif
#if UNITY_ANDROID
		LocalNotification.CancelNotification(1);
		LocalNotification.CancelNotification(2);
#endif
    }
}
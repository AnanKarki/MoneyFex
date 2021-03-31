using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SAutoPaymentScheduler
    {
        
            private static SAutoPaymentScheduler _instance;
            private List<Timer> timers = new List<Timer>();
            private SAutoPaymentScheduler() { }
            public static SAutoPaymentScheduler Instance => _instance ?? (_instance = new SAutoPaymentScheduler());
            public void ScheduleTask(int hour, int min, double intervalInHour, Action task)
            {
                DateTime now = DateTime.Now;
                DateTime firstRun = new DateTime(now.Year, now.Month, now.Day, hour, min, 0, 0);
                if (now > firstRun)
                {
                    firstRun = firstRun.AddDays(1);
                }
                TimeSpan timeToGo = firstRun - now;
                if (timeToGo <= TimeSpan.Zero)
                {
                    timeToGo = TimeSpan.Zero;
                }
                var timer = new Timer(x =>
                {
                    task.Invoke();
                }, null, timeToGo, TimeSpan.FromHours(intervalInHour));
                timers.Add(timer);
            
        }

    }

    public static class MyScheduler
    {
        public static void IntervalInSeconds(int hour, int sec, double interval, Action task)
        {
            interval = interval / 3600;
            SAutoPaymentScheduler.Instance.ScheduleTask(hour, sec, interval, task);
        }
        public static void IntervalInMinutes(int hour, int min, double interval, Action task)
        {
            interval = interval / 60;
            SAutoPaymentScheduler.Instance.ScheduleTask(hour, min, interval, task);
        }
        public static void IntervalInHours(int hour, int min, double interval, Action task)
        {
            SAutoPaymentScheduler.Instance.ScheduleTask(hour, min, interval, task);
        }
        public static void IntervalInDays(int hour, int min, double interval, Action task)
        {
            interval = interval * 24;
            SAutoPaymentScheduler.Instance.ScheduleTask(hour, min, interval, task);
        }
    }



}
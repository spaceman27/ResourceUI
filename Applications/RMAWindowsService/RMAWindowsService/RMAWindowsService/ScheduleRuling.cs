using System;
using System.Timers;

namespace RMAWindowsService
{
    class ScheduleRuling
    {
        public string OccurBy { get; set; } // day, week, month
        public int OccurEvery { get; set; } // 2 (weeks)
        public DayOfWeek[] Frequent { get; set; } // { "Wed", "Sun" }  - Ascending
        public string[] StartTime { get; set; } // { "2:00", "14:00" }  - Ascending
        private System.Timers.Timer Timer;

        public void StartTimer(Action obj)
        {
            double interval = GetNextInterval();
            Timer = new System.Timers.Timer
            {
                Interval = interval,
                Enabled = true,
                AutoReset = true
            };
            Timer.Elapsed += (sender, e) => Timer_Elapsed(sender, e, obj);
            Timer.Start();
        }


        private void Timer_Elapsed(object sender, ElapsedEventArgs e, Action obj)
        {
            obj(); // invoke method here
            RecycleTimer(); // recycle the timer
        }

        private double GetNextInterval()
        {
            TimeSpan nextTime = new TimeSpan(0);
            double interval = 0;
            int day = 0;
            switch (OccurBy)
            {
                case "day":
                    nextTime = GetNextTime(DateTime.Now, DateTime.Today);
                    interval = nextTime.Ticks == 0 ? GetNextTime(DateTime.Now, DateTime.Today.AddDays(1)).TotalMilliseconds : nextTime.TotalMilliseconds;
                    break;
                case "week":
                    GetNextWeekDay(DateTime.Now, DateTime.Today);
                    break;
                case "month":
                    break;
            }
            return interval;
        }
        private TimeSpan GetNextTime(DateTime checkDateTime, DateTime expectedDateNoTime)
        {
            var nowTimespan = checkDateTime;
            foreach (var time in StartTime)
            {
                var iTimespan = expectedDateNoTime.AddSeconds(TimeSpan.Parse(time).TotalSeconds);
                if (nowTimespan < iTimespan)
                {
                    return iTimespan.Subtract(nowTimespan);
                }
            }
            return new TimeSpan(0);
        }

        private TimeSpan GetNextWeekDay(DateTime checkDateTime, DateTime expectedDateNoTime)
        {
            var currentWeekday = (int)checkDateTime.DayOfWeek;
            int minClosestWeekday = 7;
            foreach (var weekday in Frequent)
            {
                var intWeekday = (int)weekday;
                if (intWeekday > currentWeekday && intWeekday < minClosestWeekday)
                {
                    minClosestWeekday = intWeekday;
                }
            }

            return new TimeSpan(0);
        }

        private void RecycleTimer()
        {
            double interval = GetNextInterval();
            Timer.Interval = interval;
        }
    }
}

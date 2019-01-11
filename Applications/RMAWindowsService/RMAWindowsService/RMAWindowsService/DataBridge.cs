using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace RMAWindowsService
{
    class DataBridge
    {
        string cultureName = "en-US";
        private ScheduleRuling activeDirectoryRule;
        private ScheduleRuling sharePointRule;

        public DataBridge()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);

            sharePointRule = new ScheduleRuling()
            {
                OccurBy = "day",
                StartTime = new[] { "1:00", "13:00" }
            };

            // Register for new timer
            activeDirectoryRule = new ScheduleRuling()
            {
                OccurBy = "day",
                OccurEvery = 2,
                Frequent = new DayOfWeek[] { DayOfWeek.Wednesday, DayOfWeek.Sunday },
                StartTime = new[] { "1:03", "13:03" }
            };
        }

        private void RunAd()
        {
            ADSecurity.Run();
        }

        private void RunSharePointUpdate()
        {
            SharePointLDAPUpdate.Run();
        }
        public bool Start()
        {
            sharePointRule.StartTimer(RunSharePointUpdate);
            activeDirectoryRule.StartTimer(RunAd);
            return true;
        }

        public bool Stop()
        {
            return true;
        }
    }
}

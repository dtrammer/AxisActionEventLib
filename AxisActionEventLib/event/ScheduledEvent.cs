using ActionEventLib.events;
using ActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxisActionEventLib.events
{
    public class ScheduledEvent
    {
        public string eventID;
        public string Name;
        public ICalendar Schedule;

        public override string ToString()
        {
            return (!string.IsNullOrEmpty(eventID) ? @"<even:EventID>" + eventID + @"</even:EventID>" : "") +
                   (!string.IsNullOrEmpty(Name) ? @"<even:Name>" + Name + @"</even:Name>" : "") +
                   @"<even:Schedule><even:ICalendar Dialect=""http://www.axis.com/vapix/ws/ical1"">" + Schedule.ToString() + @"</even:ICalendar></even:Schedule>";
        }
    }
}

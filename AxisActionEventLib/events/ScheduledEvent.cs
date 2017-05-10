using ActionEventLib.events;
using ActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.events
{
    public class ScheduledEvent
    {
        public string EventID;
        public string Name;
        public ICalendar Schedule;

        public ScheduledEvent() { }

        /// <summary>
        /// Represents a Scheduled Event either recurrent or scheduled
        /// </summary>
        /// <param name="Name">The nice name of the event</param>
        /// <param name="Schedule">An ICalendar object, either recurrent or Scheduled</param>
        public ScheduledEvent(string Name, ICalendar Schedule)
        {
            this.Name = Name;
            this.Schedule = Schedule;
        }

        public override string ToString()
        {
            string msg  ="";

            if(!string.IsNullOrEmpty(EventID))
                msg += @"<even:EventID>" + EventID + @"</even:EventID>";

            if(!string.IsNullOrEmpty(Name))
                msg += @"<even:Name>" + Name + @"</even:Name>";

            msg += @"<even:Schedule><even:ICalendar Dialect=""http://www.axis.com/vapix/ws/ical1"">" + Schedule.ToString() + @"</even:ICalendar></even:Schedule>";

            return msg;
        }
    }
}

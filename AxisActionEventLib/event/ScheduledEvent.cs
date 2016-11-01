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
    }
}

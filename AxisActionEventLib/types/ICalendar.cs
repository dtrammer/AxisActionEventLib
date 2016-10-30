using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.types
{
    /// <summary>
    /// Class used for the event service to specify a schedule filter
    /// </summary>
    public class ICalendar
    {
        public DateTime StartDate;
        public DateTime EndDate;

        public int FrequencyRule;

        public override string ToString() {
            return StartDate.ToString("DTSTART:yyyyMMddThhmmssZ") + "\r\n" + EndDate.ToString("DTEND:yyyyMMddThhmmssZ") + "\r\n" + "RRULE:FREQ=WEEKLY";
        }
    }
}

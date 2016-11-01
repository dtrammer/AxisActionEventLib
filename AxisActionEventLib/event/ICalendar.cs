using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActionEventLib.events
{
    /// <summary>
    /// Class used by the event service to setup a scheduled event
    /// It represents a schedule in the following format
    /// DTSTART:19700103T000000\n
    /// DTEND:19700105T000000\n
    /// RRULE:FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR
    /// </summary>
    public class ICalendar
    {
        public DateTime StartDateTime;
        public DateTime EndDateTime;

        public string Rrule = "FREQ=WEEKLY;BYDAY=MO,TU,WE,TH,FR";

        public int FrequencyRule;

        public ICalendar() { }

        public ICalendar(string ICalendarTag)
        {
            var start = Regex.Match(ICalendarTag, @"(?<=DTSTART:).*?(?=\n)").Value;

            this.StartDateTime = new DateTime(int.Parse(start.Substring(0, 4)), int.Parse(start.Substring(4, 2)), int.Parse(start.Substring(6, 2)) , int.Parse(start.Substring(9, 2)) , int.Parse(start.Substring(11, 2)) , int.Parse(start.Substring(13, 2)) );

            start = Regex.Match(ICalendarTag, @"(?<=DTEND:).*?(?=\n)").Value;

            this.EndDateTime = new DateTime(int.Parse(start.Substring(0, 4)), int.Parse(start.Substring(4, 2)), int.Parse(start.Substring(6, 2)), int.Parse(start.Substring(9, 2)), int.Parse(start.Substring(11, 2)), int.Parse(start.Substring(13, 2)));

            this.Rrule = Regex.Match(ICalendarTag, @"(?<=RRULE:).*").Value;
        }

        public override string ToString() {
            return StartDateTime.ToString("DTSTART:yyyyMMddThhmmss") + "\r\n" + EndDateTime.ToString("DTEND:yyyyMMddThhmmss") + "\r\nRRULE:" + Rrule;
        }
    }
}

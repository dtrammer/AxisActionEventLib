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
        private int m_interval=0;
        private PulseInterval m_recurrence;

        private ScheduleFrequency m_scheduleFrequency;
        private DateTime m_startDateTime;
        private DateTime m_endDateTime;
        private ScheduleDay[] m_days;
        private ScheduleMonth[] m_months;

        private string m_ICalendarTag = string.Empty;


        #region constructors

        /// <summary>
        /// Constructor to create a recurrence (pulse event)
        /// </summary>
        /// <param name="Interval"></param>
        /// <param name="Recurrence"></param>
        public ICalendar(int Interval , PulseInterval Recurrence )
        {
            if (Interval == 0 || Interval < 0)
                throw new Exception("[ICalendar] Interval must be higher than 0");

            m_interval = Interval;
            m_recurrence = Recurrence;
        }

        /// <summary>
        /// Constructor to create a daily schedule and specify for which days it applies
        /// </summary>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="Days">An array of ICalendarDays elements that represent days of the week (ex: MO, TU, WE ...)</param>
        public ICalendar(ScheduleTime StartTime , ScheduleTime EndTime , ScheduleDay[] Days)
        {
            m_scheduleFrequency = ScheduleFrequency.DAILY;
            m_startDateTime = new DateTime(1970, 01, 01, StartTime.Hour, StartTime.Minutes, StartTime.Seconds);
            m_endDateTime = new DateTime(1970, 01, 01, EndTime.Hour, EndTime.Minutes, EndTime.Seconds);
            m_days = Days;
        }

        /// <summary>
        /// Constructor for a weekly schedule
        /// </summary>
        /// <param name="StartDay"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndDay"></param>
        /// <param name="EndTime"></param>
        public ICalendar(ScheduleDay StartDay , ScheduleTime StartTime , ScheduleDay EndDay , ScheduleTime EndTime)
        {
            m_scheduleFrequency = ScheduleFrequency.WEEKLY;
            m_startDateTime = new DateTime(1970, 01, (int)StartDay, StartTime.Hour, StartTime.Minutes, StartTime.Seconds);
            m_endDateTime = new DateTime(1970, 01, (int)EndDay, EndTime.Hour, EndTime.Minutes, EndTime.Seconds);
        }

        /// <summary>
        /// Constructor for a monthly schedule and specify for which months it applies
        /// </summary>
        /// <param name="StartDate">Start date, value between 1 - 31</param>
        /// <param name="StartTime">Start time</param>
        /// <param name="EndDate">End date, value between 1-31</param>
        /// <param name="EndTime">End time</param>
        /// <param name="Months"></param>
        public ICalendar(int StartDate , ScheduleTime StartTime , int EndDate , ScheduleTime EndTime , ScheduleMonth[] Months)
        {
            m_scheduleFrequency = ScheduleFrequency.MONTHLY;
            m_startDateTime = new DateTime(1970, 01, StartDate, StartTime.Hour, StartTime.Minutes, StartTime.Seconds);
            m_endDateTime = new DateTime(1970, 01, EndDate, EndTime.Hour, EndTime.Minutes, EndTime.Seconds);
            if (!Months.Any())
                throw new Exception("[ICalendar:MonthlySchedule] Months array argument cannot be empty");
            else
                m_months = Months;
        }

        /// <summary>
        /// Constructor for a yearly schedule 
        /// </summary>
        /// <param name="StartMonth"></param>
        /// <param name="StartDate">The date, value between 1-31</param>
        /// <param name="StartTime"></param>
        /// <param name="EndMonth"></param>
        /// <param name="EndDate">The date, value between 1-31</param>
        /// <param name="Endtime"></param>
        public ICalendar(ScheduleMonth StartMonth, int StartDate , ScheduleTime StartTime , ScheduleMonth EndMonth , int EndDate , ScheduleTime Endtime)
        {
            m_scheduleFrequency = ScheduleFrequency.YEARLY;
            m_startDateTime = new DateTime(1970, (int)StartMonth, StartDate, StartTime.Hour, StartTime.Minutes, StartTime.Seconds);
            m_endDateTime = new DateTime(1970, (int)EndMonth, EndDate, Endtime.Hour, Endtime.Minutes, Endtime.Seconds);

        }
        #endregion

        /// <summary>
        /// Constructor used for GetScheduledEvents response parsing
        /// </summary>
        /// <param name="ICalendarTag">GetScheduledEvents(...) response body</param>
        public ICalendar(string ICalendarTag)
        {
            m_ICalendarTag = ICalendarTag;
        }

        public override string ToString() {
            if (string.IsNullOrEmpty(m_ICalendarTag))
            {
                StringBuilder sb = new StringBuilder();
                if (m_interval > 0)
                    return "DTSTART:19700101T000000" + "\r\n" + "RRULE:FREQ=" + m_recurrence + ";INTERVAL=" + m_interval;
                else
                {
                    sb.Append("DTSTART:");
                    sb.AppendLine(m_startDateTime.ToString("yyyyMMddTHHmmss"));
                    sb.Append("DTEND:");
                    sb.AppendLine(m_endDateTime.ToString("yyyyMMddTHHmmss"));

                    switch (m_scheduleFrequency)
                    {
                        case ScheduleFrequency.DAILY:
                            sb.Append("RRULE:FREQ=WEEKLY;BYDAY=");
                            foreach (ScheduleDay d in m_days)
                                sb.Append(d + ",");
                            sb.Remove(sb.Length - 1, 1);
                            break;
                        case ScheduleFrequency.WEEKLY:
                            sb.Append("RRULE:FREQ=WEEKLY");
                            break;
                        case ScheduleFrequency.MONTHLY:
                            sb.Append("RRULE:FREQ=YEARLY;BYMONTH=");
                            foreach (ScheduleMonth m in m_months)
                                sb.Append((int)m + ",");
                            sb.Remove(sb.Length - 1, 1);
                            break;
                        case ScheduleFrequency.YEARLY:
                            sb.Append("RRULE:FREQ=YEARLY");
                            break;
                    }
                    return sb.ToString();
                }
            }else
                return m_ICalendarTag;
        }

        private enum ScheduleFrequency
        {
            DAILY, WEEKLY, MONTHLY, YEARLY
        }
    }

    public struct ScheduleTime
    {
        public int Hour;
        public int Minutes;
        public int Seconds;

        public ScheduleTime(int Hour , int Minutes = 0)
        {
            if (Hour > 23 || Minutes > 59)
                throw new Exception("[FrequencyTime] Invalid value specified four hour or minutes");

            this.Hour = Hour;
            this.Minutes = Minutes;
            this.Seconds = 0;
        }
    }
    public enum PulseInterval
    {
        SECONDLY, MINUTELY, HOURLY, DAILY
    }
    public enum ScheduleDay
    {
        SU = 4, MO, TU , WE , TH, FR , SA
    }
    public enum ScheduleMonth
    {
        JAN = 1, FEB , MAR , APR ,MAY , JUN , JUL , AUG , SEP , OCT , NOV , DEC
    }
}

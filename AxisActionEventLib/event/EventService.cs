
using ActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.events
{
    /// <summary>
    /// Class that provides methods to query the Events webservice of Axis devices
    /// Wsdl -> http://www.axis.com/vapix/ws/event1/EventService.wsdl
    /// Documentation -> http://www.axis.com/partner_pages/vapix_library/#/subjects/t10037719/section/t10008227/display?section=t10008227-t10003111
    /// </summary>
    public class EventService : SOAPRequest
    {
        /// <summary>
        /// Method to retrieve the configured events in the device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">deviceCredentials that will be used for the request</param>
        public async Task<ServiceResponse> GetEventsInstancesAsync(string IP , string User , string Password) {
            return base.parseGetEventInstancesResponse(await base.sendRequestAsync(IP, User , Password , @"<even:GetEventInstances />"));
        }

        /// <summary>
        /// Method to retrieve the scheduled events configured in the device
        /// </summary>
        /// <param name="IP">Device ip address</param>
        /// <param name="Credentials">User credentials for the request</param>
        /// <param name="ScheduleType">Possible values "Interval" or "Pulse" see  for more info</param>
        public async Task<ServiceResponse> GetScheduledEventsAsync(string IP , string User, string Password , String ScheduleType = "") {
            if (string.IsNullOrEmpty(ScheduleType))
                return await base.sendRequestAsync(IP, User, Password,  @"<even:GetScheduledEvents />");
            else
                return await base.sendRequestAsync(IP, User, Password, @"<even:GetScheduledEvents><even:ScheduleFilter><even:ScheduleType>" + ScheduleType + @"</even:ScheduleFilter></even:GetScheduledEvents>");
        }

        /// <summary>
        /// Method that can be used to change the state of a virtual input port on the device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="PortNumber">The port number of the virtual input</param>
        /// <param name="Active">The new state to set the port too</param>
        /// <returns></returns>
        public async Task<ServiceResponse> ChangeVirtualInputStateAsync(string IP, string User, string Password, int PortNumber , bool Active) {
            String bodyAction = @"<even:ChangeVirtualInputState>" +
                                @"<even:port>" + PortNumber + @"</even:port>" +
                                @"<even:active>" + (Active ? 1 : 0) + @"</even:active>" +
                                @"</even:ChangeVirtualInputState>";

            return await base.sendRequestAsync(  IP , User , Password , bodyAction );
        }

        /// <summary>
        /// Method that can be used to create a scheduled event in the device 
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="Schedule">A ICalendar object that indicates time validity for the event</param>
        /// <param name="EventID">your own ID you would like to give to the event</param>
        /// <param name="Name">A name for the event</param>
        /// <returns></returns>
        public async Task<ServiceResponse> AddScheduledEventAsync(string IP, string User, string Password, ICalendar Schedule, string EventID = "" , string Name = "") {
            String bodyAction = @"<even:AddScheduledEvent>" +
                                @"<even:NewScheduledEvent>" +
                                (!string.IsNullOrEmpty(EventID) ? @"<even:EventID>" + EventID + @"</even:EventID>" : "") +
                                (!string.IsNullOrEmpty(Name) ? @"<even:Name>" + Name + @"</even:Name>" : "") +
                                @"<even:Schedule><even:ICalendar Dialect=""http://www.axis.com/vapix/ws/ical1"">" + Schedule.ToString() + @"</even:ICalendar></even:Schedule>" +
                                @"</even:NewScheduledEvent>" + 
                                @"</even:AddScheduledEvent>";

            return await base.sendRequestAsync( IP , User , Password, bodyAction);
        }

        /// <summary>
        /// Method that can be used to remove a scheduled event based on the eventid
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="EventID">The ID of the scheduled event</param>
        /// <returns></returns>
        public async Task<ServiceResponse> RemoveScheduledEventAsync(string IP, string User, string Password, string EventID) {
            String bodyAction = @"<even:RemoveScheduledEvent>" +
                                @"<even:EventID>" + EventID + @"</even:EventID>" +
                                @"</even:RemoveScheduledEvent>";

            return await base.sendRequestAsync( IP , User , Password , bodyAction);
        }
    }


}

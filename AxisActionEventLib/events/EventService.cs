
using ActionEventLib.events;
using ActionEventLib.types;
using AxisActionEventLib.events;
using AxisActionEventLib.types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        /// Method to retrieve the available eventsinstances in the device. 
        /// Those can be used to setup EventTriggers when creating ActionRules (events)
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User name used for the request</param>
        /// <param name="Password">Password used for the request</param>
        /// <returns>GetEventInstancesResponse</returns>
        public async Task<GetEventInstancesResponse> GetEventsInstancesAsync(string IP , string User , string Password) {
            return parseGetEventInstancesResponse(await base.sendRequestAsync(IP, User , Password , @"<even:GetEventInstances />"));
        }

        /// <summary>
        /// Method to retrieve the scheduled events configured in the device
        /// </summary>
        /// <param name="IP">Device ip address</param>
        /// <param name="Credentials">User credentials for the request</param>
        /// <param name="ScheduleType">Possible values "Interval" or "Pulse"</param>
        /// <returns>GetScheduledEventsResponse</returns>
        public async Task<GetScheduledEventsResponse> GetScheduledEventsAsync(string IP , string User, string Password , String ScheduleType = "") {
            if (string.IsNullOrEmpty(ScheduleType))
                return this.parseGetScheduledEventsResponse(await base.sendRequestAsync(IP, User, Password,  @"<even:GetScheduledEvents />"));
            else
                return this.parseGetScheduledEventsResponse(await base.sendRequestAsync(IP, User, Password, @"<even:GetScheduledEvents><even:ScheduleFilter><even:ScheduleType>" + ScheduleType + @"</even:ScheduleFilter></even:GetScheduledEvents>"));
        }



        /// <summary>
        /// Method that can be used to change the state of a virtual input port on the device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User name used for the request</param>
        /// <param name="Password">Password used for the request</param>
        /// <param name="PortNumber">The port number of the virtual input possible values usually 1->30</param>
        /// <param name="Active">The new state to set the port too</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> Change_VirtualInputStateAsync(string IP, string User, string Password, int PortNumber , bool Active) {
            String bodyAction = @"<even:ChangeVirtualInputState>" +
                                @"<even:port>" + PortNumber + @"</even:port>" +
                                @"<even:active>" + (Active ? 1 : 0) + @"</even:active>" +
                                @"</even:ChangeVirtualInputState>";

            return this.parseChangeVirtualInputStateResponse(await base.sendRequestAsync(  IP , User , Password , bodyAction ));
        }

        /// <summary>
        /// Method that can be used to create a scheduled event in the device
        /// The eventID field of the passed ScheduledEvent object will be updated on success
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User name used for the request</param>
        /// <param name="Password">Password used for the request</param>
        /// <param name="Schedule">An ICalendar object that indicates dateTime validity for the event</param>
        /// <param name="Event">A ScheduledEvent instance</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> Add_ScheduledEventAsync(string IP, string User, string Password, ScheduledEvent Event) {
            return await base.sendRequestAsync( IP , User , Password, @"<even:AddScheduledEvent><even:NewScheduledEvent>" + Event.ToString() + @"</even:NewScheduledEvent></even:AddScheduledEvent>");
        }
        
        /// <summary>
        /// Method use to edit an existing scheduled event
        /// </summary>
        /// <param name="IP"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        /// <param name="EventID"></param>
        /// <param name="NewEvent"></param>
        /// <returns></returns>
        public async Task<ServiceResponse> Edit_ScheduledEventAsync(string IP, string User, string Password, string EventID , ScheduledEvent NewEvent)
        {
            ServiceResponse response = await this.Remove_ScheduledEventAsync(IP, User, Password, EventID);
            if(response.IsSuccess)
                response = await this.Add_ScheduledEventAsync(IP, User, Password, NewEvent);

            return response;
        }

        /// <summary>
        /// Method that can be used to remove a scheduled event based on the eventid
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="User">User name used for the request</param>
        /// <param name="Password">Password used for the request</param>
        /// <param name="EventID">The ID of the scheduled event to remove</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> Remove_ScheduledEventAsync(string IP, string User, string Password, string EventID) {
            String bodyAction = @"<even:RemoveScheduledEvent>" +
                                @"<even:EventID>" + EventID + @"</even:EventID>" +
                                @"</even:RemoveScheduledEvent>";

            return await base.sendRequestAsync( IP , User , Password , bodyAction);
        }
        
        #region XML response parsing methods
        private GetEventInstancesResponse parseGetEventInstancesResponse(ServiceResponse Response)
        {
            GetEventInstancesResponse response = Response.Factory<GetEventInstancesResponse>();
            if (response.IsSuccess)
            {
                try
                {
                    IEnumerable<XElement> configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "GetEventInstancesResponse").Element(NS_WTOPIC + "TopicSet").Elements();
                    List<EventTrigger> EventTriggers = new List<EventTrigger>();
                    string topic = "";

                    foreach (XElement el in configResponse) //Iterate all tns1: topics
                    {
                        topic = "tns1:" + el.Name.LocalName + "/tnsaxis:";

                        if (!el.HasAttribute(NS_WTOPIC + "topic"))
                            getTopics(topic, EventTriggers, el);
                    }

                    response.EventInstances = EventTriggers.ToList();
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[ParseEventInstancesResponse] " + ex.Message;
                }
            }

            return response;
        }
        //Recursive method to build the eventinstance topic string & extract Eventtrigger parameters (example of topic: tns1:Device/tnsaxis:Device/IO/VirtualPort)
        private EventTrigger newEventTrigger;
        private void getTopics(String Topic, List<EventTrigger> Triggers, XElement Element)
        {
            try
            {
                Topic += Element.Name.LocalName + "/";

                if (!Element.HasAttribute(NS_WTOPIC + "topic"))
                    foreach (XElement el in Element.Elements())
                        getTopics(Topic, Triggers, el);
                else
                {
                    //create new eventTrigger with Topic & IsProperty attribute
                    newEventTrigger = new EventTrigger(Topic.Remove(Topic.Length - 1, 1), Element.Element(NS_EVENT + "MessageInstance").HasAttribute(NS_EVENT + "isProperty"));

                    //Check for SourceInstance Tag and parse the SimpleItemInstance
                    if (Element.Element(NS_EVENT + "MessageInstance").HasElement(NS_EVENT + "SourceInstance"))
                        this.parseEventTriggerParam(Element.Element(NS_EVENT + "MessageInstance").Element(NS_EVENT + "SourceInstance").Element(NS_EVENT + "SimpleItemInstance"), newEventTrigger);

                    //Check for DataInstance Tag and parse the simpleiteminstance
                    if (Element.Element(NS_EVENT + "MessageInstance").HasElement(NS_EVENT + "DataInstance"))
                        this.parseEventTriggerParam(Element.Element(NS_EVENT + "MessageInstance").Element(NS_EVENT + "DataInstance").Element(NS_EVENT + "SimpleItemInstance"), newEventTrigger);

                    Triggers.Add(newEventTrigger);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[getTopics] " + ex.Message);
            }
        }
        private void parseEventTriggerParam(XElement SimpleItemInstance , EventTrigger NewEventTrigger)
        {

            NewEventTrigger.Params.Add(new EventTriggerParam(SimpleItemInstance.GetAttributeValue("Name"), SimpleItemInstance.GetAttributeValue(NS_EVENT + "NiceName"), "", SimpleItemInstance.HasAttribute("onvif-element")));

            if (SimpleItemInstance.Elements().Count() > 0)
            {
                foreach (XElement subElement in SimpleItemInstance.Elements())
                    NewEventTrigger.Params[NewEventTrigger.Params.Count - 1].DefaultValues.Add(subElement.Value);
            }
        }
        private ServiceResponse parseChangeVirtualInputStateResponse(ServiceResponse Response)
        {
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "ChangeVirtualInputStateResponse");
                Response.Content = configResponse.Element(NS_EVENT + "stateChanged").Value;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Content = "[parseChangeVirtualInputStateResponse] " + ex.Message;
            }

            return Response;
        }
        private GetScheduledEventsResponse parseGetScheduledEventsResponse(ServiceResponse Response)
        {
            GetScheduledEventsResponse response = Response.Factory<GetScheduledEventsResponse>();

            if (response.IsSuccess)
            {
                try
                {
                    XElement ScheduledEventsResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "GetScheduledEventsResponse").Element(NS_EVENT + "ScheduledEvents");
                    foreach (XElement element in ScheduledEventsResponse.Elements())
                    {
                        response.ScheduledEvents.Add(new AxisActionEventLib.events.ScheduledEvent()
                        {
                            EventID = element.Element(NS_EVENT + "EventID").Value,
                            Name = element.Element(NS_EVENT + "Name").Value,
                            Schedule = new ICalendar(element.Element(NS_EVENT + "Schedule").Element(NS_EVENT + "ICalendar").Value)
                        });
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Content = "[parseGetScheduledEventsResponse] " + ex.Message;
                }
            }

            return response;
        }
        private ServiceResponse parseAddScheduledEventResponse(ServiceResponse Response, ScheduledEvent Event)
        {
            if (Response.IsSuccess)
                Event.EventID = Response.SOAPContent.Element(NS_EVENT + "AddScheduledEventResponse").Element(NS_EVENT + "EventID").Value;

            return Response;
        }
        #endregion
    }

}

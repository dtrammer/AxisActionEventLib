
using ActionEventLib.events;
using ActionEventLib.types;
using AxisActionEventLib.events;
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
        /// Method to retrieve the configured events in the device
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">deviceCredentials that will be used for the request</param>
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
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="PortNumber">The port number of the virtual input</param>
        /// <param name="Active">The new state to set the port too</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> ChangeVirtualInputStateAsync(string IP, string User, string Password, int PortNumber , bool Active) {
            String bodyAction = @"<even:ChangeVirtualInputState>" +
                                @"<even:port>" + PortNumber + @"</even:port>" +
                                @"<even:active>" + (Active ? 1 : 0) + @"</even:active>" +
                                @"</even:ChangeVirtualInputState>";

            return this.parseChangeVirtualInputStateResponse(await base.sendRequestAsync(  IP , User , Password , bodyAction ));
        }

        /// <summary>
        /// Method that can be used to create a scheduled event in the device 
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="Schedule">An ICalendar object that indicates dateTime validity for the event</param>
        /// <param name="EventID">An ID for the event</param>
        /// <param name="Name">A name for the event</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> AddScheduledEventAsync(string IP, string User, string Password, ScheduledEvent Event) {
            return await base.sendRequestAsync( IP , User , Password, @"<even:AddScheduledEvent><even:NewScheduledEvent>" + Event.ToString() + @"</even:NewScheduledEvent></even:AddScheduledEvent>");
        }

        /// <summary>
        /// Method that can be used to remove a scheduled event based on the eventid
        /// </summary>
        /// <param name="IP">The device ip address</param>
        /// <param name="Credentials">User credentials that can be used for the request</param>
        /// <param name="EventID">The ID of the scheduled event</param>
        /// <returns>ServiceResponse</returns>
        public async Task<ServiceResponse> RemoveScheduledEventAsync(string IP, string User, string Password, string EventID) {
            String bodyAction = @"<even:RemoveScheduledEvent>" +
                                @"<even:EventID>" + EventID + @"</even:EventID>" +
                                @"</even:RemoveScheduledEvent>";

            return await base.sendRequestAsync( IP , User , Password , bodyAction);
        }

        
        #region XML response parsing methods
        private GetEventInstancesResponse parseGetEventInstancesResponse(ServiceResponse Response)
        {
            GetEventInstancesResponse response = new GetEventInstancesResponse();
            response.IsSuccess = Response.IsSuccess;
            response.SOAPContent = Response.SOAPContent;
            response.HttpStatusCode = Response.HttpStatusCode;
            response.Content = Response.Content;
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "GetEventInstancesResponse").Element(NS_WTOPIC + "TopicSet");
                string topic;
                List<EventTrigger> EventTriggers = new List<EventTrigger>();

                foreach (XElement el in configResponse.Elements()) //Find all topics
                {
                    topic = "tns1:" + el.Name.LocalName + "/tnsaxis:";

                    if (el.Attribute(NS_WTOPIC + "topic") == null)
                        EventTriggers = getTopics(topic, EventTriggers, el);
                }

                response.Instances = EventTriggers;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Content = "[ParseEventInstancesResponse] " + ex.Message;
            }

            return response;
        }
            private List<EventTrigger> getTopics(string Topic, List<EventTrigger> Triggers, XElement Element)
        {
            try
            {
                Topic += Element.Name.LocalName + "/";

                if (Element.Attribute(NS_WTOPIC + "topic") == null)
                    foreach (XElement el in Element.Elements())
                        Triggers = getTopics(Topic, Triggers, el);
                else
                {

                    if (Element.Element(NS_EVENT + "MessageInstance").Attribute(NS_EVENT + "isProperty") != null)
                    {
                        Triggers.Add(new EventTrigger() { isSimple = false, TopicExpression = Topic.Substring(0, Topic.Length - 1) });
                        Triggers[Triggers.Count - 1].Params = getSimpleInstances(Triggers[Triggers.Count - 1].Params, Element);
                    }
                    else
                        Triggers.Add(new EventTrigger() { isSimple = true, TopicExpression = Topic.Substring(0, Topic.Length - 1) });
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[getTopics] " + ex.Message);
            }
            return Triggers;
        }
            private List<EventTriggerParams> getSimpleInstances(List<EventTriggerParams> Params, XElement Element)
        {
            try
            {
                if (Element.Name.LocalName != "SimpleItemInstance")
                    foreach (XElement el in Element.Elements())
                        Params = getSimpleInstances(Params, el);
                else
                {
                    if (Element.Attribute("isPropertyState") != null)
                        Params.Add(new EventTriggerParams() { name = Element.Attribute("Name").Value, isState = true });
                    else
                    {
                        Params.Add(new EventTriggerParams() { name = Element.Attribute("Name").Value, isState = false, value = string.Empty });

                        foreach (XElement subElement in Element.Elements())
                        {
                            if (subElement.Attribute(NS_EVENT + "NiceName") != null)
                                Params[Params.Count - 1].defaultValues.Add(new Tuple<string, string>(subElement.Attribute(NS_EVENT + "NiceName").Value, subElement.Value));
                            else
                                Params[Params.Count - 1].defaultValues.Add(new Tuple<string, string>(string.Empty, subElement.Value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("[getSimpleInstances] " + ex.Message);
            }
            return Params;
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
            GetScheduledEventsResponse response = new GetScheduledEventsResponse();
            response.IsSuccess = Response.IsSuccess;
            response.SOAPContent = Response.SOAPContent;
            response.HttpStatusCode = Response.HttpStatusCode;
            response.Content = Response.Content;
            try
            {
                XElement ScheduledEventsResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "GetScheduledEventsResponse").Element(NS_EVENT + "ScheduledEvents");
                foreach(XElement element in ScheduledEventsResponse.Elements())
                {
                    response.ScheduledEvents.Add(new AxisActionEventLib.events.ScheduledEvent() {
                        eventID = element.Element(NS_EVENT + "EventID").Value,
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

            return response;
        }
        #endregion
    }

}

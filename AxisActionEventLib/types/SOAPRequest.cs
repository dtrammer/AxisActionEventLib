using ActionEventLib.action;
using ActionEventLib.events;
using ActionEventLib.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionEventLib.types
{
    /// <summary>
    /// Base class for sending HTTP requests with XML/SOAP content to the Action and Event web-services of Axis network devices
    /// The service URL is defaulted to "http://{0}/vapix/services" but can be changed by assigning the Service_URL property 
    /// </summary>
    public abstract class SOAPRequest
    {
        #region namespaces constants
        private const string NS_SOAP_ENV = @"{http://www.w3.org/2003/05/soap-envelope}";
        private const string NS_ACTION = @"{http://www.axis.com/vapix/ws/action1}";
        private const string NS_EVENT = @"{http://www.axis.com/vapix/ws/event1}";
        private const string NS_TOPIC = @"{http://docs.oasis-open.org/wsn/b-2}";
        private const string NS_WTOPIC = @"{http://docs.oasis-open.org/wsn/t-1}";
        private const string NS_TNS1 = @"{http://www.onvif.org/ver10/topics}";
        #endregion

        protected const string _message_base = @"<soap:Envelope xmlns:soap=""http://www.w3.org/2003/05/soap-envelope"" 
                                                    + xmlns:act=""http://www.axis.com/vapix/ws/action1"" xmlns:even=""http://www.axis.com/vapix/ws/event1"" xmlns:wsnt=""http://docs.oasis-open.org/wsn/b-2"" 
                                                    + xmlns:tnsaxis=""http://www.axis.com/2009/event/topics"" xmlns:tt=""http://www.onvif.org/ver10/schema"">"
                                                    + @"<soap:Body>{0}</soap:Body>"
                                                + @"</soap:Envelope>";

        private string _service_url = "http://{0}/vapix/services";
        public string Service_URL {  get { return _service_url; } set { _service_url = value; } }

        protected async Task<ServiceResponse> sendRequestAsync(string IP, string User , string Password, string Action) {

            using (HttpClient httpClient = new HttpClient(new HttpClientHandler()
            {
                Credentials = new NetworkCredential(
                        User,
                        Password
                    ).GetCredential(new Uri(@"http://localhost"), "Digest")
            }, true))
            {
                httpClient.Timeout = TimeSpan.FromSeconds(25);
                ServiceResponse serviceResponse = new ServiceResponse();

                try
                {
                    using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, string.Format(Service_URL, IP)) { Version = HttpVersion.Version10 })
                    {
                        request.Content = new StringContent(string.Format(_message_base, Action));
                        Console.WriteLine(string.Format(_message_base, Action));

                        HttpResponseMessage Response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);

                        if (Response.IsSuccessStatusCode)
                        {
                            serviceResponse.IsSuccess = true; serviceResponse.Content = await Response.Content.ReadAsStringAsync(); serviceResponse.HttpStatusCode = Response.StatusCode;
                            serviceResponse.SOAPContent = XElement.Parse(serviceResponse.Content);
                        }
                        else
                            serviceResponse.IsSuccess = false; serviceResponse.Content = await Response.Content.ReadAsStringAsync(); serviceResponse.HttpStatusCode = Response.StatusCode;

                    }
                }catch(Exception ex)
                {
                    serviceResponse.IsSuccess = false;
                    serviceResponse.Content = "[SendSOAPRequest] " + ex.Message;
                }

                return serviceResponse;
            }
        }

        #region SOAP/XML Parsing Methods
        protected ServiceResponse parseAddActionConfigResponse(ServiceResponse Response , ActionConfiguration Configuration) {
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "AddActionConfigurationResponse");
                Response.Content = configResponse.Element(NS_ACTION + "ConfigurationID").Value;
                Configuration.ConfigurationID = int.Parse(Response.Content);
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Content = "[ParseActionConfigResponse] " + ex.Message;
            }

            return Response;
        }

        protected GetActionTemplatesResponse parseGetActionTemplatesResponse(ServiceResponse Response) {
            GetActionTemplatesResponse response = new GetActionTemplatesResponse();
            try
            {
                XElement templates = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionTemplatesResponse").Element(NS_ACTION + "ActionTemplates");
                ActionTemplate newTemplate;

                foreach (XElement el in templates.Elements())
                {
                    newTemplate = new ActionTemplate();

                    newTemplate.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;

                    if (el.Element(NS_ACTION + "RecipientTemplate") != null)
                        newTemplate.RecipientTemplate = el.Element(NS_ACTION + "RecipientTemplate").Value;

                    foreach (XElement param in el.Element(NS_ACTION + "Parameters").Elements())
                        newTemplate.Parameters.Add(param.Attribute("Name").Value, string.Empty);

                    response.Templates.Add(newTemplate);
                }
            }catch(Exception ex)
            {
                response.IsSuccess = false;
                response.Content = "[ParseActionTemplatesResponse] " + ex.Message;
            }

            return response;

        }
        protected GetRecipientTemplatesResponse parseGetRecipientTemplatesResponse (ServiceResponse Response) {
            GetRecipientTemplatesResponse response = new GetRecipientTemplatesResponse();
            try
            {
                XElement templates = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetRecipientTemplatesResponse").Element(NS_ACTION + "RecipientTemplates");
                RecipientTemplate newTemplate;

                foreach (XElement el in templates.Elements())
                {
                    newTemplate = new RecipientTemplate();

                    newTemplate.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;

                    foreach (XElement param in el.Element(NS_ACTION + "Parameters").Elements())
                        newTemplate.Parameters.Add(param.Attribute("Name").Value, string.Empty);

                    response.Templates.Add(newTemplate);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Content = "[ParseRecipientTemplatesResponse] " + ex.Message;
            }

            return response;
        }
        protected GetActionConfigurationsResponse parseGetActionConfigResponse(ServiceResponse Response) {
            GetActionConfigurationsResponse response = new GetActionConfigurationsResponse();
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionConfigurationsResponse").Element(NS_ACTION + "ActionConfigurations");

                ActionConfiguration conf;

                foreach(XElement el in configResponse.Elements())
                {
                    conf = new ActionConfiguration();
                    conf.ConfigurationID = int.Parse(el.Element(NS_ACTION + "ConfigurationID").Value);
                    conf.Name = el.Element(NS_ACTION + "Name").Value;
                    conf.actionTemplate = new ActionTemplate() { TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value };

                    foreach (XElement element in el.Element(NS_ACTION + "Parameters").Elements())
                        conf.actionTemplate.Parameters.Add(element.Attribute("Name").Value, element.Attribute("Value").Value);

                    response.Configurations.Add(conf);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Content = "[ParseActionConfigResponse] " + ex.Message;
            }

            return response;
        }
        protected GetActionRulesResponse parseGetActionRulesResponse(ServiceResponse Response) {
            GetActionRulesResponse response = new GetActionRulesResponse();
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetActionRulesResponse").Element(NS_ACTION + "ActionRules");
                ActionRule rule;
                string topicExpression;

                foreach (XElement el in configResponse.Elements())
                {
                    rule = new ActionRule();
                    //Parse
                    rule.RuleID = int.Parse(el.Element(NS_ACTION + "RuleID").Value);
                    rule.Name = el.Element(NS_ACTION + "Name").Value;
                    rule.Enabled = bool.Parse(el.Element(NS_ACTION + "Enabled").Value);

                    if(el.Element(NS_ACTION + "StartEvent") != null)
                    {
                        rule.Trigger = new EventTrigger();
                        topicExpression = el.Element(NS_ACTION + "StartEvent").Element(NS_TOPIC + "TopicExpression").Value;
                        if (el.Element(NS_ACTION + "StartEvent").Element(NS_TOPIC + "MessageContent") != null)
                            rule.Trigger.setExtTopic(topicExpression, el.Element(NS_ACTION + "StartEvent").Element(NS_TOPIC + "MessageContent").Value);
                        else
                            rule.Trigger.setSimpleTopic(topicExpression);
                    }

                    if(el.Element(NS_ACTION + "Conditions") != null)
                    {
                        EventTrigger newTrigger = new EventTrigger();
                        foreach(XElement condition in el.Element(NS_ACTION + "Conditions").Elements())
                        {
                            newTrigger = new EventTrigger();
                            topicExpression = condition.Element(NS_TOPIC + "TopicExpression").Value;
                            if (condition.Element(NS_TOPIC + "MessageContent") != null)
                                newTrigger.setExtTopic(topicExpression, condition.Element(NS_TOPIC + "MessageContent").Value, true);
                            else
                                newTrigger.setSimpleTopic(topicExpression, true);

                            rule.TriggerConditions.Add(newTrigger);
                        }
                       
                    }

                    int ActionConfID = int.Parse(el.Element(NS_ACTION + "PrimaryAction").Value);

                    rule.Configuration = new ActionConfiguration() { ConfigurationID = ActionConfID };

                    response.ActionRules.Add(rule);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Content = "[ParseActionRulesResponse] " + ex.Message;
            }

            return response;
        }
        protected GetRecipientConfigurationsResponse parseGetRecipientConfigurations(ServiceResponse Response) {
            GetRecipientConfigurationsResponse response = new GetRecipientConfigurationsResponse();
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_ACTION + "GetRecipientConfigurationsResponse").Element(NS_ACTION + "RecipientConfigurations");
                RecipientConfiguration recipient;

                foreach (XElement el in configResponse.Elements())
                {
                    recipient = new RecipientConfiguration();
                    recipient.ConfigurationID = int.Parse(el.Element(NS_ACTION + "ConfigurationID").Value);
                    recipient.TemplateToken = el.Element(NS_ACTION + "TemplateToken").Value;
                    recipient.Name = el.Element(NS_ACTION + "Name").Value;

                    foreach (XElement element in el.Element(NS_ACTION + "Parameters").Elements())
                    {
                        recipient.Parameters.Add(
                            element.Attribute("Name").Value , element.Attribute("Value").Value
                            );
                    }

                    response.Configurations.Add(recipient);
                }
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.Content = "[ParseRecipientConfigurations] " + ex.Message;
            }

            return response;
        }
        protected GetEventInstancesResponse parseGetEventInstancesResponse(ServiceResponse Response)
        {
            GetEventInstancesResponse response = new GetEventInstancesResponse();
            try
            {
                XElement configResponse = Response.SOAPContent.Element(NS_SOAP_ENV + "Body").Element(NS_EVENT + "GetEventInstancesResponse").Element(NS_WTOPIC + "TopicSet");
                string topic;
                List<EventTrigger> EventTriggers = new List<EventTrigger>();

                foreach(XElement el in configResponse.Elements()) //Find all topics
                {
                    topic = "tns1:" + el.Name.LocalName + "/tnsaxis:";

                    if(el.Attribute(NS_WTOPIC + "topic") == null)
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

        private List<EventTrigger> getTopics(string Topic , List<EventTrigger> Triggers , XElement Element)
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
            }catch(Exception ex)
            {
                throw new Exception("[getTopics] " + ex.Message);
            }
            return Triggers;
        }
        private List<EventTriggerParams> getSimpleInstances(List<EventTriggerParams> Params , XElement Element)
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
            }catch(Exception ex)
            {
                throw new Exception("[getSimpleInstances] " + ex.Message);
            }
            return Params;
        }
        #endregion

    }
}

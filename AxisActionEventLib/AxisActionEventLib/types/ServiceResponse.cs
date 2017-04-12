using ActionEventLib.action;
using ActionEventLib.events;
using ActionEventLib.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionEventLib.types
{
    public class ServiceResponse
    {
        public bool IsSuccess;
        public HttpStatusCode HttpStatusCode;
        public XElement SOAPContent;
        public string Content;

        public T Factory<T>() where T : ServiceResponse , new()
        {
            return new T() { IsSuccess = this.IsSuccess , HttpStatusCode = this.HttpStatusCode , SOAPContent = this.SOAPContent , Content = this.Content };
        }
    }
    /// <summary>
    /// Adds a List<Actiontemplate> to the response object
    /// </summary>
    public class GetActionTemplatesResponse : ServiceResponse { public Dictionary<string, ActionTemplate> Templates = new Dictionary<string, ActionTemplate>(); }
    public class GetRecipientTemplatesResponse : ServiceResponse { public Dictionary<string,RecipientTemplate> Templates = new Dictionary<string, RecipientTemplate>(); }
    public class GetActionConfigurationsResponse : ServiceResponse { public List<ActionConfiguration> Configurations = new List<ActionConfiguration>(); }
    public class GetActionRulesResponse : ServiceResponse { public List<ActionRule> ActionRules = new List<ActionRule>(); }
    public class GetRecipientConfigurationsResponse : ServiceResponse { public List<RecipientConfiguration> Configurations = new List<RecipientConfiguration>(); }
    public class GetEventInstancesResponse : ServiceResponse { public List<EventTrigger> EventInstances = new List<EventTrigger>(); }
    public class GetScheduledEventsResponse : ServiceResponse { public List<ScheduledEvent> ScheduledEvents = new List<ScheduledEvent>();  }
}

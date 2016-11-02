using ActionEventLib.action;
using ActionEventLib.events;
using ActionEventLib.templates;
using AxisActionEventLib.events;
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
            T response = new T();

            response.IsSuccess = this.IsSuccess;
            response.HttpStatusCode = this.HttpStatusCode;
            response.SOAPContent = this.SOAPContent;
            response.Content = this.Content;

            return response;
        }
    }

    public class GetActionTemplatesResponse : ServiceResponse { public List<ActionTemplate> Templates = new List<ActionTemplate>(); }
    public class GetRecipientTemplatesResponse : ServiceResponse { public List<RecipientTemplate> Templates = new List<RecipientTemplate>(); }
    public class GetActionConfigurationsResponse : ServiceResponse { public List<ActionConfiguration> Configurations = new List<ActionConfiguration>(); }
    public class GetActionRulesResponse : ServiceResponse { public List<ActionRule> ActionRules = new List<ActionRule>(); }
    public class GetRecipientConfigurationsResponse : ServiceResponse { public List<RecipientConfiguration> Configurations = new List<RecipientConfiguration>(); }
    public class GetEventInstancesResponse : ServiceResponse { public List<EventTrigger> Instances = new List<EventTrigger>(); }

    public class GetScheduledEventsResponse : ServiceResponse { public List<ScheduledEvent> ScheduledEvents = new List<ScheduledEvent>();  }
}

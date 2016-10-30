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
    }

    public sealed class GetActionTemplatesResponse : ServiceResponse { public List<ActionTemplate> Templates; }

    public sealed class GetRecipientTemplatesResponse : ServiceResponse { public List<RecipientTemplate> Templates; }
    public sealed class GetActionConfigurationsResponse : ServiceResponse { public List<ActionConfiguration> Configurations; }
    
    public sealed class GetActionRulesResponse : ServiceResponse { public List<ActionRule> ActionRules; }

    public sealed class GetRecipientConfigurationsResponse : ServiceResponse { public List<RecipientConfiguration> Configurations; }
    
    public sealed class GetEventInstancesResponse : ServiceResponse { public List<EventTrigger> Instances; }
}

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
            return new T() { IsSuccess = this.IsSuccess , HttpStatusCode = this.HttpStatusCode , SOAPContent = this.SOAPContent , Content = this.Content };
        }
    }

    public class GetActionTemplatesResponse : ServiceResponse { public List<ActionTemplate> Templates = new List<ActionTemplate>(); }
    public class GetRecipientTemplatesResponse : ServiceResponse { public List<RecipientTemplate> Templates = new List<RecipientTemplate>(); }
    public class GetActionConfigurationsResponse : ServiceResponse { public List<ActionConfiguration> Configurations = new List<ActionConfiguration>(); }
    public class GetActionRulesResponse : ServiceResponse { public List<ActionRule> ActionRules = new List<ActionRule>(); }
    public class GetRecipientConfigurationsResponse : ServiceResponse { public List<RecipientConfiguration> Configurations = new List<RecipientConfiguration>(); }
    public class GetEventInstancesResponse : ServiceResponse { public List<EventTrigger> EventInstances = new List<EventTrigger>(); }
    public class GetScheduledEventsResponse : ServiceResponse { public List<ScheduledEvent> ScheduledEvents = new List<ScheduledEvent>();  }

    public class GetTemplatesAndEventInstancesResponse : ServiceResponse
    {
        public Dictionary<string, RecipientTemplate> RecipientTemplates = new Dictionary<string, RecipientTemplate>();
        public Dictionary<string, ActionTemplate> ActionTemplates = new Dictionary<string, ActionTemplate>();
        public Dictionary<string, EventTrigger> EventInstances = new Dictionary<string, EventTrigger>();

        public void AddRecipientTemplates(IEnumerable<RecipientTemplate> Templates)
        {
            foreach (RecipientTemplate rt in Templates)
                if (!this.RecipientTemplates.ContainsKey(rt.TemplateToken))
                    this.RecipientTemplates.Add(rt.TemplateToken, rt);
        }
        public void AddActionTemplates(IEnumerable<ActionTemplate> Templates)
        {
            foreach (ActionTemplate at in Templates)
                if (!this.ActionTemplates.ContainsKey(at.TemplateToken))
                    this.ActionTemplates.Add(at.TemplateToken, at);
        }
        public void AddEventInstances(IEnumerable<EventTrigger> Events)
        {
            foreach(EventTrigger et in Events)
                if(!this.EventInstances.ContainsKey(et.TopicExpression))
                    this.EventInstances.Add(et.TopicExpression, et);
        }

        public void resolveRecipientTemplates()
        {
            //foreach(ActionTemplate at in ActionTemplates)
            //    if(!string.IsNullOrEmpty(at.RecipientTemplate))
            //        at.recipientTemplateObj = RecipientTemplates.Single(x => x.TemplateToken == at.RecipientTemplate);
        }
    }
}

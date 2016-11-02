using ActionEventLib.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.action
{
    /// <summary>
    /// Class representing an Actionrule, base to setup an event on a device. 
    /// It contains a reference to an ActionConfiguration object that is needed to setup the event.
    /// </summary>
    public class ActionRule
    {
        public int RuleID = 0;
        public string Name;
        public bool Enabled;

        public string ActivationTimeOut = "PT0S";

        public EventTrigger Trigger;
        public List<EventTrigger> TriggerConditions = new List<EventTrigger>();

        public ActionConfiguration Configuration;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"<act:Name>" + this.Name + "</act:Name>");
            sb.Append(@"<act:Enabled>" + (this.Enabled ? "true" : "false") + "</act:Enabled>");
            
            if(this.Trigger != null)
                sb.Append(@"<act:StartEvent>" + this.Trigger.ToString() + "</act:StartEvent>");

            if(this.TriggerConditions.Count > 0)
            {
                sb.Append(@"<act:Conditions>");
                foreach (EventTrigger ev in TriggerConditions)
                    sb.Append(@"<act:Condition>" + ev.ToString() +"</act:Condition>");
                sb.Append(@"</act:Conditions>");
            }
            sb.Append(@"<act:ActivationTimeout>" + this.ActivationTimeOut + "</act:ActivationTimeout>");
            sb.Append(@"<act:PrimaryAction>" + Configuration.ConfigurationID + "</act:PrimaryAction>");
 
            return sb.ToString();
        }
    }
}

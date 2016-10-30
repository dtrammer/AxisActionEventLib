using ActionEventLib.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.action
{
    public class ActionRule
    {
        public int RuleID;
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
            sb.Append(@"<act:Enabled>" + this.Enabled + "</act:Enabled>");
            
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

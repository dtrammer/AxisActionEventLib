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
        public bool Enabled = true;

        private int _activationTimeOut = 0;
        public string ActivationTimeOut { get { return "PT" + _activationTimeOut + "S"; } private set { } }

        public EventTrigger Trigger; //StartEvent
        private List<EventTrigger> TriggerConditions = new List<EventTrigger>(); //ExtraConditions
        public ActionConfiguration Configuration = new ActionConfiguration();

        public void AddExtraCondition(EventTrigger ExtraCondition)
        {
            if (!ExtraCondition.isProperty)
                throw new Exception("SimpleEventInstanceException : A simple event instance (That is not a PropertyState) can't be used as an extra condition");

            this.TriggerConditions.Add(ExtraCondition);
        }
        public void RemoveExtraCondition(EventTrigger ExtraCondition)
        {
            this.TriggerConditions.Remove(ExtraCondition);
        }
        public void SetActivationTimeout(int Seconds)
        {
            this._activationTimeOut = Seconds;
        }

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
            sb.Append(@"<act:PrimaryAction>" + Configuration.ConfigID + "</act:PrimaryAction>");
 
            return sb.ToString();
        }
    }
}

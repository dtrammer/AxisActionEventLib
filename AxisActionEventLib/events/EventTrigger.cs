using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ActionEventLib.events
{
    public class EventTrigger
    {
        public bool isProperty { get; set; }
        public string TopicExpression { get; set; }
        public List<EventTriggerParam> Params = new List<EventTriggerParam>();

        public EventTrigger(string TopicExpression, bool IsProperty = false, string MessageContent = "") {
            this.TopicExpression = TopicExpression;
            this.isProperty = IsProperty;
            if (!string.IsNullOrEmpty(MessageContent))
                foreach (Match m in Regex.Matches(MessageContent, @"(?<=boolean.{3,3}SimpleItem.{1,1}).*?(?=\])", RegexOptions.Singleline))
                    this.Params.Add(new EventTriggerParam(Regex.Match(m.Value, @"(?<=@Name="").*?(?="")").Value, "", Regex.Match(m.Value, @"(?<=@Value="").*(?="")").Value));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<wsnt:TopicExpression Dialect=""http://docs.oasis-open.org/wsn/t-1/TopicExpression/Concrete"">" + TopicExpression + "</wsnt:TopicExpression>");

            if (Params.Count > 0)
            {
                sb.Append(@"<wsnt:MessageContent Dialect=""http://www.onvif.org/ver10/tev/messageContentFilter/ItemFilter"">");

                for(int cnt = 0; cnt < this.Params.Count; cnt++)
                {
                    sb.Append(this.Params[cnt].ToString());
                    sb.Append(" and ");
                }

                sb.Remove(sb.Length - 5, 5);

                sb.Append(@"</wsnt:MessageContent>");
            }

            return sb.ToString();
        }
    }

    public class EventTriggerParam
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string NiceName { get; set; }
        public bool OnvifElement { get; set; }

        public List<string> DefaultValues = new List<string>(); //List of possible values for this param

        public EventTriggerParam(string Name , string NiceName = "", string Value = "" , bool OnvifElement = false )
        {
            this.Name = Name;
            this.Value = Value;
            this.NiceName = NiceName;
        }

        public override string ToString()
        {
            return @"boolean(//SimpleItem[@Name=""" + this.Name + @""" and @Value=""" + this.Value + @"""])";
        }
    }
}

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
        public bool isSimple;
        public bool isCondition;
        public string TopicExpression;
        public List<EventTriggerParams> Params = new List<EventTriggerParams>();

        public void setSimpleTopic(string TopicExpression, bool isCondition = false)
        {
            this.isSimple = true;
            this.isCondition = isCondition;
            this.TopicExpression = TopicExpression;
        }
        public void setExtTopic(string TopicExpression , string MessageContent ,  bool isCondition = false)
        {
            this.isSimple = false;
            this.isCondition = isCondition;
            this.TopicExpression = TopicExpression;
            
            foreach (Match m in Regex.Matches(MessageContent , @"(?<=boolean.{3,3}SimpleItem.{1,1}).*?(?=\])",RegexOptions.Singleline))
                Params.Add(new EventTriggerParams() { name = Regex.Match(m.Value, @"(?<=@Name="").*?(?="")").Value , value = Regex.Match(m.Value, @"(?<=@Value="").*(?="")").Value });

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<wsnt:TopicExpression Dialect=""http://docs.oasis-open.org/wsn/t-1/TopicExpression/Concrete"">" + TopicExpression + "</wsnt:TopicExpression>");
            if(!this.isSimple)
            {
                sb.Append(@"<wsnt:MessageContent Dialect=""http://www.onvif.org/ver10/tev/messageContentFilter/ItemFilter"">");

                string messageContent = "";
                foreach (EventTriggerParams element in this.Params)
                {
                        messageContent += @"boolean(//SimpleItem[@Name=""" + element.name + @""" and @Value=""" + element.value + @"""]) and ";
                }
                sb.Append(messageContent.Substring(0, messageContent.Length - 5));
                sb.Append(@"</wsnt:MessageContent>");
            }

            return sb.ToString();
        }
    }

    public class EventTriggerParams
    {
        public string name;
        public bool isState;
        public List<Tuple<string,string>> defaultValues = new List<Tuple<string, string>>();
        public string value;
    }
}

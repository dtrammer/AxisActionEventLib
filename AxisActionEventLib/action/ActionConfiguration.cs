
using ActionEventLib.templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.action
{
    public class ActionConfiguration
    {
        public int ConfigurationID;
        public string Name;
        public ActionTemplate actionTemplate;

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<act:Name>" + this.Name + @"</act:Name>");
            sb.Append(actionTemplate.ToString());
            return sb.ToString();
        }
    }
}

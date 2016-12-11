
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
        public int ConfigID { get; set; }
        public string Name { get; set; }
        public string TemplateToken { get; set; }
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public ActionConfiguration(ActionTemplate Template=null)
        {
            if (Template != null)
                this.Set_From_Template(Template);
        }
        public void Set_From_Template(ActionTemplate Template)
        {
            this.TemplateToken = Template.TemplateToken;
            foreach (string s in Template.Get_Parameters())
                this.Parameters.Add(s, string.Empty);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"<act:Name>" + this.Name + @"</act:Name>");
			sb.Append(@"<act:TemplateToken>" + this.TemplateToken + @"</act:TemplateToken>");
            if(this.Parameters.Count > 0)
            {
                sb.Append("<act:Parameters>");
                foreach (KeyValuePair<string, string> kv in this.Parameters)
                    sb.Append("<act:Parameter Name=\"" + kv.Key + "\" Value=\"" + kv.Value  + @"""/>");
                sb.Append("</act:Parameters>");
            } 
            return sb.ToString();
        }
    }
}

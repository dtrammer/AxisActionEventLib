using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.action
{
    public class RecipientConfiguration
    {
        public int ConfigurationID = 0;
        public string Name;
        public string TemplateToken;
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<act:Name>" + this.Name + "</act:Name>");
            sb.Append("<act:TemplateToken>" + this.TemplateToken + "</act:TemplateToken>");

            if (this.Parameters.Count > 0)
            {
                sb.Append("<act:Parameters>");
                foreach (KeyValuePair<string, string> entry in this.Parameters)
                    sb.Append("<act:Parameter Name=\"" + entry.Key + "\" Value=\"" + entry.Value + "\"/>");
                sb.Append("</act:Parameters>");
            }

            return sb.ToString();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.templates
{
    /// <summary>
    /// Class representing an Action Template, used to setup an ActionConfiguration
    /// All the parameters fields of the templates for Axis devices so far have string as datatype
    /// </summary>
    public class ActionTemplate
    {
        public string TemplateToken;
        public string RecipientTemplate;
        public RecipientTemplate recipientTemplate;
        public Dictionary<string,string> Parameters = new Dictionary<string, string>(); //Key = Parameter Name attribute

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<act:TemplateToken>" + this.TemplateToken + "</act:TemplateToken>");
            sb.Append("<act:Parameters>");
            foreach (KeyValuePair<string, string> entry in this.Parameters)
                sb.Append("<act:Parameter Name=\"" + entry.Key + "\" Value=\"" + entry.Value + "\"/>");

            if (recipientTemplate != null)
                sb.Append(recipientTemplate.ToString());

            sb.Append("</act:Parameters>");

            return sb.ToString();
        }
    }
}

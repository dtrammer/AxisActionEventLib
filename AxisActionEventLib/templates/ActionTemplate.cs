using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.templates
{
    /// <summary>
    /// Class representing an Action Template, used to setup an ActionConfiguration
    /// </summary>
    public class ActionTemplate
    {
        private string _templateToken;
        public string TemplateToken
        {
            get { return _templateToken; }
            set {
                    _templateToken = value;
                    if (_templateToken.Contains("unlimited"))
                        this.IsUnlimited = true;
                }
        }

        public string RecipientTemplate;
        public RecipientTemplate recipientTemplateObj;
        //All the parameters fields of the templates for Axis devices so far have string as datatype
        public Dictionary<string,string> Parameters = new Dictionary<string, string>();
        public bool IsUnlimited = false;

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<act:TemplateToken>" + this.TemplateToken + "</act:TemplateToken>");
            sb.Append("<act:Parameters>");
            foreach (KeyValuePair<string, string> entry in this.Parameters)
                sb.Append("<act:Parameter Name=\"" + entry.Key + "\" Value=\"" + entry.Value + "\"/>");

            if (recipientTemplateObj != null)
                sb.Append(recipientTemplateObj.ToString());

            sb.Append("</act:Parameters>");

            return sb.ToString();
        }
    }
}

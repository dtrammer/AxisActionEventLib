using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActionEventLib.templates
{
    /// <summary>
    /// Class representing a Recipient template used to setup an ActionConfiguration
    /// All the parameters fields of the templates for Axis devices so far have string as datatype
    /// </summary>
    public class RecipientTemplate
    {
        public string TemplateToken;
        public Dictionary<string, string> Parameters = new Dictionary<string, string>(); 

        public List<string> Get_Parameters()
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, string> kv in this.Parameters)
                result.Add(kv.Key);
            return result;
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> entry in this.Parameters)
            {
                sb.Append("<act:Parameter Name=\"" + entry.Key + "\" Value=\"" + entry.Value + "\"/>");
            }
            return sb.ToString();
        }
    }
}

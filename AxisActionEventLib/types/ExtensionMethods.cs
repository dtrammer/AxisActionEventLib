using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ActionEventLib.types
{
    public static class ExtensionMethods
    {
        #region System.Linq.XElement extension methods
        public static bool HasElement(this XElement Element,  string ElementName)
        {
            if (Element.Element(ElementName) != null)
                return true;
            else
                return false;
        }
        public static string GetElementValue(this XElement Element , string ElementName)
        {
            if (Element.HasElement(ElementName))
                return Element.Element(ElementName).Value;
            else
                return string.Empty;
        }
        public static bool HasAttribute(this XElement Element , string AttributeName)
        {
            if (Element.Attribute(AttributeName) != null)
                return true;
            else
                return false;
        }
        public static string GetAttributeValue(this XElement Element, string AttributeName)
        {
            if (Element.HasAttribute(AttributeName))
                return Element.Attribute(AttributeName).Value;
            else
                return string.Empty;
        }
        public static string GetNameSpacePrefix(this XElement Element)
        {
            if (Element.Name.Namespace != string.Empty)
            {
                string prefix = Element.GetPrefixOfNamespace(Element.Name.Namespace);
                if (prefix != string.Empty && prefix != "")
                    return prefix + ":";
                else
                    return string.Empty;
            }else
                return string.Empty;
        }
        #endregion
    }
}

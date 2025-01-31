﻿using System.Xml.Linq;

namespace TRX_Merger.Utilities
{
    public static class ExtensionMethods
    {
        public static void SetDefaultXmlNamespace(this XElement xelem, XNamespace xmlns)
        {
            if (xelem.Name.NamespaceName == string.Empty)
                xelem.Name = xmlns + xelem.Name.LocalName;
            foreach (var e in xelem.Elements())
                e.SetDefaultXmlNamespace(xmlns);
        }

        public static string GetAttributeValue(this XElement elem, string attribute)
        {
            return elem.Attribute(attribute)?.Value;
        }
    }
}

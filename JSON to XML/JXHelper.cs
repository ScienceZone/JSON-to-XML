using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JSON_to_XML
{
    static class JXHelper
    {
        /// <summary>
        /// Converts xmlDocument to plain text.
        /// </summary>
        /// <param name="xmlDocument">An XmlDocument to be converted.</param>
        /// <returns>Text representation of xmlDocument.</returns>
        public static string ToText(this XmlDocument xmlDocument)
        {
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb, Parser.XmlSettings))
            {
                xmlDocument.WriteTo(xmlWriter);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Reads contents of the specified JSON file.
        /// </summary>
        /// <param name="fileName">Name of an input JSON file.</param>
        /// <returns>Text representation of a JSON file.</returns>
        public static string ReadJSONFromFile(string fileName)
        {
            using (StreamReader jsonReader = new StreamReader(fileName))
            {
                Parser.XmlSettings.Encoding = jsonReader.CurrentEncoding;
                return jsonReader.ReadToEnd();
            }
        }
    }
}

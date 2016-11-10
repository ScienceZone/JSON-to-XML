using System;
using System.Collections.Generic;
using System.Windows;
using System.IO;
using System.Text;
using System.Xml;

namespace JSON_to_XML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static XmlWriter xmlWriter;

        static XmlWriterSettings xmlSettings;

        static string json;
        static public string Json { get { return json; } }
        
        /// <summary>
        /// Gets the text representation of the current XML document.
        /// </summary>
        static public string Xml
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                using (xmlWriter = XmlWriter.Create(sb, xmlSettings))
                {
                    xmlDoc.WriteTo(xmlWriter);
                }
                return sb.ToString();
            }
        }

        static public String RootElementName { get; set; }
        
        //for future use
        static bool SpaceIndentation { get; set; } = false;
        static int spaceCount = 4;
        static int SpaceCount
        {
            get { return spaceCount; }
            set { spaceCount = value; }
        }
        
        static XmlDocument xmlDoc;

        static App()
        {
            json = string.Empty;

            xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = "\t";

            RootElementName = "root";
        }

        /// <summary>
        /// Reads contents of the specified JSON file.
        /// </summary>
        /// <param name="fileName">Name of an input JSON file</param>
        static public void ReadJSONFromFile(string fileName)
        {
            using (StreamReader jsonReader = new StreamReader(fileName))
            {
                json = jsonReader.ReadToEnd();
                xmlSettings.Encoding = jsonReader.CurrentEncoding;                
            }
        }

        /// <summary>
        /// Parses current JSON file into an XML document.
        /// </summary>
        static public void Parse()
        {
            xmlDoc = Parser.JsonToXml(json);
        }       

        /// <summary>
        /// Writes resulting XML to a file with the specified name.
        /// </summary>
        /// <param name="fileName">Name of an output XML file</param>
        public static void WriteXMLtoFile(string fileName)
        {
            using (xmlWriter = XmlWriter.Create(fileName, xmlSettings))
            {
                xmlDoc.WriteTo(xmlWriter);
            }
        }
    }
}

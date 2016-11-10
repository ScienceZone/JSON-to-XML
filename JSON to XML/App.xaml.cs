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

        static public void ReadJSONFromFile(string fileName)
        {
            using (StreamReader jsonReader = new StreamReader(fileName))
            {
                json = jsonReader.ReadToEnd();
                xmlSettings.Encoding = jsonReader.CurrentEncoding;                
            }
        }

        //Parsing the whole file
        static public void Parse()
        {
            xmlDoc = new XmlDocument();                                    
            json.Trim();            
            XmlNode root = xmlDoc.CreateElement(RootElementName);            

            if (json[0] == '{')
                ParseObject(json, root);
            if (json[0] == '[')
                ParseArray(json, root);

            xmlDoc.AppendChild(root);
        }

        ///<summary>
        ///A method for parsing a JSON object.
        ///Assuming the string starts with '{' and ends with '}'
        ///</summary>
        static int ParseObject(string str, XmlNode currentNode)
        {
            str = str.Substring(1, str.Length - 2).Trim();
            int curlyBrackets = 0, brackets = 0, lastPairStart = 0;
            bool areQuotesOk = true;

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case ',':
                        if (curlyBrackets == 0 && brackets == 0 && areQuotesOk)
                        {
                            ParsePair(str.Substring(lastPairStart, i - lastPairStart).Trim(), currentNode);
                            lastPairStart = i + 1;
                        }
                        break;
                    case '[':
                        brackets++;
                        break;
                    case '{':
                        curlyBrackets++;
                        break;
                    case ']':
                        brackets--;
                        break;
                    case '}':
                        curlyBrackets--;
                        break;
                    case '\"':
                        areQuotesOk = !areQuotesOk;
                        break;
                    default:
                        break;
                }
            }

            ParsePair(str.Substring(lastPairStart, str.Length - lastPairStart).Trim(), currentNode);
            
            return 0;
        }

        /// <summary>
        /// A method for parsing a JSON array.
        /// </summary>
        /// <param name="str">A string that represents a JSON array.
        /// Assuming that it starts with '[' and ends with ']'. </param>
        /// <param name="parent"></param>
        /// <returns></returns>
        //
        //It almost duplicates the ParseObject method, gotta think how to get rid of that
        static int ParseArray(string str, XmlNode parent)
        {
            int curlyBrackets = 0, brackets = 0, lastValueStart = 0;
            bool areQuotesOk = true;            
            
            str = str.Substring(1, str.Length - 2).Trim();

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case ',':
                        if (curlyBrackets == 0 && brackets == 0 && areQuotesOk)
                        {
                            XmlNode node = xmlDoc.CreateElement("element");

                            //parsing an array element's value
                            ParseValue(str.Substring(lastValueStart, i - lastValueStart).Trim(), node);
                            lastValueStart = i + 1;

                            parent.AppendChild(node);
                        }
                        break;
                    case '[':
                        brackets++;
                        break;
                    case '{':
                        curlyBrackets++;
                        break;
                    case ']':
                        brackets--;
                        break;
                    case '}':
                        curlyBrackets--;
                        break;
                    case '\"':
                        areQuotesOk = !areQuotesOk;
                        break;
                    default:
                        break;
                }
            }

            XmlNode lastNode = xmlDoc.CreateElement("element");

            //parsing an array element's value
            ParseValue(str.Substring(lastValueStart, str.Length - lastValueStart).Trim(), lastNode);

            parent.AppendChild(lastNode);

            return 0;
        }

        //A method for parsing name:value pair inside an object
        static int ParsePair(string str, XmlNode parentNode)
        {
            //what if there's a colon inside an object's key name?
            int colonIndex = str.IndexOf(':');

            XmlNode node = xmlDoc.CreateElement(str.Substring(0, colonIndex).Trim().Trim('\"')); //derp

            //parsing the element value
            ParseValue(str.Substring(colonIndex + 1).Trim(), node);

            parentNode.AppendChild(node);   //derp

            return 0;
        }

        //A method for parsing values of name:value pairs and array elements
        static int ParseValue(string str, XmlNode currentNode)
        {
            //less lines than if i used a switch block
            if (str[0] != '{' && str[0] != '[')
            {
                currentNode.InnerText = str.Trim().Trim('\"');
            }
            else
            {
                if (str[0] == '{')
                    ParseObject(str, currentNode);
                else
                    ParseArray(str, currentNode);
            }

            return 0;
        }

        public static void WriteXMLtoFile(string fileName)
        {
            using (xmlWriter = XmlWriter.Create(fileName, xmlSettings))
            {
                xmlDoc.WriteTo(xmlWriter);
            }
        }
    }
}

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
        static int ParseObject(string jsonObject, XmlNode currentNode)
        {
            jsonObject = jsonObject.Substring(1, jsonObject.Length - 2).Trim();
            int curlyBrackets = 0, brackets = 0, lastPairStart = 0;
            bool areQuotesOk = true;

            for (int i = 0; i < jsonObject.Length; i++)
            {
                switch (jsonObject[i])
                {
                    case ',':
                        if (curlyBrackets == 0 && brackets == 0 && areQuotesOk)
                        {
                            ParsePair(jsonObject.Substring(lastPairStart, i - lastPairStart).Trim(), currentNode);
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

            ParsePair(jsonObject.Substring(lastPairStart, jsonObject.Length - lastPairStart).Trim(), currentNode);
            
            return 0;
        }

        /// <summary>
        /// A method for parsing a JSON array.
        /// </summary>
        /// <param name="jsonArray">A string that represents a JSON array.
        /// Assuming that it starts with '[' and ends with ']'. </param>
        /// <param name="parent"></param>
        /// <returns></returns>
        //
        //It almost duplicates the ParseObject method, gotta think how to get rid of that
        static int ParseArray(string jsonArray, XmlNode parent)
        {
            int curlyBrackets = 0, brackets = 0, lastValueStart = 0;
            bool areQuotesOk = true;            
            
            jsonArray = jsonArray.Substring(1, jsonArray.Length - 2).Trim();

            for (int i = 0; i < jsonArray.Length; i++)
            {
                switch (jsonArray[i])
                {
                    case ',':
                        if (curlyBrackets == 0 && brackets == 0 && areQuotesOk)
                        {
                            XmlNode node = xmlDoc.CreateElement("element");                            
                            ParseValue(jsonArray.Substring(lastValueStart, i - lastValueStart).Trim(), node);
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
            ParseValue(jsonArray.Substring(lastValueStart, jsonArray.Length - lastValueStart).Trim(), lastNode);
            parent.AppendChild(lastNode);

            return 0;
        }

        //A method for parsing name:value pair inside an object
        static int ParsePair(string jsonPair, XmlNode parentNode)
        {
            //bool areQuotesOk = true;
            ////what if there's a colon inside an object's key name?
            //int colonIndex = 0;
            //while (colonIndex < str.Length - 1 && str[colonIndex] != ':' && areQuotesOk)
            //{
            //    colonIndex++;
            //}
            //if (colonIndex == str.Length - 1)
            //    throw new ArgumentException("Invalid string representation of JSON pair", "str");

            int colonIndex = jsonPair.IndexOf(':');
            XmlNode node = xmlDoc.CreateElement(jsonPair.Substring(0, colonIndex).Trim().Trim('\"'));
            ParseValue(jsonPair.Substring(colonIndex + 1).Trim(), node);
            parentNode.AppendChild(node);

            return 0;
        }

        //A method for parsing values of name:value pairs and array elements
        static int ParseValue(string jsonValue, XmlNode currentNode)
        {
            //less lines than if i used a switch block
            if (jsonValue[0] != '{' && jsonValue[0] != '[')
            {
                currentNode.InnerText = jsonValue.Trim().Trim('\"');
            }
            else
            {
                if (jsonValue[0] == '{')
                    ParseObject(jsonValue, currentNode);
                else
                    ParseArray(jsonValue, currentNode);
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

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
            xmlDoc = new XmlDocument();                                    
            json.Trim();            
            XmlNode root = xmlDoc.CreateElement(RootElementName);

            try
            {
                if (json[0] == '{')
                    ParseObject(json, root);
                if (json[0] == '[')
                    ParseArray(json, root);
            }
            catch (ArgumentException e)
            {
                throw e;
            }

            xmlDoc.AppendChild(root);
        }

        ///<summary>
        ///Parses a JSON object.
        ///Assuming the string starts with '{' and ends with '}'
        ///</summary>
        static void ParseObject(string jsonObject, XmlNode currentNode)
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
                            try
                            {
                                ParsePair(jsonObject.Substring(lastPairStart, i - lastPairStart).Trim(), currentNode);
                            }
                            catch (ArgumentException e)
                            {
                                throw e;
                            }
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

            try
            {
                ParsePair(jsonObject.Substring(lastPairStart, jsonObject.Length - lastPairStart).Trim(), currentNode);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Parses a JSON array.
        /// </summary>
        /// <param name="jsonArray">A string that represents a JSON array.
        /// Assuming that it starts with '[' and ends with ']'. </param>
        /// <param name="parent"></param>
        /// <returns></returns>
        //It almost duplicates the ParseObject method, gotta think how to get rid of that
        static void ParseArray(string jsonArray, XmlNode parent)
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
                            try
                            {
                                ParseValue(jsonArray.Substring(lastValueStart, i - lastValueStart).Trim(), node);
                            }
                            catch (ArgumentException e)
                            {
                                throw e;
                            }
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
            try
            {
                ParseValue(jsonArray.Substring(lastValueStart, jsonArray.Length - lastValueStart).Trim(), lastNode);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            parent.AppendChild(lastNode);
        }

        ///<summary>
        ///Parses a name:value pair within a JSON object.
        ///</summary>
        static void ParsePair(string jsonPair, XmlNode parentNode)
        {
            bool areQuotesOk = true;
            //what if there's a colon inside an object's key name?
            int colonIndex = 0;
            while (colonIndex < jsonPair.Length - 1 && jsonPair[colonIndex] != ':' && areQuotesOk)
            {
                if (jsonPair[colonIndex] == '"')
                    areQuotesOk = !areQuotesOk;
                colonIndex++;
            }
            
            if(colonIndex == jsonPair.Length - 1)
                throw new ArgumentException(string.Format("Pair: [{0}] has no value.", jsonPair), "str");

            string name = jsonPair.Substring(0, colonIndex).Trim().Trim('\"');
            if (name.Length == 0)
                throw new ArgumentException(string.Format("Pair [{0}] has no name.", jsonPair), "str");
            XmlNode node = xmlDoc.CreateElement(name);
            try
            {
                ParseValue(jsonPair.Substring(colonIndex + 1).Trim(), node);
            }
            catch (ArgumentException e)
            {
                throw e;
            }
            parentNode.AppendChild(node);
        }

        ///<summary>
        ///Parses the value of a JSON name:value pair or a JSON array element.
        ///</summary>
        static void ParseValue(string jsonValue, XmlNode currentNode)
        {
            //less lines than if i used a switch block
            if (jsonValue[0] != '{' && jsonValue[0] != '[')
            {
                currentNode.InnerText = jsonValue.Trim().Trim('\"');
            }
            else
            {
                try
                {
                    if (jsonValue[0] == '{')
                        ParseObject(jsonValue, currentNode);
                    else
                        ParseArray(jsonValue, currentNode);
                }
                catch (ArgumentException e)
                {
                    throw e;
                }
            }
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

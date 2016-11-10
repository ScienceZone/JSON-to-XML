using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JSON_to_XML
{
    class Parser
    {
        public static XmlWriterSettings XmlSettings
        { get; set; }

        public static string RootElementName { get; set; }
        
        static Parser()
        {
            XmlSettings = new XmlWriterSettings();
            XmlSettings.Indent = true;
            XmlSettings.IndentChars = "\t";

            RootElementName = "root";
        }

        public static XmlDocument JsonToXml(string json)
        {
            XmlDocument xml = new XmlDocument();
            XmlNode root = xml.CreateElement(RootElementName);

            json = json.Trim();
            
            if (json[0] == '{')
                ParseObject(json, root);
            if (json[0] == '[')
                ParseArray(json, root);

            xml.AppendChild(root);
            return xml;
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
                            XmlNode node = parent.OwnerDocument.CreateElement("element");
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

            XmlNode lastNode = parent.OwnerDocument.CreateElement("element");
            ParseValue(jsonArray.Substring(lastValueStart, jsonArray.Length - lastValueStart).Trim(), lastNode);
        }

        ///<summary>
        ///Parses a name:value pair within a JSON object.
        ///</summary>
        static void ParsePair(string jsonPair, XmlNode parentNode)
        {
            bool areQuotesOk = true;
            int colonIndex = 0;
            while (colonIndex < jsonPair.Length - 1 && !(jsonPair[colonIndex] == ':' && areQuotesOk))
            {
                if (jsonPair[colonIndex] == '"')
                    areQuotesOk = !areQuotesOk;
                colonIndex++;
            }

            if (colonIndex == jsonPair.Length - 1)
                throw new ArgumentException(string.Format("Pair: [{0}] has no value.", jsonPair), "str");

            string name = jsonPair.Substring(0, colonIndex).Trim().Trim('\"');
            if (name.Length == 0)
                throw new ArgumentException(string.Format("Pair [{0}] has no name.", jsonPair), "str");
            XmlNode node = parentNode.OwnerDocument.CreateElement(name);
            ParseValue(jsonPair.Substring(colonIndex + 1).Trim(), node);
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
                if (jsonValue[0] == '{')
                    ParseObject(jsonValue, currentNode);
                else
                    ParseArray(jsonValue, currentNode);
            }
        }
    }
}

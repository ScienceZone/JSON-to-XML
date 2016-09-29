using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Text;

namespace JSON_to_XML
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static StreamWriter XMLWriter
        { get; set; }

        static StreamReader JSONReader
        { get; set; }

        static string json = string.Empty;
        static public string JSON { get { return json; } }

        static StringBuilder xml;
        static public string XML { get { return xml.ToString(); } }
        
        static int tabCount = 0;
        //this is needed for elements' correct closing tags
        static Stack<string> xmlNodeStack;
        static string jsonEncoding;

        static public void ReadJSONFromFile(string fileName)
        {
            using (JSONReader = new StreamReader(fileName))
            {
                json = JSONReader.ReadToEnd();
                jsonEncoding = JSONReader.CurrentEncoding.BodyName;
            }
        }

        //Parsing the whole file
        static public void Parse()
        {
            xml = new StringBuilder();
            xml.Clear();

            xmlNodeStack = new Stack<string>();
            tabCount = 0;

            xml.AppendFormat("<?xml version=\"1.0\" encoding=\"{0}\"?>\n", jsonEncoding);

            json.Trim();

            xml.AppendLine("<root>");
            xmlNodeStack.Push("root");

            if (json[0] == '{')
                ParseObject(json, xml);
            if (json[0] == '[')
                ParseArray(json, xml);

            xml.AppendLine("</root>");
        }

        //A method for parsing a JSON object
        //Assuming the string starts with '{' and ends with '}'
        static int ParseObject(string str, StringBuilder destination)
        {
            tabCount++;
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
                            ParsePair(str.Substring(lastPairStart, i - lastPairStart).Trim(), destination);
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

            ParsePair(str.Substring(lastPairStart, str.Length - lastPairStart).Trim(), destination);

            tabCount--;
            return 0;
        }

        //A method for parsing a JSON object array
        //Assuming the string starts with '[' and ends with ']'
        //It almost duplicates the ParseObject method, gotta think how to get rid of that
        static int ParseArray(string str, StringBuilder destination)
        {
            int curlyBrackets = 0, brackets = 0, lastValueStart = 0;
            bool areQuotesOk = true;            

            tabCount++;
            str = str.Substring(1, str.Length - 2).Trim();

            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case ',':
                        if (curlyBrackets == 0 && brackets == 0 && areQuotesOk)
                        {
                            ApplyTabs(destination);
                            //element's opening tag
                            destination.AppendFormat("<element>");
                            //parsing an array element's value
                            ParseValue(str.Substring(lastValueStart, i - lastValueStart).Trim(), destination);
                            lastValueStart = i + 1;
                            //element's closing tag
                            destination.AppendFormat("</element>\n");
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

            ApplyTabs(destination);
            //element's opening tag
            destination.AppendFormat("<element>");
            //parsing an array element's value
            ParseValue(str.Substring(lastValueStart, str.Length - lastValueStart).Trim(), destination);
            //element's closing tag
            destination.AppendFormat("</element>\n");

            tabCount--;
            return 0;
        }

        //A method for parsing name:value pair inside an object
        static int ParsePair(string str, StringBuilder destination)
        {
            int colonIndex = str.IndexOf(':');

            //writing the opening tag
            string name = str.Substring(0, colonIndex).Trim().Trim('\"');
            ApplyTabs(destination);
            destination.AppendFormat("<{0}>", name);
            xmlNodeStack.Push(name);

            //parsing the element value
            ParseValue(str.Substring(colonIndex + 1).Trim(), destination);
            //writing the closing tag
            destination.AppendFormat("</{0}>\n", xmlNodeStack.Pop());

            return 0;
        }

        //A method for parsing values of name:value pairs and array elements
        static int ParseValue(string str, StringBuilder destination)
        {
            //less lines than if i used a switch block
            if (str[0] != '{' && str[0] != '[')
                destination.Append(str.Trim().Trim('\"'));
            else
            {
                destination.AppendLine();
                //tabCount++;
                //ApplyTabs(destination);

                if (str[0] == '{')
                    ParseObject(str, destination);
                else
                    ParseArray(str, destination);
                //destination.AppendLine();

                //tabCount--;
                ApplyTabs(destination);
            }

            return 0;
        }

        static void ApplyTabs(StringBuilder destination)
        {
            for (int i = 0; i < tabCount; i++)
                destination.Append("\t");
        }

        public static void WriteXMLtoFile(string fileName)
        {
            XMLWriter = new StreamWriter(fileName);
            XMLWriter.Write(xml.ToString());
            XMLWriter.Close();
        }
    }
}

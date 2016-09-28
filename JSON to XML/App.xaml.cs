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

        static string json;
        static StringBuilder xml;

        //-1 because it gets incremented before we start any writing
        static int tabCount = -1;
        //this is needed for elements' correct closing tags
        static Stack<string> fileObjects = new Stack<string>();

        //Parsing the whole file
        static public void Parse(string fileName)
        {
            JSONReader = new StreamReader(fileName);
            json = JSONReader.ReadToEnd();
            JSONReader.Close();

            xml = new StringBuilder();
            xml.Clear();

            json.Trim();
            if (json[0] == '{')
                ParseObject(json, xml);
            if (json[0] == '[')
                ParseArray(json, xml);
                        
            //xml.Append(json);

            //while(i < json.Length)
            //{
            //    json = json.TrimStart();
            //}
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
            tabCount++;
            str = str.Substring(1, str.Length - 2).Trim();
            int curlyBrackets = 0, brackets = 0, lastValueStart = 0;
            bool areQuotesOk = true;

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
            fileObjects.Push(name);

            //parsing the element value
            ParseValue(str.Substring(colonIndex + 1).Trim(), destination);
            //writing the closing tag
            destination.AppendFormat("</{0}>\n", fileObjects.Pop());

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

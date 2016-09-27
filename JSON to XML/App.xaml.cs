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
        static int tabCount = 0;
        static Stack<string> fileObjects = new Stack<string>();
        //this is needed to know when we should write a closing tag
        static bool isValue = false;

        //Parsing the whole file
        static public void Parse(string fileName)
        {
            int i = 0;

            JSONReader = new StreamReader(fileName);
            json = JSONReader.ReadToEnd();
            JSONReader.Close();

            xml = new StringBuilder();
            xml.Clear();
            xml.Append(json);

            //while(i < json.Length)
            //{
            //    json = json.TrimStart();
            //}
        }

        //A method for parsing a JSON object
        //Assuming the string starts with '{' and ends with '}'
        //Should probably add object stack to save object names for handling closing tags
        static int ParseObject(string str, StringBuilder destination)
        {
            str = str.Substring(1, str.Length - 1).Trim();
            string[] pairs = str.Split(',');

            foreach (string pair in pairs)
            {
                ParsePair(pair.Trim(), destination);
                //Must add writing into destination here
            }

            return 0;
        }

        //A method for parsing a JSON object array
        //Assuming the string starts with '[' and ends with ']'
        static int ParseArray(string str, StringBuilder destination)
        {
            str = str.Substring(1, str.Length - 1).Trim();
            string[] elements = str.Split(',');

            foreach (string element in elements)
            {
                ParseValue(element.Trim(), destination);
                //Must add writing into destination here
            }

            return 0;
        }

        //A method for parsing name:value pair inside an object
        static int ParsePair(string str, StringBuilder destination)
        {
            int colonIndex = str.IndexOf(':');

            //writing the element name into destination
            string name = str.Substring(0, colonIndex).Trim();
            destination.AppendFormat("<{0}>", name);
            fileObjects.Push(name);

            //parsing the element value
            ParseValue(str.Substring(colonIndex + 1).Trim(), destination);

            return 0;
        }

        //A method for parsing values of name:value pairs and array elements
        static int ParseValue(string str, StringBuilder destination)
        {
            //less lines than if i used a switch block
            if (str[0] != '{' && str[0] != '[')
                destination.Append(str);
            else
            {
                destination.AppendLine();
                tabCount++;
                ApplyTabs(destination);

                if (str[0] == '{')
                    ParseObject(str, destination);
                else
                    ParseArray(str, destination);
                destination.AppendLine();

                tabCount--;
                ApplyTabs(destination);
            }

            destination.AppendFormat("</{0}>\n", fileObjects.Pop());

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

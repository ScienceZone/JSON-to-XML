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
        static public StreamWriter XMLWriter
        { get; set; }

        static public StreamReader JSONReader
        { get; set; }

        static string json;
        static StringBuilder xml;
        static int tabCount = 0;

        //Parsing the whole file
        static public void Parse(string fileName)
        {
            int i = 0;

            JSONReader = new StreamReader(fileName);
            json = JSONReader.ReadToEnd();
            JSONReader.Close();

            xml = new StringBuilder();
            xml.Clear();

            while(i < json.Length)
            {
                json = json.TrimStart();
            }
        }

        //Parsing stuff starting with '{'
        //Should probably add object stack to save object names for handling closing tags
        static int ParseObject(string str, int first, StringBuilder destination)
        {
            destination.Append("<");
            int i = first + 1;
            bool exit = false;
            while (!exit)
            {
                switch(str[i])
                {
                    case '[':
                        ParseArray(str, i, destination);
                        break;
                    case '\"':
                    case '\'':
                        ParseQuote(str, i, destination);
                        break;
                    case ':':
                        destination.Append(">\n\t");
                        tabCount++;
                        break;
                    default:
                        break;
                }
            }
            return first + 1;
        }

        //Parsing stuff starting with '['
        static int ParseArray(string str, int first, StringBuilder destination)
        {
            return first + 1;
        }

        //Parsing stuff starting with quotation marks, either single or double
        static int ParseQuote(string str, int first, StringBuilder destination)
        {
            int t = 1;
            do
            {
                t = str.IndexOf(str[first], first + 1);
            } while (str[t - 1] != '\\');

            destination.Append(str.Substring(first + 1, t - first - 1));
            
            return t;
        }
    }
}

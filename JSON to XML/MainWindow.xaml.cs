using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;

namespace JSON_to_XML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OpenFileDialog openJsonDlg;
        SaveFileDialog saveXmlDlg;
        //a flag for checking if we opened a new JSON file
        bool isJsonChanged;

        public MainWindow()
        {
            InitializeComponent();

            openJsonDlg = new OpenFileDialog();

            openJsonDlg.DefaultExt = ".json";
            openJsonDlg.Filter = "JSON files(*.json)|*.json";
            openJsonDlg.CheckFileExists = true;
            openJsonDlg.CheckPathExists = true;
            openJsonDlg.Multiselect = false;
            openJsonDlg.Title = "Select a JSON file...";

            saveXmlDlg = new SaveFileDialog();

            saveXmlDlg.DefaultExt = ".xml";
            saveXmlDlg.Filter = "XML files(*.xml)|*.xml";
            saveXmlDlg.AddExtension = true;
            saveXmlDlg.CreatePrompt = true;
            saveXmlDlg.OverwritePrompt = true;
            saveXmlDlg.Title = "Choose XML file destination...";


            jsonTextBox.Text = String.Empty;
            //will probably change this in future
            jsonTextBox.IsReadOnly = true;
            jsonTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            jsonTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            xmlTextBox.Text = String.Empty;
            xmlTextBox.IsReadOnly = true;
            xmlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            xmlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            
            isJsonChanged = false;
            
        }

        private void OpenJsonMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (openJsonDlg.ShowDialog() == true)
            {
                App.ReadJSONFromFile(openJsonDlg.FileName);
                jsonTextBox.Text = App.JSON;
                isJsonChanged = true;
                statusTextBlock.Text = openJsonDlg.SafeFileName + " was successfully opened";
            }
            else
                statusTextBlock.Text = string.Empty;
        }

        private void SaveXmlMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (xmlTextBox.Text.CompareTo(string.Empty) == 0)
                statusTextBlock.Text = "There's nothing to save";
            else if (saveXmlDlg.ShowDialog() == true)
            {
                App.WriteXMLtoFile(saveXmlDlg.FileName);
                statusTextBlock.Text = "XML document was succesfully saved into " + saveXmlDlg.SafeFileName;
            }
        }

        private void ParseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            statusTextBlock.Text = string.Empty;
            if (App.JSON.Length == 0)
                statusTextBlock.Text = "There's nothing to parse";
            //no need to parse the old JSON again
            else if (isJsonChanged)
            {
                App.Parse();
                xmlTextBox.Text = App.XML;
                isJsonChanged = false;
                statusTextBlock.Text = openJsonDlg.SafeFileName + " was succesfully parsed into an XML document";
            }
            else
                statusTextBlock.Text = "Open a new JSON file";
        }
    }
}

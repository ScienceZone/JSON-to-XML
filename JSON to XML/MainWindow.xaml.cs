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

        public MainWindow()
        {
            InitializeComponent();

            openJsonDlg = new OpenFileDialog();

            openJsonDlg.DefaultExt = ".json";
            openJsonDlg.Filter = "JSON files(*.json)|*.json";
            openJsonDlg.CheckFileExists = true;
            openJsonDlg.CheckPathExists = true;
            openJsonDlg.Multiselect = false;

            saveXmlDlg = new SaveFileDialog();

            saveXmlDlg.DefaultExt = ".xml";
            saveXmlDlg.Filter = "XML files(*.xml)|*.xml";
            saveXmlDlg.AddExtension = true;
            saveXmlDlg.CreatePrompt = true;
            saveXmlDlg.OverwritePrompt = true;


            jsonTextBox.Text = String.Empty;
            //will probably change this in future
            jsonTextBox.IsReadOnly = true;
            jsonTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            jsonTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            xmlTextBox.Text = String.Empty;
            xmlTextBox.IsReadOnly = true;
            xmlTextBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            xmlTextBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        }

        private void openJsonButton_Click(object sender, RoutedEventArgs e)
        {
            if (openJsonDlg.ShowDialog() == true)
            {
                App.ReadJSONFromFile(openJsonDlg.FileName);
                jsonTextBox.Text = App.JSON;
                //App.Parse(openJsonDlg.FileName);
            }
        }

        private void saveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            saveXmlDlg.ShowDialog();
            App.WriteXMLtoFile(saveXmlDlg.FileName);
        }

        private void parseButton_Click(object sender, RoutedEventArgs e)
        {
            App.Parse();
            xmlTextBox.Text = App.XML;
        }
    }
}

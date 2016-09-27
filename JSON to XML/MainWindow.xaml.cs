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
        }

        private void openJsonButton_Click(object sender, RoutedEventArgs e)
        {
            if (openJsonDlg.ShowDialog() == true)
                App.Parse(openJsonDlg.FileName);
        }

        private void saveXmlButton_Click(object sender, RoutedEventArgs e)
        {
            saveXmlDlg.ShowDialog();
            App.WriteXMLtoFile(saveXmlDlg.FileName);
        }
    }
}

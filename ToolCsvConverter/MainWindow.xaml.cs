using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

namespace ToolCsvConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => this.InitializeComponent();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "csv file (*.csv; *.CSV) | *.csv; *.CSV";

            if (ofd.ShowDialog() == true)
            {
                if (!this.ConvertFile(ofd.FileName))
                {
                    MessageBox.Show("Can not convert file. Check csv format. Column delimiter must be semicolon.");
                    return;
                }

                MessageBox.Show("Convert success");
            }
        }
        
        private bool ConvertFile(string filename)
        {
            StringBuilder convertedResult = new StringBuilder();
            using(StreamReader sr = new StreamReader(filename))
            {
                string tmp = string.Empty;
                while ((tmp = sr.ReadLine()) != null)
                {
                    StringBuilder sb = new StringBuilder();
                    string[] columns = tmp.Split(';');
                    foreach (var columnCell in columns)
                    {
                        if(double.TryParse(columnCell, NumberStyles.Number, CultureInfo.GetCultureInfo("en"), out double decimalResult))
                        {
                            string str = string.Format(CultureInfo.GetCultureInfo("de-DE"), "{0:0.000}", decimalResult) + ";";
                            sb.Append(str);
                            continue;
                        }

                        sb.Append(columnCell + ";");
                    }
                    convertedResult.AppendLine(sb.ToString());
                }
            }

            int fileNumber = 1;
            bool fileNotExist = false;
            string withoutExt = filename.Remove(filename.Length - 4);
            string createNewName = $"{withoutExt}_converted ({fileNumber}).csv";
            while (!fileNotExist)
            {
                createNewName = $"{withoutExt}_converted ({fileNumber}).csv";

                if (!File.Exists(createNewName))
                {
                    break;
                }

                fileNumber++;
            }

            using(StreamWriter sw = new StreamWriter(createNewName))
            {
                sw.Write(convertedResult.ToString());
            }
            
            return true;
        }
    }
}

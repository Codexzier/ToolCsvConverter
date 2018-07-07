using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace ToolCsvConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() => this.InitializeComponent();

        /// <summary>
        /// Open the File dialog to choose a cs file to converte the decimal delimiter.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        /// <summary>
        /// Process of reading csv file, convert number format and save to a new file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns>Nothing</returns>
        private bool ConvertFile(string filename)
        {
            StringBuilder convertedResult = this.ReadCsvFile(filename);

            string createNewName = this.CreateNewFilename(filename);

            using(StreamWriter sw = new StreamWriter(createNewName))
            {
                sw.Write(convertedResult.ToString());
            }
            
            return true;
        }

        /// <summary>
        /// Read the csv file and convert all cell have a number with delimiter point.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private StringBuilder ReadCsvFile(string filename)
        {
            StringBuilder convertedResult = new StringBuilder();
            using (StreamReader sr = new StreamReader(filename))
            {
                string tmp = string.Empty;
                while ((tmp = sr.ReadLine()) != null)
                {
                    StringBuilder sb = new StringBuilder();
                    string[] columns = tmp.Split(';');
                    foreach (var columnCell in columns)
                    {
                        sb.Append(this.GetColumnResult(columnCell) + ";");
                    }
                    convertedResult.AppendLine(sb.ToString());
                }
            }

            return convertedResult;
        }

        /// <summary>
        /// Return the string with formated number. If not detected a number, it return the origin string content back.
        /// </summary>
        /// <param name="columnCell">number to convert</param>
        /// <returns>Return the result.</returns>
        private string GetColumnResult(string columnCell)
        {
            if (double.TryParse(columnCell, NumberStyles.Number, CultureInfo.GetCultureInfo("en"), out double decimalResult))
            {
                return string.Format(CultureInfo.GetCultureInfo("de-DE"), "{0:0.000}", decimalResult); 
            }

            return columnCell;
        }

        /// <summary>
        /// Create a new filename by exist name. Add to the filename converted and count number up, if the file exist
        /// </summary>
        /// <param name="filename">Source name from filename</param>
        /// <returns>Return a unique filename.</returns>
        private string CreateNewFilename(string filename)
        {
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
            return createNewName;
        }
    }
}

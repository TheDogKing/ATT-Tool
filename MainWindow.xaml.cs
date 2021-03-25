using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ATT_Tool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string mstrfilepath;
        private VModel VModel;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            VModel = new VModel();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".att", // Default file extension
                Filter = "PDMS ATT (.att)|*.att|Text documents (.txt)|*.txt" // Filter files by extension
            };
            bool? result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == false)
            {
                return;
            }
            save.IsEnabled = true;
            // Open document
            mstrfilepath = dlg.FileName;
            tbfilepath.Text = mstrfilepath;
            readfile(mstrfilepath);
            DataContext = VModel;
            //Action<string> showMethod = VModel.readfile;
            //showMethod.BeginInvoke(mstrfilepath, null, null);
            //this.Dispatcher.BeginInvoke(showMethod, mstrfilepath);
            //readfile(mstrfilepath);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
            {
                DefaultExt = ".att", // Default file extension
                Filter = "PDMS ATT (.att)|*.att|Text documents (.txt)|*.txt" // Filter files by extension
            };
            bool? result = dlg.ShowDialog();
            // Process open file dialog box results
            if (result == false)
            {
                return;
            }
            savefile(mstrfilepath, dlg.FileName);
        }

            private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VModel = new VModel();
            DataContext = VModel;
        }
        public void readfile(string mstrfilepath)
        {
            try
            {
                using (FileStream fs = new FileStream(mstrfilepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Create an instance of StreamReader to read from a file.
                    // The using statement also closes the StreamReader.
                    using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                    {
                        string line;
                        // Read and display lines from the file until the end of
                        // the file is reached.
                        model modelitem = new model();
                        int linenum = 1;
                        while ((line = sr.ReadLine()) != null)
                        {
                            linenum++;
                            if (linenum < 6 || (!line.ToUpper().Contains("NAME:=") && !line.ToUpper().Contains("TYPE:=") && !line.ToUpper().Contains("NEW ")) || line.Contains("NEW Header Information"))
                            {
                                continue;
                            }
                            int index = line.IndexOf("NEW");
                            if (index != -1 && VModel.models.Count == 0)
                            {
                                modelitem = new model() { spacecount = index };
                                VModel.models.Add(modelitem);
                            }
                            if (index != -1 && !(modelitem.name is null))
                            {
                                model tempmodel = new model() { spacecount = index };
                                if (modelitem.spacecount < index)
                                {
                                    modelitem.Items.Add(tempmodel);
                                    tempmodel.parent = modelitem;
                                }
                                else if (modelitem.spacecount == index)
                                {
                                    if (modelitem.parent is null)
                                    {
                                        VModel.models.Add(tempmodel);
                                    }
                                    else
                                    {
                                        modelitem.parent.Items.Add(tempmodel);
                                        tempmodel.parent = modelitem.parent;
                                    }
                                }
                                else
                                {
                                    while (!(modelitem.parent is null))
                                    {
                                        modelitem = modelitem.parent;
                                        if (modelitem.spacecount == index)
                                        {
                                            if (modelitem.parent is null)
                                            {
                                                VModel.models.Add(tempmodel);
                                                break;
                                            }
                                            else
                                            {
                                                modelitem.parent.Items.Add(tempmodel);
                                                tempmodel.parent = modelitem.parent;
                                                break;
                                            }
                                        }
                                    }

                                }
                                modelitem = tempmodel;
                            }
                            if (line.ToUpper().Contains("NAME:="))
                            {
                                modelitem.name = line.Split(new string[] { ":=" }, StringSplitOptions.None)[1].Trim();
                            }
                            else if (line.ToUpper().Contains("TYPE:="))
                            {
                                modelitem.type = line.Split(new string[] { ":=" }, StringSplitOptions.None)[1].Trim();
                            }
                            Console.WriteLine(line);
                        }
                    }
                }
                MessageBox.Show("load success");
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }
        }

        public void savefile(string oldfilepath, string newfilepath)
        {
            try
            {
                using (FileStream fs = new FileStream(oldfilepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    // Create an instance of StreamReader to read from a file.
                    // The using statement also closes the StreamReader.
                    using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default))
                    {
                        using (FileStream nfs = new FileStream(newfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            // Create an instance of StreamReader to read from a file.
                            // The using statement also closes the StreamReader.
                            using (StreamWriter sw = new StreamWriter(nfs, sr.CurrentEncoding))
                            {
                                string line;
                                // Read and display lines from the file until the end of
                                // the file is reached.
                                model modelitem = new model();
                                int linenum = 1;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    linenum++;
                                    if (linenum < 6 || (!line.ToUpper().Contains("NAME:=") && !line.ToUpper().Contains("TYPE:=") && !line.ToUpper().Contains("NEW ")) || line.Contains("NEW Header Information"))
                                    {
                                        sw.WriteLine(line);
                                        continue;
                                    }
                                    int index = line.IndexOf("NEW");
                                    if (index != -1 && VModel.models.Count == 0)
                                    {
                                        modelitem = new model() { spacecount = index };
                                        VModel.models.Add(modelitem);
                                    }
                                    if (index != -1 && !(modelitem.name is null))
                                    {
                                        model tempmodel = new model() { spacecount = index };
                                        if (modelitem.spacecount < index)
                                        {
                                            modelitem.Items.Add(tempmodel);
                                            tempmodel.parent = modelitem;
                                        }
                                        else if (modelitem.spacecount == index)
                                        {
                                            if (modelitem.parent is null)
                                            {
                                                VModel.models.Add(tempmodel);
                                            }
                                            else
                                            {
                                                modelitem.parent.Items.Add(tempmodel);
                                                tempmodel.parent = modelitem.parent;
                                            }
                                        }
                                        else
                                        {
                                            while (!(modelitem.parent is null))
                                            {
                                                modelitem = modelitem.parent;
                                                if (modelitem.spacecount == index)
                                                {
                                                    if (modelitem.parent is null)
                                                    {
                                                        VModel.models.Add(tempmodel);
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        modelitem.parent.Items.Add(tempmodel);
                                                        tempmodel.parent = modelitem.parent;
                                                        break;
                                                    }
                                                }
                                            }

                                        }
                                        modelitem = tempmodel;
                                    }
                                    if (line.ToUpper().Contains("NAME:="))
                                    {
                                        modelitem.name = line.Split(new string[] { ":=" }, StringSplitOptions.None)[1].Trim();
                                        sw.WriteLine(line.Replace(modelitem.name, modelitem.mname));
                                        continue;
                                    }
                                    else if (line.ToUpper().Contains("TYPE:="))
                                    {
                                        modelitem.type = line.Split(new string[] { ":=" }, StringSplitOptions.None)[1].Trim();
                                    }
                                    sw.WriteLine(line);
                                }
                            }
                        }
                    }
                }
                MessageBox.Show("save success");
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine(e.Message);
            }
        }
    }
}

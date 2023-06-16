using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace WPF_ForbiddenWordsFinder
{
    /// <summary>
    /// Логика взаимодействия для WindowTask.xaml
    /// </summary>
    
    public partial class WindowTask : Window
    {
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        List<string> ppat = new List<string>() { };

        List<string> ppatCopy = new List<string>() { };

        List<string> result = new List<string>() { };

        Regex reg; 

        string path = null;

        string exten;

        char ch = '\\';

        string[] temp;

        int pause = 0;

        public WindowTask()
        {                       
            InitializeComponent();
            pbStatus.Value = 0;
            lable.Content = "prepair...";

        }        

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            pbStatus.IsIndeterminate = true;
            CancellationToken token = cancelTokenSource.Token;
                lable.Content = "Процес виконує завдання";
                temp = tbWord.Text.Split(" ");              // заборонені слова

                exten = comboBox1.Text;                     // розширення файлу
                ppat.Clear();
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("Hey!!! Забули задати розширення файлу");
                    return;
                }
                if (tbFolderName.Text == "") { MessageBox.Show("Задайте ім'я теки! ..?"); tbFolderName.Focus(); }
                else
                {
                    path = tbFolderName.Text;               // назва теки
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            MessageBox.Show("Така тека вже існує.");
                            return;
                        }
                        else
                        {
                            DirectoryInfo di = Directory.CreateDirectory(path);        //  створити теку
                            MessageBox.Show($"Директорія створена {Directory.GetCreationTime(path).ToString()}.");                            
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"The process failed: {ex.ToString()}");
                    }
                }
            // //////////////////////////////////////////////////////////////////////////////////////////////////  
            await Task.Delay(500);
            pbStatus.IsIndeterminate = false;
            while (pause != 0) { await Task.Delay(2000); } // пауза 
            lable.Content = " пошук дисків на компьютері";
            pbStatus.Value = 5;
            await Task.Delay(2500);
            ProgBar(5, 30);
            await Task.Delay(2500);
            //lable.Content = Task.CurrentId;
            var drives = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed);    // з'ясую які є диски на компьютері
            while (pause != 0) { await Task.Delay(2000); } // пауза                                                                                  
            lable.Content =  $" пошук файлів з розширенням {exten}";
            await Task.Delay(5000);
            ProgBar(30, 60);
            drives.AsParallel().ForAll(d =>
            {
                FileFinder.GetAllFiles(d.RootDirectory.FullName, exten, ppat);                // знайду в цих дисках текстові файли шлях в List ppat
            });
            result.Add($"Знайдено {ppat.Count} файлів");
           // lstBox.ItemsSource = result;
            // //////////////////////////////////////////////////////////////////////////////////////////////////
            while(pause != 0) { await Task.Delay(2000); } // пауза
            lable.Content = " відбір файлів за заданими словами ... ";
            await Task.Delay(5000);
            ProgBar(60, 90);
            try
            {               
                Parallel.ForEach<string>(ppat, new ParallelOptions { CancellationToken = token }, FindWord);   // зі списку файлу виберу ті що містять задані слова                
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Операцію перервано");
            }
            finally
            {
                cancelTokenSource.Dispose();
            }
            lable.Content = " копіювання файлів ... ";
            await Task.Delay(2500);
            ProgBar(90, 101);
            await Task.Delay(2500);

            string message = $"Файли скопійовано в теку {path}";
            lable.Content = message;            
            //lstBox.ItemsSource = result;
            btnNewFile.Visibility = Visibility.Visible;
            //lstBox.ItemsSource = ppat; // назви файлів в ліст бокс            
        }     

        //////////////////////////////////////////////////////////////////////////////////////////////////
        private async void btnNewFile_Click(object sender, RoutedEventArgs e)                   // перевірка файлів на наявність заборонених слів
        {
            ProgBar(5, 101);
            lable.Content = " заміна заборонених слів на ******* ";
            await Task.Delay(5000);
            string[] temp = tbWord.Text.Split(" ");                                             // заборонені слова
            int cnt = 0;
                try
                {
                    if (Directory.Exists(path))
                    {
                        string pathSource = @"..\..\..\..\WPF_ForbiddenWordsFinder\bin\Debug\net6.0-windows" + "\\" + path;
                        string pathCopy = @"..\..\..\..\WPF_ForbiddenWordsFinder\bin\Debug\net6.0-windows" + @"\" + path + "Copy";
                        string pathFinaly = @"..\..\..\..\WPF_ForbiddenWordsFinder\bin\Debug\net6.0-windows" + @"\" + path + "Finaly";
                        char[] sym = {' ', '-'};                       
                        DirectoryInfo di = Directory.CreateDirectory(pathCopy);
                        pathSource.AsParallel().ForAll(d =>
                        {
                           FileFinder.GetAllFiles(path, exten, ppatCopy);                             // закину в List всі обрані файли
                        });
                        var uniq = ppatCopy.Distinct();
                        foreach (var pt in uniq)
                        {
                            try
                            {                            
                                    string line = File.ReadAllText(pt);
                                    string[] lines = line.Split(sym);
                                    for (int i = 0; i < temp.Length; i++)                              
                                    line = line.Replace(temp[i], "*******");                                                                 
                                    File.WriteAllText(pathCopy + "\\" + System.IO.Path.GetFileName(pt), line);
                                 for (int i = 0; i < temp.Length; i++)
                                 {
                                     var txtline = from word in lines
                                                   where word.Equals(temp[i], StringComparison.InvariantCultureIgnoreCase)
                                                   select word;
                                     int wordCount = txtline.Count();
                                     if(wordCount > 0)
                                     result.Add($"{temp[i]} зустрічається {wordCount} разів");
                                 }
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }
                catch (Exception ex) { }
            lstBox.ItemsSource = result;            
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {               
                CancellationToken token = cancelTokenSource.Token;
                Task task = Task.Run(() =>
                { cancelTokenSource.Cancel(); });
                lable.Content = task.Status;
            }
            catch (Exception ex) { }           
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////

        private  void FindWord(string arg1, ParallelLoopState arg2, long arg3)
        {
            try
            {              
                using (FileStream fstream = new FileStream(arg1, FileMode.Open))
                {
                    using (StreamReader read = new StreamReader(fstream))
                    {
                        string line;
                        line = read.ReadToEnd();
                        //string text = File.ReadAllText(pthr);
                        string[] txt = line.Split(" ");
                        if (txt.Length > 1)
                            for (int i = 0; i < temp.Length; i++)
                            {
                                foreach (var st in txt)
                                {
                                    if (st.Equals(temp[i]))
                                    {
                                        File.Copy(arg1, @"..\..\..\..\WPF_ForbiddenWordsFinder\bin\Debug\net6.0-windows" + "\\" + path + "\\" + System.IO.Path.GetFileName(arg1));
                                        break;
                                    }
                                }
                            }
                        //string line;
                        //line = read.ReadToEnd();
                        ////string text = File.ReadAllText(pthr);
                        //string[] txt = line.Split(" ");
                        //for (int i = 0; i < temp.Length; i++)
                        //{
                        //    foreach (var st in txt)
                        //    {
                        //        if (st.Equals(temp[i]))
                        //            File.Copy(arg1, @"..\..\..\..\WPF_ForbiddenWordsFinder\bin\Debug\net6.0-windows" + "\\" + path + "\\" + System.IO.Path.GetFileName(arg1));
                        //        break;
                        //    }
                        //}
                    }
                }

            }
            catch (Exception ex) { }
        }

        public static void SearchDirectory(string dir)   // рекурсивний пошук файлів
        {
            try
            {                
                foreach (string d in Directory.GetDirectories(dir))
                {
                     System.IO.Path.GetFileName(d);                    
                     SearchDirectory(d);
                }
            }
            catch (Exception ex)
            { }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            pause = 1;
            btnPause.Visibility = Visibility.Collapsed;
            btnRestart.Visibility = Visibility.Visible;           
        }

        private void btnRestart_Click(object sender, RoutedEventArgs e)
        {
            pause = 0;
            btnPause.Visibility = Visibility.Visible;
            btnRestart.Visibility = Visibility.Collapsed;
        }

        public async void ProgBar(int i, int n)
        {
            for (; i < n; i++)
            {
                pbStatus.Value = i;
                await Task.Delay(100);
            }
        }
    }
}

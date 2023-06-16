using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_ForbiddenWordsFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AppContext db = new AppContext();
        List<User> users;
        
       
        public MainWindow()
        {            
                InitializeComponent();
                Loaded += MainWindow_Loaded;           
        }
       
        /// //////////////////////////////////////////////////////       
        // metanit
        // при загрузке окна
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // гарантируем, что база данных создана
            db.Database.EnsureCreated();
            // загружаем данные из БД
            db.Users.Load();
            // и устанавливаем данные в качестве контекста
            DataContext = db.Users.Local.ToObservableCollection();
        }
        /// //////////////////////////////////////////////////////       
        private void Reg_Click(object sender, RoutedEventArgs e)
        {
            long phoneBefore = 0;
            string phoneAfter;
            string login = tbLogin.Text.Trim();
            string passw1 = pbPassw1.Password.Trim();
            string passw2 = pbPassw2.Password.Trim();
            try
            {
                if (tbPhone.Text != "")
                { phoneBefore = Convert.ToInt64(tbPhone.Text.Trim()); }
            }
            catch { }

            if (login.Length < 5)
            {
                tbLogin.ToolTip = "Логін має бути більше 5 знаків";
                tbLogin.Background = Brushes.Pink;
            }
            else tbLogin.Background = Brushes.Transparent;
            if (passw1.Length < 5)
            {
                pbPassw1.ToolTip = "Пароль має бути більше 5 знаків";
                pbPassw1.Background = Brushes.Pink;
            }
            else pbPassw1.Background = Brushes.Transparent;
            if (passw2 != passw1)
            {
                pbPassw2.ToolTip = "Паролі не співпадають";
                pbPassw2.Background = Brushes.Pink;
            }
            else pbPassw2.Background = Brushes.Transparent;

            if (phoneBefore != 0)
                tbPhone.Text = phoneBefore.ToString(Mask[Country.UA]);
            phoneAfter = tbPhone.Text.Trim();

            if (login != "" && passw1 != "" && phoneAfter != "")
            {
                User user = new User(login, passw1, phoneAfter);
                db.Users.Add(user);
                db.SaveChanges();
            }
        }
        private void Ent_Click(object sender, RoutedEventArgs e)
        {
            User usr = null; User log = null; User pas = null;
            using (AppContext db = new AppContext())
            {
                string login = tbLogin.Text.Trim();
                string passw1 = pbPassw1.Password.Trim();
                usr = db.Users.Where(d => d.Login == login && d.Pass == passw1).FirstOrDefault();
                log = db.Users.Where(l => l.Login == login).FirstOrDefault();
                pas = db.Users.Where(p => p.Pass == passw1).FirstOrDefault();
            }
            if (usr != null)
            {
                tbLogin.Background = Brushes.Transparent;
                pbPassw1.Background = Brushes.Transparent;
                MessageBox.Show("Авторізація пройшла успішно!!!", "В І Т А Ю!");
                WindowTask windowTask = new WindowTask();
                windowTask.Show();
                // this.Hide();
            }
            else
            {
                if (log == null || log.Login != tbLogin.Text.Trim()) tbLogin.Background = Brushes.Pink;
                else tbLogin.Background = Brushes.Transparent;
                if (pas == null || pas.Pass != pbPassw1.Password.Trim()) pbPassw1.Background = Brushes.Pink;
                else pbPassw1.Background = Brushes.Transparent;
            }
        }
        private void Forget_Click(object sender, RoutedEventArgs e)
        {
            users = db.Users.ToList();
            string str = "";
            foreach (User user in users)
            {
                str += "Login : " + user.Login + " " +
                    "   Password : " + user.Pass + " ";
            }
            example.Text = str;
            btnReg.Visibility = Visibility.Hidden;
            btnEnt.Visibility = Visibility.Visible;
            btnDlt.Visibility = Visibility.Visible;
            pbPassw2.Visibility = Visibility.Hidden;
            tbPhone.Visibility = Visibility.Hidden;
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////
        /// </summary>
        enum Country
        {
            UA,
            IT,
            UK
        }

        Dictionary<Country, string> Mask = new Dictionary<Country, string>
        {
            [Country.UA] = "+380(##)###-##-##",
            [Country.IT] = "+390(##)###-##-##",
            [Country.UK] = "+44(###)###-##-##"
        };
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////////
        /// </summary>       

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            btnReg.Visibility = Visibility.Hidden;
            btnEnt.Visibility = Visibility.Visible;
            btnDlt.Visibility = Visibility.Visible;
            pbPassw2.Visibility = Visibility.Hidden;
            tbPhone.Visibility = Visibility.Hidden;
        }

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            btnEnt.Visibility = Visibility.Hidden;
            btnReg.Visibility = Visibility.Visible;
            btnDlt.Visibility = Visibility.Hidden;
            pbPassw2.Visibility = Visibility.Visible;
            tbPhone.Visibility = Visibility.Visible;
        }

        private void Dlt_Click(object sender, RoutedEventArgs e)
        {
            // users.Clear();
            List<User> users = db.Users.ToList();
            string login = tbLogin.Text.Trim();
            string passw1 = pbPassw1.Password.Trim();
            string phoneAfter = tbPhone.Text.Trim();
            foreach (var us in users)
            {
                if (us.Login == login && us.Pass == passw1)
                {
                    db.Users.Remove(us);
                    db.SaveChanges();
                }
            }
        }    
    }
}

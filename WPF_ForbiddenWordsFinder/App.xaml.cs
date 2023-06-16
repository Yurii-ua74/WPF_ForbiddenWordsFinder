using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WPF_ForbiddenWordsFinder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex = null;    // блокування повторного запуску

        protected override void OnStartup(StartupEventArgs e)
        {
            const string appName = "MyAppName";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("Не пустуйте! Додаток вже працює\nта не може бути запущеним ще раз");
                //app is already running! Exiting the application  
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }
    }
}

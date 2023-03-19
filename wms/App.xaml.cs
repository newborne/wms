using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using wms.View;

namespace wms
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        LoginView loginView = new LoginView();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            showLoginView();
            Application.Current.Shutdown();
        }

        private void showLoginView()
        { 
            loginView=new LoginView();
            loginView.ShowDialog();
            if (loginView.window.DialogResult == true)
            {
                showMainView();
            }
        }
        private void showMainView()
        {
            MainView mainView = new MainView();
            mainView.ShowDialog();
            if (mainView.window.DialogResult == true)
            {
                showLoginView();
            }
        }
    }
}

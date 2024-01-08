using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Netstat_ano.Messages;
using Netstat_ano.Models;
using Netstat_ano.ViewModels;
using Netstat_ano.Views;
using System.ComponentModel;
using System.Windows;

namespace Netstat_ano
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<MainWindowViewModel>();

            RegisterSelfAction();
        }

        private void RegisterSelfAction()
        {
            WeakReferenceMessenger.Default.Register<CMessage>(this, (recipient, message) =>
            {
                if (message != null && message.Sender == DataContext)
                {
                    switch (message.Tag)
                    {
                        case "CloseWindow":
                            Close();
                            break;

                        case "ExitApp":
                            Application.Current?.Shutdown();
                            break;

                        case "ShowAbout":
                            AboutWindow aboutWindow = new()
                            {
                                Owner = this
                            };
                            aboutWindow.ShowDialog();
                            break;

                        case "ShowSettings":
                            SettingsWindow settingsWindow = new()
                            {
                                Owner = this
                            };
                            settingsWindow.ShowDialog();
                            break;

                        case
                            "ShowConnectionInfo":
                            if (message.Content is ConnectionInfo connectionInfo && connectionInfo != null)
                            {
                                ConnectionInfoWindowViewModel? vm = App.Current.Services.GetService<ConnectionInfoWindowViewModel>();
                                if (vm != null)
                                {
                                    vm.ConnInfo = connectionInfo;
                                    ConnectionInfoWindow connectionInfoWindow = new()
                                    {
                                        Owner = this,
                                        DataContext = vm
                                    };
                                    connectionInfoWindow.ShowDialog();
                                }
                            }
                            break;

                        case "KillProcessFailed":
                            MessageBox.Show($"Kill Process Failed.{message.Content}", "Kill Process Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;

                        default:
                            break;
                    }
                }
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
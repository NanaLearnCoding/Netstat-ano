using CommunityToolkit.Mvvm.Messaging;
using Netstat_ano.Messages;
using System.Windows;

namespace Netstat_ano.Views
{
    /// <summary>
    /// ConnectionInfoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionInfoWindow : Window
    {
        public ConnectionInfoWindow()
        {
            InitializeComponent();
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
                        case "KillProcessSuccess":
                            Close();
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
    }
}

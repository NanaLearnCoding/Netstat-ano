using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Netstat_ano.Messages;
using Netstat_ano.ViewModels;
using System.Windows;

namespace Netstat_ano.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<SettingsWindowViewModel>();
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
                        case "CloseSettingsWindow":
                            Close();
                            break;

                        case "ApplyChangeLanguage":
                            Close();
                            string? caption = TryFindResource("Lang_LanguageChanged") as string;
                            string? messageBoxText = TryFindResource("Lang_RestartAppForLanguageChangedTakeEffect") as string;
                            MessageBox.Show(messageBoxText, caption, MessageBoxButton.OK);
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}

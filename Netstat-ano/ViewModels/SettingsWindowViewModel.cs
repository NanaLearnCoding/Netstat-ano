using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Netstat_ano.Messages;
using Netstat_ano.Models;
using Netstat_ano.Properties;

namespace Netstat_ano.ViewModels
{
    partial class SettingsWindowViewModel : ObservableObject
    {
        public static List<LanguageInfo> Languages =>
            new() { new LanguageInfo("English", "en-US", "Langs/en-US.xaml"), new LanguageInfo("中文", "zh-CN", "Langs/zh-CN.xaml"), };

        [ObservableProperty]
        public int connectionsCount = 0;

        [ObservableProperty]
        public LanguageInfo? selectedLanguageInfo;

        [RelayCommand]
        private async Task ChangeLanguage()
        {
            CMessage cMessage = new(this);
            string nowCultureName = Settings.Default.CurrentCultureName;
            if (SelectedLanguageInfo != null && !nowCultureName.Equals(SelectedLanguageInfo.CultureName))
            {
                await Task.Run(() =>
                {
                    Settings.Default.CurrentCultureName = SelectedLanguageInfo.CultureName;
                    Settings.Default.Save();
                });
                cMessage.Tag = "ApplyChangeLanguage";
            }
            else
            {
                cMessage.Tag = "CloseSettingsWindow";
            }
            WeakReferenceMessenger.Default.Send(cMessage);
        }


        [RelayCommand]
        private void CloseSettingsWindow()
        {
            CMessage cMessage = new(this, "CloseSettingsWindow");
            WeakReferenceMessenger.Default.Send(cMessage);
        }
    }
}

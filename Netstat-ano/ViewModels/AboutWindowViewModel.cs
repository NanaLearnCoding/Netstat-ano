using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace Netstat_ano.ViewModels
{
    public partial class AboutWindowViewModel : ObservableObject
    {
        private const string GitHubUrl = "https://github.com/NanaLearnCoding";


        [RelayCommand]
        private static void JumpToLink()
        {
            Task.Run(() =>
            {
                Process.Start("explorer.exe", GitHubUrl);
            });
        }
    }
}

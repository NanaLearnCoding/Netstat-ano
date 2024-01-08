using Microsoft.Extensions.DependencyInjection;
using Netstat_ano.ViewModels;
using System.Windows;

namespace Netstat_ano.Views
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<AboutWindowViewModel>();
        }
    }
}
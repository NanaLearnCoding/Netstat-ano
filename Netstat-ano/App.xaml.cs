using Microsoft.Extensions.DependencyInjection;
using Netstat_ano.Properties;
using Netstat_ano.ViewModels;
using System.Windows;

namespace Netstat_ano
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Mutex? mutex;

        public App()
        {
            Services = ConfigureServices();
            this.InitializeComponent();
        }

        public new static App Current => (App)Application.Current;

        protected override void OnStartup(StartupEventArgs e)
        {
            string? appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            mutex = new Mutex(true, appName, out bool createdNew);
            if (createdNew != true)
            {
                Application.Current.Shutdown();
                return;
            }

            base.OnStartup(e);
            LoadLanguage();
        }

        public IServiceProvider Services { get; }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            //register viewModels
            services.AddTransient(mainWindowViewModel => new MainWindowViewModel { IsActive = true });
            services.AddTransient<AboutWindowViewModel>();
            services.AddTransient<ConnectionInfoWindowViewModel>();
            services.AddTransient<SettingsWindowViewModel>();
            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Load Language
        /// </summary>
        private static void LoadLanguage()
        {
            string langResourceOriginal = "";
            string currentCultureName = Settings.Default.CurrentCultureName;
            if (string.IsNullOrEmpty(currentCultureName))
            {
                currentCultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
            }

            langResourceOriginal = currentCultureName switch
            {
                "en-US" => @"Langs/en-US.xaml",
                "zh-CN" => @"Langs/zh-CN.xaml",
                _ => @"Langs/en-US.xaml",
            };
            List<ResourceDictionary> dictionaryList = new();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            ResourceDictionary? langResourceDic = dictionaryList.FirstOrDefault(item =>
            {
                return (item.Source != null && item.Source.OriginalString.Contains(langResourceOriginal));
            });
            if (langResourceDic != null)
            {
                Current.Resources.MergedDictionaries.Remove(langResourceDic);
                Current.Resources.MergedDictionaries.Add(langResourceDic);
            }


        }
    }
}

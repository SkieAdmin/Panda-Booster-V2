using System;
using System.IO;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Panda_Booster_V2
{
    public class AppConfig
    {
        public string SelectedServer { get; set; } = "";
    }

    public static class ConfigManager
    {
        private static readonly string ConfigDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
        private static readonly string ConfigPath = Path.Combine(ConfigDir, "Config.json");

        public static AppConfig Load()
        {
            if (!Directory.Exists(ConfigDir))
                Directory.CreateDirectory(ConfigDir);

            if (!File.Exists(ConfigPath))
            {
                var defaultConfig = new AppConfig();
                Save(defaultConfig);
                return defaultConfig;
            }

            var json = File.ReadAllText(ConfigPath);
            return JsonConvert.DeserializeObject<AppConfig>(json) ?? new AppConfig();
        }

        public static void Save(AppConfig config)
        {
            if (!Directory.Exists(ConfigDir))
                Directory.CreateDirectory(ConfigDir);

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(ConfigPath, json);
        }
    }

    public partial class MainWindow : Window
    {
        private bool _isLoading = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConsoleLog(string text)
        {
            TextRange textRange = new TextRange(ConsoleBox.Document.ContentStart, ConsoleBox.Document.ContentEnd);
            string existword = textRange.Text;
            string assword = existword + "\n" + "[Client] - " + text;
            ConsoleBox.Document.Blocks.Clear();

            Paragraph paragraph = new Paragraph();
            paragraph.Margin = new Thickness(0);
            paragraph.Inlines.Add(new Run(assword));

            ConsoleBox.Document.Blocks.Add(paragraph);
            ConsoleBox.ScrollToEnd();
        }

        private void NetworkConfiguration(string serverlist, string ip1, string ip2, bool Reset = false)
        {
            try
            {
                var netConfig = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var netConfigs = netConfig.GetInstances();

                foreach (ManagementObject net in netConfigs)
                {
                    var ipProperties = net.Properties["IPEnabled"];
                    if (ipProperties != null && (bool)ipProperties.Value)
                    {
                        if (Reset)
                        {
                            var resetDnsMethod = net.GetMethodParameters("SetDNSServerSearchOrder");
                            resetDnsMethod["DNSServerSearchOrder"] = null;
                            net.InvokeMethod("SetDNSServerSearchOrder", resetDnsMethod, null);
                            ConsoleLog("Resetting DNS Server");
                            ConsoleLog("Ping booster successfully disconnected");
                        }
                        else
                        {
                            var dnsServers = new[] { ip1, ip2 };
                            var setDnsMethod = net.GetMethodParameters("SetDNSServerSearchOrder");
                            setDnsMethod["DNSServerSearchOrder"] = dnsServers;
                            net.InvokeMethod("SetDNSServerSearchOrder", setDnsMethod, null);
                            ConsoleLog("Connecting to " + serverlist);
                            ConsoleLog("Ping booster successfully connected to " + serverlist);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting DNS: " + ex.Message);
            }
        }

        private void UI_DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void UnloadBooster()
        {
            NetworkConfiguration("Default Server", "0", "0", true);
            Environment.Exit(0);
        }

        private void Exitbtn_MouseDown(object sender, MouseButtonEventArgs e) => UnloadBooster();

        private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e) => UnloadBooster();

        private string GetSelectedServerName()
        {
            if (ServerOne.IsChecked == true) return "ServerOne";
            if (ServerTwo.IsChecked == true) return "ServerTwo";
            if (ServerThree.IsChecked == true) return "ServerThree";
            if (ServerFour.IsChecked == true) return "ServerFour";
            if (ServerFive.IsChecked == true) return "ServerFive";
            if (ServerSix.IsChecked == true) return "ServerSix";
            if (ServerSeven.IsChecked == true) return "ServerSeven";
            return "";
        }

        private void ApplyServerByName(string name)
        {
            switch (name)
            {
                case "ServerOne": ServerOne.IsChecked = true; break;
                case "ServerTwo": ServerTwo.IsChecked = true; break;
                case "ServerThree": ServerThree.IsChecked = true; break;
                case "ServerFour": ServerFour.IsChecked = true; break;
                case "ServerFive": ServerFive.IsChecked = true; break;
                case "ServerSix": ServerSix.IsChecked = true; break;
                case "ServerSeven": ServerSeven.IsChecked = true; break;
            }
        }

        private void ServerSelecton_Checked(object sender, RoutedEventArgs e)
        {
            if (_isLoading) return;

            if (ServerOne.IsChecked == true)
                NetworkConfiguration("Server #1", "1.1.1.1", "1.0.0.1");
            else if (ServerTwo.IsChecked == true)
                NetworkConfiguration("Server #2", "1.1.1.2", "1.0.0.2");
            else if (ServerThree.IsChecked == true)
                NetworkConfiguration("Server #3", "81.218.119.11", "209.88.198.133");
            else if (ServerFour.IsChecked == true)
                NetworkConfiguration("Server #4", "8.8.8.8", "8.8.4.4");
            else if (ServerFive.IsChecked == true)
                NetworkConfiguration("Server #5", "216.146.35.35", "216.146.36.36");
            else if (ServerSix.IsChecked == true)
                NetworkConfiguration("Server #6", "208.67.222.222", "208.67.220.220");
            else if (ServerSeven.IsChecked == true)
                NetworkConfiguration("Server #7", "91.217.137.37", "192.71.245.208");
            else
                NetworkConfiguration("Default Server", "0", "0", true);

            ConfigManager.Save(new AppConfig { SelectedServer = GetSelectedServerName() });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _isLoading = false;

            var config = ConfigManager.Load();
            if (!string.IsNullOrEmpty(config.SelectedServer))
            {
                ApplyServerByName(config.SelectedServer);
                // This triggers ServerSelecton_Checked which applies DNS + saves config
            }
        }
    }
}
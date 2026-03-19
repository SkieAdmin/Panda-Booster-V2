using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Panda_Booster_V2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ConsoleLog(string text)
		{
			TextRange textRange = new TextRange(ConsoleBox.Document.ContentStart, ConsoleBox.Document.ContentEnd);
			string existword = textRange.Text;
			string assword = existword + "\n" + "[Client] - " + text;
			ConsoleBox.Document.Blocks.Clear(); // clear existing content

			Paragraph paragraph = new Paragraph();
			paragraph.Margin = new Thickness(0);

			Run run = new Run(assword);
			paragraph.Inlines.Add(run);

			ConsoleBox.Document.Blocks.Add(paragraph);
			ConsoleBox.ScrollToEnd();

		}

		private Visual GetDescendantByType(Visual element, Type type)
		{
			if (element == null) return null;
			if (element.GetType() == type) return element;

			Visual foundElement = null;
			if (element is FrameworkElement frameworkElement)
			{
				frameworkElement.ApplyTemplate();
			}

			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
			{
				Visual visual = VisualTreeHelper.GetChild(element, i) as Visual;
				foundElement = GetDescendantByType(visual, type);
				if (foundElement != null) break;
			}

			return foundElement;
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
						if (Reset == true)
						{
							var resetDnsMethod = net.GetMethodParameters("SetDNSServerSearchOrder");
							resetDnsMethod["DNSServerSearchOrder"] = null;
							net.InvokeMethod("SetDNSServerSearchOrder", resetDnsMethod, null);
							//
							ConsoleLog("Resetting DNS Server");
							ConsoleLog("Ping booster successfully disconnected");
						}
						else
						{
							var dnsServers = new[] { ip1, ip2 };
							var setDnsMethod = net.GetMethodParameters("SetDNSServerSearchOrder");
							setDnsMethod["DNSServerSearchOrder"] = dnsServers;
							net.InvokeMethod("SetDNSServerSearchOrder", setDnsMethod, null);
							//LOL
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

		private void Exitbtn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			UnloadBooster();
		}


		private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			UnloadBooster();
		}

		private void ServerSelecton_Checked(object sender, RoutedEventArgs e)
		{
			if (ServerOne.IsChecked == true)
			{
				NetworkConfiguration("Server #1", "1.1.1.1", "1.0.0.1");
			}
			else if(ServerTwo.IsChecked == true)
			{
				NetworkConfiguration("Server #2", "1.1.1.2", "1.0.0.2");
			}
			else if(ServerThree.IsChecked == true)
			{
				NetworkConfiguration("Server #3", "81.218.119.11", "209.88.198.133");
			}
			else if(ServerFour.IsChecked == true)
			{
				NetworkConfiguration("Server #4", "8.8.8.8", "8.8.4.4");
			}
			else if (ServerFive.IsChecked == true)
			{
				NetworkConfiguration("Server #5", "216.146.35.35", "216.146.36.36");
			}
			else if(ServerSix.IsChecked == true)
			{
				NetworkConfiguration("Server #6", "208.67.222.222", "208.67.220.220");
			}
			else if (ServerSeven.IsChecked == true)
			{
				NetworkConfiguration("Server #7", "91.217.137.37", "192.71.245.208");
			}
			else
			{
				NetworkConfiguration("Default Server", "0", "0", true);
			}

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			//KeySystemAPI.Security.LaunchSecureBrowser("https://ravenscosmoid.com/iLcQgFgz18JlsuG/63013", "", "", false);
		}
	}
}

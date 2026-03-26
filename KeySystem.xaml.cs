using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using KeySystemLib;

namespace Panda_Booster_V2
{
	/// <summary>
	/// Interaction logic for KeySystem.xaml
	/// </summary>
	public partial class KeySystem : Window
	{
		public KeySystem()
		{
			InitializeComponent();
		}



		private void GetKeyBTN_Click(object sender, RoutedEventArgs e)
		{
			Panda.Auth.LaunchSecureBrowser("pandadevkit", "rawrr");
		}

		private void UI_DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left && e.ButtonState == MouseButtonState.Pressed)
				DragMove();
		}

		private void Exitbtn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Environment.Exit(0);
		}


		private void Minimize_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.WindowState = WindowState.Minimized;
		}

		private void AutoValidate(string text)
		{
            string KeyBox = text;
            if (Panda.Auth.Validate("pandadevkit", KeyBox, "test"))
            {
                new MainWindow().Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("The Key is no longer valid, Please Obtain New Key", "Panda", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
		private void ValidateKeyBTN_Click(object sender, RoutedEventArgs e)
		{
			string KeyBox = KeyInput.Text;
			if (Panda.Auth.Validate("pandadevkit", KeyBox, "test"))
			{
				File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "./PandaConfig/Key.cfg", KeyBox);
				new MainWindow().Show();
				this.Close();
			}
			else
			{
				MessageBox.Show("Key not valid");
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "./PandaConfig/"))
				Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "./PandaConfig/");
			if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "./PandaConfig/Key.cfg"))
			{
				string key = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "./PandaConfig/Key.cfg");
				AutoValidate(key);
			}
		}
	}
}

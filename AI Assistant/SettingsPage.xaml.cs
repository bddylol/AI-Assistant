using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AI_Assistant
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class SettingsPage : Page
	{
		string localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
		string configPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "config.json");

		public SettingsPage()
		{
			this.InitializeComponent();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			{
				if (!File.Exists(configPath))
				{
					AppConfig config = new AppConfig
					{
						ApiKey = "",
						BaseUrl = "https://api.openai.com/v1"
					};

					string baseConfig = JsonSerializer.Serialize(config);

					File.WriteAllText(configPath, baseConfig);
				}

				string jsonContent = File.ReadAllText(configPath);

				try
				{
					AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonContent);

					if (config != null)
					{
						apiField.Password = config.ApiKey;
						baseField.Text = config.BaseUrl;
					}
				}
				catch (JsonException ex)
				{
					ContentDialog dialog = new ContentDialog();

					// XamlRoot must be set in the case of a ContentDialog running in a Desktop app
					dialog.XamlRoot = this.XamlRoot;
					dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
					dialog.Title = "Configuration File Error";
					dialog.Content = ex.Message;
					dialog.PrimaryButtonText = "Ok";
					dialog.DefaultButton = ContentDialogButton.Primary;

					var result = await dialog.ShowAsync();
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// parse the config file

			AppConfig config = new AppConfig
			{
				ApiKey = apiField.Password,
				BaseUrl = baseField.Text
			};

			string baseConfig = JsonSerializer.Serialize(config);

			File.WriteAllText(configPath, baseConfig);

			ContentDialog dialog = new ContentDialog();

			// XamlRoot must be set in the case of a ContentDialog running in a Desktop app

			dialog.XamlRoot = this.XamlRoot;
			dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
			dialog.Title = "Configuration Saved";
			dialog.Content = "Your configuration has been saved.";
			dialog.PrimaryButtonText = "Ok";
			dialog.DefaultButton = ContentDialogButton.Primary;

			var result = dialog.ShowAsync();
		}
	}
}

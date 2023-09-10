using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AI_Assistant
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class ChatsPage : Page
	{
		string localFolder = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
		string configPath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "config.json");

		List<ChatMessage> ai_messages = new List<ChatMessage>
		{
			// prompt
			new ChatMessage(ChatMessageRole.System, "You are a helpful assistant.")
		};

		OpenAIAPI api;

		public ChatsPage()
		{
			this.InitializeComponent();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
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

			Debug.WriteLine(configPath);

			try
			{
				AppConfig config = JsonSerializer.Deserialize<AppConfig>(jsonContent);

				if (config.ApiKey == "" || config.BaseUrl == "")
				{
					ContentDialog dialog = new ContentDialog();

					// XamlRoot must be set in the case of a ContentDialog running in a Desktop app
					dialog.XamlRoot = this.XamlRoot;
					dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
					dialog.Title = "Missing Keys";
					dialog.Content = "Please configure your API keys and or the base url in the settings tab.";
					dialog.PrimaryButtonText = "Visit Settings";
					dialog.PrimaryButtonClick += Primary_Click;
					dialog.CloseButtonText = "Ignore";
					dialog.DefaultButton = ContentDialogButton.Primary;

					var result = await dialog.ShowAsync();
				} else
				{
					api = new OpenAIAPI(config.ApiKey);
					api.ApiUrlFormat = config.BaseUrl + "/{1}";
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

			Create_Message(true, "Welcome to the AI Assistant application created by bddylol made in WinUI3!");
		}

		private void Primary_Click(object sender, ContentDialogButtonClickEventArgs e)
		{
			Frame.Navigate(typeof(SettingsPage));
		}

		private void Create_Message(bool isAssistant, string message)
		{
			// Create Border
			Border border = new Border();
			border.BorderThickness = new Thickness(0);
			border.BorderBrush = (Brush)App.Current.Resources["AccentAAFillColorDefaultBrush"];

			// Create StackPanel 1
			StackPanel stackPanel1 = new StackPanel();
			stackPanel1.Width = double.NaN; // Auto
	
			stackPanel1.Spacing = 8;
			stackPanel1.Padding = new Thickness(16);

			// Create StackPanel 2
			StackPanel stackPanel2 = new StackPanel();
			stackPanel2.Orientation = Orientation.Horizontal;
			stackPanel2.Spacing = 8;

			// Create FontIcon
			FontIcon fontIcon = new FontIcon();
			fontIcon.Glyph = "\uEA8C"; // The Unicode character for the given glyph

			// Create TextBlock 1
			TextBlock textBlock1 = new TextBlock();
			textBlock1.Text = "Me";
			textBlock1.Style = (Style)App.Current.Resources["FlyoutPickerTitleTextBlockStyle"];

			// Create TextBlock 2
			TextBlock textBlock2 = new TextBlock();
			textBlock2.Text = message;
			textBlock2.Style = (Style)App.Current.Resources["BodyTextBlockStyle"];

			// Add FontIcon and TextBlock 1 to StackPanel 2
			stackPanel2.Children.Add(fontIcon);
			stackPanel2.Children.Add(textBlock1);

			// Add StackPanel 2 and TextBlock 2 to StackPanel 1
			stackPanel1.Children.Add(stackPanel2);
			stackPanel1.Children.Add(textBlock2);

			if (isAssistant)
			{
				border.BorderThickness = new Thickness(0, 0, 0, 0);
				textBlock1.Text = "Assistant";
				fontIcon.Glyph = "\xE99A";
			}

			// Add StackPanel 1 to Border
			border.Child = stackPanel1;

			messages.Children.Add(border);
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			Create_Message(false, textInput.Text);


			ai_messages.Add(new ChatMessage(ChatMessageRole.User, textInput.Text)); // Use .Add to append to the List

			textInput.Text = "";

			var result = await api.Chat.CreateChatCompletionAsync(new OpenAI_API.Chat.ChatRequest()
			{
				Model = Model.ChatGPTTurbo,
				Temperature = 0.1,
				Messages = ai_messages.ToArray() // Convert the List to an array when passing to the API
			});

			sendButton.IsEnabled = false;
			textInput.IsEnabled = false;

			Create_Message(true, result.Choices[0].Message.Content);

			ai_messages.Add(new ChatMessage(ChatMessageRole.Assistant, result.Choices[0].Message.Content)); // Use .Add to append to the List

			// log ai_messages
			Debug.WriteLine(JsonSerializer.Serialize(ai_messages));


			sendButton.IsEnabled = true;
			textInput.IsEnabled = true;
		}
	}


	class AppConfig
	{
		public string ApiKey { get; set; }
		public string BaseUrl { get; set; }
	}
}

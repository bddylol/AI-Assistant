using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AI_Assistant
{
	/// <summary>
	/// An empty window that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainWindow : Window
	{
		private OverlappedPresenter _presenter;
		public MainWindow()
		{
			this.InitializeComponent();
			//this.ExtendsContentIntoTitleBar = true;
			//this.SetTitleBar(AppTitleBar);
			this.SystemBackdrop = new MicaBackdrop();
			this.AppWindow.Resize(new Windows.Graphics.SizeInt32(550, 650));


			var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
			Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
			Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
			if (appWindow is not null)
			{

				Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(windowId, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
				if (displayArea is not null)
				{
					var CenteredPosition = appWindow.Position;
					CenteredPosition.X = ((displayArea.WorkArea.Width - appWindow.Size.Width) / 2);
					CenteredPosition.Y = ((displayArea.WorkArea.Height - appWindow.Size.Height) / 2);
					appWindow.Move(CenteredPosition);
					_presenter = appWindow.Presenter as OverlappedPresenter;
					_presenter.IsAlwaysOnTop = true;
				}
			}
		}

		private void navigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			var item = args.InvokedItemContainer;

			Debug.WriteLine(item.Tag.ToString());

			if (item == null)
			{
				return;
			}

			if (item.Tag.ToString() == "Chat")
			{
				contentFrame.Navigate(typeof(ChatsPage));
			}
			if (item.Tag.ToString() == "PreviousChats")
			{
				//contentFrame.Navigate(typeof(ChatsPage));
			}
			if (item.Tag.ToString() == "Settings")
			{
				contentFrame.Navigate(typeof(SettingsPage));
			}
		}

		private void navigation_Loaded(object sender, RoutedEventArgs e)
		{
			navigation.SelectedItem = navigation.MenuItems[0];
			contentFrame.Navigate(typeof(ChatsPage));
		}
	}
}

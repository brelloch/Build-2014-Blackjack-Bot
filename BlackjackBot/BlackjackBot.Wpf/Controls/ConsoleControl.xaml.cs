using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace BlackjackBot.Wpf.Controls
{
	/// <summary>
	/// Interaction logic for ConsoleControl.xaml
	/// </summary>
	public partial class ConsoleControl : UserControl
	{
		public string ConsoleText
		{
			get { return (string)GetValue(ConsoleTextProperty); }
			set { SetValue(ConsoleTextProperty, value); }
		}

		// Using a DependencyProperty as the backing store for ConsoleText.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ConsoleTextProperty =
			DependencyProperty.Register("ConsoleText", typeof(string), typeof(ConsoleControl), new PropertyMetadata(string.Empty));

		Timer _loggerTimer;
		readonly StringWriter _debugWriter = new StringWriter(); 
		
		public ConsoleControl()
		{
			InitializeComponent();
		}

		private void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			_loggerTimer = new Timer(Logger, null, 1000, 500);

            Trace.AutoFlush = true;
			TextWriterTraceListener listen = new TextWriterTraceListener(_debugWriter);
			Debug.Listeners.Add(listen);

            //writes to the bin folder
            string path = DateTime.Now.ToFileTime() + ".txt"; 
            TextWriterTraceListener file = new TextWriterTraceListener(path);
            Debug.Listeners.Add(file);
		}

		private void Logger(object state)
		{
			try
			{
				Dispatcher.Invoke(() => ConsoleText = _debugWriter.ToString());
			}
			catch
			{
				// we don't care
			}
		}
	}
}

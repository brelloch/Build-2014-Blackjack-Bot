using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BlackjackBot.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for PlayerControl.xaml
    /// </summary>
    public partial class PlayerControl : UserControl
    {
		public bool IsDealer
		{
			get { return (bool)GetValue(IsDealerProperty); }
			set { SetValue(IsDealerProperty, value); }
		}

		// Using a DependencyProperty as the backing store for IsDealer.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty IsDealerProperty =
			DependencyProperty.Register("IsDealer", typeof(bool), typeof(PlayerControl), new PropertyMetadata(false));

	    public PlayerControl()
        {
            InitializeComponent();

			var dpd = DependencyPropertyDescriptor.FromProperty(PlayerControl.IsDealerProperty, typeof(PlayerControl));
			dpd.AddValueChanged(this, IsDealerChanged);
        }

	    private void IsDealerChanged(object sender, EventArgs e)
	    {
		    if(IsDealer)
				Background = new SolidColorBrush(Color.FromRgb(0x4C,0x00,0x00));
	    }
    }
}

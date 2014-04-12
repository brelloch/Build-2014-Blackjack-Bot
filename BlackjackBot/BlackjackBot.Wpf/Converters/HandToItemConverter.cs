using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using BlackjackBot.Shared;

namespace BlackjackBot.Wpf
{
	public class HandToItemConverter : IValueConverter
	{
		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if(value == null)
				return null;

			List<string> images = new List<string>();

			Hand hand = (value as Hand);
			foreach(Card card in hand.Cards)
			{
				string name = card.Suit.ToString().Substring(0, 2) + (int)card.FaceVal;
				images.Add("/BlackjackBot.Wpf;component/images/Cards/" + name.ToLower() + ".gif");
			}

			return images;
		}

		/// <summary>
		/// Converts a value. 
		/// </summary>
		/// <returns>
		/// A converted value. If the method returns null, the valid null value is used.
		/// </returns>
		/// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

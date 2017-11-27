using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace JewishOrganizer.Converters
{
	public class DecimalToStringConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, String language)
		{
			if (value is null)
			{
				return "0";
			}
			return value.ToString();
		}

		public Object ConvertBack(Object value, Type targetType, Object parameter, String language)
		{
			if (value is null)
			{
				return Decimal.Zero;
			}
			//Fuck localizations
			var v = value.ToString().Replace(',', '.');
			if (!String.IsNullOrEmpty(v))
			{
				Decimal.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out Decimal res);
				return res;
			}
			else
				return Decimal.Zero;
		}
	}
}

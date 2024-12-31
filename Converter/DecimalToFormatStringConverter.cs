using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace IMKernelUI.Converter;

public class DecimalToFormatStringConverter:IValueConverter {
	public object Convert( object value, Type targetType, object parameter, CultureInfo culture ) {
		if( value is double doubleValue )
			return doubleValue.ToString("F3", culture); // 保留三位小数
		return value;
	}

	public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture ) {
		if( double.TryParse(value?.ToString( ), out double result) )
			return result; // 将字符串转换回 double
		return DependencyProperty.UnsetValue;
	}
}

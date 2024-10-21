using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace gip.vb.mobile.Helpers
{
    public class ACValueListIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<ACValue> list = value as IEnumerable<ACValue>;
            if (list != null && list.Any() && parameter != null)
            {
                string param = parameter as string;
                string[] paramSplited = param.Split(new char[] { ':' });
                
                int index = 0;
                if (int.TryParse(paramSplited[0], out index))
                {
                    if (list.Count() >= index + 1)
                    {
                        if (paramSplited[1] == nameof(ACValue.ACIdentifier))
                            return list.ElementAt(index).ACIdentifier;

                        if (paramSplited[1] == nameof(ACValue.Value))
                            return list.ElementAt(index).Value;

                        if (paramSplited[1] == nameof(ACValue.ACCaption))
                            return list.ElementAt(index).ACCaption;
                    }
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

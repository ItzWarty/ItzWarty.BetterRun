using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ItzWarty.BetterRun {
   public class ThimoConverter : IValueConverter {
      public object Convert(object value, Type targetType, object parameter,
          System.Globalization.CultureInfo culture) {
         Image image = value as Image;
         if (image != null) {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = ms;
            bi.EndInit();
            return bi;
         }
         return null;
      }

      public object ConvertBack(object value, Type targetType,
       object parameter, System.Globalization.CultureInfo culture) {
         throw new NotImplementedException();
      }
   }
}

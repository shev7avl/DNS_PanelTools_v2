using Autodesk.Revit.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

//namespace DSK.Application.AppGUI
//{
//    public class Button
//    {
//        private readonly RibbonPanel _ribbonPanel;
//        private readonly Bitmap _bitmap;

//        private readonly string _classRoute;
//        private readonly string _name;
//        private readonly string _text;

//        /// <summary>
//        /// Button creator
//        /// </summary>
//        /// <param name="panel">RibbonPanel that button will be placed on</param>
//        /// <param name="bitmap">Bitmap with 32x32px icon</param>
//        /// <param name="classRoute">Full name of IExternalCommand object</param>
//        /// <param name="name">Internal button name</param>
//        /// <param name="text">Text that's gonna be written on UI</param>
//        public Button(RibbonPanel panel, Bitmap bitmap, string classRoute, string name, string text)
//        {
//            _ribbonPanel = panel;
//            _bitmap = bitmap;
//            _classRoute = classRoute;
//            _name = name;
//            _text = text;

//            ImageSource imageSource = Convert(_bitmap);

//            PushButtonData pbData = new PushButtonData(_name, _text, Assembly.GetExecutingAssembly().Location, _classRoute);
//            PushButton pButton = _ribbonPanel.AddItem(pbData) as PushButton;
//            pButton.LargeImage = imageSource;
//        }
//        private static BitmapImage Convert(Image image)
//        {
//            using (var memory = new MemoryStream())
//            {
//                image.Save(memory, ImageFormat.Png);
//                memory.Position = 0;

//                var bitmapImage = new BitmapImage();
//                bitmapImage.BeginInit();
//                bitmapImage.StreamSource = memory;
//                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
//                bitmapImage.EndInit();
//                return bitmapImage;
//            }
//        }
//    }
//}

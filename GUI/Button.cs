using Autodesk.Revit.UI;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DSKPrim.PanelTools.GUI
{
    public class Button
    {
        readonly RibbonPanel RibbonPanel;
        readonly Bitmap Bitmap;
        readonly string ClassRoute;
        string Name;
        string Text;
        public Button(RibbonPanel panel, Bitmap bitmap, string classRoute, string name, string text)
        {
            RibbonPanel = panel;
            Bitmap = bitmap;
            ClassRoute = classRoute;
            Name = name;
            Text = text;

            ImageSource imageSource = Convert(Bitmap);

            PushButtonData pbData = new PushButtonData(Name, Text, Assembly.GetExecutingAssembly().Location, ClassRoute);
            PushButton pButton = RibbonPanel.AddItem(pbData) as PushButton;
            pButton.LargeImage = imageSource;
        }
        private static BitmapImage Convert(Image image)
        {
            using (var memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}

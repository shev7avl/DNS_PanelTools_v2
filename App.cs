using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Reflection;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace DNS_PanelTools_v2
{
    class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "TEST /n DNS Panel";
            string panelName = "DNS Panel";
            application.CreateRibbonTab(tabName);
            RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);

            PushButtonData JsonButtonData = new PushButtonData("DNS Panel", $"Создать марки", Assembly.GetExecutingAssembly().Location, "DNS_PanelTools_v2.Commands.Run");
            var JSONbutton = panel.AddItem(JsonButtonData) as PushButton;
            Image image = Properties.Resource.Test.ToBitmap();
            ImageSource imageSource = Convert(image);
            JSONbutton.LargeImage = imageSource;

            PushButtonData JsonButtonData1 = new PushButtonData("DNS Panel1", $"Создать проемы", Assembly.GetExecutingAssembly().Location, "DNS_PanelTools_v2.Commands.STRUCT_CreateOpenings");
            var JSONbutton1 = panel.AddItem(JsonButtonData1) as PushButton;
            JSONbutton1.LargeImage = imageSource;

            PushButtonData JsonButtonData2 = new PushButtonData("DNS Panel2", $"Маркисборки", Assembly.GetExecutingAssembly().Location, "DNS_PanelTools_v2.Commands.STRUCT_Assemblies");
            var JSONbutton2 = panel.AddItem(JsonButtonData2) as PushButton;
            JSONbutton2.LargeImage = imageSource;

            PushButtonData JsonButtonData3 = new PushButtonData("DNS Panel3", $"ФасадМарки", Assembly.GetExecutingAssembly().Location, "DNS_PanelTools_v2.Commands.ARCH_copyMarks");
            var JSONbutton3 = panel.AddItem(JsonButtonData3) as PushButton;
            JSONbutton3.LargeImage = imageSource;

            PushButtonData JsonButtonData5 = new PushButtonData("DNS Panel5", $"ФасадПлитка", Assembly.GetExecutingAssembly().Location, "DNS_PanelTools_v2.Commands.ARCH_SplitToParts");
            var JSONbutton5 = panel.AddItem(JsonButtonData5) as PushButton;
            JSONbutton5.LargeImage = imageSource;

            return Result.Succeeded;
        }

        public BitmapImage Convert(Image image)
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

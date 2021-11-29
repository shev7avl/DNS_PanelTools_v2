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
using Autodesk.Windows;

namespace DSKPrim.PanelTools_v2
{
    class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            string tabName = "Мастер Панелей";
            string panelName = "АКР";
            string panelName_kzhi = "КЖ.И - LOD400";
            string panelName2 = "КЖ - LOD100";
            string appName = "Запуск";
            application.CreateRibbonTab(tabName);
            Autodesk.Revit.UI.RibbonPanel panel1 = application.CreateRibbonPanel(tabName, appName);
            Autodesk.Revit.UI.RibbonPanel panel_kzhi = application.CreateRibbonPanel(tabName, panelName_kzhi);
            Autodesk.Revit.UI.RibbonPanel panel_AKR = application.CreateRibbonPanel(tabName, panelName);


            Image image = Properties.Resource.Test.ToBitmap();
            ImageSource imageSource = Convert(image);

            PushButtonData JsonButtonData = new PushButtonData("DNS Panel", $"Мастер панелей", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.Run");
            var JSONbutton = panel1.AddItem(JsonButtonData) as PushButton;
            JSONbutton.LargeImage = imageSource;

            PushButtonData JsonButtonData1 = new PushButtonData("DNS Panel", $"Создать чертежи сборки", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.STR_CreateDrawings");
            var JSONbutton1 = panel_kzhi.AddItem(JsonButtonData1) as PushButton;
            JSONbutton1.LargeImage = imageSource;



            //PushButtonData JsonButtonData1 = new PushButtonData("DNS Panel1", $"Создать проемы", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.STRUCT_CreateOpenings");
            //var JSONbutton1 = panel.AddItem(JsonButtonData1) as PushButton;
            //JSONbutton1.LargeImage = imageSource;

            Image window = Properties.Resource.window.ToBitmap();
            ImageSource windowSource = Convert(window);

            Bitmap wall = Properties.Resource.wall.ToBitmap();
            ImageSource wallSource = Convert(wall);

            Bitmap tile = Properties.Resource.tile.ToBitmap();
            ImageSource tileSource = Convert(tile);

            Bitmap copyMarks = Properties.Resource.files.ToBitmap();
            ImageSource copyMarksSource = Convert(copyMarks);

            Bitmap createDrawings = Properties.Resource.construction_plan.ToBitmap();
            ImageSource createDrawingsSource = Convert(createDrawings);

            PushButtonData arch_copyMarksData = new PushButtonData("Copy marks to facade", $"Скопировать марки", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.ARCH_copyMarks");
            var arch_copyMarksButton = panel_AKR.AddItem(arch_copyMarksData) as PushButton;
            arch_copyMarksButton.LargeImage = copyMarksSource;

            PushButtonData JsonButtonData3 = new PushButtonData("DNS Panel3", $"Разместить проемы", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.ARCH_WindowTestFork");
            var JSONbutton3 = panel_AKR.AddItem(JsonButtonData3) as PushButton;
            JSONbutton3.LargeImage = windowSource;

            PushButtonData JsonButtonData5 = new PushButtonData("DNS Panel5", $"Клинкерная плитка", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.ARCH_SplitToParts");
            var JSONbutton5 = panel_AKR.AddItem(JsonButtonData5) as PushButton;
            JSONbutton5.LargeImage = wallSource;

            PushButtonData JsonButtonData2 = new PushButtonData("DNS Panel2", $"Прямая плитка", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.ARCH_SplitToPartsStraightLayout");
            var JSONbutton2 = panel_AKR.AddItem(JsonButtonData2) as PushButton;
            JSONbutton2.LargeImage = tileSource;

            PushButtonData arch_createDrawings = new PushButtonData("Create facade drawings", $"Создать чертежи", Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools_v2.Commands.ARCH_CreateDrawings");
            var arch_createDrawingsButton = panel_AKR.AddItem(arch_createDrawings) as PushButton;
            arch_createDrawingsButton.LargeImage = createDrawingsSource;

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

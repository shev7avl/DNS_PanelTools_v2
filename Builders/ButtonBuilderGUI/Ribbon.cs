using Autodesk.Revit.UI;

namespace DSKPrim.PanelTools.GUI
{
    public class Ribbon
    {
        private string RibbonName;

        private string TabName;

        public RibbonPanel RibbonPanel;

        private UIControlledApplication UIControlledApplication;

        public Ribbon(UIControlledApplication application, string ribbonName)
        {
            TabName = "Мастер панелей";
            RibbonName = ribbonName;
            UIControlledApplication = application;

            try
            {
                UIControlledApplication.CreateRibbonTab(TabName);
                RibbonPanel = UIControlledApplication.CreateRibbonPanel(TabName, RibbonName);
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                RibbonPanel = UIControlledApplication.CreateRibbonPanel(TabName, RibbonName);
            }
        }

    }
}

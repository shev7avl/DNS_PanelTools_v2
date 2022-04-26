using Autodesk.Revit.UI;

//namespace DSK.Application.AppGUI
//{
//    public class Ribbon
//    {
//        private readonly string _ribbonName;

//        private readonly string _tabName;

//        private readonly UIControlledApplication UIControlledApplication;

//        public RibbonPanel RibbonPanel { get; private set; }

//        /// <summary>
//        /// Create a RibbonPanel
//        /// </summary>
//        /// <param name="application">Link to application</param>
//        /// <param name="ribbonName">Name of the ribbon</param>
//        public Ribbon(UIControlledApplication application, string ribbonName)
//        {
//            _tabName = "Мастер панелей";
//            _ribbonName = ribbonName;
//            UIControlledApplication = application;

//            try
//            {
//                UIControlledApplication.CreateRibbonTab(_tabName);
//                RibbonPanel = UIControlledApplication.CreateRibbonPanel(_tabName, _ribbonName);
//            }
//            catch (Autodesk.Revit.Exceptions.ArgumentException)
//            {
//                RibbonPanel = UIControlledApplication.CreateRibbonPanel(_tabName, _ribbonName);
//            }
//        }
//    }
//}

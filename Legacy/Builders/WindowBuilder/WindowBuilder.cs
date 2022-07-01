using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.WindowBuilder
{
    public class WindowBuilder
    {
        private protected PrecastPanel _panel;
        private List<RevitLinkInstance> _archLinks;
        private static readonly string _windowFamPath = 
            "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\\00_DNSPanel\\02_DNS_Семейства\\1. АР\\Окна";
        private static readonly string _doorFamPath = 
            "\\\\ir-dsk\\DNSDevelopment\\1_ДСК Приморье\\13. Проектный институт\\ДНС ПРОЕКТ РАБОЧАЯ\\BIM\\00_DNSPanel\\02_DNS_Семейства\\1. АР\\Двери";
        private readonly List<string> _windowFamNames;
        private readonly List<string> _doorFamNames;

        public WindowBuilder(PrecastPanel panel)
        { 
            _panel = panel;
            _archLinks = new FilteredElementCollector(panel.ActiveElement.Document).
                OfClass(typeof(RevitLinkInstance)).
                Where(o => o.Name.Contains("_АР")).
                Cast<RevitLinkInstance>().
                ToList();
            _windowFamNames = ParseFamilyNames(_windowFamPath);
            _doorFamNames = ParseFamilyNames(_doorFamPath);
        }

        public void Perforate(List<Element> IntersectedWindows)
        {

            if (IntersectedWindows.Count > 0 && IntersectedWindows.Count < 3)
            {
                TransactionGroup transaction = new TransactionGroup(_panel.ActiveElement.Document, $"Создание проемов - {_panel.ActiveElement.Name}");
                transaction.Start();

                Utility.Openings.SetOpeningParams(_panel.ActiveElement, IntersectedWindows);
                
                transaction.Assimilate();
            }
        }

        public List<Element> GetOpeningsFromLinks()
        {
            List<Element> openings = new List<Element>();

            foreach (var link in _archLinks)
            {
                var archDoc = link.GetLinkDocument();

                //Options options = new Options();
                //BoundingBoxXYZ panelBbox = _panel.ActiveElement.get_Geometry(options).GetBoundingBox();

                ElementIntersectsElementFilter intersectionFilter = new ElementIntersectsElementFilter(_panel.ActiveElement);
                ElementFilter levelFilter = new ElementLevelFilter(_panel.ActiveElement.LevelId);

                List<Element> windowsList = new FilteredElementCollector(archDoc).
                    OfClass(typeof(FamilyInstance)).
                    OfCategory(BuiltInCategory.OST_Windows).
                    WherePasses(intersectionFilter).
                    WherePasses(levelFilter).
                    Cast<Element>().
                    Where(o => _windowFamNames.Contains(GetFamilyName(o))).ToList();
                openings.Concat(windowsList).ToList();

                List<Element> doorsList = new FilteredElementCollector(archDoc).
                    OfClass(typeof(FamilyInstance)).
                    OfCategory(BuiltInCategory.OST_Doors).
                    WherePasses(intersectionFilter).
                    WherePasses(levelFilter).
                    Cast<Element>().
                    Where(o => _doorFamNames.Contains(GetFamilyName(o))).ToList();
                openings.Concat(doorsList).ToList();
            }
            
            //XYZ transform = revitLink.GetTransform().Origin;
            //if (elems != null)
            //{
            //    foreach (var item in elems)
            //    {
            //        LocationPoint locationPointBase = (LocationPoint)item.Location;

            //        XYZ newPoint = new XYZ(
            //            locationPointBase.Point.X + transform.X, 
            //            locationPointBase.Point.Y + transform.Y, 
            //            locationPointBase.Point.Z + transform.Z);

            //        if (Geometry.InBox(panelBbox, newPoint) && LevelEq(element, item))
            //        {
            //            IntersectedElements.Add(item);
            //        }
            //    }
            //}

            if (openings.Count > 2)
            {
                throw new ArgumentOutOfRangeException(message: $"Window limit out of range (limit: 2, currently: {openings.Count} )", new Exception($"Window limit out of range (limit: 2, currently: {openings.Count} )"));
            }
            else
            {
                Debug.WriteLine($"--> Window count: {openings.Count}");
            }

            return openings;
        }

        private static List<string> ParseFamilyNames(string path)
        {

            List<string> windowFamilyNames = new List<string>();

            string[] filePaths = Directory.GetFiles(path);
            foreach (var item in filePaths)
            {
                string name = item.Split('\\').Last().Replace(".rfa", "");
                windowFamilyNames.Add(name);
            }

            return windowFamilyNames;
        }

        private string GetFamilyName(Element element)
        { 
            FamilyInstance instance = element as FamilyInstance;
            return instance.Symbol.FamilyName;
        }

    }
}

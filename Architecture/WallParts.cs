using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using DNS_PanelTools_v2.StructuralApps;
using DNS_PanelTools_v2.StructuralApps.Panel;

namespace DNS_PanelTools_v2.Architecture
{
    public class WallParts
    {
        public Document ActiveDoc;

        public Document LinkedDocSTR;

        public Document LinkedDocARCH;

        public Element ActiveElement;

        public IPanel IntersectedPanel;

        public List<XYZ> IntersectedWindows { get; set; }

        public List<Material> FacadeMaterials { get; set; }

        public WallParts(Document document, Document linkedDocSTR, Document linkedDocARCH, Element element)
        {
            ActiveDoc = document;
            LinkedDocSTR = linkedDocSTR;
            LinkedDocARCH = linkedDocARCH;
            ActiveElement = element;
        }

        public void GetIntersectedPanel()
        {
            Options options = new Options();
            LocationCurve curve = (LocationCurve)ActiveElement.Location;
            XYZ Start = curve.Curve.GetEndPoint(0);
            XYZ End = curve.Curve.GetEndPoint(0);

            SingleStructDoc marksList = SingleStructDoc.getInstance(LinkedDocSTR);
            List<IPanel> panels = marksList.GetPanelMarks();

            foreach (IPanel item in panels)
            {
                BoundingBoxXYZ boundingBox = item.ActiveElement.get_Geometry(options).GetBoundingBox();

                if (RvtGeomStat.IsPointInsideBbox(boundingBox, Start) && RvtGeomStat.IsPointInsideBbox(boundingBox, End))
                {
                    Debug.WriteLine($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDoc, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
                }
            }

        }

        public void GetIntersectedWindows()
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(LinkedDocARCH);
            List<Element> windows = archDoc.getWindows();

            IEnumerable<Element> familySymbols = new FilteredElementCollector(ActiveDoc).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Where(o => o.Name.Contains("DNS_ПроемДляПлитки"));
            FamilySymbol familySymbol = (FamilySymbol)familySymbols.First();

            IntersectedWindows = new List<XYZ>();

            LocationCurve location = (LocationCurve)ActiveElement.Location;
            Curve curve = location.Curve;

            XYZ Start = curve.GetEndPoint(0);
            XYZ End = curve.GetEndPoint(1);

            Options options = new Options();
            
            foreach (Element window in windows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                BoundingBoxXYZ boundingBoxHostWall = hostWall.get_Geometry(options).GetBoundingBox();
                Debug.WriteLine($"------{window.Name}------");
                Debug.WriteLine($"BoundingBox MAX: {boundingBoxHostWall.Max}");
                Debug.WriteLine($"BoundingBox MIN: {boundingBoxHostWall.Min}");

                Debug.WriteLine($"Начало отрезка: {Start}");
                Debug.WriteLine($"Конец отрезка: {End}");
                
                Debug.WriteLine("------------------");
                if (RvtGeomStat.IsPointInsideBbox(boundingBoxHostWall, Start) || RvtGeomStat.IsPointInsideBbox(boundingBoxHostWall, End))
                {
                    LocationPoint locationPoint = (LocationPoint)window.Location;
                    Debug.WriteLine($"Нашли пересечение {ActiveElement.Name} с окном {window.Name}");

                    Level level = ActiveDoc.GetElement(hostWall.LevelId) as Level;

                    using (Transaction transaction = new Transaction(ActiveDoc, "Create window"))
                    {
                        transaction.Start();
                        ActiveDoc.Create.NewFamilyInstance(locationPoint.Point, familySymbol, ActiveElement, level, StructuralType.NonStructural);
                        transaction.Commit();
                    }

                    IntersectedWindows.Add(locationPoint.Point);
                }
            }


        }
        private void CreateWindow(Document doc, string fsFamilyName,
            string fsName, string levelName, string xCoord, string yCoord)
        {
            // LINQ to find the window's FamilySymbol by its type name.
            FamilySymbol familySymbol = (from fs in new FilteredElementCollector(doc).
                 OfClass(typeof(FamilySymbol)).WhereElementIsElementType().
                 Cast<FamilySymbol>()
                                         where (fs.Family.Name == fsFamilyName && fs.Name == fsName)
                                         select fs).First();

            // LINQ to find the level by its name.
            Level level = (from lvl in new FilteredElementCollector(doc).
                           OfClass(typeof(Level)).
                           Cast<Level>()
                           where (lvl.Name == levelName)
                           select lvl).First();

            // Convert coordinates to double and create XYZ point.
            double x = double.Parse(xCoord) / 304.8;  //don't forget to convert the dimensions to proper units
            double y = double.Parse(yCoord) / 304.8;  // in my project I use mm so I must divied by 304.8 to convert them from Feet

            XYZ xyz = new XYZ(x, y, level.Elevation);

            #region Find the hosting Wall (nearst wall to the insertion point)

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(Wall));

            List<Wall> walls = collector.Cast<Wall>().Where(wl => wl.LevelId == level.Id).ToList();

            Wall wall = null;

            double distance = double.MaxValue;

            foreach (Wall w in walls)
            {
                double proximity = (w.Location as LocationCurve).Curve.Distance(xyz);

                if (proximity < distance)
                {
                    distance = proximity;
                    wall = w;
                }
            }

            #endregion

            // Create window.
            using (Transaction t = new Transaction(doc, "Create window"))
            {
                t.Start();

                if (!familySymbol.IsActive)
                {
                    // Ensure the family symbol is activated.
                    familySymbol.Activate();
                    doc.Regenerate();
                }

                // Create window
                // unliss you specified a host, Rebit will create the family instance as orphabt object.
                FamilyInstance window = doc.Create.NewFamilyInstance(xyz, familySymbol, wall, StructuralType.NonStructural);
                t.Commit();
            }
            string prompt = "The element was created!";
            TaskDialog.Show("Revit", prompt);
        }

    }
}

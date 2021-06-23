using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps.Openings
{
    public class OpeningBuilder
    {
        private Document ActiveDocument;

        private Document LinkedDocument;

        private List<Element> IntersectedWindows;

        public OpeningBuilder(Document document, Document linkedDoc)
        {
            ActiveDocument = document;
            LinkedDocument = linkedDoc;
        }

        public void FindIntersectedWindows(Element element)
        {

            
            IntersectedWindows = new List<Element>();
            Options options = new Options();
            BoundingBoxXYZ panelBbox = element.get_Geometry(options).GetBoundingBox();
            

            IEnumerable<Element> listWindows = new FilteredElementCollector(LinkedDocument).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsNotElementType().ToElements().Where(o => o.Name.Contains("DNS_"));

            foreach (var item in listWindows)
            {
                LocationPoint locationPoint = (LocationPoint)item.Location;
                Debug.WriteLine($"{item.Name} попал в список");
                if (IsPointInsideBbox(panelBbox, locationPoint.Point))
                {
                    Debug.WriteLine($"{item.Name} пересекается с панелью {element.Name}");
                }
            }

            
        }

        private bool IsPointInsideBbox(BoundingBoxXYZ boundingBox, XYZ point)
        {
            double maxX = boundingBox.Max.X;
            double maxY = boundingBox.Max.Y;
            double maxZ = boundingBox.Max.Z;

            double minX = boundingBox.Min.X;
            double minY = boundingBox.Min.Y;
            double minZ = boundingBox.Min.Z;

            bool XCheck = (point.X >= minX && point.X <= maxX);
            bool YCheck = (point.Y >= minY && point.Y <= maxY);
            bool ZCheck = (point.Z >= minZ && point.Z <= maxZ);

            return XCheck && YCheck && ZCheck;

        }

        public void CreateOpening(Element element)
        { }


        private double CalculateOffset()
        {

            return 0;
        }

    }
}

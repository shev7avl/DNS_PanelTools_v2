using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.Architecture;
using DSKPrim.PanelTools_v2.StructuralApps;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.Utility;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public class WallParts: Panel
    {
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string ShortMark { get; set; }

        public override string LongMark { get; set; }

        public override string Index { get; set; }

        public override List<XYZ> IntersectedWindows { get; set; }

        private readonly Document LinkedDocSTR;


        public WallParts(Document document, Document linkedDocSTR, Element element)
        {
            ActiveDocument = document;
            LinkedDocSTR = linkedDocSTR;
            ActiveElement = element;
        }

        public WallParts(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }


        public override void CreateMarks()
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            Options options = new Options();
            LocationCurve curve = (LocationCurve)ActiveElement.Location;
            XYZ Start = curve.Curve.GetEndPoint(0);
            XYZ End = curve.Curve.GetEndPoint(0);

            SingleStructDoc marksList = SingleStructDoc.getInstance(LinkedDocSTR);
            List<Panel> panels = marksList.PanelMarks;



            foreach (Panel item in panels)
            {
                BoundingBoxXYZ boundingBox = item.ActiveElement.get_Geometry(options).GetBoundingBox();

                if (Geometry.InBox(boundingBox, Start) && Geometry.InBox(boundingBox, End))
                {
                    logger.DebugLog($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDocument, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
                }
            }
        }

        
        
    }
}

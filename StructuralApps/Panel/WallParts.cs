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
    public class WallParts : IPanel
    {
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string ShortMark { get; set; }

        public override string LongMark { get; set; }

        public override List<XYZ> IntersectedWindows { get; set; }

        private Document LinkedDocSTR;

        private Document LinkedDocARCH;

        private IPanel IntersectedPanel;

        private List<Material> FacadeMaterials { get; set; }

        private List<ElementId> FacadeParts { get; set; } 

        public WallParts(Document document, Document linkedDocSTR, Document linkedDocARCH, Element element)
        {
            ActiveDocument = document;
            LinkedDocSTR = linkedDocSTR;
            LinkedDocARCH = linkedDocARCH;
            ActiveElement = element;
        }

        public override void CreateMarks()
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

                if (Geometry.IsPointInsideBbox(boundingBox, Start) && Geometry.IsPointInsideBbox(boundingBox, End))
                {
                    Debug.WriteLine($"{ActiveElement.Name} пересекается с: {item.LongMark}");
                    Transaction transaction = new Transaction(ActiveDocument, $"Назначение марки: {item.LongMark}");
                    transaction.Start();

                    ActiveElement.LookupParameter("DNS_Код изделия полный").Set(item.LongMark);
                    ActiveElement.LookupParameter("DNS_Марка элемента").Set(item.LongMark);
                    ActiveElement.LookupParameter("ADSK_Марка конструкции").Set(item.LongMark);
                    transaction.Commit();
                }
            }

        }


        

        public void SplitToParts()
        {
            ICollection<ElementId> elementIdsToDivide = new List<ElementId>();

            ICollection<ElementId> elementIds = new List<ElementId>()
            {
                ActiveElement.Id
            };
            Debug.WriteLine($"------------");
            Debug.WriteLine($"@Начало транзакции");
            Debug.WriteLine($"@");
            if (PartUtils.AreElementsValidForCreateParts(ActiveDocument, elementIds))
            {
                using (Transaction transaction = new Transaction(ActiveDocument, "Parts creation"))
                {
                    transaction.Start();
                    PartUtils.CreateParts(ActiveDocument, elementIds);
                    transaction.Commit();
                }                
            }
            ElementFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Parts);
            elementIdsToDivide = ActiveElement.GetDependentElements(filter);

            Debug.WriteLine($"@Части для {ActiveElement.Name} Созданы успешно");
            Debug.WriteLine($"@");
            Debug.WriteLine($"@Конец транзакции");
            Debug.WriteLine($"------------");

            foreach (var item in elementIdsToDivide)
            {
                SplitGeometry.CreateSketchPlane(ActiveDocument, item);
            }
            FacadeParts = new List<ElementId>();
            FacadeParts = (List <ElementId>)ActiveElement.GetDependentElements(filter);
        }

    }
}

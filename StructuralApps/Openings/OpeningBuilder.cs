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

        private void FindIntersectedWindows(Element element)
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
                    IntersectedWindows.Add(item);
                    Debug.WriteLine($"{item.Name} пересекается с панелью {element.Name}");
                    CalculateOffset(item);
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

        private double CalculateAxialLength (XYZ pointA, XYZ pointB)
        {
            double aX = pointA.X;
            double aY = pointA.Y;
            //double aZ = pointA.Z;

            double bX = pointB.X;
            double bY = pointB.Y;
            //double bZ = pointB.Z;

            double dX = (aX - bX);
            double dY = (aY - bY);

            if (dX == 0)
            {
                return dY;
            }
            else
            {
                return dX;
            }

        }

        public void CreateOpening(Element element)
        {
            FindIntersectedWindows(element);

            TransactionGroup transactionGroup = new TransactionGroup(ActiveDocument, $"Создание проемов - {element.Name}");
            if (IntersectedWindows.Count>0 && IntersectedWindows.Count<=2)
            {

                transactionGroup.Start();

                Element window = IntersectedWindows[0];
            
                SetOpeningParams(element, window);


                transactionGroup.Assimilate();
            }
        }

        private void SetOpeningParams(Element element, Element window)
        {
            FamilyInstance familyInstanceWin = window as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;
            Transaction transaction = new Transaction(ActiveDocument, "Test_opening");
            double appWidth = Convert.ToDouble(familySymbolWin.LookupParameter("Ширина").AsValueString());
            double appHeigth = Convert.ToDouble(familySymbolWin.LookupParameter("Примерная высота").AsValueString());
            double offsetZ = Convert.ToDouble(window.LookupParameter("Высота нижнего бруса").AsValueString());

            transaction.Start();
            element.LookupParameter("ПР1.ВКЛ").Set((int)1);
            element.LookupParameter("ПР1.Отступ").SetValueString(CalculateOffset(window).ToString());
            element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
            element.LookupParameter("ПР1.Высота").SetValueString(appHeigth.ToString());
            element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
            transaction.Commit();

        }

       
        private double CalculateOffset(Element window)
        {
            FamilyInstance familyInstance = window as FamilyInstance;
            FamilySymbol familySymbol = familyInstance.Symbol;

            double appHalfWidth = Convert.ToDouble(familySymbol.LookupParameter("Примерная ширина").AsValueString())*0.5;

            Element hostWall = familyInstance.Host;

            LocationCurve hostWallCurve = (LocationCurve)hostWall.Location;
            Line line = (Line)hostWallCurve.Curve;


            LocationPoint windowPoint = (LocationPoint)window.Location;

            XYZ windowXYZ = windowPoint.Point;

            XYZ hostWallStart2 = line.GetEndPoint(0);
            XYZ hostWallEnd = line.GetEndPoint(1);

            double mmLen3 = Math.Abs(UnitUtils.ConvertFromInternalUnits(CalculateAxialLength(hostWallEnd, windowXYZ), UnitTypeId.Millimeters));


            return RoundToOnes(mmLen3 - appHalfWidth-80);
        }

        private double RoundToOnes(double value)
        {
            double remainder = value % 1;

            if (remainder >= 0.5)
            {
                value = value + (1 - remainder);
            }
            else
            {
                value = value - remainder;
            }
            return value;
        }

    }
}

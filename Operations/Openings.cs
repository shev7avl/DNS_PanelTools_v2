using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using DNS_PanelTools_v2.StructuralApps;

namespace DNS_PanelTools_v2.Operations
{
    public static class Openings
    {
        public static void PlaceFacadeOpening(Document activeDocument, XYZ locationPoint, Element window, Element elementHost)
        {
            IEnumerable<Element> familySymbols = new FilteredElementCollector(activeDocument).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().Where(o => o.Name.Contains("DNS_ПроемДляПлитки"));
            FamilySymbol familySymbol = (FamilySymbol)familySymbols.First();

            FamilyInstance windowFamInst = window as FamilyInstance;
            Element hostWall = windowFamInst.Host;
            Level level = activeDocument.GetElement(hostWall.LevelId) as Level;


            using (Transaction transaction = new Transaction(activeDocument, "Create window"))
            {
                transaction.Start();
                activeDocument.Create.NewFamilyInstance(locationPoint, familySymbol, elementHost, level, StructuralType.NonStructural);
                transaction.Commit();
            }
        }

        public static void FindIntersectedWindows_Arch(Document activeARCH, Element facadeWall, out List<Element> IntersectedWindows)
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(activeARCH);
            List<Element> windows = archDoc.getWindows();

            IntersectedWindows = new List<Element>();

            LocationCurve location = (LocationCurve)facadeWall.Location;
            Curve curve = location.Curve;

            XYZ Start = curve.GetEndPoint(0);
            XYZ End = curve.GetEndPoint(1);

            Options options = new Options();

            foreach (Element window in windows)
            {
                FamilyInstance windowFamInst = window as FamilyInstance;
                Element hostWall = windowFamInst.Host;
                BoundingBoxXYZ boundingBoxHostWall = hostWall.get_Geometry(options).GetBoundingBox();
                if (Geometry.IsPointInsideBbox(boundingBoxHostWall, Start) || Geometry.IsPointInsideBbox(boundingBoxHostWall, End))
                {                 
                    IntersectedWindows.Add(window);
                }
            }

        }

        public static void FindIntersectedWindows_Struct(Document activeARCH, Element element, out List<Element> windows)
        {
            windows = Geometry.FindPointIntersections(element, activeARCH, BuiltInCategory.OST_Windows, "DNS_");
        }

        public static void SetOpeningParams(Document document, Element element, Element window1)
        {

            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);

            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();
            element.LookupParameter("ПР1.ВКЛ").Set((int)1);
            element.LookupParameter("ПР1.Отступ").SetValueString(CalculateOffset(window1).ToString());
            element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
            element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
            element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
            transaction.Commit();

        }

        public static void SetOpeningParams(Document document, Element element, Element window1, Element window2)
        {

            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            GetOpeningParams(window2, out double appWidth2, out double appHeight2, out double offsetZ2);

            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();
            element.LookupParameter("ПР1.ВКЛ").Set((int)1);
            element.LookupParameter("ПР1.Отступ").SetValueString(CalculateOffset(window1).ToString());
            element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
            element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
            element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

            element.LookupParameter("ПР2.ВКЛ").Set((int)1);
            element.LookupParameter("ПР2.Отступ").SetValueString(CalculateOffset(window2).ToString());
            element.LookupParameter("ПР2.Ширина").SetValueString(appWidth2.ToString());
            element.LookupParameter("ПР2.Высота").SetValueString(appHeight2.ToString());
            element.LookupParameter("ПР2.ВысотаСмещение").SetValueString(offsetZ2.ToString());
            transaction.Commit();

        }

        public static void GetOpeningParams(Element window, out double appWidth, out double appHeigth, out double offsetZ)
        {
            FamilyInstance familyInstanceWin = window as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;
            appWidth = Convert.ToDouble(familySymbolWin.LookupParameter("Ширина").AsValueString());
            appHeigth = Convert.ToDouble(familySymbolWin.LookupParameter("Примерная высота").AsValueString());
            offsetZ = Convert.ToDouble(window.LookupParameter("Высота нижнего бруса").AsValueString());
        }

        public static double CalculateOffset(Element window)
        {
            FamilyInstance familyInstance = window as FamilyInstance;
            FamilySymbol familySymbol = familyInstance.Symbol;

            double appHalfWidth = Convert.ToDouble(familySymbol.LookupParameter("Примерная ширина").AsValueString()) * 0.5;

            Element hostWall = familyInstance.Host;

            LocationCurve hostWallCurve = (LocationCurve)hostWall.Location;
            Line line = (Line)hostWallCurve.Curve;


            LocationPoint windowPoint = (LocationPoint)window.Location;

            XYZ windowXYZ = windowPoint.Point;

            XYZ hostWallStart2 = line.GetEndPoint(0);
            XYZ hostWallEnd = line.GetEndPoint(1);

            double mmLen3 = Math.Abs(UnitUtils.ConvertFromInternalUnits(CalculateAxialLength(hostWallEnd, windowXYZ), UnitTypeId.Millimeters));


            return RoundToOnes(mmLen3 - appHalfWidth - 80);
        }

        public static double RoundToOnes(double value)
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

        public static double CalculateAxialLength(XYZ pointA, XYZ pointB)
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

    }
}

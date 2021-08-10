using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using DSKPrim.PanelTools_v2.StructuralApps;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Openings
    {

        #region Structural Opening


        #region Structural Utility
        public static void SetOpeningParams(Document document, RevitLinkInstance revitLink, Element element, Element window1)
        {

            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            CalculateOffset(revitLink, element, window1, out double offsetLine);
            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();

            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(options);

            element.LookupParameter("ПР1.ВКЛ").Set((int)1);
            element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
            element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
            element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
            element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

            Debug.WriteLine($"-- Панель {element.Name}: {element.Id} --");

            Debug.WriteLine($"ПР1.Отступ: {offsetLine} --");
            Debug.WriteLine($"ПР1.Ширина: {appWidth} --");
            Debug.WriteLine($"ПР1.Высота: {appHeight} --");
            Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ} --");


            Debug.WriteLine($"-------------");

            transaction.Commit();

        }

        public static void SetOpeningParams(Document document, RevitLinkInstance revitLink, Element element, Element window1, Element window2)
        {


            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            GetOpeningParams(window2, out double appWidth2, out double appHeight2, out double offsetZ2);

            CalculateOffset(revitLink, element, window1, out double offsetLine);
            CalculateOffset(revitLink, element, window2, out double offsetLine2);

            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();

            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(options);

            if (offsetLine == offsetLine2)
            {


                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

                Debug.WriteLine($"-- Панель {element.Name}: {element.Id} --");

                Debug.WriteLine($"ПР1.Отступ: {offsetLine} --");
                Debug.WriteLine($"ПР1.Ширина: {appWidth} --");
                Debug.WriteLine($"ПР1.Высота: {appHeight} --");
                Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ} --");

                Debug.WriteLine($"-------------");

                
            }

            else
            {

                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

                element.LookupParameter("ПР2.ВКЛ").Set((int)1);
                element.LookupParameter("ПР2.Отступ").SetValueString(offsetLine2.ToString());
                element.LookupParameter("ПР2.Ширина").SetValueString(appWidth2.ToString());
                element.LookupParameter("ПР2.Высота").SetValueString(appHeight2.ToString());
                element.LookupParameter("ПР2.ВысотаСмещение").SetValueString(offsetZ2.ToString());

                Debug.WriteLine($"--[2 окна] Панель {element.Name}: {element.Id} --");

                Debug.WriteLine($"ПР1.Отступ: {offsetLine}");
                Debug.WriteLine($"ПР1.Ширина: {appWidth}");
                Debug.WriteLine($"ПР1.Высота: {appHeight}");
                Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ}");

                Debug.WriteLine($"ПР2.Отступ: {offsetLine2}");
                Debug.WriteLine($"ПР2.Ширина: {appWidth2}");
                Debug.WriteLine($"ПР2.Высота: {appHeight2}");
                Debug.WriteLine($"ПР2.ВысотаСмещение: {offsetZ2}");

                Debug.WriteLine($"-------------");


            }
            transaction.Commit();

        }

        public static void SetOpeningParams(Document document, RevitLinkInstance revitLink, Element element, Element window1, Element window2, Element window3)
        {


            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            GetOpeningParams(window2, out double appWidth2, out double appHeight2, out double offsetZ2);

            CalculateOffset(revitLink, element, window1, out double offsetLine);
            CalculateOffset(revitLink, element, window2, out double offsetLine2);

            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();

            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(options);

            if (offsetLine == offsetLine2)
            {


                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

                Debug.WriteLine($"-- Панель {element.Name}: {element.Id} --");

                Debug.WriteLine($"ПР1.Отступ: {offsetLine} --");
                Debug.WriteLine($"ПР1.Ширина: {appWidth} --");
                Debug.WriteLine($"ПР1.Высота: {appHeight} --");
                Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ} --");

                Debug.WriteLine($"-------------");


            }

            else
            {

                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());

                element.LookupParameter("ПР2.ВКЛ").Set((int)1);
                element.LookupParameter("ПР2.Отступ").SetValueString(offsetLine2.ToString());
                element.LookupParameter("ПР2.Ширина").SetValueString(appWidth2.ToString());
                element.LookupParameter("ПР2.Высота").SetValueString(appHeight2.ToString());
                element.LookupParameter("ПР2.ВысотаСмещение").SetValueString(offsetZ2.ToString());

                Debug.WriteLine($"--[2 окна] Панель {element.Name}: {element.Id} --");

                Debug.WriteLine($"ПР1.Отступ: {offsetLine}");
                Debug.WriteLine($"ПР1.Ширина: {appWidth}");
                Debug.WriteLine($"ПР1.Высота: {appHeight}");
                Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ}");

                Debug.WriteLine($"ПР2.Отступ: {offsetLine2}");
                Debug.WriteLine($"ПР2.Ширина: {appWidth2}");
                Debug.WriteLine($"ПР2.Высота: {appHeight2}");
                Debug.WriteLine($"ПР2.ВысотаСмещение: {offsetZ2}");

                Debug.WriteLine($"-------------");


            }
            transaction.Commit();

        }

        #endregion

        #endregion

        #region Arch Opening
        public static void CreateFacadeOpening(Document activeDocument, XYZ locationPoint, Element window, Element elementHost)
        {
            
        }

        public static void GetWindows_Arch(Document activeARCH, Element facadeWall, out List<Element> IntersectedWindows)
        {
            SingleArchDoc archDoc = SingleArchDoc.getInstance(activeARCH);
            List<Element> windows = archDoc.Windows;

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
                if (Geometry.InBox(boundingBoxHostWall, Start) || Geometry.InBox(boundingBoxHostWall, End))
                {
                    IntersectedWindows.Add(window);
                }
            }

        }

        #endregion

        #region Utility
        private static void GetOpeningParams(Element window, out double appWidth, out double appHeigth, out double offsetZ)
        {
            FamilyInstance familyInstanceWin = window as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;
            try
            {
                appWidth = Convert.ToDouble(familySymbolWin.LookupParameter("Ширина").AsValueString());
            }
            catch (NullReferenceException)
            {
                appWidth = Convert.ToDouble(window.LookupParameter("Ширина").AsValueString());
            }

            try
            {
                appHeigth = Convert.ToDouble(familySymbolWin.LookupParameter("Высота").AsValueString());
            }
            catch (NullReferenceException)
            {
                appHeigth = Convert.ToDouble(window.LookupParameter("Высота").AsValueString());
            }

            try
            {
                offsetZ = Convert.ToDouble(window.LookupParameter("Высота нижнего бруса").AsValueString())+80;
            }
            catch (NullReferenceException)
            {
                offsetZ = Convert.ToDouble(familySymbolWin.LookupParameter("Высота нижнего бруса").AsValueString())+80;
            }
            
        }

        public static void CalculateOffset(RevitLinkInstance revitLink, Element hostPanel, Element window, out double offset)
        {
            FamilyInstance familyInstance = window as FamilyInstance;
            FamilySymbol familySymbol = familyInstance.Symbol;
            double appHalfWidth;
            try
            {
                appHalfWidth = Convert.ToDouble(familySymbol.LookupParameter("Ширина").AsValueString()) * 0.5;
            }
            catch (NullReferenceException)
            {
                appHalfWidth = Convert.ToDouble(window.LookupParameter("Ширина").AsValueString()) * 0.5;
            }

            XYZ linkOriginPoint = revitLink.GetTransform().Origin;

            LocationPoint windowPoint = (LocationPoint)window.Location;
            LocationPoint panelPoint = (LocationPoint)hostPanel.Location;

            XYZ windowXYZ = windowPoint.Point + linkOriginPoint;
            XYZ panelXYZ = panelPoint.Point;

            double mmLen = Math.Abs(UnitUtils.ConvertFromInternalUnits(CalculateAxialLength(panelXYZ, windowXYZ), UnitTypeId.Millimeters));

            offset = RoundToOnes(mmLen - appHalfWidth);
            RoundToStep(offset, 300, out offset);
        }

        private static void RoundToStep(double value, double step, out double result)
        {
            double remainder = value % step;
            if (remainder >= 0.5*step)
            {
                result = value + (step - remainder);
            }
            else
            {
                result = value - remainder;
            }
        }

        private static double RoundToOnes(double value)
        {
            double remainder = value % 1;

            if (remainder >= 0.5)
            {
                value += (1 - remainder);
            }
            else
            {
                value -= remainder;
            }
            return value;
        }

        private static double CalculateAxialLength(XYZ pointA, XYZ pointB)
        {
            double aX = pointA.X;
            double aY = pointA.Y;
            //double aZ = pointA.Z;

            double bX = pointB.X;
            double bY = pointB.Y;
            //double bZ = pointB.Z;

            double dX = (aX - bX);
            double dY = (aY - bY);

            if (Math.Abs(dX) < Math.Abs(dY))
            {
                return dY;
            }
            else
            {
                return dX;
            }

        }
        #endregion









    }
}

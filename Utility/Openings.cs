using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace DSKPrim.PanelTools.Utility
{
    public static class Openings
    {

        #region Structural Opening


        #region Structural Utility
        public static void SetOpeningParams(Document document, RevitLinkInstance revitLink, Element element, Element window1)
        {

            Debug.WriteLine("Вызван статический метод Utility.Openings.SetOpeningParams()");
            Debug.WriteLine($"Назначаем параметры проема элементу {element.Id}");

            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            CalculateOffset(revitLink, element, window1, out double offsetLine);
            Transaction transaction = new Transaction(document, "Test_opening");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            transaction.Start();

            try
            {
                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
                Debug.WriteLine($"Успешно");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                throw;
            }

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

            Debug.WriteLine("Вызван статический метод Utility.Openings.SetOpeningParams()");
            Debug.WriteLine($"Назначаем параметры проема элементу {element.Id}");


            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            GetOpeningParams(window2, out double appWidth2, out double appHeight2, out double offsetZ2);

            CalculateOffset(revitLink, element, window1, out double offsetLine);
            CalculateOffset(revitLink, element, window2, out double offsetLine2);

            Transaction transaction = new Transaction(document, "Test_opening");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            transaction.Start();

            if (offsetLine == offsetLine2)
            {

                try
                {
                    element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                    element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                    element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                    element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                    element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
                    Debug.WriteLine($"Успешно");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                    throw;
                }

                Debug.WriteLine($"-- Панель {element.Name}: {element.Id} --");

                Debug.WriteLine($"ПР1.Отступ: {offsetLine} --");
                Debug.WriteLine($"ПР1.Ширина: {appWidth} --");
                Debug.WriteLine($"ПР1.Высота: {appHeight} --");
                Debug.WriteLine($"ПР1.ВысотаСмещение: {offsetZ} --");

                Debug.WriteLine($"-------------");
                
            }

            else
            {
                try
                {
                    if (offsetLine2 > offsetLine)
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
                    }
                    else
                    {
                        element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                        element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine2.ToString());
                        element.LookupParameter("ПР1.Ширина").SetValueString(appWidth2.ToString());
                        element.LookupParameter("ПР1.Высота").SetValueString(appHeight2.ToString());
                        element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ2.ToString());

                        element.LookupParameter("ПР2.ВКЛ").Set((int)1);
                        element.LookupParameter("ПР2.Отступ").SetValueString(offsetLine.ToString());
                        element.LookupParameter("ПР2.Ширина").SetValueString(appWidth.ToString());
                        element.LookupParameter("ПР2.Высота").SetValueString(appHeight.ToString());
                        element.LookupParameter("ПР2.ВысотаСмещение").SetValueString(offsetZ.ToString());
                    }
                    
                    Debug.WriteLine($"Успешно");
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                    throw;
                }


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
        /// <summary>
        /// Создаём по подгруженной связи КР проемы в фасадных стенах
        /// </summary>
        /// <param name="activeDocument">Активный документ АР</param>
        /// <param name="elementHost">Объект стены в который будет вставояться окно</param>
        public static void CreateFacadeOpening(Document activeDocument, Element elementHost)
        {
            Element panel;
            SortedList<string, List<double>> WindowsWithParameters;
            CreateWindowsData(activeDocument, elementHost, out panel, out WindowsWithParameters);

            if (WindowsWithParameters.Keys.Count > 0)
            {
                if (WindowsWithParameters.Keys.Contains("ПР1.ВКЛ: 1"))
                {
                    PlaceWindow(activeDocument, elementHost, panel, WindowsWithParameters);
                }
                if (WindowsWithParameters.Keys.Contains("ПР2.ВКЛ: 1"))
                {
                    PlaceWindow(activeDocument, elementHost, panel, WindowsWithParameters, win1: false);
                }
            }


            // TODO: придумать как в нужном месте сделать вырез
        }

        private static void CreateWindowsData(Document activeDocument, Element elementHost, out Element panel, out SortedList<string, List<double>> WindowsWithParameters)
        {
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(activeDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР")|| doc.Name.Contains("_КЖ"));

            List<RevitLinkInstance> linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList();

            List<Document> LinkedStructs = new List<Document>();

            foreach (var item in linkedDocSTR)
            {
                LinkedStructs.Add(item.GetLinkDocument());
            }

            panel = default;

            List<double> ParValues;

            ElementIntersectsElementFilter panelFilter = new ElementIntersectsElementFilter(elementHost);
            

            foreach (var item in LinkedStructs)
            {
                List<FamilySymbol> nsFamSymbol = new FilteredElementCollector(item).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                ElementId nsFamId = nsFamSymbol.Where(o => o.FamilyName.Contains("NS_Empty")).FirstOrDefault().Id;

                FamilyInstanceFilter typeFilter = new FamilyInstanceFilter(item, nsFamId);
                panel = new FilteredElementCollector(item).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(panelFilter).WherePasses(typeFilter).FirstOrDefault();
                if (panel != null)
                {
                    break;
                }
            }

            WindowsWithParameters = new SortedList<string, List<double>>();
            if (panel != null)
            {
                ParValues = new List<double>
                {
                    Double.Parse(panel.LookupParameter("ПР1.Отступ").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР1.Ширина").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР1.Высота").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР1.ВысотаСмещение").AsValueString()),
                    Double.Parse(panel.LookupParameter("Выступ_Старт").AsValueString()),
                    Double.Parse(panel.LookupParameter("СН").AsValueString())
                };
                Debug.WriteLine($"{panel.Id}  --  ПР1.ВКЛ: {panel.LookupParameter("ПР1.ВКЛ").AsInteger()}");

                WindowsWithParameters.Add($"ПР1.ВКЛ: {panel.LookupParameter("ПР1.ВКЛ").AsInteger()}", ParValues);

                ParValues = new List<double>
                {
                    Double.Parse(panel.LookupParameter("ПР2.Отступ").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.Ширина").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.Высота").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.ВысотаСмещение").AsValueString()),
                    Double.Parse(panel.LookupParameter("Выступ_Старт").AsValueString()),
                    Double.Parse(panel.LookupParameter("СН").AsValueString())
                };
                Debug.WriteLine($"{panel.Id}  --  ПР2.ВКЛ: {panel.LookupParameter("ПР2.ВКЛ").AsInteger()}");
                WindowsWithParameters.Add($"ПР2.ВКЛ: {panel.LookupParameter("ПР2.ВКЛ").AsInteger()}", ParValues);
            }
            
        }

        private static void PlaceWindow(Document activeDocument, Element elementHost, Element panel, SortedList<string, List<double>> WindowsWithParameters, bool win1 = true)
        {
            ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elementHost);
            List<ElementId> windows = new FilteredElementCollector(activeDocument).OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().WherePasses(filter).ToElementIds().ToList();

            if (windows.Count == 0)
            {
                List<double> tempValues = new List<double>();
                if (win1)
                {
                    tempValues = WindowsWithParameters["ПР1.ВКЛ: 1"];
                }
                else
                {
                    tempValues = WindowsWithParameters["ПР2.ВКЛ: 1"];
                }


                GeometryElement geometryObject = elementHost.get_Geometry(new Options());
                Solid geomSolid = null;
                
                //TODO: Везде по коду заменить вот этот цикл на коллекцию солидов
                foreach (GeometryObject item in geometryObject)
                {
                    if (item is Solid solid)
                    {
                        geomSolid = solid;
                    }
                }
                FaceArray faceArray = geomSolid.Faces;

                Face face = faceArray.get_Item(0);
                for (int i = 1; i < 5; i++)
                {
                    if (faceArray.get_Item(i).Area > face.Area)
                    {
                        face = faceArray.get_Item(i);
                    }
                }

                LocationCurve wallCurve = (LocationCurve)elementHost.Location;
                XYZ startWall = wallCurve.Curve.GetEndPoint(1);

                Line directionalBasis = (Line)wallCurve.Curve;

                XYZ newPoint = default;
                FamilyInstance panelFI = (FamilyInstance)panel;
                if (Math.Abs(panelFI.HandOrientation.X) == 1)
                {
                    double deltaAxis = UnitUtils.ConvertToInternalUnits(tempValues[0]+tempValues[4]-tempValues[5]-6, DisplayUnitType.DUT_MILLIMETERS) + 0.5 * UnitUtils.ConvertToInternalUnits(tempValues[1], DisplayUnitType.DUT_MILLIMETERS);
                    double newX = startWall.X - deltaAxis * directionalBasis.Direction.X;
                    double newY = startWall.Y;
                    double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3]-80, DisplayUnitType.DUT_MILLIMETERS);

                    newPoint = new XYZ(newX, newY, newZ);
                }
                else if (Math.Abs(panelFI.HandOrientation.Y) == 1)
                {
                    double deltaAxis = UnitUtils.ConvertToInternalUnits(tempValues[0] + tempValues[4] - tempValues[5] - 6, DisplayUnitType.DUT_MILLIMETERS) + 0.5 * UnitUtils.ConvertToInternalUnits(tempValues[1], DisplayUnitType.DUT_MILLIMETERS);
                    double newY = startWall.Y - deltaAxis * directionalBasis.Direction.Y;
                    double newX = startWall.X;
                    double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3] - 80, DisplayUnitType.DUT_MILLIMETERS);

                    newPoint = new XYZ(newX, newY, newZ);
                }

                try
                {
                    XYZ pointOnFace = face.Project(newPoint).XYZPoint;

                    FamilySymbol windowSymbol = new FilteredElementCollector(activeDocument).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().Where(o => o.Name == "DNS_ПроемДляПлитки").First();

                    using (Transaction t = new Transaction(activeDocument, "Create window"))
                    {
                        t.Start();

                        if (!windowSymbol.IsActive)
                        {
                            // Ensure the family symbol is activated.
                            windowSymbol.Activate();
                            activeDocument.Regenerate();
                        }

                        // Create window
                        // unliss you specified a host, Rebit will create the family instance as orphabt object.
                        FamilyInstance window = activeDocument.Create.NewFamilyInstance(pointOnFace, windowSymbol, elementHost, StructuralType.NonStructural);
                        t.Commit();

                        t.Start();
                        activeDocument.Regenerate();
                        double height = tempValues[2] + 20;
                        window.get_Parameter(BuiltInParameter.DOOR_HEIGHT).SetValueString(height.ToString());
                        window.get_Parameter(BuiltInParameter.FURNITURE_WIDTH).SetValueString(tempValues[1].ToString());
                        t.Commit();
                    }
                }
                catch (Exception)
                {

                }
            }

            
        }

        #endregion

        #region Utility
        private static void GetOpeningParams(Element window, out double appWidth, out double appHeigth, out double offsetZ)
        {
            FamilyInstance familyInstanceWin = window as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;


            Debug.WriteLine("Вызван статический метод Utility.Openings.GetOpeningParams()");

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


            Debug.WriteLine("Вызван статический метод Utility.Openings.CalculateOffset()");
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

            double mmLen = Math.Abs(UnitUtils.ConvertFromInternalUnits(CalculateAxialLength(panelXYZ, windowXYZ), DisplayUnitType.DUT_MILLIMETERS));

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

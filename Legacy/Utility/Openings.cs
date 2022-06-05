using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using DSKPrim.PanelTools.Facade;
using DSKPrim.PanelTools.ProjectEnvironment;

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

            WindowParameter[] windowParameters = CreateWindowsData(activeDocument, elementHost);

            foreach (var window in windowParameters)
            {
                if (window.Exists)
                {
                    PlaceWindow(activeDocument, elementHost, window);
                }

            }

        }

        private static WindowParameter[] CreateWindowsData(Document activeDocument, Element elementHost)
        {
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(activeDocument).
                OfClass(typeof(RevitLinkInstance)).
                WhereElementIsNotElementType().
                Where(doc => doc.Name.Contains("_КР") || doc.Name.Contains("_КЖ"));

            List<RevitLinkInstance> linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList();

            List<Document> LinkedStructs = new List<Document>();

            foreach (var item in linkedDocSTR)
            {
                if (item.GetLinkDocument() != null)
                {
                    LinkedStructs.Add(item.GetLinkDocument());
                }
            }

            Element panel = default;

            ElementIntersectsElementFilter panelFilter = new ElementIntersectsElementFilter(elementHost);

            Transform activeDocTransform = activeDocument.ActiveProjectLocation.GetTotalTransform();

            string NSPanelName = "NS_Empty";

            foreach (var item in LinkedStructs)
            {
                Transform linkTransform = item.ActiveProjectLocation.GetTotalTransform();

                FilteredElementCollector panelCollector = new FilteredElementCollector(item).
                        OfCategory(BuiltInCategory.OST_StructuralFraming).
                        OfClass(typeof(FamilyInstance));

                    panel = panelCollector.
                        WherePasses(panelFilter).
                        Cast<FamilyInstance>().
                        Where(x => x.Symbol.FamilyName.Contains(NSPanelName)).
                        FirstOrDefault();

                if (panel != null)
                {
                    break;
                }
                else
                {
                    throw new NullReferenceException(message: $"Не удалось найти экземпляр семейства {NSPanelName}" +
                            $" для данной стены, ID : {elementHost.Id}. Проверьте наименования семейств и убедитесь, что " +
                            "координаты получены из связи КР");
                } 
            }

            WindowParameter[] windowParameters = new WindowParameter[2];

            windowParameters[0] = new WindowParameter(panel, 1);
            windowParameters[1] = new WindowParameter(panel, 2);

            return windowParameters;
            
        }

        private static void PlaceWindow(Document activeDocument, Element elementHost, WindowParameter window)
        {
            ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elementHost);
            List<ElementId> windows = new FilteredElementCollector(activeDocument).
                OfCategory(BuiltInCategory.OST_Windows).WhereElementIsElementType().WherePasses(filter).ToElementIds().ToList();

            if (windows.Count == 0)
            {
                double[] tempValues = window.ParameterValues;

                FacadeDescription facade = new FacadeDescription(activeDocument, elementHost.Id, CreateParts: false);
                XYZ insertionPoint = CalculateInsertionPoint(facade, tempValues);

                try
                {

                    XYZ pointOnFace = facade.PlanarFace.Project(insertionPoint).XYZPoint;

                    FamilySymbol windowSymbol = new FilteredElementCollector(activeDocument).
                        OfClass(typeof(FamilySymbol)).
                        Cast<FamilySymbol>().
                        Where(o => o.Name == "DNS_ПроемДляПлитки").First();

                    using (Transaction t = new Transaction(activeDocument, "Create window"))
                    {
                        t.Start();

                        if (!windowSymbol.IsActive)
                        {
                            windowSymbol.Activate();
                            activeDocument.Regenerate();
                        }

                        FamilyInstance windowInstance = activeDocument.Create.
                            NewFamilyInstance(
                            pointOnFace,
                            windowSymbol,
                            elementHost,
                            level: (Level)activeDocument.GetElement(elementHost.LevelId),
                            StructuralType.NonStructural);

                        t.Commit();

                        t.Start();
                        activeDocument.Regenerate();
                        double height = tempValues[2];
                        windowInstance.get_Parameter(BuiltInParameter.DOOR_HEIGHT).SetValueString(height.ToString());
                        windowInstance.get_Parameter(BuiltInParameter.FURNITURE_WIDTH).SetValueString(tempValues[1].ToString());
                        t.Commit();
                    }
                }
                catch (Exception)
                {

                }
            }

            else
            {
                string message = "В панели уже есть проёмы. Удалите их и попробуйте заново";
                throw new InvalidOperationException(message);
            }

        }

        private static XYZ CalculateInsertionPoint(FacadeDescription facade, double[] tempValues)
        {
            
            LocationCurve wallCurve = (LocationCurve)facade.WallElement.Location;

            double wallOffset = Double.Parse(facade.WallElement.get_Parameter(BuiltInParameter.WALL_BASE_OFFSET).AsValueString());

            XYZ startWall = wallCurve.Curve.GetEndPoint(1);
            XYZ newPoint = default;
            double deltaAxis = UnitUtils.ConvertToInternalUnits(tempValues[0] + tempValues[4] - tempValues[5], DisplayUnitType.DUT_MILLIMETERS)
                    + 0.5 * UnitUtils.ConvertToInternalUnits(tempValues[1], DisplayUnitType.DUT_MILLIMETERS);


            if (Math.Abs(facade.DirectionalBasis.X) == 1)
            {
                double newX = startWall.X - deltaAxis * facade.DirectionalBasis.X;
                double newY = startWall.Y;
                double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3] + wallOffset, DisplayUnitType.DUT_MILLIMETERS);

                newPoint = new XYZ(newX, newY, newZ);
            }
            else if (Math.Abs(facade.DirectionalBasis.Y) == 1)
            {
                double newY = startWall.Y - deltaAxis * facade.DirectionalBasis.Y;
                double newX = startWall.X;
                double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3] + wallOffset, DisplayUnitType.DUT_MILLIMETERS);

                newPoint = new XYZ(newX, newY, newZ);
            }

            return newPoint;
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

            AddinSettings settings = AddinSettings.GetSettings();
            TileModule tileModule = settings.GetTileModule();
            double step = tileModule.ModuleWidth + tileModule.ModuleGap;

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

            RoundToStep(offset, step, out offset);
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

    internal class WindowParameter
    {

        internal int Number;

        internal bool Exists;

        internal double[] ParameterValues;

        /// <param name="panel">Элемент панели из связи</param>
        /// <param name="number">Номер проема в формате ПР1 или ПР2</param>
        internal WindowParameter(Element panel, int number)
        {
            Number = number;
            Exists = false;
            ParameterValues = new double[6];

            ParameterMap parameterMap = panel.ParametersMap;

            if (parameterMap.get_Item($"ПР{Number}.ВКЛ").AsInteger() == 1)
            {
                Exists = true;
                ParameterValues[0] = Double.Parse(parameterMap.get_Item($"ПР{Number}.Отступ").AsValueString());
                ParameterValues[1] = Double.Parse(parameterMap.get_Item($"ПР{Number}.Ширина").AsValueString());
                ParameterValues[2] = Double.Parse(parameterMap.get_Item($"ПР{Number}.Высота").AsValueString());
                ParameterValues[3] = Double.Parse(parameterMap.get_Item($"ПР{Number}.ВысотаСмещение").AsValueString());
                ParameterValues[4] = Double.Parse(parameterMap.get_Item("Выступ_Старт").AsValueString());
                ParameterValues[5] = Double.Parse(parameterMap.get_Item("СН").AsValueString());
            }        
        }

    }
}

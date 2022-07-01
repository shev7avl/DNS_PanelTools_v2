using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using DSKPrim.PanelTools.Facade;
using DSKPrim.PanelTools.Legacy.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;

namespace DSKPrim.PanelTools.Utility
{

    public static class Openings
    {

        #region Structural Opening


        #region Structural Utility

        public static Window UpdateWindow(Element element, Element windowElement)
        {

            Window window = new Window(element);
            window.OffsetXY = CalculateOffset(element, windowElement);

            return window;
        }

        public static void SetWindowParameters(ParameterMap map, Window window, int index = 1)
        {
            if (index > 2) throw new ArgumentException("--> Index is too big");
            if (index < 1) throw new ArgumentException("--> Index is too small");

            string windowPrefix = $"ПР{index}";
            try
            {
                map.get_Item($"{windowPrefix}.ВКЛ").Set((int)1);
                map.get_Item($"{windowPrefix}.Отступ").SetValueString(window.OffsetXY.ToString());
                map.get_Item($"{windowPrefix}.Ширина").SetValueString(window.Width.ToString());
                map.get_Item($"{windowPrefix}.Высота").SetValueString(window.Height.ToString());
                map.get_Item($"{windowPrefix}.ВысотаСмещение").SetValueString(window.OffsetZ.ToString());
                Debug.WriteLine($"Успешно");
            }
            catch (Exception e)
            {
                Debug.WriteLine($"--> Cannot create window: {e.Message}");
                throw;
            }
        }

        public static void SetOpeningParams(Element element, List<Element> windows)
        {

            FamilyInstance instance = element as FamilyInstance;
            ParameterMap map = instance.ParametersMap;

            List<Window> windowsList = windows.Select(o => UpdateWindow(element, o)).ToList();

            Window window1 = default;
            Window window2 = default;

            if (windowsList.Count == 1)
            {
                window1 = windowsList[0];

            }
            if (windowsList.Count == 2)
            {
                window1 = windowsList[0];
                window2 = windowsList[1];

                if (window1.OffsetXY > window2.OffsetXY)
                {
                    var temp = window2;
                    window2 = window1;
                    window1 = temp;
                }
                if (window1.OffsetXY == window2.OffsetXY)
                {
                    window2 = null;
                }
            }
            if (windowsList.Count > 2) throw new ArgumentException($"--> Too many windows ({windows.Count})");


            Transaction transaction = new Transaction(element.Document, "Test_opening");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            transaction.Start();
            if (window1 != null) SetWindowParameters(map, window1, index: 1);
            transaction.Commit();

            transaction.Start();
            if (window2 != null) SetWindowParameters(map, window2, index: 2);
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

        public static double CalculateOffset(Element hostPanel, Element window)
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

            //XYZ linkOriginPoint = revitLink.GetTransform().Origin;

            XYZ linkOriginPoint = window.Document.ActiveProjectLocation.GetTransform().Origin;

            LocationPoint windowPoint = (LocationPoint)window.Location;
            LocationPoint panelPoint = (LocationPoint)hostPanel.Location;

            XYZ windowXYZ = windowPoint.Point + linkOriginPoint;
            XYZ panelXYZ = panelPoint.Point;

            double mmLen = Math.Abs(UnitUtils.ConvertFromInternalUnits(CalculateAxialLength(panelXYZ, windowXYZ), DisplayUnitType.DUT_MILLIMETERS));  
            double offset = Math.Round(mmLen - appHalfWidth);

            return RoundToStep(offset, step);
        }
        
        private static double RoundToStep(double value, double step)
        {
            double remainder = value % step;
            if (remainder >= 0.5*step)
            {
                return value + (step - remainder);
            }
            else
            {
                return value - remainder;
            }
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

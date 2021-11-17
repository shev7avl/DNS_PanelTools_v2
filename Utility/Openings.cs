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
            Logger.Logger logger = Logger.Logger.getInstance();

            logger.Separate();
            logger.WriteLog("Вызван статический метод Utility.Openings.SetOpeningParams()");
            logger.WriteLog($"Назначаем параметры проема элементу {element.Id}");

            GetOpeningParams(window1, out double appWidth, out double appHeight, out double offsetZ);
            CalculateOffset(revitLink, element, window1, out double offsetLine);
            Transaction transaction = new Transaction(document, "Test_opening");
            transaction.Start();

            FailureHandlingOptions options = transaction.GetFailureHandlingOptions();
            options.SetFailuresPreprocessor(new WarningDiscard());
            transaction.SetFailureHandlingOptions(options);

            try
            {
                element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
                logger.WriteLog($"Успешно");
            }
            catch (Exception e)
            {
                logger.WriteLog($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                logger.LogError(e);
                throw;
            }

            logger.DebugLog($"-- Панель {element.Name}: {element.Id} --");

            logger.DebugLog($"ПР1.Отступ: {offsetLine} --");
            logger.DebugLog($"ПР1.Ширина: {appWidth} --");
            logger.DebugLog($"ПР1.Высота: {appHeight} --");
            logger.DebugLog($"ПР1.ВысотаСмещение: {offsetZ} --");

            logger.DebugLog($"-------------");

            transaction.Commit();

        }
        
        public static void SetOpeningParams(Document document, RevitLinkInstance revitLink, Element element, Element window1, Element window2)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            logger.Separate();
            logger.WriteLog("Вызван статический метод Utility.Openings.SetOpeningParams()");
            logger.WriteLog($"Назначаем параметры проема элементу {element.Id}");


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

                try
                {
                    element.LookupParameter("ПР1.ВКЛ").Set((int)1);
                    element.LookupParameter("ПР1.Отступ").SetValueString(offsetLine.ToString());
                    element.LookupParameter("ПР1.Ширина").SetValueString(appWidth.ToString());
                    element.LookupParameter("ПР1.Высота").SetValueString(appHeight.ToString());
                    element.LookupParameter("ПР1.ВысотаСмещение").SetValueString(offsetZ.ToString());
                    logger.WriteLog($"Успешно");
                }
                catch (Exception e)
                {
                    logger.WriteLog($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                    logger.LogError(e);
                    throw;
                }

                logger.DebugLog($"-- Панель {element.Name}: {element.Id} --");

                logger.DebugLog($"ПР1.Отступ: {offsetLine} --");
                logger.DebugLog($"ПР1.Ширина: {appWidth} --");
                logger.DebugLog($"ПР1.Высота: {appHeight} --");
                logger.DebugLog($"ПР1.ВысотаСмещение: {offsetZ} --");

                logger.DebugLog($"-------------");
                
            }

            else
            {
                try
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
                    logger.WriteLog($"Успешно");
                }
                catch (Exception e)
                {
                    logger.WriteLog($"В элементе ID: {element.Id} возникла ошибка {e.Message}");
                    logger.LogError(e);
                    throw;
                }


                logger.DebugLog($"--[2 окна] Панель {element.Name}: {element.Id} --");

                logger.DebugLog($"ПР1.Отступ: {offsetLine}");
                logger.DebugLog($"ПР1.Ширина: {appWidth}");
                logger.DebugLog($"ПР1.Высота: {appHeight}");
                logger.DebugLog($"ПР1.ВысотаСмещение: {offsetZ}");

                logger.DebugLog($"ПР2.Отступ: {offsetLine2}");
                logger.DebugLog($"ПР2.Ширина: {appWidth2}");
                logger.DebugLog($"ПР2.Высота: {appHeight2}");
                logger.DebugLog($"ПР2.ВысотаСмещение: {offsetZ2}");

                logger.DebugLog($"-------------");


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
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(activeDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));

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
                    Double.Parse(panel.LookupParameter("ПР1.ВысотаСмещение").AsValueString())
                };
                Debug.WriteLine($"{panel.Id}  --  ПР1.ВКЛ: {panel.LookupParameter("ПР1.ВКЛ").AsInteger()}");

                WindowsWithParameters.Add($"ПР1.ВКЛ: {panel.LookupParameter("ПР1.ВКЛ").AsInteger()}", ParValues);

                ParValues = new List<double>
                {
                    Double.Parse(panel.LookupParameter("ПР2.Отступ").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.Ширина").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.Высота").AsValueString()),
                    Double.Parse(panel.LookupParameter("ПР2.ВысотаСмещение").AsValueString())
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
                XYZ startWall = wallCurve.Curve.GetEndPoint(0);

                XYZ newPoint = default;
                FamilyInstance panelFI = (FamilyInstance)panel;
                if (Math.Abs(panelFI.HandOrientation.X) == 1)
                {
                    double deltaAxis = UnitUtils.ConvertToInternalUnits(tempValues[0], UnitTypeId.Millimeters) + 0.5 * UnitUtils.ConvertToInternalUnits(tempValues[1], UnitTypeId.Millimeters);
                    double newX = startWall.X + deltaAxis * panelFI.HandOrientation.X;
                    double newY = startWall.Y;
                    double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3], UnitTypeId.Millimeters);

                    newPoint = new XYZ(newX, newY, newZ);
                }
                else if (Math.Abs(panelFI.HandOrientation.Y) == 1)
                {
                    double deltaAxis = UnitUtils.ConvertToInternalUnits(tempValues[0], UnitTypeId.Millimeters) + 0.5 * UnitUtils.ConvertToInternalUnits(tempValues[1], UnitTypeId.Millimeters);
                    double newY = startWall.Y + deltaAxis * panelFI.HandOrientation.Y;
                    double newX = startWall.X;
                    double newZ = startWall.Z + UnitUtils.ConvertToInternalUnits(tempValues[3], UnitTypeId.Millimeters);

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
                        window.get_Parameter(BuiltInParameter.DOOR_HEIGHT).SetValueString(tempValues[2].ToString());
                        window.get_Parameter(BuiltInParameter.FURNITURE_WIDTH).SetValueString(tempValues[1].ToString());
                        t.Commit();
                    }
                }
                catch (Exception)
                {

                }
            }

            
        }

        private static int GetOrientation(Element element)
        {
            BoundingBoxXYZ boxXYZ = element.get_Geometry(new Options()).GetBoundingBox();

            double minX = boxXYZ.Min.X;
            double minY = boxXYZ.Min.Y;

            double maxX = boxXYZ.Max.X;
            double maxY = boxXYZ.Max.Y;

            //X orientation
            if (Math.Abs(maxX - minX) > Math.Abs(maxY - minY)) 
                return 1;
            //Y orientation
            else if (Math.Abs(maxX - minX) < Math.Abs(maxY - minY)) 
                return -1;
            //error
            else 
                return 0;
        }

        public static void CreateWindow(Document doc, string fsFamilyName,
            string fsName, string levelName, string xCoord, string yCoord)
        {
            // LINQ to find the window's FamilySymbol by its type name.
            FamilySymbol familySymbol = (from fs in new FilteredElementCollector(doc).
                 OfClass(typeof(FamilySymbol)).
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
        }

        
        #endregion

        #region Utility
        private static void GetOpeningParams(Element window, out double appWidth, out double appHeigth, out double offsetZ)
        {
            FamilyInstance familyInstanceWin = window as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;
            Logger.Logger logger = Logger.Logger.getInstance();

            logger.WriteLog("Вызван статический метод Utility.Openings.GetOpeningParams()");

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
            Logger.Logger logger = Logger.Logger.getInstance();

            logger.WriteLog("Вызван статический метод Utility.Openings.CalculateOffset()");
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

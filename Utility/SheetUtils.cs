using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Builders;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Collections.Generic;
using System.Linq;

namespace DSKPrim.PanelTools.Utility
{
    public static class SheetUtils
    {
        public static void PlaceViewsOnSheet(Document document, Sheet sheet, ViewReference viewReference)
        {
            if (viewReference.GetCreatedViewsWithCoords() != null)
            {
                foreach (var item in viewReference.GetCreatedViewsWithCoords().Keys)
                {
                    if (viewReference.ViewTemplate.ToString().Contains("SCHEDULE"))
                    {
                        ScheduleSheetInstance.Create(document, sheet.SheetLink.Id, item.Id, viewReference.GetCreatedViewsWithCoords()[item]);
                    }
                    else
                    {
                        if (Viewport.CanAddViewToSheet(document, sheet.SheetLink.Id, item.Id))
                        {
                            Viewport.Create(document, sheet.SheetLink.Id, item.Id, viewReference.GetCreatedViewsWithCoords()[item]);
                        }                     
                    }
                }
            }
        }

        public static List<Sheet> CreateSheetList(Document document, ElementId id, int quantity)
        {
            ViewsEnvironment views = ViewsEnvironment.getInstance(document);
            List<Sheet> sheets = new List<Sheet>();
            ElementId title_F4 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SHEET_FORM_4_TITLE);
            ElementId title_F6 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SHEET_FORM_6);

            for (int i = 1; i <= quantity; i++)
            {
                ElementId curTempId;
                if (i == 1)
                {
                    curTempId = title_F4;
                }
                else
                {
                    curTempId = title_F6;
                }
                ViewSheet viewSheet = AssemblyViewUtils.CreateSheet(document, id, curTempId);
                Sheet sheet = new Sheet(viewSheet, i);
                sheets.Add(sheet);
            }
            return sheets;
        }
        [System.Obsolete("Планируется использовать Sheet вместо ViewSheet")]
        public static void CreateSheets(Document document, ElementId id, int number, out List<ViewSheet> sheets)
        {
            ViewsEnvironment views = ViewsEnvironment.getInstance(document);
            sheets = new List<ViewSheet>();
            ElementId title_F4 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SHEET_FORM_4_TITLE);
            ElementId title_F6 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SHEET_FORM_6);

            for (int i = 0; i < number; i++)
            {
                ElementId curTempId;
                if (i == 0)
                {
                    curTempId = title_F4;
                }
                else
                {
                    curTempId = title_F6;
                }
                ViewSheet sheet = AssemblyViewUtils.CreateSheet(document, id, curTempId);
                sheets.Add(sheet);
            }
        }
        
        /// <summary>
        /// Method for rotating plan view.
        /// It rotates plan view according to the Front view orientation
        /// </summary>
        /// <param name="document">Active runtime document</param>
        /// <param name="viewSection">View to rotate</param>
        /// <param name="orientation">Front view orientation</param>
        public static void RotatePlanView(Document document, ViewSection viewSection, AssemblyDetailViewOrientation orientation)
        {
            View view = (View)viewSection;
            double rotationalAngle = 0;


            SubTransaction rotation = new SubTransaction(document);


            rotation.Start();
            view.CropBoxVisible = true;
            rotation.Commit();

            IList<Element> visibleElementsOnView = new FilteredElementCollector(document, view.Id).OfCategory(BuiltInCategory.OST_Viewers).WhereElementIsNotElementType().ToElements();
            Element cropBoxElement = visibleElementsOnView.Where(o => o.Name.Contains(viewSection.Name)).FirstOrDefault();

            BoundingBoxXYZ cropBoxBounding = cropBoxElement.get_BoundingBox(view);
            XYZ top = cropBoxBounding.Max;
            XYZ bottom = cropBoxBounding.Min;
            XYZ cropBoxCenter = 0.5 * (bottom + top);
            XYZ cropBoxCenterUp = new XYZ(cropBoxCenter.X, cropBoxCenter.Y, cropBoxCenter.Z + 1 );
            Line rotationalAxis = Line.CreateBound(cropBoxCenter, cropBoxCenterUp);

            rotation.Start();
            if (orientation == AssemblyDetailViewOrientation.ElevationLeft)
            {
                rotationalAngle = (3*System.Math.PI / 2);
            }
            else if (orientation == AssemblyDetailViewOrientation.ElevationRight)
            {
                rotationalAngle = (System.Math.PI / 2);
            }
            else if (orientation == AssemblyDetailViewOrientation.ElevationBack)
            {
                rotationalAngle = System.Math.PI;
            }
            ElementTransformUtils.RotateElement(document, cropBoxElement.Id, rotationalAxis, rotationalAngle);
            rotation.Commit();

            rotation.Start();
            view.CropBoxVisible = false;
            rotation.Commit();
        }

        public static void SetSheetParameters(Document document, ElementId id, ViewSheet sheet, int n)
        {
            Element assembly = document.GetElement(id);
            Element view = (Element)sheet;

            view.get_Parameter(BuiltInParameter.SHEET_NAME).Set(assembly.Name);
            view.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(n.ToString());



        }

        public static void CreateFacadeDrawing(Document document, ElementId id, ViewSheet sheet)
        {

            //Элемент "Обобщенная модель" для ссылок
            Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_Parts).FirstElement();

            ViewsEnvironment views = ViewsEnvironment.getInstance(document);

            ElementId viewFront = views.GetElementIdFromViewTemplateName(ViewTemplateName.FACADE_VIEW_FRONT_VIEW);
            ElementId sched1 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SCHEDULE_FACADE);
            ElementId sched2 = views.GetElementIdFromViewTemplateName(ViewTemplateName.SCHEDULE_FACADE_TYPES);
            ElementId leg1 = views.GetElementIdFromViewTemplateName(ViewTemplateName.LEGEND_FACADE_ANNOTATION);
            ElementId leg2 = views.GetElementIdFromViewTemplateName(ViewTemplateName.LEGEND_FACADE_DESCRIPTION);
            ElementId leg3 = views.GetElementIdFromViewTemplateName(ViewTemplateName.LEGEND_FACADE_LAYOUT_SCHEME);

            ViewSection vsUp2 = AssemblyViewUtils.CreateDetailSection(document, id, AssemblyDetailViewOrientation.ElevationRight, viewFront, isAssigned: true);


            //Создаем виды и размещаем на листах
            //1 лист
            ViewSchedule ptSched1 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, schedTypeElement.Category.Id, sched1, false);

            ViewSchedule ptSched2 = AssemblyViewUtils.CreateSingleCategorySchedule(document, id, schedTypeElement.Category.Id, sched2, false);

            Viewport.Create(document, sheet.Id, vsUp2.Id, new XYZ(-0.830265408004621, 0.814931612421939, -0.2404));

            ScheduleSheetInstance.Create(document, sheet.Id, ptSched1.Id, new XYZ(-0.62336, 0.44361, 0));
            ScheduleSheetInstance.Create(document, sheet.Id, ptSched2.Id, new XYZ(-0.62336, 0.31714889661308, 0));

            if (leg1 != null)
            {
                Viewport.Create(document, sheet.Id, leg1, new XYZ(-0.471315624987842, 0.21358, 0));
            }
            if (leg2 != null)
            {
                Viewport.Create(document, sheet.Id, leg2, new XYZ(-1.14009, 0.14663, -3.89432076330052E-17));
            }
            if (leg3 != null)
            {
                Viewport.Create(document, sheet.Id, leg3, new XYZ(-0.7956, 0.14663, 9.90389806990418E-17));
            }

        }

    }
}



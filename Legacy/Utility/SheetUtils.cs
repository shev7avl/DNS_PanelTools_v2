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
            if (viewReference.GetCreatedViewsWithCoords() != null && sheet.SheetLink.IsValidObject)
            {
                foreach (var item in viewReference.GetCreatedViewsWithCoords().Keys)
                {
                    if (item.IsValidObject)
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
            if (sheet.IsValidObject)
            {
                Element assembly = document.GetElement(id);
                Element view = (Element)sheet;

                string name = assembly.Name;

                if (assembly.Name.Split('-').Length > 2)
                {
                    name = string.Format("{0}-{1}", assembly.Name.Split('-')[1], assembly.Name.Split('-')[2]);
                }

                view.get_Parameter(BuiltInParameter.SHEET_NAME).Set(name);
                view.get_Parameter(BuiltInParameter.SHEET_NUMBER).Set(n.ToString());
            }
        }
    }
}



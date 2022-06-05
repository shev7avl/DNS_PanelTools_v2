using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Builders
{
    public class DrawingBuilder
    {
        protected BasePanel Panel { get; private set; }
        private protected TemplateFactory BuilderTemplate { get; set; }

        public DrawingBuilder(BasePanel panel)
        { 
            Panel = panel ?? throw new NullReferenceException(nameof(panel));
            SetBuilderTemplate();
        }

        private void SetBuilderTemplate()
        {
            if (Panel is NS_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetNsSchema());
            }
            else if (Panel is VS_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetVsSchema());
            }
            else if (Panel is PS_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetPsSchema());
            }
            else if (Panel is BP_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetBpSchema());
            }
            else if (Panel is PP_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetPpSchema());
            }
            else if (Panel is Facade_Panel)
            {
                BuilderTemplate = new TemplateFactory(DrawingSchemes.GetFacadeSchema());
            }
        }

        public void BuildSheets(Document document)
        {
            Transaction transaction = new Transaction(document, $"Создание листов панели {Panel.ShortMark}");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            ElementId elementId = Panel.AssemblyInstance.Id;

            transaction.Start();

            List<Sheet> sheets = SheetUtils.CreateSheetList(document, elementId, BuilderTemplate.GetNumberOfSheets());

            transaction.Commit();

            BuilderTemplate.Sheets = sheets;
        }

        public void BuildViews(Document document)
        {
            BuilderTemplate.ViewReferences = CreateViewReferences(document);
            Transaction transaction = new Transaction(document, "Creating the views");
            TransactionSettings.SetFailuresPreprocessor(transaction);
            transaction.Start();
            foreach (var viewReference in BuilderTemplate.ViewReferences)
            {
                SetAndCreateViews(document, viewReference);
            }
            transaction.Commit();
        }

        public void BuildDrawings(Document document)
        {
            Transaction transaction = new Transaction(document, $"Создание листов панели {Panel.AssemblyInstance.Name}");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();
            foreach (var sheet in BuilderTemplate.GetDrawingSetMapping().Keys)
            {
                foreach (var viewRef in BuilderTemplate.GetDrawingSetMapping()[sheet])
                {
                    SheetUtils.PlaceViewsOnSheet(document, sheet, viewRef);
                }
            }
            transaction.Commit();
        }

        public void BuildParameters(Document document)
        {
            Transaction transaction = new Transaction(document, $"Создание листов панели {Panel.AssemblyInstance.Name}");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            ElementId elementId = Panel.AssemblyInstance.Id;

            transaction.Start();
            for (int i = 0; i < BuilderTemplate.Sheets.Count; i++)
            {
                SheetUtils.SetSheetParameters(document, elementId, BuilderTemplate.Sheets[i].SheetLink, i + 1);
            }
            transaction.Commit();

            SetFrontView(document, Panel);
            if (Panel is PP_Panel)
            {
                SetPlanView(document, Panel);
            }
        }

        private protected List<ViewReference> CreateViewReferences(Document document)
        {
            ViewsEnvironment views = ViewsEnvironment.getInstance(document);
            List<ViewReference> viewReferences = new List<ViewReference>();
            foreach (var template in BuilderTemplate.GetViewTemplatesList())
            {
                ViewReference reference = new ViewReference(document, template);
                viewReferences.Add(reference);
            }
            return viewReferences;
        }

        private protected View CreateAssemblyView(Document document, ViewReference viewReference, bool SideSection = true)
        {
            if (AssemblyViewLogic.TemplateIs3DView(viewReference))
            {
                return AssemblyViewLogic.Create3DView(document, viewReference, Panel);
            }
            else if (AssemblyViewLogic.TemplateIsSectionView(viewReference))
            {
                return AssemblyViewLogic.CreateSectionView(document, viewReference,Panel, SideSection);
            }
            else if (AssemblyViewLogic.TemplateIsLegend(viewReference))
            {
                //Такой легенды нет в проекте
                if (AssemblyViewLogic.CreateLegendView(document, viewReference) is null)
                {
                    return null;
                }
                else
                {
                    return AssemblyViewLogic.CreateLegendView(document, viewReference);
                }
            }
            else
            {
                return AssemblyViewLogic.CreateSchedule(document, viewReference, Panel);
            }
        }

        private protected void SetAndCreateViews(Document document, ViewReference viewReference)
        {
            List<View> views = new List<View>();

            if (ViewHasSideAndPlanSections(viewReference))
            {
                views.Add(CreateAssemblyView(document, viewReference, SideSection: false));
                views.Add(CreateAssemblyView(document, viewReference, SideSection: true));
            }   
            else
            {
                views.Add(CreateAssemblyView(document, viewReference));
            }

            foreach (var item in views)
            {
                if (item != null)
                {
                    viewReference.SetCreatedViews(item);
                }   
            }
        }

        private protected static bool ViewHasSideAndPlanSections(ViewReference viewReference)
        {
            return viewReference.ViewTemplate.ToString().Contains("SECTION_VIEW") && !viewReference.ViewTemplate.ToString().Contains("JOINT");
        }

        private static void SetPlanView(Document document, BasePanel basePanel)
        {
            View sectionView = new FilteredElementCollector(document).OfClass(typeof(View)).
                Cast<View>().ToList().Where(o => basePanel.ActiveElement.AssemblyInstanceId == o.AssociatedAssemblyInstanceId
                && o.GetType() == typeof(ViewSection)
                && o.Name == "Разрез узла A").FirstOrDefault();

            if (sectionView != null)
            {
                Transaction transaction = new Transaction(document, "setPlanView");
                TransactionSettings.SetFailuresPreprocessor(transaction);
                transaction.Start();
                sectionView.EnableRevealHiddenMode();
                transaction.Commit();

                transaction.Start();
                Element planViewer = new FilteredElementCollector(document, sectionView.Id).
                    OfCategory(BuiltInCategory.OST_Viewers).
                    Where(o => o.Name.Contains("План")).FirstOrDefault();
                if (planViewer != null)
                {
                    ElementTransformUtils.MoveElement(document, planViewer.Id, new XYZ(0, 0, 
                        UnitUtils.ConvertToInternalUnits(150, DisplayUnitType.DUT_MILLIMETERS)));
                }
                transaction.Commit();

                transaction.Start();
                sectionView.DisableTemporaryViewMode(TemporaryViewMode.RevealHiddenElements);
                transaction.Commit();
            }

        }

        private static void SetFrontView(Document document, BasePanel basePanel)
        {
            FilteredElementCollector views = new FilteredElementCollector(document).OfClass(typeof(View));
            List<View> viewsTempFiltered = views.Cast<View>().ToList();
            List<View> viewsFiltered1 = viewsTempFiltered.
                Where(o => basePanel.ActiveElement.AssemblyInstanceId == o.AssociatedAssemblyInstanceId
            && o.GetType() == typeof(ViewSection)).ToList();

            Element newSection = new FilteredElementCollector(document).
                OfClass(typeof(ViewFamilyType)).FirstOrDefault(o => o.Name.Contains("Сечение_Без номера листа"));

            Transaction transaction = new Transaction(document, "hide Els");
            Utility.TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();

            if (basePanel is Facade_Panel)
            {

            }
            else
            {
                foreach (var item in viewsFiltered1)
                {
                    List<Element> viewersOnView = new FilteredElementCollector(document, item.Id).
                        OfCategory(BuiltInCategory.OST_Viewers).ToList();


                    List<Element> hideEls = viewersOnView.Where(o => o.Name.Contains(item.Name.Substring(0, 9))).ToList();
                    List<ElementId> hideIds = hideEls.Select<Element, ElementId>(o => o.Id).ToList();

                    if (hideEls.Count > 0)
                    {
                        item.HideElements(hideIds);
                        foreach (var hideEl in hideEls)
                        {
                            viewersOnView.Remove(hideEl);
                        }

                    }

                    if (viewersOnView.Count > 0 && newSection != null)
                    {
                        foreach (var viewer in viewersOnView)
                        {
                            viewer.ChangeTypeId(newSection.Id);
                        }
                    }
                }
            }
            
            transaction.Commit();
        }

        internal static class AssemblyViewLogic
        {

            internal static View CreateSchedule(Document document, ViewReference viewReference, BasePanel Panel)
            {
                if (AssemblyViewLogic.TemplateIsMaterialTakeOff(viewReference))
                {
                    return CreateMaterialTakeOff(document, viewReference, Panel);
                }
                else
                {
                    if (TemplateIsDetailSchedule(viewReference))
                    {
                        if (viewReference.ViewTemplate == ViewTemplateName.SCHEDULE_FACADE_DETAILS)
                        {
                            return CreateAssemblyDetailSchedule(document, viewReference, Panel);
                        }
                        else
                        {
                            return CreateLocationDetailSchedule(document, viewReference, Panel);
                        }
                        
                    }
                    else if (AssemblyViewLogic.TemplateIsCategorySchedule(viewReference))
                    {
                        if (viewReference.ViewTemplate == ViewTemplateName.SCHEDULE_SLAB_REINFORCEMENT)
                        {
                            return CreateReinforcementSchedule(document, viewReference, Panel);
                        }
                        else return CreateCategorySchedule(document, viewReference, Panel);
                    }
                    else
                    {
                        return default;
                    }
                }
            }

            internal static View CreateCategorySchedule(Document document, ViewReference viewReference, BasePanel Panel)
            {
                Element schedTypeElement = new FilteredElementCollector(document).OfCategory(BuiltInCategory.OST_GenericModel).FirstElement();
                if (TemplateExists(viewReference))
                {
                    return AssemblyViewUtils.CreateSingleCategorySchedule(document, Panel.AssemblyInstance.Id, schedTypeElement.Category.Id, viewReference.TemplateId, isAssigned: true);
                }
                else
                {
                    return AssemblyViewUtils.CreateSingleCategorySchedule(document, Panel.AssemblyInstance.Id, schedTypeElement.Category.Id);
                }
            }

            internal static bool TemplateIsDetailSchedule(ViewReference viewReference)
            { 
            return viewReference.ViewTemplate.ToString().Contains("DETAIL");
            }

            internal static View CreateLocationDetailSchedule(Document document, ViewReference viewReference, BasePanel basePanel)
            {
                View presetView = new FilteredElementCollector(document)
                    .OfClass(typeof(ViewSchedule))
                    .Where(o => o.Name == "DNS_Шаблон_Марки панелей").Cast<View>()
                    .First();

                var newName = $"{basePanel.AssemblyInstance.Name}";
                bool scheduleExists = (
                    new FilteredElementCollector(document).
                    OfClass(typeof(ViewSchedule)).
                    FirstOrDefault(o => o.Name.Equals(newName)) != null);
                if (scheduleExists)
                {
                    return new FilteredElementCollector(document).
                        OfClass(typeof(ViewSchedule)).
                        Cast<ViewSchedule>().
                        First(o => o.Name == newName);
                }
                else
                {
                    var locationScheduleId = presetView.Duplicate(ViewDuplicateOption.Duplicate);
                    ViewSchedule locationSchedule = (ViewSchedule)document.GetElement(locationScheduleId);
                    locationSchedule.Name = newName;

                    var comparedValue = basePanel.AssemblyInstance.Name;

                    ScheduleFieldId assemblyFieldId = locationSchedule.Definition.GetFieldId(2);
                    ScheduleFilter assemblyFilter = new ScheduleFilter(assemblyFieldId,
                        ScheduleFilterType.Equal,
                        comparedValue);

                    locationSchedule.Definition.AddFilter(assemblyFilter);

                    return locationSchedule;
                }
            }

            internal static View CreateAssemblyDetailSchedule(Document document, ViewReference viewReference, BasePanel basePanel)
            {

                View presetView = new FilteredElementCollector(document)
                      .OfClass(typeof(ViewSchedule))
                      .Where(o => o.Name == "DNS_Шаблон_Спецификация плитки (материал)").Cast<View>()
                      .First();

                var newName = $"{basePanel.ActiveElement.ParametersMap.get_Item("ADSK_Марка конструкции").AsString()} " +
                    $"/ {basePanel.AssemblyInstance.Name}";

                bool scheduleExists = (
                    new FilteredElementCollector(document).
                    OfClass(typeof(ViewSchedule)).
                    FirstOrDefault(o => o.Name.Equals(newName)) != null);

                if (scheduleExists)
                {
                    return new FilteredElementCollector(document).
                        OfClass(typeof(ViewSchedule)).
                        Cast<ViewSchedule>().
                        First(o => o.Name == newName);
                }
                else
                {
                    var locationScheduleId = presetView.Duplicate(ViewDuplicateOption.Duplicate);
                    ViewSchedule detailSchedule = (ViewSchedule)document.GetElement(locationScheduleId);

                    detailSchedule.Name = newName;

                    var assemblyName = basePanel.AssemblyInstance.Name;
                    var location = basePanel.ActiveElement.ParametersMap.get_Item("DNS_Марка элемента").AsString();
                    var mark = basePanel.ActiveElement.ParametersMap.get_Item("ADSK_Марка конструкции").AsString();

                    ScheduleFieldId assemblyFieldId = detailSchedule.Definition.GetFieldId(detailSchedule.Definition.GetFieldCount() - 1);
                    ScheduleFieldId markFieldId = detailSchedule.Definition.GetFieldId(detailSchedule.Definition.GetFieldCount() - 2);
                    ScheduleFieldId locationFieldId = detailSchedule.Definition.GetFieldId(detailSchedule.Definition.GetFieldCount() - 3);

                    ScheduleFilter assemblyFilter = new ScheduleFilter(assemblyFieldId,
                        ScheduleFilterType.Equal,
                        assemblyName);
                    ScheduleFilter markFilter = new ScheduleFilter(markFieldId,
                        ScheduleFilterType.Equal,
                        mark);
                    ScheduleFilter locationFilter = new ScheduleFilter(locationFieldId, ScheduleFilterType.Equal, location);

                    detailSchedule.Definition.AddFilter(assemblyFilter);
                    detailSchedule.Definition.AddFilter(markFilter);
                    detailSchedule.Definition.AddFilter(locationFilter);

                    return detailSchedule;
                }
                
            }

            internal static bool TemplateIsCategorySchedule(ViewReference viewReference)
            {
                return viewReference.ViewTemplate.ToString().Contains("SCHEDULE");
            }

            internal static View CreateMaterialTakeOff(Document document, ViewReference viewReference, BasePanel Panel)
            {
                if (TemplateExists(viewReference))
                {
                    return AssemblyViewUtils.CreateMaterialTakeoff(document, Panel.AssemblyInstance.Id, viewReference.TemplateId, isAssigned: true);
                }
                else
                {
                    return AssemblyViewUtils.CreateMaterialTakeoff(document, Panel.AssemblyInstance.Id);
                }
            }

            internal static View CreateReinforcementSchedule(Document document, ViewReference viewReference, BasePanel panel)
            {
                if (TemplateExists(viewReference))
                {
                    return AssemblyViewUtils.CreateSingleCategorySchedule(document, 
                        panel.AssemblyInstance.Id,
                        Category.GetCategory(document, BuiltInCategory.OST_Rebar).Id,
                        viewReference.TemplateId,
                        isAssigned: true);
                }
                else
                {
                    return AssemblyViewUtils.CreateSingleCategorySchedule(document,
                        panel.AssemblyInstance.Id,
                        Category.GetCategory(document, BuiltInCategory.OST_Rebar).Id);
                }
            }

            internal static bool TemplateIsMaterialTakeOff(ViewReference viewReference)
            {
                return viewReference.ViewTemplate.ToString().Contains("VOLUME") || viewReference.ViewTemplate.ToString().Contains("INSULATION_WOOL") || viewReference.ViewTemplate.ToString().Contains("INSULATION_XPS");
            }

            internal static View Create3DView(Document document, ViewReference viewReference, BasePanel Panel)
            {
                if (TemplateExists(viewReference))
                {
                    return AssemblyViewUtils.Create3DOrthographic(document, Panel.AssemblyInstance.Id, viewReference.TemplateId, isAssigned: true);
                }
                else
                {
                    return AssemblyViewUtils.Create3DOrthographic(document, Panel.AssemblyInstance.Id);
                }
            }

            internal static View CreateSectionView(Document document, ViewReference viewReference, BasePanel Panel, bool SideSection)
            {
                List<AssemblyDetailViewOrientation> orientations = Assemblies.DefineViewOrientations(document, Panel);
                if (TemplateExists(viewReference))
                {
                    if (viewReference.ViewTemplate.ToString().Contains("FRONT"))
                    {
                        return AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, orientations[0], viewReference.TemplateId, isAssigned: true);
                    }
                    else if (viewReference.ViewTemplate.ToString().Contains("SECTION"))
                    {
                        if (SideSection)
                        {
                            ViewSection output = AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, AssemblyDetailViewOrientation.HorizontalDetail, viewReference.TemplateId, isAssigned: true);
                            SheetUtils.RotatePlanView(document, output, orientations[0]);
                            return output;
                        }
                        else
                        {
                            return AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, orientations[2], viewReference.TemplateId, isAssigned: true);

                        }
                    }
                    else
                    {
                        return default;
                    }
                }
                else
                {
                    if (viewReference.ViewTemplate.ToString().Contains("FRONT"))
                    {
                        return AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, orientations[0]);
                    }
                    else if (viewReference.ViewTemplate.ToString().Contains("SECTION"))
                    {
                        if (SideSection)
                        {
                            return AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, orientations[2]);
                        }
                        else
                        {
                            ViewSection output = AssemblyViewUtils.CreateDetailSection(document, Panel.AssemblyInstance.Id, AssemblyDetailViewOrientation.HorizontalDetail);
                            SheetUtils.RotatePlanView(document, output, orientations[0]);
                            return output;
                        }
                    }
                    else return default;
                }
            }

            internal static bool TemplateIsSectionView(ViewReference viewReference)
            {
                return viewReference.ViewTemplate.ToString().Contains("VIEW");
            }

            internal static bool TemplateIs3DView(ViewReference viewReference)
            {
                return viewReference.ViewTemplate.ToString().Contains("3D_VIEW");
            }

            internal static bool TemplateExists(ViewReference viewReference)
            {
                return viewReference.TemplateId != null;
            }

            internal static bool TemplateIsLegend(ViewReference viewReference)
            {
                return viewReference.ViewTemplate.ToString().Contains("LEGEND");
            }

            internal static View CreateLegendView(Document document, ViewReference viewReference)
            {
                ViewsEnvironment views = ViewsEnvironment.getInstance(document);
                ElementId id = views.GetElementIdFromViewTemplateName(viewReference.ViewTemplate);
                if (id is null)
                {
                    return null;
                }
                else
                {
                    Element element = document.GetElement(id);
                    return (View)element;
                }
                
            }

            
        }
        
    }
}

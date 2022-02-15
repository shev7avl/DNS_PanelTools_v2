using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Linq;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateDrawings : IExternalCommand
    {
        
        public Document Document { get ; set; }
        //ИДЕЯ
        // Что если парсить json на предмет листов, видов и шаблонов?


        class PanelSelectionFilter : ISelectionFilter
        {
            public bool AllowElement(Element elem)
            {

                StructureType structureType = new StructureType(elem);

                if (structureType.GetPanelType(elem) == StructureType.Panels.None.ToString() && !elem.Category.Name.Contains("Каркас несущий") && !(elem is AssemblyInstance) )
                {
                    return false;
                }

                else return true;
            }

            public bool AllowReference(Reference reference, XYZ position)
            {
                throw new NotImplementedException();
            }
        }

        //Реализация через Panel
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.AllWorksetsAreAvailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            List<BasePanel> list_Panels = GetPanelsFromSelection(commandData);
            CreateAssemblyIfMissing(list_Panels);
            CreateDrawingForSelectedPanels(list_Panels);

            environment.Reset();

            return Result.Succeeded;
        }

        private void CreateDrawingForSelectedPanels(List<BasePanel> list_Panels)
        {
            foreach (BasePanel item in list_Panels)
            {
                BasePanelWrapper panelWrapper = new DrawingWrapper(item);
                panelWrapper.Execute(Document);
            }
        }

        private void CreateAssemblyIfMissing(List<BasePanel> list_Panels)
        {
            foreach (var item in list_Panels)
            {
                if (item.AssemblyInstance is null)
                {
                    Utility.Assemblies.CreateAssembly(Document, item);
                }
            }
            //Пересохраняем коллектор
            if (Document.IsModified && Document.IsModifiable)
            {
                SubTransaction regeneration = new SubTransaction(Document);
                regeneration.Start();
                Document.Regenerate();
                regeneration.Commit();
            }
        }

        private static List<BasePanel> GetPanelsFromSelection(ExternalCommandData commandData)
        {
            Selection selection = commandData.Application.ActiveUIDocument.Selection;
            Document document = commandData.Application.ActiveUIDocument.Document;
            StructuralEnvironment collector = StructuralEnvironment.GetInstance(document);

            IList<Reference> list_PanelRefs = selection.PickObjects(ObjectType.Element, new PanelSelectionFilter(), "Выберите панели в проекте");

            List<BasePanel> list_Panels = new List<BasePanel>();

            //Переписываем выбранные элементы в объекты класса BasePanel
            foreach (var reference in list_PanelRefs)
            {
                BasePanel panel = collector.PanelMarks.Where(o => o.ActiveElement.Id == reference.ElementId).First();
                list_Panels.Add(panel);
            }

            return list_Panels;
        }
        
    }
}

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

        //Реализация через Panel
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            List<PrecastPanel> list_Panels = GetPanelsFromSelection(commandData);
            CreateAssemblyIfMissing(list_Panels);
            CreateDrawingForSelectedPanels(list_Panels);

            environment.Reset();

            return Result.Succeeded;
        }

        private void CreateDrawingForSelectedPanels(List<PrecastPanel> list_Panels)
        {
            foreach (PrecastPanel item in list_Panels)
            {
                BasePanelWrapper panelWrapper = new DrawingWrapper(item);
                panelWrapper.Execute(Document);
            }
        }

        private void CreateAssemblyIfMissing(List<PrecastPanel> list_Panels)
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

        private static List<PrecastPanel> GetPanelsFromSelection(ExternalCommandData commandData)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            Selector selector = new Selector();
            ICollection<Element> selectedEls = selector.CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming);

            List<PrecastPanel> panelsList = new List<PrecastPanel>();

            

            foreach (var item in selectedEls)
            {
                PrecastPanel panel = new PrecastPanel(document, item);
                if (panel != null)
                {
                    //panel.ReadMarks();
                    panelsList.Add(panel);
                }
            }

            return panelsList;
        }
        
    }
}

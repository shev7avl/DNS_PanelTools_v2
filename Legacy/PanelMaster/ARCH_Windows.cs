using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ARCH_Windows : Autodesk.Revit.UI.IExternalCommand
    {
        Document Document;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            AddinSettings settings = AddinSettings.GetSettings();
            Selector selector = new Selector();
            ICollection<Element> selectedWalls = selector.CollectElements(commandData, new FacadeSelectionFilter(), BuiltInCategory.OST_Walls);

            Debug.WriteLine(Document.PathName);

            foreach (var item in selectedWalls)
            {
                try
                {
                    Utility.Openings.CreateFacadeOpening(Document, item);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ошибка{e.Message} : {item.Name} - {item.Id}");
                    message = $"Ошибка{e.Message} : {item.Name} - {item.Id}";
                    return Result.Failed;
                }
            }
                         
            return Result.Succeeded;
        }
    }

}

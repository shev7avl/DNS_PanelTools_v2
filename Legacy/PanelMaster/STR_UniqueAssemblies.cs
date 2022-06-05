using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_UniqueAssemblies : IExternalCommand
    {

        public Document Document { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Selector selector = new Selector();
            List<AssemblyInstance> panels = selector
                .CollectElements(commandData, BuiltInCategory.OST_Assemblies)
                //.Select(o => Document.GetElement(o.AssemblyInstanceId))
                .Where(o => o.IsValidObject)
                .Cast<AssemblyInstance>()
                .ToList();

            Utility.Assemblies.UniquePanels(Document, panels);

            return Result.Succeeded;
        }

    }

}

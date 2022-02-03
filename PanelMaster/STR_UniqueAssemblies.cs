using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

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

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Utility.Assemblies.LeaveUniquePanels(Document);

            return Result.Succeeded;
        }

    }

}

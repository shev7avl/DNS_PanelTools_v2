using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;

namespace DSKPrim.PanelTools_v2.Commands
{
    class DisassembleAllRoutine : Routine
    {
        public override StructuralApps.Panel.Panel Behaviour { get; set; }
        public override Document Document { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;
            Logger.Logger logger = Logger.Logger.getInstance();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Utility.Assemblies.DisassembleAll(Document);

            logger.LogSuccessTime(stopWatch);
        }


    }


    

}

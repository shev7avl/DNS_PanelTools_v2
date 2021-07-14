using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools_v2.StructuralApps.Assemblies;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class CreateAssembliesRoutine
        : Routine
    {
        public override Document Document { get; set; }
        public override StructuralApps.Panel.Panel Behaviour { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);
            assemblyBuilder.AddIndex("НС");
            assemblyBuilder.AddIndex("ВС");
            assemblyBuilder.AddIndex("ПС");
            assemblyBuilder.AddIndex("ПП");
            assemblyBuilder.AddIndex("БП");
            assemblyBuilder.CreateAssemblies();
            assemblyBuilder.LeaveUniquePanels();
        }
    }
}

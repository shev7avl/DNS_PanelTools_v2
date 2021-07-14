using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using DNS_PanelTools_v2.StructuralApps.Assemblies;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STRUCT_Assemblies : IExternalCommand
    {
        public Document Document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
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



            return Result.Succeeded;
        }
    }
}

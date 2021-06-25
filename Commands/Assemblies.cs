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
    class Assemblies : IExternalCommand
    {
        public Document Document;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);
            assemblyBuilder.FillMxIdDict("НС");
            assemblyBuilder.FillMxIdDict("ВС");
            assemblyBuilder.FillMxIdDict("ПС");
            assemblyBuilder.FillMxIdDict("ПП");
            assemblyBuilder.FillMxIdDict("БП");


            return Result.Succeeded;
        }
    }
}

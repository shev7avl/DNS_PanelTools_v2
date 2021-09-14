using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.StructuralApps;
using DSKPrim.PanelTools_v2.StructuralApps.Assemblies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Commands
{
    class UniqueAssemblies : Routine
    {
        public override StructuralApps.Panel.Panel Behaviour { get; set; }
        public override Document Document { get; set; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);

            assemblyBuilder.LeaveUniquePanels();

        }


    }


    

}

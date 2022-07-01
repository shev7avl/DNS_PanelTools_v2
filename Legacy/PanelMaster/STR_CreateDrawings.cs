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
using Autofac;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateDrawings : IExternalCommand
    {
        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;

            TransactionSettings.CheckWorksets(document);

            var selector = new Selector();
            var panels = selector.
                CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming).
                Select(e => new PrecastPanel(e)).
                Where(p => p.AssemblyInstance != null).
                ToList();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<DrawingOperation>().As<IPanelOperation>();
            var container = containerBuilder.Build();


            using (var scope = container.BeginLifetimeScope())
            {
                var operation = scope.Resolve<IPanelOperation>();
                operation.ExecuteRange(panels);
            }

            return Result.Succeeded;
        }

    }
}

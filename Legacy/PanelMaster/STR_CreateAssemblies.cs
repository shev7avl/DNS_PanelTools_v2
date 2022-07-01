using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autofac;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Diagnostics;
using DSKPrim.PanelTools.Utility;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Legacy.Panel;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateAssemblies
        : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            var document = commandData.Application.ActiveUIDocument.Document;

            try
            {
                TransactionSettings.CheckWorksets(document);

                var selector = new Selector();
                var panels = selector.
                    CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming).
                    Select(o => new PrecastPanel(o)).
                    ToList();

                var containerBuilder = new ContainerBuilder();
                containerBuilder.RegisterType<AssemblyOperation>().As<IPanelOperation>();
                var container = containerBuilder.Build();

                using (var scope = container.BeginLifetimeScope())
                {
                    var operation = scope.Resolve<IPanelOperation>();
                    operation.ExecuteRange(panels);

                }

            }
            catch (Exception e)
            {
                message = $"ОШИБКА: {e.Message}";

                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }

    
}

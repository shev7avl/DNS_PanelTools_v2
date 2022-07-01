using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;
using Autofac;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateOpenings
        : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var document = commandData.Application.ActiveUIDocument.Document;
            try
            {
                TransactionSettings.CheckWorksets(document);
            
            var selector = new Selector();
            var precastPanels = selector.
                CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming).
                Select(e => new PrecastPanel(e)).
                Where(p => p.StructureCategory.IsValidForWindowCreation()).
                ToList();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<WindowOperation>().As<IPanelOperation>();
            var container = containerBuilder.Build();

                using (var scope = container.BeginLifetimeScope())
                {
                    var operation = scope.Resolve<IPanelOperation>();
                    operation.ExecuteRange(precastPanels);
                }
                TransactionSettings.ReleaseWorksets(document);
            }
            catch (Exception e)
            {
                message = $"Ошибка {e.Message}";
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools_v2.StructuralApps;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using DSKPrim.PanelTools_v2.Utility;
using System.Diagnostics;

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
            Logger.Logger logger = Logger.Logger.getInstance();
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Document = commandData.Application.ActiveUIDocument.Document;

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Element> elements = selection.PickElementsByRectangle(new AssemblyCreationFilter());

            SingleStructDoc structDoc = SingleStructDoc.getInstance(Document, elements);
            structDoc.PanelMarks.Sort(Utility.Assemblies.CompareElementIdsByZCoord);


            int counter = 1;
            foreach (var item in structDoc.PanelMarks)
            {
                logger.DebugLog($"Итерация: {counter}//{structDoc.PanelMarks.Count}");
                logger.DebugLog($"Панель: {item.ShortMark}");
                Utility.Assemblies.CreateAssembly(Document, logger, item);
                counter++;
            }

            //AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);
            //assemblyBuilder.CreateAssemblies();

            structDoc.Dispose();

            logger.LogSuccessTime(stopWatch);

        }

       

    }

    class AssemblyCreationFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {

            StructureType structureType = new StructureType(elem);

            if (structureType.GetPanelType(elem) == StructureType.Panels.None.ToString() && elem.Category.Name.Contains("Каркас несущий"))
            {
                return false;
            }

            else return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

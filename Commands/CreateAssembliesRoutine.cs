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
            structDoc.PanelMarks.Sort(CompareElementIdsByZCoord);


            int counter = 1;
            foreach (var item in structDoc.PanelMarks)
            {
                logger.DebugLog($"Итерация: {counter}//{structDoc.PanelMarks.Count}");
                logger.DebugLog($"Панель: {item.ShortMark}");
                CreateAssembly(logger, item);
                counter++;
            }

            //AssemblyBuilder assemblyBuilder = new AssemblyBuilder(Document);
            //assemblyBuilder.CreateAssemblies();

            structDoc.Dispose();

            logger.LogSuccessTime(stopWatch);

        }

        private void CreateAssembly(Logger.Logger logger, StructuralApps.Panel.Panel item)
        {
            if (item is IAssembler assembler)
            {
                Outline outline = new Outline(item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Min, item.ActiveElement.get_Geometry(new Options()).GetBoundingBox().Max);
                ElementFilter filter = new BoundingBoxIntersectsFilter(outline);
                FilteredElementCollector fecIntersect = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(filter);

                List<Element> intersected = fecIntersect.ToList();

                assembler.SetAssemblyElements();
                if (item is NS_Panel || item is VS_Panel)
                {
                    foreach (var ints in intersected)
                    {
                        SetPanelBehaviour(ints);
                        if (Behaviour is NS_Panel || Behaviour is VS_Panel)
                        {
                            IAssembler assembler1 = (IAssembler)Behaviour;
                            assembler.TransferFromPanel(assembler1);
                        }
                    }
                }

                AssemblyCreationTransaction(logger, item, assembler);

            }
        }

        private void AssemblyCreationTransaction(Logger.Logger logger, StructuralApps.Panel.Panel item, IAssembler assembler)
        {
            Transaction transaction = new Transaction(Document, "CreateAssembly");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            IFailuresPreprocessor preprocessor = new WarningDiscard();
            opts.SetFailuresPreprocessor(preprocessor);
            transaction.SetFailureHandlingOptions(opts);

            transaction.Start();
            try
            {
                AssemblyInstance instance = AssemblyInstance.Create(Document, assembler.AssemblyElements, item.ActiveElement.Category.Id);
                transaction.Commit();

                transaction.Start();
                try
                {
                    instance.AssemblyTypeName = item.ShortMark;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                }

                transaction.Commit();

            }


            catch (Autodesk.Revit.Exceptions.ArgumentException)
            {
                logger.DebugLog($"Произошла ошибка в панели {item.ShortMark} на уровне {item.ActiveElement.LevelId}");
                AssemblyInstance instance = AssemblyInstance.Create(Document, new List<ElementId>() { item.ActiveElement.Id }, item.ActiveElement.Category.Id);
                transaction.Commit();

                transaction.Start();
                try
                {
                    instance.AssemblyTypeName = item.ShortMark;
                }
                catch (Autodesk.Revit.Exceptions.ArgumentException)
                {
                    instance.AssemblyTypeName = $"{item.ShortMark} ID{item.ActiveElement.Id}";
                }

                transaction.Commit();
            }
        }

        private int CompareElementIdsByZCoord(StructuralApps.Panel.Panel x, StructuralApps.Panel.Panel y)
        {
            Element elX = x.ActiveElement;
            Element elY = y.ActiveElement;

            BoundingBoxXYZ boxX = elX.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ boxY = elY.get_Geometry(new Options()).GetBoundingBox();

            if (boxX.Min.Z > boxY.Min.Z)
            {
                return 1;
            }
            else if (boxX.Min.Z == boxY.Min.Z)
            {
                return 0;
            }
            else
            {
                return -1;
            }

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

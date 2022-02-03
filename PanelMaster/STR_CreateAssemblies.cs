using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STR_CreateAssemblies
        : IExternalCommand
    {
        public  Document Document { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document = commandData.Application.ActiveUIDocument.Document;
            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Element> _elements = selection.PickElementsByRectangle(new AssemblyCreationFilter());

            environment.GetStructuralEnvironment().PanelMarks.Sort(Utility.Assemblies.CompareElementIdsByZCoord);

            int counter = 1;
            foreach (var item in environment.GetStructuralEnvironment().PanelMarks)
            {
                Debug.WriteLine($"Итерация: {counter}//{environment.GetStructuralEnvironment().PanelMarks.Count}");
                Debug.WriteLine($"Панель: {item.ShortMark}");
                Utility.Assemblies.CreateAssembly(Document, item);
                counter++;
            }

            environment.Reset();
            return Result.Succeeded;
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

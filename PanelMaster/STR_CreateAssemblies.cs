using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
using Autodesk.Revit.Attributes;
using DSKPrim.PanelTools.ProjectEnvironment;
using System.Diagnostics;
using DSKPrim.PanelTools.Utility;
using DSKPrim.PanelTools.Panel;

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

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);

            Selector selector = new Selector();
            ICollection<Element> _elements = selector.CollectElements(commandData, new PanelSelectionFilter(), BuiltInCategory.OST_StructuralFraming);
            List<BasePanel> panels = environment.GetStructuralEnvironment().PanelMarks.Where(o => _elements.Contains(o.ActiveElement)).ToList();
            panels.Sort(Utility.Assemblies.CompareElementIdsByZCoord);

            int counter = 1;
            try
            {
                foreach (var item in panels)
                {
                    Debug.WriteLine($"Итерация: {counter}//{environment.GetStructuralEnvironment().PanelMarks.Count}");
                    Debug.WriteLine($"Панель: {item.ShortMark}");
                    Utility.Assemblies.CreateAssembly(Document, item);
                    counter++;
                }
            }
            catch (Exception e)
            {
                message = $"ОШИБКА: {e.Message}";
                return Result.Failed;
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

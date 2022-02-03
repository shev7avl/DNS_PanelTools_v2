using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace DSKPrim.PanelTools
{
    internal abstract class CommandBuilder
    {
        private SelectionType SelectionType;
        internal abstract SelectionType SetSelectionType();

        internal virtual ICollection<Element> CollectElements(ExternalCommandData commandData, ISelectionFilter selectionFilter, BuiltInCategory builtInCategory)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;
            ICollection<Element> chosenElements = new List<Element>();

            if (SelectionType == SelectionType.AllElements)
            {
                chosenElements = new FilteredElementCollector(document).OfCategory(builtInCategory).WhereElementIsNotElementType().ToElements();
            }
            else if (SelectionType == SelectionType.CropBox)
            {
                Selection selection = commandData.Application.ActiveUIDocument.Selection;
                chosenElements = selection.PickElementsByRectangle();
            }
            else if (SelectionType == SelectionType.SingleElement)
            {
                Selection selection = commandData.Application.ActiveUIDocument.Selection;
                chosenElements = new List<Element>()
                {
                document.GetElement(selection.PickObject(ObjectType.Element).ElementId)
                };
            }
            else if (SelectionType == SelectionType.MultipleElements)
            {
                Selection selection = commandData.Application.ActiveUIDocument.Selection;
                IList<Reference> references = selection.PickObjects(ObjectType.Element);

                chosenElements = new List<Element>();
                foreach (var item in references)
                {
                    chosenElements.Add(document.GetElement(item.ElementId));
                }
            }
            return chosenElements;

    }

        internal abstract Result ExecuteCommand(ExternalCommandData commandData, ICollection<BasePanel> panels);
        
    }
}

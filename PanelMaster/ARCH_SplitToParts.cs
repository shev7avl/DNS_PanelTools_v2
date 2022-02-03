using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_SplitToParts : Autodesk.Revit.UI.IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            ActiveDocument = commandData.Application.ActiveUIDocument.Document;

            //List<Element> list_Walls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(o => o.Name.Contains("DNS_")).ToList();

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");


            Debug.WriteLine(ActiveDocument.PathName);

            
            foreach (var reference in list_Walls)
            {
                Element item = ActiveDocument.GetElement(reference.ElementId);
                try
                {
                    Utility.Parts.SplitToParts(ActiveDocument, item);
                    Utility.Parts.ExcludeStitches(ActiveDocument, item);
                    Debug.WriteLine(item.Name);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ошибка{e.Message} : {item.Name} - {item.Id}");
                }
            }
                         
            return Result.Succeeded;
        }
    }

    public class FacadeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {

            if (elem.GetType() == typeof(Wall) && elem.Name.Contains("DNS_Фасад"))
            {
                return true;
            }

            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }

}

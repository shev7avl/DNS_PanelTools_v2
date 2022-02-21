using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_Windows : Autodesk.Revit.UI.IExternalCommand
    {
        Document Document;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }
            //List<Element> list_Walls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(o => o.Name.Contains("DNS_")).ToList();

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");


            Debug.WriteLine(Document.PathName);

            foreach (var reference in list_Walls)
            {
                Element item = Document.GetElement(reference.ElementId);
                try
                {
                    Utility.Openings.CreateFacadeOpening(Document, item);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"Ошибка{e.Message} : {item.Name} - {item.Id}");
                }
            }
                         
            return Result.Succeeded;
        }
    }

}

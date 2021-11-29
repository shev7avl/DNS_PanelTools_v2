using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools_v2.Architecture;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_WindowTestFork : Autodesk.Revit.UI.IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;

            //List<Element> list_Walls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType().Where(o => o.Name.Contains("DNS_")).ToList();

            Selection selection = commandData.Application.ActiveUIDocument.Selection;

            IList<Reference> list_Walls = selection.PickObjects(ObjectType.Element, new FacadeSelectionFilter(), "Выберите стены DNS_Фасад или DNS_Фасад2");


            logger.DebugLog(ActiveDocument.PathName);

            foreach (var reference in list_Walls)
            {
                Element item = ActiveDocument.GetElement(reference.ElementId);
                try
                {
                    Utility.Openings.CreateFacadeOpening(ActiveDocument, item);
                }
                catch (Exception e)
                {
                    logger.DebugLog($"Ошибка{e.Message} : {item.Name} - {item.Id}");
                }
            }
                         
            return Result.Succeeded;
        }
    }

}

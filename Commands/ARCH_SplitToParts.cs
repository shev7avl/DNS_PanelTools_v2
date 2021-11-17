using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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
    class ARCH_SplitToParts : IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Logger.Logger logger = Logger.Logger.getInstance();
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector fecWalls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            logger.DebugLog(ActiveDocument.PathName);

            foreach (var item in fecWalls)
            {
                try
                {
                    Utility.Parts.SplitToParts(ActiveDocument, item);
                    Utility.Parts.ExcludeStitches(ActiveDocument, item);
                    logger.DebugLog(item.Name);
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

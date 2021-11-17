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
    class ARCH_copyMarks : IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Logger.Logger logger = Logger.Logger.getInstance();

            ActiveDocument = commandData.Application.ActiveUIDocument.Document;
            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocARCH = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            logger.DebugLog(linkedDocARCH.PathName);

            foreach (Element item in fecWalls)
            {
                WallParts wallParts = new WallParts(ActiveDocument, linkedDocSTR, item);
                wallParts.CreateMarks();
                logger.DebugLog(item.Name);
            }


            return Result.Succeeded;
        }
    }

}

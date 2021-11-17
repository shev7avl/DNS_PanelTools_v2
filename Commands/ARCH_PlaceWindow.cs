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
    class ARCH_PlaceWindow : Routine
    {
        public override StructuralApps.Panel.Panel Behaviour { get; set; }
        public override Document Document { get; set ; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Document = commandData.Application.ActiveUIDocument.Document;
            Logger.Logger logger = Logger.Logger.getInstance();

            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            logger.DebugLog(Document.PathName);

            foreach (Element item in fecWalls)
            {
                Utility.Openings.CreateFacadeOpening(Document, item);

                logger.DebugLog(item.Name);
            }
        }

        
    }

}

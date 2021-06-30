using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DNS_PanelTools_v2.StructuralApps.Openings;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STRUCT_CreateOpenings : IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;

            FilteredElementCollector fecStruct = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            FilteredElementCollector fecLinks = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType();
            Document LinkedDocument = fecLinks.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();


            Debug.WriteLine(LinkedDocument.PathName);
            OpeningBuilder openingBuilder = new OpeningBuilder(ActiveDocument, LinkedDocument);
            Element item = fecStruct.Cast<Element>().ToList()[0];

                openingBuilder.CreateOpening(item);
                Debug.WriteLine(item.Name);
                
           


            return Result.Succeeded;
        }
    }
}

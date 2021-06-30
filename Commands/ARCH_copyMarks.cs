﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DNS_PanelTools_v2.Architecture;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_copyMarks : IExternalCommand
    {
        Document ActiveDocument;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;
            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));
            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocARCH = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            Debug.WriteLine(linkedDocARCH.PathName);

            foreach (Element item in fecWalls)
            {
                WallParts wallParts = new WallParts(ActiveDocument, linkedDocSTR, linkedDocARCH, item);
                wallParts.CreateMarks();
                Debug.WriteLine(item.Name);
            }


            return Result.Succeeded;
        }
    }

}

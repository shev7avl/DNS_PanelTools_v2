using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Diagnostics;
using DNS_PanelTools_v2.StructuralApps.Panel;
using DNS_PanelTools_v2.StructuralApps;

namespace DNS_PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class STRUCT_CreateOpenings : IExternalCommand
    {
        Document ActiveDocument;
        //Document LinkedStruct;
        Document LinkedArch;
        public Base_Panel Behaviour { get; set; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ActiveDocument = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<Element> fecLinksARCH = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_АР"));
            //IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));

            FilteredElementCollector fecStruct = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType();
            FilteredElementCollector fecWalls = new FilteredElementCollector(ActiveDocument).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            LinkedArch = fecLinksARCH.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            //LinkedStruct = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();
            List<Element> els;
            if (fecStruct.GetElementCount() == 0)
            {
                els = fecWalls.Cast<Element>().ToList();
            }
            else
            {
                els = fecStruct.Cast<Element>().ToList();
            }

            foreach (Element item in els)
            {
                SetPanelBehaviour(item);
                if (Behaviour is IPerforable perforable)
                {
                        Debug.WriteLine($"Панель: {item.Name}; Id: {item.Id}");
                        IPerforable panel = perforable;
                        panel.GetOpenings(LinkedArch, out List<Element> IntersectedWindows);
                        panel.Perforate(IntersectedWindows);
                }
            }  
            return Result.Succeeded;
        }

        protected void SetPanelBehaviour(Element element)
        {
            StructureType structureType = new StructureType(element);
            string type = structureType.GetPanelType(element);
            if (element.Name.Contains("Фасад") )
            {
                WallParts wallParts = new WallParts(ActiveDocument, element);
                Behaviour = wallParts;
            }
            else if(type == StructureType.Panels.NS.ToString())
            {
                NS_Panel nS = new NS_Panel(ActiveDocument, element);
                Behaviour = nS;
            }
            else if (type == StructureType.Panels.VS.ToString())
            {
                VS_Panel vS = new VS_Panel(ActiveDocument, element);
                Behaviour = vS;
            }
            else if(type == StructureType.Panels.BP.ToString())
            {
                BP_Panel bP = new BP_Panel(ActiveDocument, element);
                Behaviour = bP;
            }
            else if(type == StructureType.Panels.PS.ToString())
            {
                PS_Panel pS = new PS_Panel(ActiveDocument, element);
                Behaviour = pS;
            }
            else if(type == StructureType.Panels.PP.ToString())
            {
                PP_Panel pP = new PP_Panel(ActiveDocument, element);
                Behaviour = pP;
            }

        }
    }
}

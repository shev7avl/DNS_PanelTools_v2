using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools_v2.Architecture;
using DSKPrim.PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools_v2.Commands
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class ARCH_copyMarks : Routine
    {

        public override StructuralApps.Panel.Panel Behaviour { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Document Document { get; set ; }

        public override void ExecuteRoutine(ExternalCommandData commandData)
        {
            Logger.Logger logger = Logger.Logger.getInstance();

            Document = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            Dictionary<Element, List<string>> facadePanel;
            Dictionary<Element, Dictionary<string, Element>> facadeParts;
            

            foreach (Element item in fecWalls)
            {
                CreateFacadePanelPartsData(linkedDocSTR, out facadePanel, out facadeParts, item);

                SetFacadePartsParameters(facadePanel, facadeParts, item);
            }
        }
        /// <summary>
        /// Назначает параметры фасадной стене и фасадным частям
        /// </summary>
        /// <param name="facadePanel"></param>
        /// <param name="facadeParts"></param>
        /// <param name="item"></param>
        private static void SetFacadePartsParameters(Dictionary<Element, List<string>> facadePanel, Dictionary<Element, Dictionary<string, Element>> facadeParts, Element item)
        {
            foreach (var facade in facadePanel.Keys)
            {
                //TODO: Вписать нужные параметры трансфера Панель - Фасад
                facade.get_Parameter(new Guid()).SetValueString("");
                facade.get_Parameter(new Guid()).SetValueString("");
            }
            foreach (var part in facadeParts.Keys)
            {
                string code = "";

                foreach (string materialCode in facadeParts.Values.First().Keys)
                {
                    code += materialCode;
                    code += ".";
                }
                //TODO: Вписать нужные параметры для кода материала
                item.get_Parameter(new Guid()).SetValueString(code);

                foreach (Element el in facadeParts.Values.First().Values)
                {
                    //TODO: Вписать нужные параметры (параметры для частей фасада)
                    el.get_Parameter(new Guid()).SetValueString("");
                    el.get_Parameter(new Guid()).SetValueString("");
                    el.get_Parameter(new Guid()).SetValueString("");
                }
            }
        }

        /// <summary>
        /// Записывает данные о параметрах панели фасада, параметрах материала, наборе частей (плитки)
        /// </summary>
        /// <param name="linkedDocSTR"></param>
        /// <param name="facadePanel"></param>
        /// <param name="facadeParts"></param>
        /// <param name="item"></param>
        private void CreateFacadePanelPartsData(Document linkedDocSTR, out Dictionary<Element, List<string>> facadePanel, out Dictionary<Element, Dictionary<string, Element>> facadeParts, Element item)
        {
            ElementIntersectsElementFilter facadeIntersectionFilter = new ElementIntersectsElementFilter(item);
            facadePanel = new Dictionary<Element, List<string>>();
            facadeParts = new Dictionary<Element, Dictionary<string, Element>>();

            Element panel = new FilteredElementCollector(linkedDocSTR).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(facadeIntersectionFilter).ToElements().FirstOrDefault();
            List<Element> parts = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Parts).WhereElementIsNotElementType().WherePasses(facadeIntersectionFilter).ToElements().ToList();

            facadePanel.Add(item, new List<string>()
                {
                    //TODO: Вписать нужные параметры трансфера Панель - Фасад
                    panel.get_Parameter(new Guid()).AsValueString(),
                    panel.get_Parameter(new Guid()).AsValueString()
                });

            Dictionary<string, Element> temp = new Dictionary<string, Element>();

            foreach (var pt in parts)
            {
                Material material = (Material)Document.GetElement(pt.GetMaterialIds(returnPaintMaterials: false).FirstOrDefault());
                string code = material.get_Parameter(new Guid()).AsValueString();
                temp.Add(
                    code,
                    pt
                    );
            };

            facadeParts.Add(item, temp);
        }
    }

}

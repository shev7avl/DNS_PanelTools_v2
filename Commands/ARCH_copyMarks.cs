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
    class ARCH_copyMarks : Autodesk.Revit.UI.IExternalCommand
    {

        public Document Document { get; set ; }

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Logger.Logger logger = Logger.Logger.getInstance();     

            Document = commandData.Application.ActiveUIDocument.Document;

            IEnumerable<Element> fecLinksSTRUCT = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_RvtLinks).WhereElementIsNotElementType().Where(doc => doc.Name.Contains("_КР") || doc.Name.Contains("_КЖ"));
            FilteredElementCollector fecWalls = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();

            Document linkedDocSTR = fecLinksSTRUCT.Cast<RevitLinkInstance>().ToList()[0].GetLinkDocument();

            Dictionary<Element, List<string>> facadePanel;
            Dictionary<Element, Dictionary<string, List<ElementId>>> facadeParts;


            foreach (Element item in fecWalls)
            {
                CreateFacadePanelPartsData(linkedDocSTR, out facadePanel, out facadeParts, item);

                SetFacadePartsParameters(Document, facadePanel, facadeParts, item);
            }

            return Result.Succeeded;
        }
        /// <summary>
        /// Назначает параметры фасадной стене и фасадным частям
        /// </summary>
        /// <param name="facadePanel"></param>
        /// <param name="facadeParts"></param>
        /// <param name="item"></param>
        private static void SetFacadePartsParameters(Document document, Dictionary<Element, List<string>> facadePanel, Dictionary<Element, Dictionary<string, List<ElementId>>> facadeParts, Element item)
        {

            List<string> parameterValues;
            Transaction transaction = new Transaction(document, "Setting params");
            FailureHandlingOptions opts = transaction.GetFailureHandlingOptions();
            opts.SetFailuresPreprocessor(new Utility.WarningDiscard());
            transaction.SetFailureHandlingOptions(opts);

            using (transaction)
            {

                transaction.Start();
                foreach (var facade in facadePanel.Keys)
                {
                    facadePanel.TryGetValue(facade, out parameterValues);

                    string fullCode = parameterValues[0];
                    string shortCode = parameterValues[1];

                    //TODO: Вписать нужные параметры трансфера Панель - Фасад
                    facade.get_Parameter(new Guid(Properties.Resource.DNS_Полная_марка_изделия)).Set(fullCode);
                    facade.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).Set(shortCode);
                   
                    if (facadeParts.Count > 0)
                    {
                        foreach (var part in facadeParts.Keys)
                        {
                            string code = "";
                            List<string> materialCodes;
                            if (facadeParts.Values.First().Count > 0)
                            {
                                code = String.Join(".", facadeParts.Values.First().Keys);                              
                            }
                           
                            //TODO: Вписать нужные параметры для кода материала
                            item.get_Parameter(new Guid(Properties.Resource.DNS_Марка_элемента)).Set(code);

                            foreach (List<ElementId> elIds in facadeParts.Values.First().Values)
                            {
                                foreach (ElementId elId in elIds)
                                {
                                    Element el = document.GetElement(elId);
                                    el.get_Parameter(new Guid(Properties.Resource.DNS_Полная_марка_изделия)).Set(fullCode);
                                    el.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).Set(shortCode);
                                    el.get_Parameter(new Guid(Properties.Resource.DNS_Марка_элемента)).Set(code);
                                }
                                //TODO: Вписать нужные параметры (параметры для частей фасада)
                            }
                        }
                    }
                }
                transaction.Commit();
            }
        }

        /// <summary>
        /// Записывает данные о параметрах панели фасада, параметрах материала, наборе частей (плитки)
        /// </summary>
        /// <param name="linkedDocSTR"></param>
        /// <param name="facadePanel"></param>
        /// <param name="facadeParts"></param>
        /// <param name="item"></param>
        private void CreateFacadePanelPartsData(Document linkedDocSTR, out Dictionary<Element, List<string>> facadePanel, out Dictionary<Element, Dictionary<string, List<ElementId>>> facadeParts, Element item)
        {
            ElementIntersectsElementFilter facadeIntersectionFilter = new ElementIntersectsElementFilter(item);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter(new Outline(item.get_Geometry(new Options()).GetBoundingBox().Min, item.get_Geometry(new Options()).GetBoundingBox().Max));
            facadePanel = new Dictionary<Element, List<string>>();
            facadeParts = new Dictionary<Element, Dictionary<string, List<ElementId>>>();

            Element panel = new FilteredElementCollector(linkedDocSTR).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().WherePasses(boundingBoxIntersectsFilter).WherePasses(facadeIntersectionFilter).ToElements().FirstOrDefault();
            List<Element> parts = new FilteredElementCollector(Document).OfCategory(BuiltInCategory.OST_Parts).WhereElementIsNotElementType().WherePasses(boundingBoxIntersectsFilter).ToElements().ToList();

            facadePanel.Add(item, new List<string>()
                {
                    //TODO: Вписать нужные параметры трансфера Панель - Фасад
                    panel.get_Parameter(new Guid(Properties.Resource.DNS_Полная_марка_изделия)).AsString(),
                    panel.get_Parameter(new Guid(Properties.Resource.ADSK_Марка_изделия)).AsString()
                });

            Dictionary<string, List<ElementId>> temp = new Dictionary<string, List<ElementId>>();

            foreach (var pt in parts)
            {
                Material material = (Material)Document.GetElement(pt.GetMaterialIds(returnPaintMaterials: false).FirstOrDefault());
                if (material != null)
                {
                    string code = material.get_Parameter(new Guid("2c1f6e3d-cf4b-4c93-92fe-92e85acacd26")).AsString();
                    if (code != null)
                    {
                        code = code.Substring(code.Length - 2);
                        if (temp.ContainsKey(code))
                        {
                            temp[code].Add(pt.Id);
                        }
                        else
                        {
                            temp.Add(code, new List<ElementId>() { pt.Id });
                        }
                    }
                }               
            };

            facadeParts.Add(item, temp);
        }

        
    }

}

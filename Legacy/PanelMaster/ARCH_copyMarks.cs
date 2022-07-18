using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DSKPrim.PanelTools.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ARCH_copyMarks : IExternalCommand
    {

        public Document Document { get; set ; }

        private SelectionType SelectionType;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document = commandData.Application.ActiveUIDocument.Document;

            if (TransactionSettings.WorksetsUnavailable(Document))
            {
                message = "Рабочие наборы недоступны. Освободите ВСЕ рабочие наборы";
                return Result.Failed;
            }

            AddinSettings settings = AddinSettings.GetSettings();
            Selector selector = new Selector();
            ICollection<Element> els = selector.CollectElements(commandData, new FacadeSelectionFilter(), BuiltInCategory.OST_Walls);

            try
            {
                foreach (Element item in els)
                {
                    Facade_Panel facade = new Facade_Panel(item);
                    facade.CreateMarks();
                }
            }
            catch (Exception e)
            {
                message = $"Ошибка {e.Message} \n" +
                   $"{e.InnerException}" +
                   $"{e.Source}";
                return Result.Failed;
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

            Transaction transaction = new Transaction(document, "Setting params");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            using (transaction)
            {

                transaction.Start();
                foreach (var facade in facadePanel.Keys)
                {
                    facadePanel.TryGetValue(facade, out List<string> parameterValues);

                    string fullCode = parameterValues[0];
                    string shortCode = parameterValues[1];
                    string posCode = parameterValues[2];

                    ParameterMap parameters = facade.ParametersMap;

                    parameters.get_Item("DNS_Код изделия полный").Set(fullCode);
                    parameters.get_Item("ADSK_Марка конструкции").Set(shortCode);
                    parameters.get_Item("DNS_Марка элемента").Set(posCode);
                   
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
            facadePanel = new Dictionary<Element, List<string>>();
            facadeParts = new Dictionary<Element, Dictionary<string, List<ElementId>>>();

            ElementIntersectsElementFilter facadeIntersectionFilter = new ElementIntersectsElementFilter(item);
            BoundingBoxIntersectsFilter boundingBoxIntersectsFilter = new BoundingBoxIntersectsFilter
                (new Outline(item.get_Geometry(new Options())
                .GetBoundingBox().Min,
                item.get_Geometry(new Options())
                .GetBoundingBox().Max));


            Element panel = new FilteredElementCollector(linkedDocSTR).
                OfCategory(BuiltInCategory.OST_StructuralFraming).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                WherePasses(facadeIntersectionFilter).
                ToElements().
                FirstOrDefault();

            List<Element> parts = new FilteredElementCollector(Document).
                OfCategory(BuiltInCategory.OST_Parts).
                WhereElementIsNotElementType().
                WherePasses(boundingBoxIntersectsFilter).
                ToElements().
                ToList();

            ParameterMap parameterMap = panel.ParametersMap;

            facadePanel.Add(item, new List<string>()
                {
                parameterMap.get_Item("DNS_Код изделия полный").AsString(),
                parameterMap.get_Item("ADSK_Марка конструкции").AsString(),
                parameterMap.get_Item("DNS_Марка элемента").AsString(),
                }) ;

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

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.PanelMaster
{
    [Transaction(mode: TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class STR_CopyArchMarks : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Dictionary<string, string> archLocationColorMap = new Dictionary<string, string>();

            var document = commandData.Application.ActiveUIDocument.Document;

            var facadeLink = new FilteredElementCollector(document).
                OfClass(typeof(RevitLinkInstance)).
                OfCategory(BuiltInCategory.OST_RvtLinks).
                Where(a => a.Name.ToLower().Contains("фасад")).
                Cast<RevitLinkInstance>().
                FirstOrDefault();

            if (facadeLink == null)
            {
                throw new Exception("--> Не удалось найти файл АКР. Имя файла должно содержать \"Фасад\"");
            }

            var facadeParts = new FilteredElementCollector(facadeLink.GetLinkDocument()).
                OfCategory(BuiltInCategory.OST_Parts).
                ToList();

            var panels = new FilteredElementCollector(document).
                OfClass(typeof(FamilyInstance)).
                OfCategory(BuiltInCategory.OST_StructuralFraming).
                Where(e => CheckStructureType(e)).
                ToList();



            foreach (var item in facadeParts)
            {
                var key = item.ParametersMap.get_Item("DNS_Марка элемента").AsString();
                var value = item.ParametersMap.get_Item("DNS_Код фасада").AsString();
                if (!archLocationColorMap.ContainsKey(key) && value != null)
                {
                    archLocationColorMap.Add(key, value);
                }
            }

            Transaction transaction = new Transaction(document, "copy parameters");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();

            foreach (var item in panels)
            {
                var key = item.ParametersMap.get_Item("DNS_Марка элемента").AsString();
                if (archLocationColorMap.TryGetValue(key, out string value))
                {
                    item.ParametersMap.get_Item("DNS_Код фасада").Set(value);
                }              
            }

            transaction.Commit();

            return Result.Succeeded;
        }

        private bool CheckStructureType(Element element)
        { 
            var strType = new StructureType(element);
            var type = strType.GetPanelType(element);
            if (type == StructureType.PanelTypes.NS_PANEL) return true;
            else return false;
        }
    }
}

using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace DSKPrim.PanelTools.Precast
{
    internal enum PrecastType
    {
        NS_PANEL,
        VS_PANEL,
        PP_PANEL,
        PS_PANEL,
        BP_PANEL,
        FACADE_PANEL
    }
    internal static class PrecastTypeMapping
    {
        private readonly static Dictionary<PrecastType, List<string>> PrecastTypeFamilyNameMap = new Dictionary<PrecastType, List<string>>
        {
            { PrecastType.NS_PANEL, new List<string>()
            {
                "NS_Empty",
                "NS_Medium"
            } },
            { PrecastType.VS_PANEL, new List<string>()
            {
                "VS_Empty",
                "VS_Medium"
            } },
            { PrecastType.PP_PANEL, new List<string>()
            {
                "PP_Empty"
            } },
            { PrecastType.PS_PANEL, new List<string>()
            {
                "PS_Empty",
                "PS_Medium"
            } },
            { PrecastType.PS_PANEL, new List<string>()
            {
                "BP_Empty",
                "BP_Medium",
                "BP_Empty SupBranch",
                "BP_Medium SupBranch"
            } },
            { PrecastType.FACADE_PANEL, new List<string>()
            { 
            "DNS_Фасад",
            "DNS_Фасад2"
            } }
        };

        private readonly static Dictionary<PrecastType, BuiltInCategory> PrecastTypeCategoryMap = new Dictionary<PrecastType, BuiltInCategory>
        {
            { PrecastType.NS_PANEL, BuiltInCategory.OST_StructuralFraming},
            { PrecastType.VS_PANEL, BuiltInCategory.OST_StructuralFraming},
            { PrecastType.PP_PANEL, BuiltInCategory.OST_StructuralFraming},
            { PrecastType.PS_PANEL, BuiltInCategory.OST_StructuralFraming},
            { PrecastType.PS_PANEL, BuiltInCategory.OST_StructuralFraming},
            { PrecastType.FACADE_PANEL, BuiltInCategory.OST_Walls}
        };

        internal static BuiltInCategory GetBuiltInCategory(PrecastType precastType)
        {
            BuiltInCategory result;
            PrecastTypeCategoryMap.TryGetValue(precastType, out result);
            return result;
        }
    }
    
}
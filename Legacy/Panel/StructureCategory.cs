using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Panel
{

    public enum StructureType
    {
        NS_PANEL,
        NS_PANEL_EMBEDDED,
        VS_PANEL,
        VS_PANEL_EMBEDDED,
        PP_PANEL,
        BP_PANEL,
        BP_PANEL_SUPPORT,
        PS_PANEL,
        FACADE_PANEL,
        NOT_A_PANEL
    }
    public static class StructureTypeMapper
    {
        private readonly static Dictionary<StructureType, List<string>> _familyNameTypeMapping = new Dictionary<StructureType, List<string>>()
        {
            { StructureType.NS_PANEL,
                new List<string>(){"NS_Empty", "NS_Medium" } },
            { StructureType.NS_PANEL_EMBEDDED,
                new List<string>(){"N.NS_Empty", "N.NS_Medium", "Z.NS_Empty", "Z.NS_Medium"} },
            { StructureType.VS_PANEL,
                new List<string>(){"VS_Empty", "VS_Medium" } },
            { StructureType.VS_PANEL_EMBEDDED,
                new List<string>(){"N.VS_Empty", "N.VS_Medium", "Z.VS_Empty", "Z.VS_Medium"} },
            { StructureType.PP_PANEL,
                new List<string>(){"PP_Empty"} },
            { StructureType.BP_PANEL,
                new List<string>(){"BP_Empty", "BP_Medium" } },
            { StructureType.BP_PANEL_SUPPORT,
                new List<string>(){"S.BP_Empty", "S.BP_Medium" } },
            { StructureType.PS_PANEL,
                new List<string>(){"PS_Empty", "PS_Medium" } },
            { StructureType.FACADE_PANEL,
                new List<string>(){"DNS_Фасад", "DNS_Фасад2" } }
        };

        public static StructureType GetStructureType(Element element)
        {
            FamilyInstance instance = element as FamilyInstance;
            string familyName = instance.Symbol.Family.Name;

            foreach (var type in _familyNameTypeMapping.Keys)
            {
                if (_familyNameTypeMapping[type].Contains(familyName))
                {
                    return type;
                }
            }
            return StructureType.NOT_A_PANEL;
        }

       
       
    }

}

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
        VS_PANEL,
        PP_PANEL,
        BP_PANEL,
        PS_PANEL,
        FACADE_PANEL,
        NOT_A_PANEL
    }
    public class StructureCategory
    {
        private Element _element;
        public StructureType StructureType { get { return GetStructureType(_element); } }

        public StructureCategory(Element element)
        {
            _element = element;
        }

        public StructureType GetStructureType(Element element)
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

        public bool IsValidCategory()
        {
            if (StructureType is StructureType.NOT_A_PANEL)
            {
                return false;
            }
            else return true;

        }

        private readonly Dictionary<StructureType, List<string>> _familyNameTypeMapping = new Dictionary<StructureType, List<string>>()
        {
            { StructureType.NS_PANEL, 
                new List<string>(){"NS_Empty", "NS_Medium" } },
            { StructureType.VS_PANEL,
                new List<string>(){"VS_Empty", "VS_Medium" } },
            { StructureType.PP_PANEL,
                new List<string>(){"PP_Empty"} },
            { StructureType.BP_PANEL,
                new List<string>(){"BP_Empty", "BP_Medium" } },
            { StructureType.PS_PANEL,
                new List<string>(){"PS_Empty", "PS_Medium" } },
            { StructureType.FACADE_PANEL,
                new List<string>(){"DNS_Фасад", "DNS_Фасад2" } }
        };

    }

}

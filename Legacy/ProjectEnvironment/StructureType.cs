using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    class StructureType
    {
        public Element ActiveElement { get; }

        public enum PanelTypes
        {
            NS_PANEL,
            VS_PANEL,
            PP_PANEL,
            BP_PANEL,   
            PS_PANEL,
            FACADE_PANEL,
            NOT_A_PANEL
        }
        
        public StructureType(Element element)
        {
            ActiveElement = element;
        }

        public PanelTypes GetPanelType(Element element)
        {
            string familyName;

            var cat = Category.GetCategory(element.Document, BuiltInCategory.OST_StructuralFraming);

            if (element.Category.Name.Contains("Каркас несущий"))
            {
                FamilyInstance family = element as FamilyInstance;
                familyName = family.Symbol.FamilyName;
            }
            else familyName = element.Name;

            var result = PanelTypes.NOT_A_PANEL;
            if (panelTypesMap.ContainsKey(familyName))
            {
                result = panelTypesMap[familyName];
            }
            return result;

        }
        private static readonly Dictionary<string, PanelTypes> panelTypesMap = new Dictionary<string, PanelTypes>()
        {
            {"NS_Empty", PanelTypes.NS_PANEL},
            {"NS_Medium", PanelTypes.NS_PANEL},
            //Навесные
            {"N.NS_Empty", PanelTypes.NS_PANEL},
            {"N.NS_Medium", PanelTypes.NS_PANEL},
            //Закладные
            {"Z.NS_Empty", PanelTypes.NS_PANEL},
            {"Z.NS_Medium", PanelTypes.NS_PANEL},

            {"VS_Empty", PanelTypes.VS_PANEL},
            {"VS_Medium", PanelTypes.VS_PANEL},
            //Навесные
            {"N.VS_Empty", PanelTypes.VS_PANEL},
            {"N.VS_Medium", PanelTypes.VS_PANEL},
            //Закладные
            {"Z.VS_Empty", PanelTypes.VS_PANEL},
            {"Z.VS_Medium", PanelTypes.VS_PANEL},

            {"PP_Empty", PanelTypes.PP_PANEL},

            {"PS_Empty", PanelTypes.PS_PANEL},
            {"PS_Medium", PanelTypes.PS_PANEL},

            {"BP_Empty", PanelTypes.BP_PANEL},
            {"BP_Medium", PanelTypes.BP_PANEL},

            {"DNS_Фасад", PanelTypes.FACADE_PANEL},
            {"DNS_Фасад 2", PanelTypes.FACADE_PANEL},
        };

    }
}

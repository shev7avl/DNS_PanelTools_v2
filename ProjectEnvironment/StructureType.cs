using System;
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

            if (element.Name.Contains("НС") || element.Name.Contains("есущая"))
            {
                return PanelTypes.NS_PANEL;
            }
            else if (element.Name.Contains("ВС"))
            {
                return PanelTypes.VS_PANEL;
            }
            else if (element.Name.Contains("ПП"))
            {
                return PanelTypes.PP_PANEL;
            }
            else if (element.Name.Contains("ПС") || element.Name.Contains("П_100-"))
            {
                return PanelTypes.PS_PANEL;
            }
            else if (element.Name.Contains("БП"))
            {
                return PanelTypes.BP_PANEL;
            }
            else if (element.Name.Contains("Фасад"))
            {
                return PanelTypes.FACADE_PANEL;
            }
            else
            {
                return PanelTypes.NOT_A_PANEL;
            }
        }

    }
}

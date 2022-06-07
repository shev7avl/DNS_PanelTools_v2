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

        private StructureType GetStructureType(Element element)
        {

            if (element.Name.Contains("НС") || element.Name.Contains("есущая"))
            {
                return StructureType.NS_PANEL;
            }
            else if (element.Name.Contains("ВС"))
            {
                return StructureType.VS_PANEL;
            }
            else if (element.Name.Contains("ПП"))
            {
                return StructureType.PP_PANEL;
            }
            else if (element.Name.Contains("ПС") || element.Name.Contains("П_100-"))
            {
                return StructureType.PS_PANEL;
            }
            else if (element.Name.Contains("БП"))
            {
                return StructureType.BP_PANEL;
            }
            else if (element.Name.Contains("Фасад"))
            {
                return StructureType.FACADE_PANEL;
            }
            else
            {
                return StructureType.NOT_A_PANEL;
            }
        }

        public bool IsValidCategory()
        {
            if (StructureType is StructureType.NOT_A_PANEL)
            {
                return false;
            }
            else return true;

        }

    }

}

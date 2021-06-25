using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DNS_PanelTools_v2.StructuralApps
{
    class StructureType
    {
        public Element ActiveElement { get; }

        [Flags]
        public enum Panels
        {
            NS = 0x00000001, // 1
            VS = 0x00000002, //2
            PP = 0x00000003,//3
            VS_elev = 0x00000004, //4
            BP = 0x0000003D,//61
            VS_balc = 0x0000003E, //62      
            PS = 0x00000007, //7
            Balcony = VS_balc | BP,
            VS_all = VS_balc | VS_elev | VS
        }

        public StructureType(Element element)
        {
            ActiveElement = element;
        }

        public string GetPanelType(Element element)
        {
            if (element.Name.Contains("НС"))
            {
                return Panels.NS.ToString();
            }
            else if (element.Name.Contains("ВС"))
            {
                return Panels.VS.ToString();
            }
            else if (element.Name.Contains("ПП"))
            {
                return Panels.PP.ToString();
            }
            else if (element.Name.Contains("ПС") || element.Name.Contains("П_100-"))
            {
                return Panels.PS.ToString();
            }
            else if (element.Name.Contains("БП"))
            {
                return Panels.BP.ToString();
            }
            else
            {
                return Panels.NS.ToString();
            }
        }

    }
}

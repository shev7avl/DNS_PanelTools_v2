using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Panel;
using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.MarkBuilder
{
    public class ShortMarkSchema
    {
        private readonly PrecastPanel _panel;

        private StructureType PanelType { get { return _panel.StructureCategory.StructureType; } }

        public ShortMarkSchema(PrecastPanel panel)
        {

            _panel = panel;
        }

        public string GetMark()
        {
            switch (this.PanelType)
            {
                case StructureType.NOT_A_PANEL:
                    return "";
                    break;
                case StructureType.NS_PANEL:
                    return GetNSMarkSchema();
                    break;
                case StructureType.VS_PANEL:
                    return GetSimpleMarkSchema();
                    break;
                case StructureType.PP_PANEL:
                    return GetSimpleMarkSchema();
                    break;
                case StructureType.BP_PANEL:
                    return GetSimpleMarkSchema();
                    break;
                case StructureType.PS_PANEL:
                    return GetPSMarkSchema();
                    break;
                default:
                    return "";
                    break;
            }
        }

        private string GetNSMarkSchema()
        {
            return $"НС {_panel.Mark.LongMark.Split('_')[1]}-{_panel.Mark.Index}";
        }

        private string GetPSMarkSchema()
        {
            return $"ПС {_panel.Mark.LongMark.Split('_')[1]}-{_panel.Mark.Index}";
        }

        private string GetSimpleMarkSchema()
        {
            return $"{_panel.Mark.LongMark.Split('_')[0]}-{_panel.Mark.Index}";


        }

           }
}

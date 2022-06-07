using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Builders.MarkBuilder
{

    public class MarkBuilder
    {
        private readonly PrecastPanel _panel;
        private readonly ParameterMap _instanceMap;
        private readonly ParameterMap _symbolMap;

        private Mark Mark { get; set; }

        public MarkBuilder(PrecastPanel panel)
        {
            _panel = panel;
            var elementFamily = _panel.ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            _instanceMap = elementFamily.ParametersMap;
            _symbolMap = familySymbol.ParametersMap;

            Mark = new Mark();
        }

        public MarkBuilder CreateLongMark()
        {
            LongMarkSchema schema = new LongMarkSchema(_panel);
            Mark.LongMark = schema.GetMark();
            return this;
        }

        public MarkBuilder CreateIndex()
        {
            string indexValue = _instanceMap.get_Item("ADSK_Номер изделия").AsString();
            if (indexValue is null || indexValue.Length == 0)
            {
                Mark.Index = $"ID{_panel.ActiveElement.Id}";
            }
            else Mark.Index = indexValue;
            return this;
        }
        public MarkBuilder CreateShortMark()
        {
            ShortMarkSchema schema = new ShortMarkSchema(_panel);
            Mark.ShortMark = schema.GetMark();
            return this;
        }

        
    }    
}
           
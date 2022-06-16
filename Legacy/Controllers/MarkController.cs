using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Builders.MarkBuilder;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Legacy.Controllers
{

    public class MarkController
    {
        private readonly PrecastPanel _panel;
        private readonly ParameterMap _instanceMap;
        private readonly ParameterMap _symbolMap;

        private Mark Mark { get; set; }

        public MarkController(PrecastPanel panel)
        {
            _panel = panel;
            var elementFamily = _panel.ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            _instanceMap = elementFamily.ParametersMap;
            _symbolMap = familySymbol.ParametersMap;   
        }

        public Mark Create()
        {
            Mark = new Mark();
            this.CreateLongMark();
            this.CreateIndex();
            this.CreateShortMark();
            return this.Mark;
        }

        public Mark Read()
        {
            return _panel.Mark;
        }

        public Mark Update()
        {
            var newMark = this.Create();
            if (newMark.IsEqual(_panel.Mark))
            {
                return _panel.Mark;
            }
            else return newMark;
        }

        public Mark Delete()
        {
            var emptyMark = new Mark();
            this._panel.Mark = emptyMark;
            return emptyMark;
        }

        public void Write()
        {
            Document document = _panel.ActiveDocument;
            Transaction writeParameters = new Transaction(document, "Writing marks");
            TransactionSettings.SetFailuresPreprocessor(writeParameters);

            using (writeParameters)
            {
                writeParameters.Start();
                SubTransaction sub = new SubTransaction(document);
                sub.Start();
                _instanceMap.get_Item("DNS_Полная марка изделия").Set(_panel.Mark.LongMark);
                sub.Commit();
                sub.Start();
                _instanceMap.get_Item("ADSK_Номер изделия").Set(_panel.Mark.Index);
                sub.Commit();
                sub.Start();
                _instanceMap.get_Item("ADSK_Марка изделия").Set(_panel.Mark.ShortMark);
                sub.Commit();
                writeParameters.Commit();
            }
        }
        

        private MarkController CreateLongMark()
        {
            LongMarkSchema schema = new LongMarkSchema(_panel);
            Mark.LongMark = schema.GetMark();
            return this;
        }
        private MarkController CreateIndex()
        {
            string indexValue = _instanceMap.get_Item("ADSK_Номер изделия").AsString();
            if (indexValue is null || indexValue.Length == 0)
            {
                Mark.Index = $"ID{_panel.ActiveElement.Id}";
            }
            else Mark.Index = indexValue;
            return this;
        }
        private MarkController CreateShortMark()
        {
            ShortMarkSchema schema = new ShortMarkSchema(_panel);
            Mark.ShortMark = schema.GetMark();
            return this;
        }

        
    }    
}
           
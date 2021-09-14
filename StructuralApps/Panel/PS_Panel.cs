using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools_v2.Utility;

namespace DSKPrim.PanelTools_v2.StructuralApps.Panel
{
    public class PS_Panel : Panel, IAssembler
    {
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }

        public override string Index { get; set; }
        public List<ElementId> AssemblyElements { get; set; }

        public List<ElementId> OutList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<ElementId> PVLList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IAssembler TransferPal { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public PS_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        public event TransferHandler TransferRequested;

        public override void CreateMarks()
        {

            LongMark = $"ПС {GetPanelCode()}";

            Guid ADSK_panelNum = new Guid("a531f6df-1e58-48e0-8c14-77cf7c1809b8");
            if (ActiveElement.get_Parameter(ADSK_panelNum).AsString() == "")
            {
                Index = $"{ActiveElement.Id}-Id";
            }
            else
            {
                Index = ActiveElement.get_Parameter(ADSK_panelNum).AsString();
            }

            ShortMark = $"ПС {LongMark.Split('_')[1]} - {Index}";
            SetMarks();
        }

        

        private string GetPanelCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;
            string i1 = ActiveElement.LookupParameter("СТАРТ").AsValueString();
            string i2 = ActiveElement.LookupParameter("ФИНИШ").AsValueString();
            string i3 = Marks.AsDecimString(ActiveElement, "ГабаритДлина");
            string i4 = Marks.AsDecimString(familySymbol, "ГабаритВысота");
            string i5 = Marks.AsDecimString(familySymbol, "ГабаритТолщина");
            
            string i6 = Marks.AsDecimString(ActiveElement, "WELDA_Отступ");
            string i7 = Marks.AsDecimString(ActiveElement, "WELDA_Шаг");
            string i8 = ActiveElement.LookupParameter("WELDA_Количество").AsValueString();

            string i9 = Marks.AsDecimString(ActiveElement, "TR 24_Сверху_Отступ");
            string i10 = Marks.AsDecimString(ActiveElement, "TR 24_Сверху_Шаг");
            string i11 = ActiveElement.LookupParameter("TR 24_Сверху_Количество").AsValueString();

           
            return $"{i1}.{i2}_{i3}.{i4}.{i5}_{i6}.{i7}.{i8}_{i9}.{i10}.{i11}";
        }

        public void SetAssemblyElements()
        {
            AssemblyElements = new List<ElementId>();

            FamilyInstance family = (FamilyInstance)ActiveElement;

            foreach (var item in family.GetSubComponentIds())
            {
                this.AssemblyElements.Add(item);
                FamilyInstance element = (FamilyInstance)ActiveDocument.GetElement(item);
                if (element.Name.Contains("Каркас") || element.Name.Contains("Сетка") || element.Name.Contains("Пенополистирол_Массив"))
                {
                    AssemblyElements.AddRange(element.GetSubComponentIds());
                }
            }

            this.AssemblyElements.Add(ActiveElement.Id);
        }

        public void TransferFromPanel(IAssembler panel)
        {

        }

        public void InTransferHandler(object senger, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void ExTransferHandler(object senger, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

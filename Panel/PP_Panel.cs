using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Utility;

namespace DSKPrim.PanelTools.Panel
{
    public class PP_Panel : BasePanel, IAssembler
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
        public override AssemblyInstance AssemblyInstance { get; set; }

        public PP_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        public event TransferHandler TransferRequested;

        public override void CreateMarks()
        {

            Marks.CheckAndSetMark(ActiveElement, Properties.Resource.DNS_Полная_марка_изделия, $"ПП {GetPanelCode()}{GetClosureCode()}", out string longMark);
            LongMark = longMark;

            Marks.CheckAndSetIndex(ActiveElement, Properties.Resource.ADSK_Номер_изделия, out string index);
            Index = index;

            Marks.CheckAndSetMark(ActiveElement, Properties.Resource.ADSK_Марка_изделия, $"{LongMark.Split('_')[0]}-{Index}", out string shortMark);
            ShortMark = shortMark;

        }

        private string GetPanelCode()
        {
            string i3 = Marks.AsDecimString(ActiveElement, "ADSK_Размер_Длина");
            string i4 = Marks.AsDecimString(ActiveElement, "ADSK_Размер_Ширина");
            string i5 = ActiveElement.LookupParameter("КодНагрузки").AsString();
            return $"{i3}.{i4}-{i5}";
        }
        private string GetClosureCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;

            bool closureBool = familySymbol.LookupParameter("Вырезы").AsValueString() == "Да";

            string closureCode = "";

            if (closureBool)
            {
                string w1 = Marks.AsDecimString(ActiveElement, "Вырезы_Отступ_Начало");
                string w2 = Marks.AsDecimString(familySymbol, "Вырезы_Шаг");
                string w3 = ActiveElement.LookupParameter("Отверстия_Количество").AsValueString();
                string w4 = Marks.AsDecimString(ActiveElement, "Вырезы_Отступ_Конец");
                closureCode = $"_{w1}.{w2}.{w3}.{w4}";
            }

            return closureCode;
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

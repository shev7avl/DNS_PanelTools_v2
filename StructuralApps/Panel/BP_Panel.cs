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
    public class BP_Panel : Panel, IAssembler
    {
        #region Fields
        public override Document ActiveDocument { get; set; }

        public override Element ActiveElement { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }

        public override string Index { get; set; }
        public List<ElementId> AssemblyElements { get; set; }
        public List<ElementId> OutList { get; set; }
        public List<ElementId> PVLList { get; set; }
        public IAssembler TransferPal { get; set; }

        #endregion

        #region Constructor
        public BP_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;

        }

        public event TransferHandler TransferRequested;
        #endregion

        #region Public Methods

        public override void CreateMarks()
        {
            LongMark = $"БП {GetPanelCode()}";

            Guid ADSK_panelNum = new Guid("a531f6df-1e58-48e0-8c14-77cf7c1809b8");
            if (ActiveElement.get_Parameter(ADSK_panelNum).AsString() == "")
            {
                Index = $"{ActiveElement.Id}-Id";
            }
            else
            {
                Index = ActiveElement.get_Parameter(ADSK_panelNum).AsString();
            }

            ShortMark = $"{LongMark.Split('_')[0]} - {Index}";

            SetMarks();
        }

        #endregion

        #region Private Methods

        private string GetPanelCode()
        {
            var elementFamily = ActiveElement as FamilyInstance;
            var familySymbol = elementFamily.Symbol;
            string i3 = Marks.AsDecimString(familySymbol, "Плита_Длина");
            string i4 = Marks.AsDecimString(familySymbol, "Плита_Ширина");
            string i5 = Marks.AsDecimString(familySymbol, "Плита_Толщина");
            string i6 = Marks.AsDecimString(familySymbol, "Кронштейн_Отступ");
            string i7 = Marks.AsDecimString(familySymbol, "Кронштейн_Шаг");
            string i8 = familySymbol.LookupParameter("Кронштейн_Количество").AsValueString();
            string i9 = familySymbol.LookupParameter("Отверстия_ПривязкаСлева").AsValueString();
            string i10 = familySymbol.LookupParameter("Отверстия_ПривязкаСправа").AsValueString();
            string i11 = familySymbol.LookupParameter("Отверстия_ПривязкаСпереди").AsValueString();
            string i12 = familySymbol.LookupParameter("Отверстия_РасстояниеМеждуПоY").AsValueString();

            return $"{i3}.{i4}.{i5}_{i6}.{i7}.{i8}.1_{i9}.{i10}_{i11}.{i12}";
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
        #endregion

    }  
}

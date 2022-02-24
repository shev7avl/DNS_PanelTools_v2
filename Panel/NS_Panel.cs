﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;

using DSKPrim.PanelTools.Utility;

namespace DSKPrim.PanelTools.Panel
{
    class NS_Panel : BasePanel, IPerforable, IAssembler
    {
        #region Fields&Props
        public override Document ActiveDocument { get; set; }
        public override Element ActiveElement { get; set; }
        public List<XYZ> IntersectedWindows { get; set; }

        public override string LongMark { get; set; }

        public override string ShortMark { get; set; }

        public override string Index { get; set; }

        public List<ElementId> AssemblyElements { get; set; }
        public List<ElementId> OutList { get; set; }

        public List<ElementId> PVLList { get; set; }
        #endregion

        #region Constructor
        public NS_Panel(Document document, Element element)
        {
            ActiveDocument = document;
            ActiveElement = element;
        }

        #endregion



        #region IPerforable
        void IPerforable.Perforate(List<Element> IntersectedWindows,RevitLinkInstance revitLink)
        {

            TransactionGroup transaction = new TransactionGroup(ActiveDocument, $"Создание проемов - {ActiveElement.Name}");
            transaction.Start();

            Debug.WriteLine("Вырезаем проем");
            if (IntersectedWindows.Count == 1)
            {   
                Element window = IntersectedWindows[0];
                Utility.Openings.SetOpeningParams(ActiveDocument, revitLink, ActiveElement, window);
            }
            else if (IntersectedWindows.Count == 2)
            {
                Element window1 = IntersectedWindows[0];
                Element window2 = IntersectedWindows[1];
                Utility.Openings.SetOpeningParams(ActiveDocument, revitLink, ActiveElement, window1, window2);
            }

            transaction.Assimilate();
        }

        void IPerforable.GetOpeningsFromLink(Document linkedArch, RevitLinkInstance revitLink, out List<Element> IntersectedWindows)
        {

            Debug.WriteLine($"Обрабатываем элемент ID:{ActiveElement.Id}");
            IntersectedWindows = Geometry.IntersectedOpenings(ActiveElement, revitLink, linkedArch, windows: true);
            if (IntersectedWindows.Count == 0)
            {
                IntersectedWindows = Geometry.IntersectedOpenings(ActiveElement, revitLink, linkedArch, windows: false);
            }
            Debug.WriteLine($"Найдено проемов:{IntersectedWindows.Count}");
        }
        #endregion

        #region IAssembler

        public IAssembler TransferPal { get; set; }
        public override AssemblyInstance AssemblyInstance { get; set; }

        public event TransferHandler TransferRequested;

        public void TransferFromPanel(IAssembler panel)
        {

            Debug.WriteLine("Вызов метода NS_Panel.TransferFromPanel()");
            if (panel.AssemblyElements == null)
            {
                panel.SetAssemblyElements();
            }

            if (ChangeClause(this, panel))
            {
                TransferRequested += ExTransferHandler;
                TransferRequested += InTransferHandler;
                TransferRequested.Invoke(panel, new EventArgs());
                TransferRequested -= ExTransferHandler;
                TransferRequested -= InTransferHandler;
            }
        }

        private bool ChangeClause(IAssembler x, IAssembler y)
        {

            BasePanel xPanel = (BasePanel)x;
            BasePanel yPanel = (BasePanel)y;

            bool vSnSlvl = ((xPanel is NS_Panel && yPanel is VS_Panel) || (yPanel is NS_Panel && xPanel is VS_Panel)) && (xPanel.ActiveElement.LevelId == yPanel.ActiveElement.LevelId);

            int lvlX = int.Parse(ActiveDocument.GetElement(xPanel.ActiveElement.LevelId).Name.Substring(ActiveDocument.GetElement(xPanel.ActiveElement.LevelId).Name.Length - 2, 2));
            int lvlY = int.Parse(ActiveDocument.GetElement(yPanel.ActiveElement.LevelId).Name.Substring(ActiveDocument.GetElement(yPanel.ActiveElement.LevelId).Name.Length - 2, 2));

            bool nSlvl = (xPanel is NS_Panel && yPanel is NS_Panel) && (lvlY - lvlX == 1);

            return vSnSlvl || nSlvl;

        }

        public void InTransferHandler(object sender, EventArgs e)
        {

            Debug.WriteLine("Вызов метода NS_Panel.InTransferHandler()");
            IAssembler assembler = (IAssembler)sender;
            if (assembler is NS_Panel)
            {
                foreach (var item in assembler.OutList)
                {
                    assembler.AssemblyElements.Remove(item);
                }
                assembler.OutList = null;
            }
        }

        public void ExTransferHandler(object sender, EventArgs e)
        {
            IAssembler assembler = (IAssembler)sender;

            Debug.WriteLine("Вызов метода NS_Panel.ExTransferHandler()");

            if (assembler.OutList == null)
            {
                assembler.SetAssemblyElements();
            }

            if (assembler is NS_Panel)
            {
                foreach (var item in assembler.OutList)
                {
                    if (IfIntersects(this, item))
                    {
                        this.AssemblyElements.Add(item);
                    }
                }
            }

            else if (assembler is VS_Panel)
            {
                foreach (var item in assembler.PVLList)
                {
                    if (IfIntersects(this, item))
                    {
                        this.AssemblyElements.Add(item);
                    }
                }
            }
           
        }

        private bool IfIntersects(IAssembler assembler, ElementId elementId)
        {
            Element element = ActiveDocument.GetElement(elementId);
            LocationPoint _point = (LocationPoint)element.Location;
            XYZ point = _point.Point;

            BasePanel panel = (BasePanel)assembler;
            BoundingBoxXYZ boxXYZ = panel.ActiveElement.get_Geometry(new Options()).GetBoundingBox();

            return Utility.Geometry.InBox(boxXYZ, point);

        }

        public void SetAssemblyElements()
        {

            Debug.WriteLine("Вызов метода NS_Panel.SetAssemblyElements()");

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

            //Удаляем нижние элементы
            int n = 1;
            if (family.Symbol.Family.Name.Contains("Medium"))
            {
                n = 5;
            }
            int q = ActiveElement.GetParameters("Количество пазов")[0].AsInteger() * n;

            AssemblyElements.Sort(CompareElementIdsByZCoord);
            OutList = AssemblyElements.GetRange(0, q);
            AssemblyElements.RemoveRange(0, q);

            this.AssemblyElements.Add(ActiveElement.Id);

        }

        #endregion

        public override void CreateMarks()
        {

            Debug.WriteLine("Вызов метода NS_Panel.CreateMarks()");

            Marks.CheckAndSetMark(ActiveElement, Properties.Resource.DNS_Полная_марка_изделия, $"НС {GetPanelCode()}_{GetClosureCode()}", out string longMark);
            LongMark = longMark;

            Marks.CheckAndSetIndex(ActiveElement, Properties.Resource.ADSK_Номер_изделия, out string index);
            Index = index;

            Marks.CheckAndSetMark(ActiveElement, Properties.Resource.ADSK_Марка_изделия, $"НС {LongMark.Split('_')[1]}-{Index}", out string shortMark);
            ShortMark = shortMark;

        }

        public override bool Equal(BasePanel panelMark)
        {
            NS_Panel panel = (NS_Panel)panelMark;
           
            if (LongMark == panel.LongMark && AssemblyElements.Count == panel.AssemblyElements.Count)
            {
                return true;
            }

            else return false;
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
            string[] temp_i6 = ActiveElement.LookupParameter("Тип PVL_СТАРТ").AsString().Split(' ');
            string i6 = temp_i6[1];
            string[] temp_i7 = ActiveElement.LookupParameter("Тип PVL_ФИНИШ").AsString().Split(' ');
            string i7 = temp_i7[1];
            return $"{i1}.{i2}_{i3}.{i4}.{i5}_{i6}.{i7}";
        }
        private string GetClosureCode()
        {
            bool Closure1 = ActiveElement.LookupParameter("ПР1.ВКЛ").AsValueString() == "Да";
            bool Closure2 = ActiveElement.LookupParameter("ПР2.ВКЛ").AsValueString() == "Да";
            string window1 = "";

            if (Closure1)
            {
                string w1 = Marks.AsDecimString(ActiveElement, "ПР1.Отступ");
                string w2 = Marks.AsDecimString(ActiveElement, "ПР1.Ширина");
                string w3 = Marks.AsDecimString(ActiveElement, "ПР1.Высота");
                string w4 = Marks.AsDecimString(ActiveElement, "ПР1.ВысотаСмещение");
                window1 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string window2 = "";
            if (Closure2)
            {
                string w1 = Marks.AsDecimString(ActiveElement, "ПР2.Отступ");
                string w2 = Marks.AsDecimString(ActiveElement, "ПР2.Ширина");
                string w3 = Marks.AsDecimString(ActiveElement, "ПР2.Высота");
                string w4 = Marks.AsDecimString(ActiveElement, "ПР2.ВысотаСмещение");
                window2 = $"{w2}.{w3}.{w4}.{w1}";
            }
            string windows;
            if (Closure1 && Closure2)
            {
                windows = $"{window1}_{window2}";
            }
            else if (Closure1 || Closure1)
            {
                windows = $"{window1}{window2}";
            }
            else
            {
                windows = "Г";
            }

            return windows;
        }

        private int CompareElementIdsByZCoord(ElementId x, ElementId y)
        {
            Element elX = ActiveDocument.GetElement(x);
            Element elY = ActiveDocument.GetElement(y);

            BoundingBoxXYZ boxX = elX.get_Geometry(new Options()).GetBoundingBox();
            BoundingBoxXYZ boxY = elY.get_Geometry(new Options()).GetBoundingBox();

            if (boxX.Min.Z > boxY.Min.Z)
            {
                return 1;
            }
            else if (boxX.Min.Z == boxY.Min.Z)
            {
                return 0;
            }
            else
            {
                return -1;
            }

        }


    }
}
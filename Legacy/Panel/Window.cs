using Autodesk.Revit.DB;
using System;

namespace DSKPrim.PanelTools.Legacy.Panel
{
    public class Window
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public double OffsetZ { get; set; }
        public double OffsetXY { get; set; }

        public Window()
        { }

        public Window(Element element)
        {
            FamilyInstance familyInstanceWin = element as FamilyInstance;
            FamilySymbol familySymbolWin = familyInstanceWin.Symbol;

            ParameterMap i_Parameters = familyInstanceWin.ParametersMap;
            ParameterMap s_Parameters = familySymbolWin.ParametersMap;

            Width = i_Parameters.Contains("Ширина") ? Convert.ToDouble(i_Parameters.get_Item("Ширина").AsValueString()) : Convert.ToDouble(s_Parameters.get_Item("Ширина").AsValueString());
            Height = i_Parameters.Contains("Высота") ? Convert.ToDouble(i_Parameters.get_Item("Высота").AsValueString()) : Convert.ToDouble(s_Parameters.get_Item("Высота").AsValueString());
            OffsetZ = i_Parameters.Contains("Высота нижнего бруса") ? Convert.ToDouble(i_Parameters.get_Item("Высота нижнего бруса").AsValueString()) : Convert.ToDouble(s_Parameters.get_Item("Высота нижнего бруса").AsValueString());

        }
    }
}
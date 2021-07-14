using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.Utility
{
    public static class Marks
    {
        #region ParameterConversion
        public static string AsDecimString(Element element, string lkp)
        {
            return Math.Round(Convert.ToDouble(element.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }
        public static string AsDecimString(FamilySymbol elementFamily, string lkp)
        {
            return Math.Round(Convert.ToDouble(elementFamily.LookupParameter(lkp).AsValueString()) / 10, 0).ToString();
        }
        #endregion

    }
}

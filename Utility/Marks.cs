using System;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools.Utility
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

        public static void CheckAndSetMark(Element element, string guidName, string interpolatedString, out string longMark)
        {
            if (element.get_Parameter(new Guid(guidName)).AsString() is null)
            {
                longMark = interpolatedString;
            }
            else if (element.get_Parameter(new Guid(guidName)).AsString().Length == 0)
            {
                longMark = interpolatedString;
            }
            else
            {
                longMark = interpolatedString;
            }
        
        }

        public static void CheckAndSetIndex(Element element, string guidName, out string index)
        {
            if (element.get_Parameter(new Guid(guidName)).AsString() is null)
            {
                index = $"ID{element.Id}";
            }
            else if (element.get_Parameter(new Guid(guidName)).AsString() == "")
            {
                index = $"ID{element.Id}";
            }
            else
            {
                index = element.get_Parameter(new Guid(guidName)).AsString();
            }
        }

        public static void CheckAndWriteMarks(Element element, string nameGuid, string value)
        {
            element.get_Parameter(new Guid(nameGuid)).Set(value);
        }
        #endregion

    }
}

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Builders
{
    public class Sheet
    {
        //Начинаем нумерацию с 1
        public int Index;
        public ViewSheet SheetLink;

        public Sheet(ViewSheet sheet, int index)
        {
            Index = index;
            SheetLink = sheet;
        }
    }
}

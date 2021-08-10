using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace DSKPrim.PanelTools_v2.Utility
{
    public class StructComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            if (!(x is Element) && !(y is Element))
            {
                throw new ArgumentException();
            }

            else
            {
                Element elX = (Element)x;
                Element elY = (Element)y;

                

            }
            return 1;
        }

        
    }
}

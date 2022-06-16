using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Panel
{
    public abstract class BasePanelWrapper
    {
        protected abstract PrecastPanel WrappeePanel { get; set; }

        public BasePanelWrapper(PrecastPanel panel)
        {
            WrappeePanel = panel;
        }

        public abstract void Execute(Document document);
    }
}

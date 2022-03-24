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
        protected abstract BasePanel WrappeePanel { get; set; }

        public BasePanelWrapper(BasePanel panel)
        {
            WrappeePanel = panel;
        }

        public abstract void Execute(Document document);
    }
}

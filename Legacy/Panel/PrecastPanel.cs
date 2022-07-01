using System;
using System.Diagnostics;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Legacy.Builders.MarkBuilder;
using DSKPrim.PanelTools.Legacy.Controllers;
using DSKPrim.PanelTools.Legacy.Panel;

namespace DSKPrim.PanelTools.Panel
{

    /// <summary>
    /// Base class, that should be inherited by all new panel types.
    /// Panel wraps API Element of Panel (BuiltInCategory.OST_StructuralFraming) with the document.
    /// Allows further wrapping with interfaces to create suitable panel classes.
    /// </summary>
    public class PrecastPanel
    {
        public Element ActiveElement { get; set; }
        public AssemblyInstance AssemblyInstance { get; set; }
        public Mark Mark { get; set; }
        public StructureCategory StructureCategory { get; set; }

        public PrecastPanel(Element element)
        {
            ActiveElement = element;
            if (element.AssemblyInstanceId.IntegerValue != -1)
            {
                AssemblyInstance = element.Document.GetElement(element.AssemblyInstanceId) as AssemblyInstance;
            }
            Mark = new Mark();
            StructureCategory = new StructureCategory(element);
        }

    }
}

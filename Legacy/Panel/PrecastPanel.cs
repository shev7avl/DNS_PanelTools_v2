using System;
using System.Collections.Generic;
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
        public StructureType StructureType { get; set; }

        public PrecastPanel(Element element)
        {
            ActiveElement = element;
            if (element.AssemblyInstanceId.IntegerValue != -1)
            {
                AssemblyInstance = element.Document.GetElement(element.AssemblyInstanceId) as AssemblyInstance;
            }
            Mark = new Mark();
            StructureType = StructureTypeMapper.GetStructureType(element);
        }
        public bool IsValidCategory()
        {
            if (StructureType is StructureType.NOT_A_PANEL)
            {
                return false;
            }
            else return true;

        }

        public bool IsValidForWindowCreation()
        {
            List<StructureType> validTypes = new List<StructureType>
            {
                StructureType.NS_PANEL,
                StructureType.NS_PANEL_EMBEDDED,
                StructureType.VS_PANEL,
                StructureType.VS_PANEL_EMBEDDED
            };

            if (validTypes.Contains(StructureType)) return true;
            else return false;
        }
    }
}

using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using DSKPrim.PanelTools.ProjectEnvironment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.PanelMaster
{
    class SelectionFilters
    {
    }

    internal class FacadeSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {

            StructureCategory structureType = new StructureCategory(elem);

            if (structureType.GetPanelType(elem) == StructureCategory.PanelTypes.NOT_A_PANEL && !elem.Category.Name.Contains("Каркас несущий") && !(elem is AssemblyInstance))
            {
                return false;
            }

            else return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }

    internal class PanelSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            StructureCategory structureType = new StructureCategory(elem);
            bool elIsCorrect = structureType.GetPanelType(elem) != StructureCategory.PanelTypes.NOT_A_PANEL && elem.Category.Name.Contains("Каркас несущий");

            if (elIsCorrect)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

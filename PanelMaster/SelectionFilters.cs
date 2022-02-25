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

            StructureType structureType = new StructureType(elem);

            if (structureType.GetPanelType(elem) == StructureType.Panels.None.ToString() && !elem.Category.Name.Contains("Каркас несущий") && !(elem is AssemblyInstance))
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
            bool elIsCorrect = (elem.Name.Contains("_Empty") || elem.Name.Contains("_Medium")) && elem.Category.Name.Contains("Каркас несущий");
            if (elIsCorrect)
            {
                return true;
            }

            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

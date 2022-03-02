using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Panel;

namespace DSKPrim.PanelTools.PanelMaster
{
    public abstract class Routine
    {

        public abstract BasePanel Behaviour { get; set; }

        public abstract Document Document { get; set; }

        public abstract void ExecuteRoutine(ExternalCommandData commandData);

    }
}

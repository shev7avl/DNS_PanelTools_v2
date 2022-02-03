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

        public virtual void SetPanelBehaviour(Element element)
        {
            StructureType structureType = new StructureType(element);
            if (element.IsValidObject)
            {
                string type = structureType.GetPanelType(element);
                if (type != StructureType.Panels.None.ToString())
                {
                    if (type == StructureType.Panels.NS.ToString())
                    {
                        NS_Panel nS = new NS_Panel(Document, element);
                        Behaviour = nS;
                    }
                    if (type == StructureType.Panels.VS.ToString())
                    {
                        VS_Panel vS = new VS_Panel(Document, element);
                        Behaviour = vS;
                    }
                    if (type == StructureType.Panels.BP.ToString())
                    {
                        BP_Panel bP = new BP_Panel(Document, element);
                        Behaviour = bP;
                    }
                    if (type == StructureType.Panels.PS.ToString())
                    {
                        PS_Panel pS = new PS_Panel(Document, element);
                        Behaviour = pS;
                    }
                    if (type == StructureType.Panels.PP.ToString())
                    {
                        PP_Panel pP = new PP_Panel(Document, element);
                        Behaviour = pP;
                    }
                }
                else
                {
                    Behaviour = null;
                }
            }
            
            

        }

        public static void GetPanelBehaviour(Document document, Element element, out BasePanel panel)
        {
            panel = null;
            StructureType structureType = new StructureType(element);
            if (element.IsValidObject)
            {
                string type = structureType.GetPanelType(element);
                if (type != StructureType.Panels.None.ToString())
                {
                    if (type == StructureType.Panels.NS.ToString())
                    {
                        NS_Panel nS = new NS_Panel(document, element);
                        panel = nS;
                    }
                    if (type == StructureType.Panels.VS.ToString())
                    {
                        VS_Panel vS = new VS_Panel(document, element);
                        panel = vS;
                    }
                    if (type == StructureType.Panels.BP.ToString())
                    {
                        BP_Panel bP = new BP_Panel(document, element);
                        panel = bP;
                    }
                    if (type == StructureType.Panels.PS.ToString())
                    {
                        PS_Panel pS = new PS_Panel(document, element);
                        panel = pS;
                    }
                    if (type == StructureType.Panels.PP.ToString())
                    {
                        PP_Panel pP = new PP_Panel(document, element);
                        panel = pP;
                    }
                }
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Builders;

namespace DSKPrim.PanelTools.Panel
{
    public class DrawingWrapper : BasePanelWrapper
    {

        protected override PrecastPanel WrappeePanel { get; set; }

        private DrawingBuilder Behaviour { get; set; }

        public DrawingWrapper(PrecastPanel panel) : base(panel)
        {
            WrappeePanel = panel ?? throw new NullReferenceException(nameof(panel));
            Behaviour = new DrawingBuilder(WrappeePanel);
        }

        public override void Execute(Document document)
        {
            Behaviour.BuildSheets(document);
            Behaviour.BuildViews(document);
            Behaviour.BuildDrawings(document);
            Behaviour.BuildParameters(document);
        }
    }
}
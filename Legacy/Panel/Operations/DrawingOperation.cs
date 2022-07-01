using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Builders;

namespace DSKPrim.PanelTools.Panel
{
    public class DrawingOperation : IPanelOperation
    {

        public void Execute(PrecastPanel panel)
        {
            var drawingBuilder = new DrawingBuilder(panel);

            drawingBuilder.BuildSheets();
            drawingBuilder.BuildViews();
            drawingBuilder.BuildDrawings();
            drawingBuilder.BuildParameters();
        }

        public void ExecuteRange(ICollection<PrecastPanel> panels)
        {
            foreach (var item in panels)
            {
                this.Execute(item);
            }
        }
    }
}
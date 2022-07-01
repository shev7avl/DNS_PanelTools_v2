using DSKPrim.PanelTools.Legacy.Builders.WindowBuilder;
using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Panel
{
    public class WindowOperation : IPanelOperation
    {

        public WindowOperation()
        {
            
        }

        public void Execute(PrecastPanel panel)
        {

            var windowBuilder = new WindowBuilder(panel);
            var openings = windowBuilder.GetOpeningsFromLinks();
            windowBuilder.Perforate(openings);

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

using DSKPrim.PanelTools.Legacy.Builders.AssemblyBuilder;
using DSKPrim.PanelTools.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Panel
{
    public class AssemblyOperation : IPanelOperation
    {

        public AssemblyOperation()
        {
            
        }
        public void Execute(PrecastPanel panel)
        {
            var builder = new AssemblyBuilder(panel);
            builder.CollectAssemblyElements();
            builder.CreateAssembly();
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

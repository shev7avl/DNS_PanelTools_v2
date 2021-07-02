using DNS_PanelTools_v2.StructuralApps.Panel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNS_PanelTools_v2.Commands
{
    public interface IRoutine
    {
        IRoutine CreateRoutine(Base_Panel panel);

        void Run();

    }
}

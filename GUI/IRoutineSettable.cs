using DNS_PanelTools_v2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DNS_PanelTools_v2.GUI
{
    public interface IRoutineSettable
    {
        Commands.Base_Routine Routine { get; set; }

    }
}

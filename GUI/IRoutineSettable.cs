using DSKPrim.PanelTools_v2.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSKPrim.PanelTools_v2.GUI
{
    public interface IRoutineSettable
    {
        Commands.Routine Routine { get; set; }

    }
}

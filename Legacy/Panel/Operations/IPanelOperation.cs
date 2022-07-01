
using System.Collections.Generic;

namespace DSKPrim.PanelTools.Panel
{
    public interface IPanelOperation
    {
        void Execute(PrecastPanel panel);

        void ExecuteRange(ICollection<PrecastPanel> panels);

    }
}

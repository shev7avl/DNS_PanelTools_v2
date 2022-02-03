using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    internal class Settings
    {
        internal SelectionType SelectionType = SelectionType.AllElements;

        internal TileModule TileModule = new TileModule();

        private static Settings Instance;

        private Settings()
        {
            if (Instance is null)
            {
                Instance = new Settings();
            }
        }

        internal static Settings GetSettings()
        {
            if (Instance is null)
            {
                Instance = new Settings();
            }
            return Instance;
        }

        internal SelectionType GetSelectionType()
        {
            return SelectionType;
        }
        internal void SetSelectionType(SelectionType selection)
        {
            SelectionType = selection;
        }

        internal TileModule GetTileModule()
        {
            return TileModule;
        }
        internal void SetTileModule(int width, int heigth, int gap)
        {
            TileModule = new TileModule(width, heigth, gap);
        }


    }
}

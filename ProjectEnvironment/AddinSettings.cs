using DSKPrim.PanelTools.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.ProjectEnvironment
{
    internal class AddinSettings
    {
        private SelectionType SelectionType;

        private TileSectionType TileSectionType;

        private TileModule TileModule;

        private static AddinSettings Instance;

        private AddinSettings()
        {
            SelectionType = SelectionType.AllElements;
            TileModule = new TileModule();
            TileSectionType = TileSectionType.TILE_LAYOUT_STRAIGHT;
           
        }


        internal static AddinSettings GetSettings()
        {
            if (Instance is null)
            {
                Instance = new AddinSettings();
            }
            return Instance;

        }

        internal TileSectionType GetTileSectionType()
        {
            return TileSectionType;
        }

        internal void SetTileSectionType(TileSectionType type)
        {
            TileSectionType = type;
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

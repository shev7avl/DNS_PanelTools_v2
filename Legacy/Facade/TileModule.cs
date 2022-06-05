namespace DSKPrim.PanelTools.Facade
{
    internal class TileModule
    {
        internal int ModuleWidth = 288;

        internal int ModuleHeight = 88;

        internal int ModuleGap = 12;

        internal TileModule(int Width, int Height, int Gap)
        {
            ModuleWidth = Width;
            ModuleHeight = Height;
            ModuleGap = Gap;
        }

        internal TileModule()
        { }
    }
}
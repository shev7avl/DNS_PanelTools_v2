namespace DSKPrim.PanelTools.ProjectEnvironment
{
    internal class TileModule
    {
        internal int ModuleWidth = 300;

        internal int ModuleHeight = 100;

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
using Autodesk.Revit.DB;
using DSKPrim.PanelTools.ProjectEnvironment;
using DSKPrim.PanelTools.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSKPrim.PanelTools.Facade
{
    internal abstract class TileAlgorythm
    {
        internal abstract FacadeDescription FacadeDescription { get; }
        internal static TileAlgorythm AlgorythmFabric(Document document, ElementId wallElementId, TileSectionType tileSectionType)
        {
            if (tileSectionType == TileSectionType.TILE_LAYOUT_BRICK)
            {
                //return new BrickAlgorytm(document, wallElementId);
            }
            else if (tileSectionType == TileSectionType.TILE_LAYOUT_STRAIGHT)
            {
                return new StraightAlgorythm(document, wallElementId);
            }
            else if (tileSectionType == TileSectionType.TILE_LAYOUT_FRONT)
            {
                //return new FrontAlgorythm(document, wallElementId);
            }
            return new StraightAlgorythm(document, wallElementId);
        }
        internal abstract void Execute(Document document);
       
    }

    internal class StraightAlgorythm : TileAlgorythm
    {

        internal override FacadeDescription FacadeDescription { get; }

        internal StraightAlgorythm(Document document, ElementId wallElementId)
        {
            FacadeDescription = new FacadeDescription(document, wallElementId);
        }

        internal override void Execute(Document document)
        {
            AddinSettings settings = AddinSettings.GetSettings();
            Transaction transaction = new Transaction(document, "Creating a SketchPlane");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();
            ICollection<ElementId> refiD = new List<ElementId>();
            SketchPlane sketchPlane = SketchPlane.Create(document, FacadeDescription.Plane);
            ICollection<ElementId> partsId = new List<ElementId>() { FacadeDescription.WallPart.Id };

            IList<Curve> curves = Architecture.SplitGeometry.CreateTileOutlay(FacadeDescription);

            PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);
            transaction.Commit();

            transaction.Start();
            maker.get_Parameter(BuiltInParameter.PARTMAKER_PARAM_DIVISION_GAP).SetValueString($"{settings.GetTileModule().ModuleGap}");
            transaction.Commit();
        }
    }

    internal class BrickAlgorytm : TileAlgorythm
    {
        internal override FacadeDescription FacadeDescription { get; }

        internal BrickAlgorytm(Document document, ElementId wallElementId)
        {
            FacadeDescription = new FacadeDescription(document, wallElementId);
        }

        internal override void Execute(Document document)
        {
            AddinSettings settings = AddinSettings.GetSettings();
            Transaction transaction = new Transaction(document, "Creating a SketchPlane");
            TransactionSettings.SetFailuresPreprocessor(transaction);

            transaction.Start();
            ICollection<ElementId> refiD = new List<ElementId>();
            SketchPlane sketchPlane = SketchPlane.Create(document, FacadeDescription.Plane);
            ICollection<ElementId> partsId = new List<ElementId>() { FacadeDescription.WallPart.Id };

            IList<Curve> curves = Architecture.SplitGeometry.CreateBrickOutlay(FacadeDescription);

            PartMaker maker = PartUtils.DivideParts(document, partsId, refiD, curves, sketchPlane.Id);
            transaction.Commit();

            transaction.Start();
            maker.get_Parameter(BuiltInParameter.PARTMAKER_PARAM_DIVISION_GAP).SetValueString($"{settings.GetTileModule().ModuleGap}");
            transaction.Commit();




        }
    }

    //internal class FrontAlgorythm : TileAlgorythm
    //{
    //    internal override FacadeDescription FacadeDescription { get; }

    //    internal FrontAlgorythm(Document document, ElementId wallElementId)
    //    {
    //        FacadeDescription = new FacadeDescription(document, wallElementId);
    //    }

    //    internal override void Execute()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

using Autodesk.Revit.UI;
using DSK.Application.AppGUI;
using System.Collections.Generic;
using System.Reflection;

namespace DSKPrim.PanelTools
{
    class App : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            ButtonBuilder lod100Builder = new ButtonBuilder(application);
            lod100Builder.BuildRibbon("Мастер панелей" ,"КЖ - LOD100").BuildButtons(
                new List<Button>
            {
                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.id_card.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_SetMarks",
                "Create marks",
                "Создать марки"),
                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.open_door.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_CreateOpenings",
                "Create panel openings",
                "Создать проемы"),
                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.product.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.STR_CreateAssemblies", "Create assemblies", "Создать сборки"),
                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.demolition.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.STR_DisassembleAll", "Disassemble all", "Разобрать сборки")
            });

            ButtonBuilder lod400Builder = new ButtonBuilder(application);
            lod400Builder.BuildRibbon("Мастер панелей", "КЖ.И - LOD400").BuildButtons(
                new List<Button>
            {
                new Button(lod400Builder.SubPanel.Ribbon.RibbonPanel, PanelTools.Properties.Resource.element.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.STR_UniqueAssemblies", "Leave Unique Assemblies", "Оставить уникальные сборки"),
                new Button(lod400Builder.SubPanel.Ribbon.RibbonPanel, PanelTools.Properties.Resource.technical_drawing.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.STR_CreateDrawings", "Create panel drawings", "Создать чертежи КЖ.И")
            }
                );

            ButtonBuilder archBuilder = new ButtonBuilder(application);
            archBuilder.BuildRibbon("Мастер панелей", "АКР").BuildButtons(
                new List<Button>
            {
                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.files.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.ARCH_copyMarks", "Copy marks", "Скопировать марки"),
                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.window.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.ARCH_Windows", "Create openings", "Создать проемы"),
                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.wall.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.ARCH_SplitToParts", "Split to parts", "Создать плитку"),
                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.construction_plan.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.ARCH_CreateDrawings", "Create Drawings", "Создать чертежи")
            }
                );

            ButtonBuilder settingsBuilder = new ButtonBuilder(application);
            settingsBuilder.BuildRibbon("Мастер панелей", "Настройки").BuildButtons(
                new List<Button>
            {
                new Button(settingsBuilder.SubPanel.Ribbon.RibbonPanel, Properties.Resource.settings.ToBitmap(),Assembly.GetExecutingAssembly().Location, "DSKPrim.PanelTools.PanelMaster.SettingsCommand", "Settings", "Настройки")
            }
                );

            return Result.Succeeded;
        }

    }
}

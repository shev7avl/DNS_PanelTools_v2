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
            lod100Builder.BuildRibbon("Мастер панелей", "КЖ - LOD100").BuildButtons(
                new List<Button>
            {
                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.s_createMarks.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_SetMarks",
                "Create marks",
                "Создать марки"),

                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.s_createOpening.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_CreateOpenings",
                "Create panel openings",
                "Создать проемы"),

                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.s_createAssemblies.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_CreateAssemblies",
                "Create assemblies",
                "Создать сборки"),

                new Button(lod100Builder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.s_disassembleAssemblies.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_DisassembleAll",
                "Disassemble all",
                "Разобрать сборки")
            });

            ButtonBuilder lod400Builder = new ButtonBuilder(application);
            lod400Builder.BuildRibbon("Мастер панелей", "КЖ.И - LOD400").BuildButtons(
                new List<Button>
            {
                new Button(lod400Builder.SubPanel.Ribbon.RibbonPanel,
                PanelTools.Properties.Resource.s_distinctAssemblies.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_UniqueAssemblies",
                "Leave Unique Assemblies",
                "Оставить уникальные сборки"),

                new Button(lod400Builder.SubPanel.Ribbon.RibbonPanel,
                PanelTools.Properties.Resource.s_createDrawings.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.STR_CreateDrawings",
                "Create panel drawings",
                "Создать чертежи КЖ.И")
            }
                );

            ButtonBuilder archBuilder = new ButtonBuilder(application);
            archBuilder.BuildRibbon("Мастер панелей", "АКР").BuildButtons(
                new List<Button>
            {


                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.s_createOpening.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.ARCH_Windows",
                "Create openings",
                "Создать проемы"),

                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.a_createTile.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.ARCH_SplitToParts",
                "Split to parts",
                "Создать плитку"),

                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.a_copyMarks.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.ARCH_copyMarks",
                "Copy marks",
                "Скопировать марки"),

                new Button(archBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.a_createDrawings.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.ARCH_CreateDrawings",
                "Create Drawings",
                "Создать чертежи")
            }
                );

            ButtonBuilder settingsBuilder = new ButtonBuilder(application);
            settingsBuilder.BuildRibbon("Мастер панелей", "Настройки").BuildButtons(
                new List<Button>
            {
                new Button(settingsBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.settings.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.SettingsCommand",
                "Settings",
                "Настройки")
            }
                );

            ButtonBuilder exportBuilder = new ButtonBuilder(application);
            settingsBuilder.BuildRibbon("Мастер панелей", "Экспорт").BuildButtons(
                new List<Button>
            {
                new Button(settingsBuilder.SubPanel.Ribbon.RibbonPanel,
                Properties.Resource.settings.ToBitmap(),
                Assembly.GetExecutingAssembly().Location,
                "DSKPrim.PanelTools.PanelMaster.ExportSchedules",
                "export",
                "Экспорт")
            }
                );

            //ButtonBuilder testBuilder = new ButtonBuilder(application);
            //testBuilder.BuildRibbon("Мастер панелей", "Тестовые функции").BuildButtons(
            //    new List<Button>
            //{
            //    new Button(testBuilder.SubPanel.Ribbon.RibbonPanel,
            //    Properties.Resource.Test.ToBitmap(),
            //    Assembly.GetExecutingAssembly().Location,
            //    "DSKPrim.PanelTools.PanelMaster.PlaceEmbedded",
            //    "Place Embedded",
            //    "Разместить закладную")
            //}
            //   );

            return Result.Succeeded;
        }

    }
}

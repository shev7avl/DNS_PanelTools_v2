using Autodesk.Revit.UI;
using System.Collections.Generic;

//namespace DSK.Application.AppGUI
//{
//    /// <summary>
//    /// Inherit this class to build ribbon with buttons
//    /// 
//    /// Example:
//    /// 
//    /// public Test: ButtonBuilder{
//    /// 
//    /// public Test(UIControlledApplication controlledApplication)
//    ///     {
//    ///     ControlledApplication = controlledApplication;
//    ///     
//    ///     List<Button> buttons = ...;
//    /// 
//    ///     Reset();
//    ///     }
//    /// }
//    /// 
//    /// Test.BuildRibbon("Test");
//    /// Test.BuildButtons(List<Button> buttons);
//    /// </summary>
//    public class ButtonBuilder
//    {

//        /// <summary>
//        /// SubPanel Instance
//        /// </summary>
//        public virtual SubPanel SubPanel { get; set; }

//        /// <summary>
//        /// Revit UIControlled Application link. Fill this property with builder constructor
//        /// </summary>
//        public virtual UIControlledApplication ControlledApplication { get; set; }

//        /// <summary>
//        /// Creates a new ribbon on SubPanel. Run this command first.
//        /// </summary>
//        /// <param name="name">Name of a ribbon</param>
//        /// 

//        public ButtonBuilder(UIControlledApplication application)
//        {
//            ControlledApplication = application;
//            SubPanel = new SubPanel();
//        }

//        public virtual ButtonBuilder BuildRibbon(string name)
//        {
//            SubPanel.Ribbon = new Ribbon(ControlledApplication, name);
//            return this;
//        }
//        /// <summary>
//        /// Creates an array buttons on a built ribbon.
//        /// </summary>
//        /// <param name="buttons"></param>
//        public virtual ButtonBuilder BuildButtons(List<Button> buttons)
//        {
//            SubPanel.Buttons = buttons;
//            return this;
//        }
//    }
//}

using Autodesk.Revit.DB;
using DSKPrim.PanelTools.Panel;
using DSKPrim.PanelTools.ProjectEnvironment;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSKPrim.PanelTools.GUI
{
    public partial class PanelSelector : System.Windows.Forms.Form
    {
        private Document Document;
        private object[] PanelCollection;
        public List<Element> SelectedElements;
        public PanelSelector(Document document)
        {
            InitializeComponent();
            SetDocument(document);
            SetCheckboxValues();
            ShowDialog();
        }

        public void SetDocument(Document document)
        {
            Document = document;
        }

        public void SetCheckboxValues()
        {
            PanelSelectionBox.Items.Clear();
            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);
            int PanelQuantity = environment.GetStructuralEnvironment().PanelMarks.Count();
            PanelCollection = new object[PanelQuantity];

            for (int i = 0; i < PanelQuantity; i++)
            {
                PanelCollection[i] = environment.GetStructuralEnvironment().PanelMarks[i].Mark.ShortMark;
            }

            ListBox.ObjectCollection objectCollection = new ListBox.ObjectCollection(PanelSelectionBox, PanelCollection);
            PanelSelectionBox.Items.AddRange(PanelCollection);
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public List<Element> SetCheckedPanels()
        {
            CommonProjectEnvironment environment = CommonProjectEnvironment.GetInstance(Document);
            List<Element> CheckedPanels = new List<Element>();

            foreach (var item in PanelSelectionBox.CheckedItems)
            {
                PrecastPanel _panel = environment.GetStructuralEnvironment().PanelMarks.Where(o => o.Mark.ShortMark == item.ToString()).FirstOrDefault();
                if (_panel != null)
                {
                    CheckedPanels.Add(_panel.ActiveElement);
                }
            }

            if (CheckedPanels.Count == 0)
            {
                throw new NullReferenceException("CheckedPanels is empty. Check line: 59");
            }
            return CheckedPanels;
        }

        private void PanelSelector_Load(object sender, EventArgs e)
        {

        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SelectedElements = SetCheckedPanels();
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

using DSKPrim.PanelTools_v2.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSKPrim.PanelTools_v2.GUI
{
    public partial class PanelWizard : Form
    {
        public Routine Routine { get; set; }
        public PanelWizard()
        {
            InitializeComponent();
            SetPanelVisibility(1);
        }
        [STAThread]
        public Routine GetRoutine()
        {
            return Routine;
        }
        [STAThread]
        private void SetPanelVisibility(int panelNum)
        {
            Font m_highFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            Font m_commonFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            switch (panelNum)
            {
                case 1:
                    this.Step1Panel.Visible = true;
                    this.Step2Panel.Visible = false;
                    this.BackButton.Enabled = true;
                    this.NextButton.Text = "Далее >";
                    this.ChooseCommandLabel.ForeColor = System.Drawing.Color.Gray;
                    this.ChooseCommandLabel.Font = m_commonFont;
                    this.ChooseProjSectionLabel.ForeColor = System.Drawing.Color.Black;
                    this.ChooseProjSectionLabel.Font = m_highFont;
                    break;
                case 2:
                    this.Step1Panel.Visible = false;
                    this.Step2Panel.Visible = true;
                    this.BackButton.Enabled = true;
                    this.NextButton.Text = "Завершить";
                    this.ChooseCommandLabel.ForeColor = System.Drawing.Color.Black;
                    this.ChooseCommandLabel.Font = m_highFont;
                    this.ChooseProjSectionLabel.ForeColor = System.Drawing.Color.Gray;
                    this.ChooseProjSectionLabel.Font = m_commonFont;
                    break;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
                SetPanelVisibility(1);
        }
        
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (this.Step1Panel.Visible)
            {
                SelectOperation(ProjectPartSelector.SelectedIndex);
                SetPanelVisibility(2);
            }
            else if (this.Step2Panel.Visible)
            {
                SelectCommand(CommandSelector.SelectedItem.ToString());
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        [STAThread]
        private void SelectOperation(int index)
        {
            switch (index)
            {
                case (0):
                    this.CommandSelector.Items.Clear();
                    this.CommandSelector.Text = "";
                    this.CommandSelector.Items.Add("Создать проемы");
                    this.CommandSelector.Items.Add("Создать марки");
                    this.CommandSelector.Items.Add("Создать сборки");
                    this.CommandSelector.Items.Add("Разобрать сборки");
                    break;
                case (1):
                    this.CommandSelector.Items.Clear();
                    this.CommandSelector.Text = "";
                    this.CommandSelector.Items.Add("Уникальные сборки");
                    this.CommandSelector.Items.Add("Создать виды и листы");
                    break;
                case (2):
                    this.CommandSelector.Items.Clear();
                    this.CommandSelector.Text = "";
                    this.CommandSelector.Items.Add("Создать проемы в фасаде");
                    this.CommandSelector.Items.Add("Скопировать марки");
                    this.CommandSelector.Items.Add("Создать сборки фасада");
                    this.CommandSelector.Items.Add("Создать виды и листы фасада");
                    break;
            }
        }

        private void SelectCommand(string opName)
        {
            switch (opName)
            {
                case ("Создать проемы"):
                    Routine = new CreateOpeningsRoutine();
                    break;
                case ("Создать марки"):
                    Routine = new SetMarksRoutine();
                    break;
                case ("Создать сборки"):
                    Routine = new CreateAssembliesRoutine();
                    break;
                case ("Разобрать сборки"):
                    Routine = new DisassembleAllRoutine();
                    break;
                case ("Уникальные сборки"):
                    Routine = new UniqueAssemblies();
                    break;
                case ("Создать виды и листы"):
                    Routine = new SheetsAndViewsCreationRoutine();
                    break;
                case ("Создать проемы в фасаде"):
                    break;
                case ("Скопировать марки"):
                    break;
                case ("Создать сборки фасада"):
                    break;
                case ("Создать виды и листы фасада"):
                    break;
            }
        }
    }
}

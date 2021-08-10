

namespace DSKPrim.PanelTools_v2.GUI
{
    partial class PanelWizard
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.HelpButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.ChooseCommandLabel = new System.Windows.Forms.Label();
            this.ChooseProjSectionLabel = new System.Windows.Forms.Label();
            this.CancelButton = new System.Windows.Forms.Button();
            this.BackButton = new System.Windows.Forms.Button();
            this.NextButton = new System.Windows.Forms.Button();
            this.Step1Panel = new System.Windows.Forms.Panel();
            this.ProjectPartSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Step2Panel = new System.Windows.Forms.Panel();
            this.CommandSelector = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            this.Step1Panel.SuspendLayout();
            this.Step2Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.HelpButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.ChooseCommandLabel);
            this.groupBox1.Controls.Add(this.ChooseProjSectionLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(211, 175);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Создание панелей";
            // 
            // HelpButton
            // 
            this.HelpButton.BackColor = System.Drawing.Color.MediumBlue;
            this.HelpButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HelpButton.ForeColor = System.Drawing.Color.White;
            this.HelpButton.Location = new System.Drawing.Point(169, 130);
            this.HelpButton.Name = "HelpButton";
            this.HelpButton.Size = new System.Drawing.Size(35, 35);
            this.HelpButton.TabIndex = 3;
            this.HelpButton.Text = "?";
            this.HelpButton.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(113, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Справка";
            // 
            // ChooseCommandLabel
            // 
            this.ChooseCommandLabel.AutoSize = true;
            this.ChooseCommandLabel.Location = new System.Drawing.Point(18, 98);
            this.ChooseCommandLabel.Name = "ChooseCommandLabel";
            this.ChooseCommandLabel.Size = new System.Drawing.Size(122, 13);
            this.ChooseCommandLabel.TabIndex = 1;
            this.ChooseCommandLabel.Text = "2. Выберите операцию";
            // 
            // ChooseProjSectionLabel
            // 
            this.ChooseProjSectionLabel.AutoSize = true;
            this.ChooseProjSectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChooseProjSectionLabel.Location = new System.Drawing.Point(18, 35);
            this.ChooseProjSectionLabel.Name = "ChooseProjSectionLabel";
            this.ChooseProjSectionLabel.Size = new System.Drawing.Size(178, 13);
            this.ChooseProjSectionLabel.TabIndex = 0;
            this.ChooseProjSectionLabel.Text = "1. Выберите раздел проекта";
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(123, 278);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(100, 30);
            this.CancelButton.TabIndex = 1;
            this.CancelButton.Text = "Отмена";
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(229, 278);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(100, 30);
            this.BackButton.TabIndex = 2;
            this.BackButton.Text = "<  Назад";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // NextButton
            // 
            this.NextButton.Location = new System.Drawing.Point(335, 278);
            this.NextButton.Name = "NextButton";
            this.NextButton.Size = new System.Drawing.Size(100, 30);
            this.NextButton.TabIndex = 3;
            this.NextButton.Text = "Далее  >";
            this.NextButton.UseVisualStyleBackColor = true;
            this.NextButton.Click += new System.EventHandler(this.NextButton_Click);
            // 
            // Step1Panel
            // 
            this.Step1Panel.Controls.Add(this.ProjectPartSelector);
            this.Step1Panel.Controls.Add(this.label4);
            this.Step1Panel.Location = new System.Drawing.Point(235, 12);
            this.Step1Panel.Name = "Step1Panel";
            this.Step1Panel.Size = new System.Drawing.Size(200, 250);
            this.Step1Panel.TabIndex = 4;
            // 
            // ProjectPartSelector
            // 
            this.ProjectPartSelector.FormattingEnabled = true;
            this.ProjectPartSelector.Items.AddRange(new object[] {
            "КЖ - LOD 100",
            "КЖ.И - LOD 400",
            "АКР - Фасад"});
            this.ProjectPartSelector.Location = new System.Drawing.Point(30, 117);
            this.ProjectPartSelector.Name = "ProjectPartSelector";
            this.ProjectPartSelector.Size = new System.Drawing.Size(121, 21);
            this.ProjectPartSelector.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Выберите раздел проекта";
            // 
            // Step2Panel
            // 
            this.Step2Panel.Controls.Add(this.CommandSelector);
            this.Step2Panel.Controls.Add(this.label5);
            this.Step2Panel.Location = new System.Drawing.Point(229, 12);
            this.Step2Panel.Name = "Step2Panel";
            this.Step2Panel.Size = new System.Drawing.Size(200, 247);
            this.Step2Panel.TabIndex = 2;
            // 
            // CommandSelector
            // 
            this.CommandSelector.FormattingEnabled = true;
            this.CommandSelector.Location = new System.Drawing.Point(30, 117);
            this.CommandSelector.Name = "CommandSelector";
            this.CommandSelector.Size = new System.Drawing.Size(121, 21);
            this.CommandSelector.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(30, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Выберите операцию";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DSKPrim.PanelTools_v2.Properties.Resources.DSK_Prim;
            this.pictureBox1.Location = new System.Drawing.Point(33, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(141, 69);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // PanelWizard
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(446, 317);
            this.Controls.Add(this.Step2Panel);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.Step1Panel);
            this.Controls.Add(this.NextButton);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PanelWizard";
            this.Text = "Мастер создания панелей";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.Step1Panel.ResumeLayout(false);
            this.Step1Panel.PerformLayout();
            this.Step2Panel.ResumeLayout(false);
            this.Step2Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button HelpButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ChooseCommandLabel;
        private System.Windows.Forms.Label ChooseProjSectionLabel;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Button NextButton;
        private System.Windows.Forms.Panel Step1Panel;
        private System.Windows.Forms.ComboBox ProjectPartSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel Step2Panel;
        private System.Windows.Forms.ComboBox CommandSelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
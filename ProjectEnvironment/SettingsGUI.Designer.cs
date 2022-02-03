
namespace DSKPrim.PanelTools.ProjectEnvironment
{
    partial class SettingsGUI
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectSingleButton = new System.Windows.Forms.RadioButton();
            this.SelectMultipleButton = new System.Windows.Forms.RadioButton();
            this.CropBoxButton = new System.Windows.Forms.RadioButton();
            this.SelectAllButton = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.GapValueTB = new System.Windows.Forms.TextBox();
            this.HeigthValueTB = new System.Windows.Forms.TextBox();
            this.WidthValueTB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveSettingsButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectSingleButton);
            this.panel1.Controls.Add(this.SelectMultipleButton);
            this.panel1.Controls.Add(this.CropBoxButton);
            this.panel1.Controls.Add(this.SelectAllButton);
            this.panel1.Location = new System.Drawing.Point(12, 66);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(152, 129);
            this.panel1.TabIndex = 0;
            // 
            // SelectSingleButton
            // 
            this.SelectSingleButton.AutoSize = true;
            this.SelectSingleButton.Location = new System.Drawing.Point(3, 82);
            this.SelectSingleButton.Name = "SelectSingleButton";
            this.SelectSingleButton.Size = new System.Drawing.Size(97, 17);
            this.SelectSingleButton.TabIndex = 3;
            this.SelectSingleButton.Text = "Один элемент";
            this.SelectSingleButton.UseVisualStyleBackColor = true;
            this.SelectSingleButton.CheckedChanged += new System.EventHandler(this.SelectSingleButton_CheckedChanged);
            // 
            // SelectMultipleButton
            // 
            this.SelectMultipleButton.AutoSize = true;
            this.SelectMultipleButton.Location = new System.Drawing.Point(4, 59);
            this.SelectMultipleButton.Name = "SelectMultipleButton";
            this.SelectMultipleButton.Size = new System.Drawing.Size(139, 17);
            this.SelectMultipleButton.TabIndex = 2;
            this.SelectMultipleButton.Text = "Несколько элементов";
            this.SelectMultipleButton.UseVisualStyleBackColor = true;
            this.SelectMultipleButton.CheckedChanged += new System.EventHandler(this.SelectMultipleButton_CheckedChanged);
            // 
            // CropBoxButton
            // 
            this.CropBoxButton.AutoSize = true;
            this.CropBoxButton.Location = new System.Drawing.Point(3, 35);
            this.CropBoxButton.Name = "CropBoxButton";
            this.CropBoxButton.Size = new System.Drawing.Size(99, 17);
            this.CropBoxButton.TabIndex = 1;
            this.CropBoxButton.Text = "Выбор рамкой";
            this.CropBoxButton.UseVisualStyleBackColor = true;
            this.CropBoxButton.CheckedChanged += new System.EventHandler(this.CropBoxButton_CheckedChanged);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.AutoSize = true;
            this.SelectAllButton.Checked = true;
            this.SelectAllButton.Location = new System.Drawing.Point(3, 11);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(96, 17);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.TabStop = true;
            this.SelectAllButton.Text = "Выбирать все";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.CheckedChanged += new System.EventHandler(this.SelectAllButton_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.GapValueTB);
            this.panel2.Controls.Add(this.HeigthValueTB);
            this.panel2.Controls.Add(this.WidthValueTB);
            this.panel2.Location = new System.Drawing.Point(208, 66);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(214, 100);
            this.panel2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(151, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Шаг, мм";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(76, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Высота, мм";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Ширина, мм";
            // 
            // GapValueTB
            // 
            this.GapValueTB.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.GapValueTB.Location = new System.Drawing.Point(154, 35);
            this.GapValueTB.Name = "GapValueTB";
            this.GapValueTB.Size = new System.Drawing.Size(57, 20);
            this.GapValueTB.TabIndex = 3;
            this.GapValueTB.Text = "12";
            this.GapValueTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.GapValueTB.TextChanged += new System.EventHandler(this.GapValueTB_TextChanged);
            // 
            // HeigthValueTB
            // 
            this.HeigthValueTB.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.HeigthValueTB.Location = new System.Drawing.Point(80, 35);
            this.HeigthValueTB.Name = "HeigthValueTB";
            this.HeigthValueTB.Size = new System.Drawing.Size(57, 20);
            this.HeigthValueTB.TabIndex = 2;
            this.HeigthValueTB.Text = "100";
            this.HeigthValueTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.HeigthValueTB.TextChanged += new System.EventHandler(this.HeigthValueTB_TextChanged);
            // 
            // WidthValueTB
            // 
            this.WidthValueTB.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.WidthValueTB.Location = new System.Drawing.Point(3, 35);
            this.WidthValueTB.Name = "WidthValueTB";
            this.WidthValueTB.Size = new System.Drawing.Size(57, 20);
            this.WidthValueTB.TabIndex = 0;
            this.WidthValueTB.Text = "300";
            this.WidthValueTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.WidthValueTB.TextChanged += new System.EventHandler(this.WidthValueTB_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(8, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(166, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Выбирать элементы";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(204, 31);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Габариты плитки";
            // 
            // SaveSettingsButton
            // 
            this.SaveSettingsButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.SaveSettingsButton.Location = new System.Drawing.Point(321, 174);
            this.SaveSettingsButton.Name = "SaveSettingsButton";
            this.SaveSettingsButton.Size = new System.Drawing.Size(98, 32);
            this.SaveSettingsButton.TabIndex = 6;
            this.SaveSettingsButton.Text = "Сохранить";
            this.SaveSettingsButton.UseVisualStyleBackColor = false;
            this.SaveSettingsButton.Click += new System.EventHandler(this.SaveSettingsButton_Click);
            this.SaveSettingsButton.MouseEnter += new System.EventHandler(this.SaveSettingsButton_MouseEnter);
            this.SaveSettingsButton.MouseLeave += new System.EventHandler(this.SaveSettingsButton_MouseLeave);
            // 
            // CancelButton
            // 
            this.CancelButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton.Location = new System.Drawing.Point(205, 174);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(98, 32);
            this.CancelButton.TabIndex = 7;
            this.CancelButton.Text = "Отмена";
            this.CancelButton.UseVisualStyleBackColor = false;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            this.CancelButton.MouseEnter += new System.EventHandler(this.CancelButton_MouseEnter);
            this.CancelButton.MouseLeave += new System.EventHandler(this.CancelButton_MouseLeave);
            // 
            // SettingsGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 218);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveSettingsButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsGUI";
            this.Text = "Настройки";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton SelectSingleButton;
        private System.Windows.Forms.RadioButton SelectMultipleButton;
        private System.Windows.Forms.RadioButton CropBoxButton;
        private System.Windows.Forms.RadioButton SelectAllButton;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox HeigthValueTB;
        private System.Windows.Forms.TextBox WidthValueTB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button SaveSettingsButton;
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.TextBox GapValueTB;
    }
}
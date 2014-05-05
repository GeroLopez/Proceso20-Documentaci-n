namespace Proceso20
{
    partial class diag1
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
            this.textfe1 = new System.Windows.Forms.TextBox();
            this.textfe2 = new System.Windows.Forms.TextBox();
            this.calen1 = new System.Windows.Forms.MonthCalendar();
            this.calen2 = new System.Windows.Forms.MonthCalendar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textusu = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textdur = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textfe1
            // 
            this.textfe1.Location = new System.Drawing.Point(37, 39);
            this.textfe1.Name = "textfe1";
            this.textfe1.Size = new System.Drawing.Size(78, 20);
            this.textfe1.TabIndex = 0;
            this.textfe1.TextChanged += new System.EventHandler(this.textfe1_TextChanged);
            // 
            // textfe2
            // 
            this.textfe2.Location = new System.Drawing.Point(287, 39);
            this.textfe2.Name = "textfe2";
            this.textfe2.Size = new System.Drawing.Size(78, 20);
            this.textfe2.TabIndex = 1;
            this.textfe2.TextChanged += new System.EventHandler(this.textfe2_TextChanged);
            // 
            // calen1
            // 
            this.calen1.Location = new System.Drawing.Point(37, 92);
            this.calen1.MaxSelectionCount = 1;
            this.calen1.Name = "calen1";
            this.calen1.ScrollChange = 1;
            this.calen1.TabIndex = 2;
            this.calen1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calen1_DateSelected);
            // 
            // calen2
            // 
            this.calen2.Location = new System.Drawing.Point(287, 92);
            this.calen2.MaxSelectionCount = 1;
            this.calen2.Name = "calen2";
            this.calen2.ScrollChange = 1;
            this.calen2.TabIndex = 3;
            this.calen2.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.calen2_DateSelected);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "inicial";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(284, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "final";
            // 
            // textusu
            // 
            this.textusu.Location = new System.Drawing.Point(529, 215);
            this.textusu.Name = "textusu";
            this.textusu.Size = new System.Drawing.Size(79, 20);
            this.textusu.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(526, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Usuario (3 letras)";
            // 
            // textdur
            // 
            this.textdur.Location = new System.Drawing.Point(526, 92);
            this.textdur.Name = "textdur";
            this.textdur.Size = new System.Drawing.Size(82, 20);
            this.textdur.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(523, 61);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Total (minutos)";
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(100, 308);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(78, 37);
            this.button1.TabIndex = 10;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(287, 308);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(78, 37);
            this.button2.TabIndex = 11;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // diag1
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(720, 399);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textdur);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textusu);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.calen2);
            this.Controls.Add(this.calen1);
            this.Controls.Add(this.textfe2);
            this.Controls.Add(this.textfe1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "diag1";
            this.ShowInTaskbar = false;
            this.Text = "diag1";
            this.Load += new System.EventHandler(this.diag1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textfe1;
        private System.Windows.Forms.TextBox textfe2;
        private System.Windows.Forms.MonthCalendar calen1;
        private System.Windows.Forms.MonthCalendar calen2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textusu;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textdur;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}
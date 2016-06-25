namespace NS_ReplDL
{
    partial class ReplaceDL
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
            if ( disposing && ( components != null ) )
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
            this.comboBoxBefore = new System.Windows.Forms.ComboBox();
            this.comboBoxAfter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonReplace = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboBoxBefore
            // 
            this.comboBoxBefore.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.comboBoxBefore.Font = new System.Drawing.Font("Courier New",9F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,( (byte)( 0 ) ));
            this.comboBoxBefore.FormattingEnabled = true;
            this.comboBoxBefore.ItemHeight = 15;
            this.comboBoxBefore.Location = new System.Drawing.Point(12,28);
            this.comboBoxBefore.MaxDropDownItems = 30;
            this.comboBoxBefore.Name = "comboBoxBefore";
            this.comboBoxBefore.Size = new System.Drawing.Size(167,23);
            this.comboBoxBefore.TabIndex = 0;
            this.comboBoxBefore.Text = "C:";
            this.comboBoxBefore.Click += new System.EventHandler(this.comboBoxBefore_Click);
            // 
            // comboBoxAfter
            // 
            this.comboBoxAfter.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.comboBoxAfter.Font = new System.Drawing.Font("Courier New",9F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,( (byte)( 0 ) ));
            this.comboBoxAfter.FormattingEnabled = true;
            this.comboBoxAfter.IntegralHeight = false;
            this.comboBoxAfter.ItemHeight = 15;
            this.comboBoxAfter.Location = new System.Drawing.Point(12,74);
            this.comboBoxAfter.MaxDropDownItems = 30;
            this.comboBoxAfter.Name = "comboBoxAfter";
            this.comboBoxAfter.Size = new System.Drawing.Size(167,23);
            this.comboBoxAfter.TabIndex = 1;
            this.comboBoxAfter.Text = "C:";
            this.comboBoxAfter.Click += new System.EventHandler(this.comboBoxAfter_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12,9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134,13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select drive to be replaced";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12,55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(124,13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Select replacement drive";
            // 
            // buttonReplace
            // 
            this.buttonReplace.Location = new System.Drawing.Point(51,117);
            this.buttonReplace.Name = "buttonReplace";
            this.buttonReplace.Size = new System.Drawing.Size(75,23);
            this.buttonReplace.TabIndex = 4;
            this.buttonReplace.Text = "&Replace";
            this.buttonReplace.UseVisualStyleBackColor = true;
            this.buttonReplace.Click += new System.EventHandler(this.buttonReplace_Click);
            // 
            // ReplaceDL
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192,152);
            this.Controls.Add(this.buttonReplace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxAfter);
            this.Controls.Add(this.comboBoxBefore);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReplaceDL";
            this.Text = "Replace drive letter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ReplaceDL_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxBefore;
        private System.Windows.Forms.ComboBox comboBoxAfter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonReplace;
    }
}
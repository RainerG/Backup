namespace NS_FavDescr
{
    partial class FavDescriptionEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxSrcPath = new System.Windows.Forms.TextBox();
            this.textBoxSrcDescr = new System.Windows.Forms.TextBox();
            this.textBoxDstDescr = new System.Windows.Forms.TextBox();
            this.textBoxDstPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonAutoSet = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxSrcPath
            // 
            this.textBoxSrcPath.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBoxSrcPath.Location = new System.Drawing.Point( 44, 12 );
            this.textBoxSrcPath.Name = "textBoxSrcPath";
            this.textBoxSrcPath.Size = new System.Drawing.Size( 282, 20 );
            this.textBoxSrcPath.TabIndex = 0;
            // 
            // textBoxSrcDescr
            // 
            this.textBoxSrcDescr.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBoxSrcDescr.Location = new System.Drawing.Point( 82, 38 );
            this.textBoxSrcDescr.Name = "textBoxSrcDescr";
            this.textBoxSrcDescr.Size = new System.Drawing.Size( 244, 20 );
            this.textBoxSrcDescr.TabIndex = 1;
            // 
            // textBoxDstDescr
            // 
            this.textBoxDstDescr.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBoxDstDescr.Location = new System.Drawing.Point( 82, 150 );
            this.textBoxDstDescr.Name = "textBoxDstDescr";
            this.textBoxDstDescr.Size = new System.Drawing.Size( 244, 20 );
            this.textBoxDstDescr.TabIndex = 3;
            // 
            // textBoxDstPath
            // 
            this.textBoxDstPath.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBoxDstPath.Location = new System.Drawing.Point( 44, 124 );
            this.textBoxDstPath.Name = "textBoxDstPath";
            this.textBoxDstPath.Size = new System.Drawing.Size( 282, 20 );
            this.textBoxDstPath.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 13, 15 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 29, 13 );
            this.label1.TabIndex = 4;
            this.label1.Text = "Path";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point( 13, 41 );
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size( 60, 13 );
            this.label2.TabIndex = 5;
            this.label2.Text = "Description";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point( 13, 153 );
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size( 60, 13 );
            this.label3.TabIndex = 7;
            this.label3.Text = "Description";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point( 13, 127 );
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size( 29, 13 );
            this.label4.TabIndex = 6;
            this.label4.Text = "Path";
            // 
            // buttonAutoSet
            // 
            this.buttonAutoSet.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buttonAutoSet.Location = new System.Drawing.Point( 16, 78 );
            this.buttonAutoSet.Name = "buttonAutoSet";
            this.buttonAutoSet.Size = new System.Drawing.Size( 75, 23 );
            this.buttonAutoSet.TabIndex = 8;
            this.buttonAutoSet.Text = "&Auto Set";
            this.buttonAutoSet.UseVisualStyleBackColor = true;
            this.buttonAutoSet.Click += new System.EventHandler( this.buttonAutoSet_Click );
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point( 251, 78 );
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size( 75, 23 );
            this.buttonOk.TabIndex = 9;
            this.buttonOk.Text = "&Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // FavDescriptionEditor
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 341, 182 );
            this.Controls.Add( this.buttonOk );
            this.Controls.Add( this.buttonAutoSet );
            this.Controls.Add( this.label3 );
            this.Controls.Add( this.label4 );
            this.Controls.Add( this.label2 );
            this.Controls.Add( this.label1 );
            this.Controls.Add( this.textBoxDstDescr );
            this.Controls.Add( this.textBoxDstPath );
            this.Controls.Add( this.textBoxSrcDescr );
            this.Controls.Add( this.textBoxSrcPath );
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size( 900, 210 );
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size( 290, 210 );
            this.Name = "FavDescriptionEditor";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FavDescriptionEditor";
            this.Load += new System.EventHandler( this.FavDescriptionEditor_Load );
            this.ResumeLayout( false );
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxSrcPath;
        public System.Windows.Forms.TextBox textBoxSrcDescr;
        public System.Windows.Forms.TextBox textBoxDstDescr;
        public System.Windows.Forms.TextBox textBoxDstPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonAutoSet;
        private System.Windows.Forms.Button buttonOk;
    }
}
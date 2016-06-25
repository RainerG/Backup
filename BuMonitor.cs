using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Backup
{
	/// <summary>
	/// Summary description for BuMonitor.
	/// </summary>
	public class BuMonitor : System.Windows.Forms.Form
	{
        private System.Windows.Forms.TextBox textBoxCurrPath;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BuMonitor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

        public void SetText(string iText)
        {
            textBoxCurrPath.Text = iText;
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
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
            this.textBoxCurrPath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxCurrPath
            // 
            this.textBoxCurrPath.Location = new System.Drawing.Point(16, 16);
            this.textBoxCurrPath.Name = "textBoxCurrPath";
            this.textBoxCurrPath.Size = new System.Drawing.Size(720, 22);
            this.textBoxCurrPath.TabIndex = 0;
            this.textBoxCurrPath.Text = "";
            // 
            // BuMonitor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
            this.ClientSize = new System.Drawing.Size(752, 60);
            this.Controls.Add(this.textBoxCurrPath);
            this.Name = "BuMonitor";
            this.Text = "BuMonitor";
            this.ResumeLayout(false);

        }
		#endregion
	}
}

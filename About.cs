using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.IO;

namespace NS_Backup
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelCreated;
        private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public About()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            DateTime tm = File.GetLastWriteTime( Application.ExecutablePath );
            string date = string.Format("{0:yyyy}-{0:MM}-{0:dd}  {0:HH}:{0:mm}:{0:ss}",tm);

            labelCreated.Text = date;
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
            this.label1 = new System.Windows.Forms.Label();
            this.labelCreated = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(216, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Release:  1.32";
            // 
            // labelCreated
            // 
            this.labelCreated.Location = new System.Drawing.Point(12, 75);
            this.labelCreated.Name = "labelCreated";
            this.labelCreated.Size = new System.Drawing.Size(216, 23);
            this.labelCreated.TabIndex = 1;
            this.labelCreated.Text = "Created: ";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(216, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Author: Rainer Gerlach";
            // 
            // About
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(240, 109);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelCreated);
            this.Controls.Add(this.label1);
            this.Name = "About";
            this.Text = "About";
            this.ResumeLayout(false);

        }
		#endregion
	}
}

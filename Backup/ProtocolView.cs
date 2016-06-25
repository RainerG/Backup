using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using NS_Utilities;
using NS_AppConfig;

namespace NS_Backup
{
	/// <summary>
	/// Summary description for ProtocolView.
	/// </summary>
	public class ProtocolView : System.Windows.Forms.Form
	{
        // Helper memories for location and size 21.04.2006
        public Point Loc;
        public Size  Siz;

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button buttonSort;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   29.03.2006
        ***************************************************************************/
        public ProtocolView(string sProtFile)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            textBox.MaxLength = (int)0x7fffffff; 

            if (! Utils.NoFile(sProtFile))
            {
                StreamReader f = new StreamReader(sProtFile);

                while(f.Peek() > 0)
                {
                    string s = f.ReadLine();
//                    if (s.Substring(0,3) == "new") 
//                    {
//                        textBoxforForeColor = Color.DarkRed;
//                    }
//                    else
//                    {
//                        textBox.ForeColor = Color.RoyalBlue;
//                    }
                    textBox.AppendText(s + "\r\n");
                }
                f.Close();
            }
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
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProtocolView));
            this.textBox = new System.Windows.Forms.TextBox();
            this.buttonSort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBox.Font = new System.Drawing.Font("Courier New", 8F);
            this.textBox.ForeColor = System.Drawing.Color.RoyalBlue;
            this.textBox.Location = new System.Drawing.Point(6, 6);
            this.textBox.MaxLength = 500000000;
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox.Size = new System.Drawing.Size(500, 266);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "";
            // 
            // buttonSort
            // 
            this.buttonSort.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonSort.Location = new System.Drawing.Point(232, 280);
            this.buttonSort.Name = "buttonSort";
            this.buttonSort.Size = new System.Drawing.Size(48, 23);
            this.buttonSort.TabIndex = 1;
            this.buttonSort.Text = "&Sort";
            this.buttonSort.Click += new System.EventHandler(this.buttonSort_Click);
            // 
            // ProtocolView
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(512, 309);
            this.Controls.Add(this.buttonSort);
            this.Controls.Add(this.textBox);
            this.DockPadding.All = 6;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProtocolView";
            this.Text = "ProtocolView";
            this.Load += new System.EventHandler(this.ProtocolView_Load);
            this.ResumeLayout(false);

        }
		#endregion

        private void buttonSort_Click(object sender, System.EventArgs e)
        {
            ArrayList al = new ArrayList(textBox.Lines);
            al.Sort();
            textBox.Clear();
            foreach(string s in al)
            {
                if (0 != s.Length)  textBox.AppendText(s + "\r\n");
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       21.04.2006
        LAST CHANGE:   21.04.2006
        ***************************************************************************/
        private void ProtocolView_Load(object sender, System.EventArgs e)
        {
            Location = Loc;
            Size     = Siz;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.05.2006
        LAST CHANGE:   06.05.2006
        ***************************************************************************/
        public int GetNrLines()
        {
            return textBox.Lines.Length;
        }
	}
}

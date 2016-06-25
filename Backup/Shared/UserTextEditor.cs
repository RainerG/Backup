using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using NS_Utilities;
using NS_AppConfig;

namespace NS_UserTextEditor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class UserTextEditor : System.Windows.Forms.Form
	{
        private string           m_FilePath;
        private bool             m_bList;
        private ComboBox         m_ComboBox;
        private DialogPosSize    m_PosSize;
        private bool             m_bSortDir;

        public List<string> m_aOutput; 

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button buttonSort;
        private System.Windows.Forms.Button buttonSave;
        private Button buttonSel;
        private Button buttonLeaveSel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


        /***************************************************************************
        SPECIFICATION: C'tor for file editing
        CREATED:       07.12.2007
        LAST CHANGE:   16.12.2007
        ***************************************************************************/
		public UserTextEditor(string sFilePath)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            m_FilePath = sFilePath;
            m_bList    = false;
            m_bSortDir = true;

            if (Utils.NoFile(sFilePath)) return;

            StreamReader f = new StreamReader(sFilePath);

            textBox.Clear();

            string line;
            while (null != (line = f.ReadLine()))
            {
                textBox.AppendText(line + "\r\n");
            }
            f.Close();
            textBox.Select();
		}

        /***************************************************************************
        SPECIFICATION: C'tor for combo boxes 
        CREATED:       07.12.2007
        LAST CHANGE:   10.12.2007
        ***************************************************************************/
        public UserTextEditor(ComboBox aComboBox)
        {
            InitializeComponent();
            m_bList    = true;
            m_ComboBox = aComboBox;

            GetComboItems();
        }


        /***************************************************************************
        SPECIFICATION: Reads the combo contents to the text box
        CREATED:       10.12.2007
        LAST CHANGE:   10.12.2007
        ***************************************************************************/
        public void GetComboItems()
        {
            textBox.Clear();
            foreach (string line in m_ComboBox.Items)
            {
                textBox.AppendText(line + "\r\n");
            }
        }

        /***************************************************************************
        SPECIFICATION: Rewrites the textbox lines to the combo items
        CREATED:       10.12.2007
        LAST CHANGE:   10.12.2007
        ***************************************************************************/
        public void SetComboItems()
        {
            m_ComboBox.Items.Clear();

            foreach ( string line in textBox.Lines )
            {
                if (line != "") m_ComboBox.Items.Add(line);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserTextEditor));
            this.textBox = new System.Windows.Forms.TextBox();
            this.buttonSort = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonSel = new System.Windows.Forms.Button();
            this.buttonLeaveSel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.textBox.Font = new System.Drawing.Font("Courier New",8.25F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,( (byte)( 0 ) ));
            this.textBox.Location = new System.Drawing.Point(6,7);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(526,212);
            this.textBox.TabIndex = 0;
            // 
            // buttonSort
            // 
            this.buttonSort.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.buttonSort.Location = new System.Drawing.Point(6,236);
            this.buttonSort.Name = "buttonSort";
            this.buttonSort.Size = new System.Drawing.Size(67,23);
            this.buttonSort.TabIndex = 1;
            this.buttonSort.Text = "&Sort";
            this.buttonSort.Click += new System.EventHandler(this.buttonSort_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.buttonSave.Location = new System.Drawing.Point(465,236);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(67,23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "Sa&ve";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonSel
            // 
            this.buttonSel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.buttonSel.Location = new System.Drawing.Point(81,236);
            this.buttonSel.Name = "buttonSel";
            this.buttonSel.Size = new System.Drawing.Size(67,23);
            this.buttonSel.TabIndex = 4;
            this.buttonSel.Text = "Sel. &all";
            this.buttonSel.UseVisualStyleBackColor = true;
            this.buttonSel.Click += new System.EventHandler(this.buttonSelAll_Click);
            // 
            // buttonLeaveSel
            // 
            this.buttonLeaveSel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.buttonLeaveSel.Location = new System.Drawing.Point(156,236);
            this.buttonLeaveSel.Name = "buttonLeaveSel";
            this.buttonLeaveSel.Size = new System.Drawing.Size(67,23);
            this.buttonLeaveSel.TabIndex = 5;
            this.buttonLeaveSel.Text = "&Leave Sel.";
            this.buttonLeaveSel.UseVisualStyleBackColor = true;
            this.buttonLeaveSel.Click += new System.EventHandler(this.buttonLeaveSel_Click);
            // 
            // UserTextEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5,13);
            this.ClientSize = new System.Drawing.Size(537,271);
            this.Controls.Add(this.buttonLeaveSel);
            this.Controls.Add(this.buttonSel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonSort);
            this.Controls.Add(this.textBox);
            this.Icon = ( (System.Drawing.Icon)( resources.GetObject("$this.Icon") ) );
            this.MinimumSize = new System.Drawing.Size(321,200);
            this.Name = "UserTextEditor";
            this.Text = "Text editor";
            this.Load += new System.EventHandler(this.UserTextEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.12.2007
        LAST CHANGE:   10.12.2007
        ***************************************************************************/
        public void Serialize(ref AppSettings a_Conf)
        {
            if ( a_Conf.IsReading )
            {
                a_Conf.DeserializeDialog(this);
                m_PosSize = new DialogPosSize(this);
            }
            else
            {
                a_Conf.SerializeDialog(this);
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   16.12.2007
        ***************************************************************************/
        private void buttonSort_Click(object sender, System.EventArgs e)
        {
            TextLineComparer aTlComp = new TextLineComparer(m_bSortDir);
             
            m_bSortDir = ! m_bSortDir;

            ArrayList al = new ArrayList(textBox.Lines);
            al.Sort(aTlComp);

            textBox.Clear();
            foreach(string s in al)
            {
                if (0 != s.Length)  textBox.AppendText(s + "\r\n");
            } 
        }

        /***************************************************************************
        SPECIFICATION: Implements the manual sorting of lines in the text box
        CREATED:       16.12.2007
        LAST CHANGE:   16.12.2007
        ***************************************************************************/
        private class TextLineComparer:IComparer
        {
            private bool m_bAsc;
            public TextLineComparer()
            {
                m_bAsc = true;
            }

            public TextLineComparer(bool a_bAscending)
            {
                m_bAsc = a_bAscending;
            }

            public int Compare(object x,object y)
            {
                String a,b;

                if ( m_bAsc )
                {
                    a = (String)x;
                    b = (String)y;
                }
                else
                {
                    b = (String)x;
                    a = (String)y;
                }

                return String.Compare(a,b);
            }
        }


        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            if (m_bList)
            {
                m_aOutput = new List<string>();
                foreach(string line in textBox.Lines)
                {
                    m_aOutput.Add(line);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            StreamWriter f = null;
            try
            {
                f = new StreamWriter(m_FilePath);

                foreach(string s in textBox.Lines)
                {
                    f.WriteLine(s);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message,"Error");
            }
            finally
            {
                f.Close();
            }
        }

        /***************************************************************************
        SPECIFICATION: Load event handler
        CREATED:       10.12.2007
        LAST CHANGE:   18.01.2014
        ***************************************************************************/
        private void UserTextEditor_Load(object sender,EventArgs e)
        {

            if ( null != m_PosSize ) m_PosSize.Write(this);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.12.2007
        LAST CHANGE:   16.12.2007
        ***************************************************************************/
        private void buttonLeaveSel_Click(object sender,EventArgs e)
        {
            string s = textBox.SelectedText;
            textBox.Clear();
            textBox.Text = s;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.12.2007
        LAST CHANGE:   16.12.2007
        ***************************************************************************/
        private void buttonSelAll_Click(object sender,EventArgs e)
        {
            textBox.HideSelection = false;
            textBox.SelectAll();
        }
        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.01.2014
        LAST CHANGE:   18.01.2014
        ***************************************************************************/
        public int GetNrLines()
        {
            return textBox.Lines.Length;
        }
	} // class
} // namespace

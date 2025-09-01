using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using NS_UserCombo;
using NS_AppConfig;
using NS_Utilities;
using NS_UserTextEditor;

namespace NS_Backup
{
    public delegate void dl_CloseProtView();

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class PreferencesForm : System.Windows.Forms.Form
	{
		/***************************************************************************
        SPECIFICATION: Members 
        CREATED:       2014
        LAST CHANGE:   12.02.2023
        ***************************************************************************/
        private DialogPosSize   m_ProtLoc;
        private DialogPosSize   m_MaskLoc;
        private ProtocolView    m_ProtView;     
        private MainDlg         m_Main;

        private System.Windows.Forms.GroupBox groupBox4;
        public  FileComboBox comboFileProt;
        private System.Windows.Forms.Button BtnShowProt;
        private System.Windows.Forms.Button BtnBrowseProt;
        private System.Windows.Forms.GroupBox groupBox3;
        public  FileComboBox comboFileMask;
        private System.Windows.Forms.Button EditFmBtn;
        private System.Windows.Forms.Button BtnBrowseFM;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.CheckBox checkRegardAge;
        public System.Windows.Forms.TextBox textTimeDelta;
        public System.Windows.Forms.TextBox textFileAge;
        public System.Windows.Forms.CheckBox checkSkipCount;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

        /***************************************************************************
        SPECIFICATION: Accessors  
        CREATED:       25.01.2016
        LAST CHANGE:   12.02.2023
        ***************************************************************************/
        public string FileMasks { get { return comboFileMask.Text; } set { comboFileMask.Text = value; } }

		/***************************************************************************
        SPECIFICATION: C'tor 
        CREATED:       2014
        LAST CHANGE:   12.02.2023
        ***************************************************************************/
        public PreferencesForm( MainDlg a_Main )
		{
			InitializeComponent();

            m_ProtLoc  = new DialogPosSize();
            m_MaskLoc  = new DialogPosSize();
            m_ProtView = a_Main.ProtView;
            m_Main     = a_Main;

            checkSkipCount.Checked = true;
            comboFileProt.AutoCompleteSource = AutoCompleteSource.FileSystem;
            comboFileMask.AutoCompleteSource = AutoCompleteSource.FileSystem;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.comboFileProt = new NS_UserCombo.FileComboBox();
            this.BtnShowProt = new System.Windows.Forms.Button();
            this.BtnBrowseProt = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.comboFileMask = new NS_UserCombo.FileComboBox();
            this.EditFmBtn = new System.Windows.Forms.Button();
            this.BtnBrowseFM = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textTimeDelta = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkRegardAge = new System.Windows.Forms.CheckBox();
            this.textFileAge = new System.Windows.Forms.TextBox();
            this.checkSkipCount = new System.Windows.Forms.CheckBox();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.comboFileProt);
            this.groupBox4.Controls.Add(this.BtnShowProt);
            this.groupBox4.Controls.Add(this.BtnBrowseProt);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Location = new System.Drawing.Point(8, 96);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(479, 80);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Protocol file";
            // 
            // comboFileProt
            // 
            this.comboFileProt.AllowDrop = true;
            this.comboFileProt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFileProt.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboFileProt.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.comboFileProt.Location = new System.Drawing.Point(24, 32);
            this.comboFileProt.MaxDropDownItems = 70;
            this.comboFileProt.Name = "comboFileProt";
            this.comboFileProt.ReadOnly = false;
            this.comboFileProt.Size = new System.Drawing.Size(352, 21);
            this.comboFileProt.Sorted = true;
            this.comboFileProt.TabIndex = 4;
            this.comboFileProt.Text = "\\temp\\BackupProtocol.txt";
            // 
            // BtnShowProt
            // 
            this.BtnShowProt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnShowProt.Location = new System.Drawing.Point(399, 21);
            this.BtnShowProt.Name = "BtnShowProt";
            this.BtnShowProt.Size = new System.Drawing.Size(62, 20);
            this.BtnShowProt.TabIndex = 0;
            this.BtnShowProt.TabStop = false;
            this.BtnShowProt.Text = "Show";
            this.BtnShowProt.Click += new System.EventHandler(this.BtnShowProt_Click);
            // 
            // BtnBrowseProt
            // 
            this.BtnBrowseProt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBrowseProt.Location = new System.Drawing.Point(399, 48);
            this.BtnBrowseProt.Name = "BtnBrowseProt";
            this.BtnBrowseProt.Size = new System.Drawing.Size(62, 20);
            this.BtnBrowseProt.TabIndex = 2;
            this.BtnBrowseProt.TabStop = false;
            this.BtnBrowseProt.Text = "Browse";
            this.BtnBrowseProt.Click += new System.EventHandler(this.BtnBrowseProt_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Location = new System.Drawing.Point(200, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 64);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.comboFileMask);
            this.groupBox3.Controls.Add(this.EditFmBtn);
            this.groupBox3.Controls.Add(this.BtnBrowseFM);
            this.groupBox3.Location = new System.Drawing.Point(8, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(479, 80);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "File mask list file";
            // 
            // comboFileMask
            // 
            this.comboFileMask.AllowDrop = true;
            this.comboFileMask.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFileMask.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboFileMask.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.comboFileMask.Location = new System.Drawing.Point(24, 32);
            this.comboFileMask.MaxDropDownItems = 70;
            this.comboFileMask.Name = "comboFileMask";
            this.comboFileMask.ReadOnly = false;
            this.comboFileMask.Size = new System.Drawing.Size(352, 21);
            this.comboFileMask.Sorted = true;
            this.comboFileMask.TabIndex = 3;
            this.comboFileMask.Text = "BackupFileMasks.txt";
            // 
            // EditFmBtn
            // 
            this.EditFmBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EditFmBtn.Location = new System.Drawing.Point(399, 21);
            this.EditFmBtn.Name = "EditFmBtn";
            this.EditFmBtn.Size = new System.Drawing.Size(62, 20);
            this.EditFmBtn.TabIndex = 7;
            this.EditFmBtn.TabStop = false;
            this.EditFmBtn.Text = "Edit";
            this.EditFmBtn.Click += new System.EventHandler(this.EditFmBtn_Click);
            // 
            // BtnBrowseFM
            // 
            this.BtnBrowseFM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBrowseFM.Location = new System.Drawing.Point(399, 48);
            this.BtnBrowseFM.Name = "BtnBrowseFM";
            this.BtnBrowseFM.Size = new System.Drawing.Size(62, 20);
            this.BtnBrowseFM.TabIndex = 2;
            this.BtnBrowseFM.TabStop = false;
            this.BtnBrowseFM.Text = "Browse";
            this.BtnBrowseFM.Click += new System.EventHandler(this.BtnBrowseFM_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textTimeDelta);
            this.groupBox1.Location = new System.Drawing.Point(8, 184);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 56);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Max. time delta";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(88, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(8, 16);
            this.label2.TabIndex = 10;
            this.label2.Text = "s";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textTimeDelta
            // 
            this.textTimeDelta.Location = new System.Drawing.Point(24, 24);
            this.textTimeDelta.Name = "textTimeDelta";
            this.textTimeDelta.Size = new System.Drawing.Size(56, 20);
            this.textTimeDelta.TabIndex = 14;
            this.textTimeDelta.Text = "3,0";
            this.textTimeDelta.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.checkRegardAge);
            this.groupBox5.Controls.Add(this.textFileAge);
            this.groupBox5.Location = new System.Drawing.Point(160, 184);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(184, 56);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Max. file age";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(128, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 23);
            this.label1.TabIndex = 11;
            this.label1.Text = "days";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkRegardAge
            // 
            this.checkRegardAge.Checked = true;
            this.checkRegardAge.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkRegardAge.Location = new System.Drawing.Point(16, 24);
            this.checkRegardAge.Name = "checkRegardAge";
            this.checkRegardAge.Size = new System.Drawing.Size(56, 24);
            this.checkRegardAge.TabIndex = 14;
            this.checkRegardAge.Text = "regard";
            // 
            // textFileAge
            // 
            this.textFileAge.Location = new System.Drawing.Point(72, 24);
            this.textFileAge.Name = "textFileAge";
            this.textFileAge.Size = new System.Drawing.Size(48, 20);
            this.textFileAge.TabIndex = 15;
            this.textFileAge.Text = "60";
            this.textFileAge.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkSkipCount
            // 
            this.checkSkipCount.Location = new System.Drawing.Point(376, 208);
            this.checkSkipCount.Name = "checkSkipCount";
            this.checkSkipCount.Size = new System.Drawing.Size(120, 24);
            this.checkSkipCount.TabIndex = 14;
            this.checkSkipCount.Text = "Skip file counting";
            // 
            // PreferencesForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(498, 250);
            this.Controls.Add(this.checkSkipCount);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(2000, 289);
            this.MinimumSize = new System.Drawing.Size(514, 289);
            this.Name = "PreferencesForm";
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreferencesForm_FormClosing);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }
		#endregion

        private void EditFmBtn_Click(object sender, System.EventArgs e)
        {
            string path = comboFileMask.Text;
            FormFmEditor dlg = new FormFmEditor(path);
            
            //Process.Start("notepad.exe", path);
            //return;

            if (! m_MaskLoc.IsInit())
            {
                dlg.Location = m_MaskLoc.Loc;
                dlg.Size     = m_MaskLoc.Siz;
            }

            dlg.ShowDialog();

            m_MaskLoc.Loc = dlg.Location;
            m_MaskLoc.Siz = dlg.Size;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   06.11.2024
        ***************************************************************************/
        public void ShowProtView()
        {
            Cursor = Cursors.Default;

            if (Utils.NoFile(comboFileProt.Text)) return;

            m_ProtView.ReadFile(comboFileProt.Text);
            if (0 == m_ProtView.GetNrLines())
            {
                MessageBox.Show("Nothing copied !","Result");
                return;
            }

            m_ProtView.Refresh();
        }


        private void BtnShowProt_Click(object sender, System.EventArgs e)
        {
            ShowProtView();
        }

        private void BtnBrowseProt_Click(object sender, System.EventArgs e)
        {
            comboFileProt.BrowseFileRead();
        }

        private void BtnBrowseFM_Click(object sender, System.EventArgs e)
        {
            comboFileMask.BrowseFileRead();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       20.12.2014
        LAST CHANGE:   20.12.2014
        ***************************************************************************/
        public void Serialize(ref AppSettings a_Conf)
        {
            if ( a_Conf.IsReading )
            {
                a_Conf.ReadDlgLocation(m_ProtLoc);
                a_Conf.ReadDlgLocation(m_MaskLoc);
                checkRegardAge.Checked = (bool)  a_Conf.Deserialize();
                checkSkipCount.Checked = (bool)  a_Conf.Deserialize();
                textTimeDelta.Text     = (string)a_Conf.Deserialize();
                textFileAge.Text       = (string)a_Conf.Deserialize();
            }
            else
            {
                a_Conf.WriteDlgLocation(m_ProtLoc);
                a_Conf.WriteDlgLocation(m_MaskLoc);
                a_Conf.Serialize(checkRegardAge.Checked);
                a_Conf.Serialize(checkSkipCount.Checked);
                a_Conf.Serialize(textTimeDelta.Text);
                a_Conf.Serialize(textFileAge.Text);
            }

            comboFileProt.Serialize(ref a_Conf);
            comboFileMask.Serialize(ref a_Conf);
            m_ProtView   .Serialize(ref a_Conf);
        }

        // obsolete 20.12.2014
        public void DlgWrite(AppSettings i_Config)
        {
            i_Config.SerializeComboBox(comboFileProt);
            i_Config.SerializeComboBox(comboFileMask);
            i_Config.WriteDlgLocation(m_ProtLoc);
            i_Config.WriteDlgLocation(m_MaskLoc);
            i_Config.Serialize(checkRegardAge.Checked);
            i_Config.Serialize(checkSkipCount.Checked);
            i_Config.Serialize(textTimeDelta.Text);
            i_Config.Serialize(textFileAge.Text);
        }

        // obsolete 20.12.2014
        public void DlgRead(AppSettings i_Config)
        {
            i_Config.DeserializeComboBox(comboFileProt);
            i_Config.DeserializeComboBox(comboFileMask);
            i_Config.ReadDlgLocation(m_ProtLoc);
            i_Config.ReadDlgLocation(m_MaskLoc);
            checkRegardAge.Checked = (bool)  i_Config.Deserialize();
            checkSkipCount.Checked = (bool)  i_Config.Deserialize();
            textTimeDelta.Text     = (string)i_Config.Deserialize();
            textFileAge.Text       = (string)i_Config.Deserialize();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       21.03.2007
        LAST CHANGE:   21.03.2007
        ***************************************************************************/
        public void ClosePrtView()
        {
            if ( this.InvokeRequired )
            {
                dl_CloseProtView d = new dl_CloseProtView(ClosePrtView);
                this.Invoke(d,new object[] {  });
            }
            else
            {
                if ( null == m_ProtView ) return;

                if ( m_ProtView.Visible )
                {
                    m_ProtView.Close();
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       12.02.2023
        LAST CHANGE:   12.02.2023
        ***************************************************************************/
        private void PreferencesForm_FormClosing( object sender, FormClosingEventArgs e )
        {
        }
    }
}

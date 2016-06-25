using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Xml;

using NS_UserCombo;
using NS_UserList;
using NS_AppConfig;
using NS_Utilities;

namespace NS_Backup
{
    public delegate void dl_ShowFilename     (string sFname, bool bAppend);
    public delegate void dl_ChangeCursor     (Cursor a_Curs);
    public delegate void dl_InscribeStartBtn (string sText);
    public delegate void dl_ShowActivity     (string sText);
    public delegate void dl_ShowFilProgBar   (int iProg);
    public delegate void dl_ShowAllProgBar   ( int iCurrFiles, int iAllFiles );
    public delegate void dl_ShowTimeSpan     (TimeSpan tSpan);

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
    /***************************************************************************
    SPECIFICATION: 
    CREATED:       08.03.2007
    LAST CHANGE:   25.03.2016
    ***************************************************************************/
    public class Form : System.Windows.Forms.Form
	{
        const int MIN_HEIGHT = 454;
        const int MIN_WIDTH  = 455;

        public event dl_ShowFilename     m_eShowFilename;
        public event dl_ChangeCursor     m_eChangeCursor;
        public event dl_InscribeStartBtn m_eInscribeStartButton;
        public event dl_ShowActivity     m_eShowActivity;

        private const int DB_VERSION     = 90;  // 05.06.2016
        private const string RELEASE     = "Release: 1.36";
        private const string INI_FNAME   = "backup.ini";

        private AppSettings             m_Config;
        private Backup                  m_Backup;
        private PreferencesForm         m_Pref;
        private bool                    m_bRunning;
        private int                     m_iOldCurrFiles; 
        private int                     m_iOldFProg;
        private Favorites               m_Favs;
        private List<string>            m_Selected;
        public  UserFileListView        listViewFiles;
        private int                     m_iNrChars;
        private bool                    m_bEnteringSelection;
        private static string           m_sArgument;
        private ProtocolView            m_ProtView;     

        private Button                  StartBtn;
        private GroupBox                groupBox1;
        private GroupBox                groupBox2;
        public  FileComboBox            comboSrc;
        public  FileComboBox            comboDest;
        public  ComboBox                comboSrcDrv;
        public  CheckBox                checkBoxFolderOnly;
        private Button                  BtnBrowseDst;
        private Button BtnBrowseSrc;
        private System.Windows.Forms.   MenuItem menuItemHelp;
        private System.Windows.Forms.   MenuItem menuAbout;
        private System.Windows.Forms.   TextBox textBoxActivity;
        private System.Windows.Forms.   TextBox textBoxFilename;
        private System.Windows.Forms.   ProgressBar progressBarFile;
        public  System.Windows.Forms.   CheckBox checkBoxDrives;
        public System.Windows.Forms.    ComboBox comboBoxDrives;
        private System.Windows.Forms.   MenuItem menuItem4;
        private System.Windows.Forms.   MenuItem menuItemFavorites;
        private System.Windows.Forms.   MenuItem menuItemFavSave;
        private                         MainMenu mainMenu;
        private System.Windows.Forms.   Button button_Swap;
        private Button                  buttonParentDir;
        private CheckBox                checkShutdown;
        private CheckBox                checkHibernate;
        private ProgressBar             progressBarAll;
        private Button                  button_SelectAll;
        private Button                  button_SelectInvert;
        private TextBox                 textBox_Timer;
        private TextBox                 textBoxCounts;
        private Label                   label1;
        private MenuItem                menuItemOptions;
        public  MenuItem                menuItemCopyOnStart;
        public  MenuItem                menuItemCopyAll;
        public  MenuItem                menuItemEvenNewer;
        public  MenuItem                menuItemIgnoreR;
        public  MenuItem                menuItemFileAge;
        private MenuItem                menuItemResizeCols;
        private MenuItem                menuItem1;
        private MenuItem                menuItemPrefer;
        private MenuItem                menuItem3;
        private MenuItem                menuItemHibernate;
        private MenuItem                menuItemShutdown;
        private MenuItem                menuItem5;
        private MenuItem                menuItemShowProt;
        private IContainer              components;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       03.03.2007
        LAST CHANGE:   21.06.2016
        ***************************************************************************/
        public Form()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
            string sCurrDir  = Directory.GetCurrentDirectory() + "\\";
            sCurrDir         = Utils.ConcatPaths( sCurrDir, SystemInformation.ComputerName );
            if ( ! Directory.Exists(sCurrDir) ) Directory.CreateDirectory( sCurrDir );
            m_Config         = new AppSettings( Utils.ConcatPaths(sCurrDir,INI_FNAME) );
            m_Config.m_OldVersion1 = 7;
            m_Config.m_OldVersion2 = 8;
            m_Favs           = new Favorites(ref menuItemFavorites,this);
            m_Backup         = null;
            m_bRunning       = false;
            m_iOldCurrFiles  = 0;
            m_iOldFProg      = 0;
            m_iNrChars       = 70;
            checkBoxDrives.Checked = true;
            m_Selected       = new List<string>();
            m_ProtView       = new ProtocolView();
            m_Pref           = new PreferencesForm( m_ProtView );
        }

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null)
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form));
            this.BtnBrowseDst = new System.Windows.Forms.Button();
            this.BtnBrowseSrc = new System.Windows.Forms.Button();
            this.StartBtn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_SelectInvert = new System.Windows.Forms.Button();
            this.button_SelectAll = new System.Windows.Forms.Button();
            this.buttonParentDir = new System.Windows.Forms.Button();
            this.checkBoxFolderOnly = new System.Windows.Forms.CheckBox();
            this.comboSrcDrv = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.comboBoxDrives = new System.Windows.Forms.ComboBox();
            this.checkBoxDrives = new System.Windows.Forms.CheckBox();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFavorites = new System.Windows.Forms.MenuItem();
            this.menuItemFavSave = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemHelp = new System.Windows.Forms.MenuItem();
            this.menuAbout = new System.Windows.Forms.MenuItem();
            this.textBoxActivity = new System.Windows.Forms.TextBox();
            this.textBoxFilename = new System.Windows.Forms.TextBox();
            this.progressBarFile = new System.Windows.Forms.ProgressBar();
            this.button_Swap = new System.Windows.Forms.Button();
            this.checkShutdown = new System.Windows.Forms.CheckBox();
            this.progressBarAll = new System.Windows.Forms.ProgressBar();
            this.textBox_Timer = new System.Windows.Forms.TextBox();
            this.textBoxCounts = new System.Windows.Forms.TextBox();
            this.checkHibernate = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.menuItemCopyOnStart = new System.Windows.Forms.MenuItem();
            this.menuItemCopyAll = new System.Windows.Forms.MenuItem();
            this.menuItemEvenNewer = new System.Windows.Forms.MenuItem();
            this.menuItemIgnoreR = new System.Windows.Forms.MenuItem();
            this.menuItemFileAge = new System.Windows.Forms.MenuItem();
            this.menuItemResizeCols = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemPrefer = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemHibernate = new System.Windows.Forms.MenuItem();
            this.menuItemShutdown = new System.Windows.Forms.MenuItem();
            this.menuItemOptions = new System.Windows.Forms.MenuItem();
            this.menuItemShowProt = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.comboDest = new NS_UserCombo.FileComboBox();
            this.listViewFiles = new NS_UserList.UserFileListView();
            this.comboSrc = new NS_UserCombo.FileComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnBrowseDst
            // 
            this.BtnBrowseDst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBrowseDst.Location = new System.Drawing.Point(403, 24);
            this.BtnBrowseDst.Name = "BtnBrowseDst";
            this.BtnBrowseDst.Size = new System.Drawing.Size(62, 22);
            this.BtnBrowseDst.TabIndex = 6;
            this.BtnBrowseDst.TabStop = false;
            this.BtnBrowseDst.Text = "Browse";
            this.BtnBrowseDst.Click += new System.EventHandler(this.BtnBrowseDst_Click);
            // 
            // BtnBrowseSrc
            // 
            this.BtnBrowseSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnBrowseSrc.Location = new System.Drawing.Point(403, 17);
            this.BtnBrowseSrc.Name = "BtnBrowseSrc";
            this.BtnBrowseSrc.Size = new System.Drawing.Size(62, 23);
            this.BtnBrowseSrc.TabIndex = 5;
            this.BtnBrowseSrc.TabStop = false;
            this.BtnBrowseSrc.Text = "Browse";
            this.BtnBrowseSrc.Click += new System.EventHandler(this.BtnBrowseSrc_Click);
            // 
            // StartBtn
            // 
            this.StartBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartBtn.Location = new System.Drawing.Point(427, 397);
            this.StartBtn.Name = "StartBtn";
            this.StartBtn.Size = new System.Drawing.Size(71, 65);
            this.StartBtn.TabIndex = 0;
            this.StartBtn.TabStop = false;
            this.StartBtn.Text = "&Copy";
            this.StartBtn.Click += new System.EventHandler(this.StartBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button_SelectInvert);
            this.groupBox1.Controls.Add(this.button_SelectAll);
            this.groupBox1.Controls.Add(this.buttonParentDir);
            this.groupBox1.Controls.Add(this.checkBoxFolderOnly);
            this.groupBox1.Controls.Add(this.comboSrcDrv);
            this.groupBox1.Controls.Add(this.listViewFiles);
            this.groupBox1.Controls.Add(this.comboSrc);
            this.groupBox1.Controls.Add(this.BtnBrowseSrc);
            this.groupBox1.Location = new System.Drawing.Point(16, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 203);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Source";
            // 
            // button_SelectInvert
            // 
            this.button_SelectInvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectInvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SelectInvert.Location = new System.Drawing.Point(405, 170);
            this.button_SelectInvert.Name = "button_SelectInvert";
            this.button_SelectInvert.Size = new System.Drawing.Size(60, 23);
            this.button_SelectInvert.TabIndex = 13;
            this.button_SelectInvert.Text = "&Invert";
            this.button_SelectInvert.UseVisualStyleBackColor = true;
            this.button_SelectInvert.Click += new System.EventHandler(this.button_SelectInvert_Click);
            // 
            // button_SelectAll
            // 
            this.button_SelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_SelectAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_SelectAll.Location = new System.Drawing.Point(331, 170);
            this.button_SelectAll.Name = "button_SelectAll";
            this.button_SelectAll.Size = new System.Drawing.Size(60, 23);
            this.button_SelectAll.TabIndex = 12;
            this.button_SelectAll.Text = "Select &all";
            this.button_SelectAll.UseVisualStyleBackColor = true;
            this.button_SelectAll.Click += new System.EventHandler(this.button_SelectAll_Click);
            // 
            // buttonParentDir
            // 
            this.buttonParentDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonParentDir.Location = new System.Drawing.Point(361, 17);
            this.buttonParentDir.Name = "buttonParentDir";
            this.buttonParentDir.Size = new System.Drawing.Size(30, 23);
            this.buttonParentDir.TabIndex = 11;
            this.buttonParentDir.Text = "[..]";
            this.buttonParentDir.UseVisualStyleBackColor = true;
            this.buttonParentDir.Click += new System.EventHandler(this.buttonParentDir_Click);
            // 
            // checkBoxFolderOnly
            // 
            this.checkBoxFolderOnly.AutoSize = true;
            this.checkBoxFolderOnly.Location = new System.Drawing.Point(278, 20);
            this.checkBoxFolderOnly.Name = "checkBoxFolderOnly";
            this.checkBoxFolderOnly.Size = new System.Drawing.Size(77, 17);
            this.checkBoxFolderOnly.TabIndex = 10;
            this.checkBoxFolderOnly.Text = "Folder only";
            this.checkBoxFolderOnly.UseVisualStyleBackColor = true;
            this.checkBoxFolderOnly.CheckedChanged += new System.EventHandler(this.checkBoxFolderOnly_CheckedChanged);
            // 
            // comboSrcDrv
            // 
            this.comboSrcDrv.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.comboSrcDrv.Location = new System.Drawing.Point(16, 18);
            this.comboSrcDrv.MaxDropDownItems = 30;
            this.comboSrcDrv.Name = "comboSrcDrv";
            this.comboSrcDrv.Size = new System.Drawing.Size(249, 22);
            this.comboSrcDrv.TabIndex = 9;
            this.comboSrcDrv.Text = "C:";
            this.comboSrcDrv.SelectedIndexChanged += new System.EventHandler(this.comboSrcDrv_SelectedIndexChanged);
            this.comboSrcDrv.Click += new System.EventHandler(this.comboSrcDrv_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.comboBoxDrives);
            this.groupBox2.Controls.Add(this.checkBoxDrives);
            this.groupBox2.Controls.Add(this.comboDest);
            this.groupBox2.Controls.Add(this.BtnBrowseDst);
            this.groupBox2.Location = new System.Drawing.Point(16, 264);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(482, 87);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Destination folder";
            // 
            // comboBoxDrives
            // 
            this.comboBoxDrives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxDrives.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.comboBoxDrives.Location = new System.Drawing.Point(124, 24);
            this.comboBoxDrives.MaxDropDownItems = 30;
            this.comboBoxDrives.Name = "comboBoxDrives";
            this.comboBoxDrives.Size = new System.Drawing.Size(267, 22);
            this.comboBoxDrives.TabIndex = 8;
            this.comboBoxDrives.Text = "C:";
            this.comboBoxDrives.SelectedIndexChanged += new System.EventHandler(this.comboBoxDrives_SelectedIndexChanged);
            this.comboBoxDrives.Click += new System.EventHandler(this.comboBoxDrives_Click);
            // 
            // checkBoxDrives
            // 
            this.checkBoxDrives.Location = new System.Drawing.Point(16, 24);
            this.checkBoxDrives.Name = "checkBoxDrives";
            this.checkBoxDrives.Size = new System.Drawing.Size(88, 24);
            this.checkBoxDrives.TabIndex = 7;
            this.checkBoxDrives.Text = "Tree 2 Drive";
            this.checkBoxDrives.CheckedChanged += new System.EventHandler(this.checkBoxDrives_CheckedChanged);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemOptions,
            this.menuItemFavorites,
            this.menuItemHelp});
            // 
            // menuItemFavorites
            // 
            this.menuItemFavorites.Index = 1;
            this.menuItemFavorites.MdiList = true;
            this.menuItemFavorites.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFavSave,
            this.menuItem4});
            this.menuItemFavorites.Text = "&Favorites";
            // 
            // menuItemFavSave
            // 
            this.menuItemFavSave.Index = 0;
            this.menuItemFavSave.Text = "&Save";
            this.menuItemFavSave.Click += new System.EventHandler(this.menuItemFavSave_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 1;
            this.menuItem4.Text = "-";
            // 
            // menuItemHelp
            // 
            this.menuItemHelp.Index = 2;
            this.menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuAbout});
            this.menuItemHelp.Text = "&Help";
            // 
            // menuAbout
            // 
            this.menuAbout.Index = 0;
            this.menuAbout.Text = "&About";
            this.menuAbout.Click += new System.EventHandler(this.menuAbout_Click);
            // 
            // textBoxActivity
            // 
            this.textBoxActivity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxActivity.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.textBoxActivity.Location = new System.Drawing.Point(16, 368);
            this.textBoxActivity.Name = "textBoxActivity";
            this.textBoxActivity.Size = new System.Drawing.Size(482, 20);
            this.textBoxActivity.TabIndex = 5;
            // 
            // textBoxFilename
            // 
            this.textBoxFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilename.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxFilename.Location = new System.Drawing.Point(16, 397);
            this.textBoxFilename.Name = "textBoxFilename";
            this.textBoxFilename.Size = new System.Drawing.Size(384, 20);
            this.textBoxFilename.TabIndex = 6;
            this.textBoxFilename.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // progressBarFile
            // 
            this.progressBarFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarFile.Location = new System.Drawing.Point(16, 423);
            this.progressBarFile.Name = "progressBarFile";
            this.progressBarFile.Size = new System.Drawing.Size(384, 16);
            this.progressBarFile.TabIndex = 7;
            // 
            // button_Swap
            // 
            this.button_Swap.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.button_Swap.Location = new System.Drawing.Point(191, 228);
            this.button_Swap.Name = "button_Swap";
            this.button_Swap.Size = new System.Drawing.Size(57, 25);
            this.button_Swap.TabIndex = 8;
            this.button_Swap.Text = "&Swap";
            this.button_Swap.Click += new System.EventHandler(this.button_Swap_Click);
            // 
            // checkShutdown
            // 
            this.checkShutdown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkShutdown.AutoSize = true;
            this.checkShutdown.Location = new System.Drawing.Point(347, 241);
            this.checkShutdown.Name = "checkShutdown";
            this.checkShutdown.Size = new System.Drawing.Size(74, 17);
            this.checkShutdown.TabIndex = 9;
            this.checkShutdown.Text = "Shutdown";
            this.checkShutdown.UseVisualStyleBackColor = true;
            // 
            // progressBarAll
            // 
            this.progressBarAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarAll.Location = new System.Drawing.Point(16, 445);
            this.progressBarAll.Name = "progressBarAll";
            this.progressBarAll.Size = new System.Drawing.Size(384, 17);
            this.progressBarAll.TabIndex = 10;
            // 
            // textBox_Timer
            // 
            this.textBox_Timer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBox_Timer.Location = new System.Drawing.Point(16, 231);
            this.textBox_Timer.Name = "textBox_Timer";
            this.textBox_Timer.ReadOnly = true;
            this.textBox_Timer.Size = new System.Drawing.Size(63, 20);
            this.textBox_Timer.TabIndex = 11;
            this.textBox_Timer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxCounts
            // 
            this.textBoxCounts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxCounts.Location = new System.Drawing.Point(84, 231);
            this.textBoxCounts.Name = "textBoxCounts";
            this.textBoxCounts.ReadOnly = true;
            this.textBoxCounts.Size = new System.Drawing.Size(100, 20);
            this.textBoxCounts.TabIndex = 12;
            this.textBoxCounts.Text = "0 / 0";
            this.textBoxCounts.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkHibernate
            // 
            this.checkHibernate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkHibernate.AutoSize = true;
            this.checkHibernate.Location = new System.Drawing.Point(347, 224);
            this.checkHibernate.Name = "checkHibernate";
            this.checkHibernate.Size = new System.Drawing.Size(72, 17);
            this.checkHibernate.TabIndex = 13;
            this.checkHibernate.Text = "Hibernate";
            this.checkHibernate.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(419, 235);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "when ready";
            // 
            // menuItemCopyOnStart
            // 
            this.menuItemCopyOnStart.Index = 0;
            this.menuItemCopyOnStart.Text = "Copy on &startup";
            this.menuItemCopyOnStart.Click += new System.EventHandler(this.menuItemCopyOnStart_Click);
            // 
            // menuItemCopyAll
            // 
            this.menuItemCopyAll.Index = 1;
            this.menuItemCopyAll.Text = "Copy &all (*.*)";
            this.menuItemCopyAll.Click += new System.EventHandler(this.menuItemCopyAll_Click);
            // 
            // menuItemEvenNewer
            // 
            this.menuItemEvenNewer.Index = 2;
            this.menuItemEvenNewer.Text = "Overwrite &even newer";
            this.menuItemEvenNewer.Click += new System.EventHandler(this.menuItemEvenNewer_Click);
            // 
            // menuItemIgnoreR
            // 
            this.menuItemIgnoreR.Index = 3;
            this.menuItemIgnoreR.Text = "Ignore &R Attribute";
            this.menuItemIgnoreR.Click += new System.EventHandler(this.menuItemIgnoreR_Click);
            // 
            // menuItemFileAge
            // 
            this.menuItemFileAge.Index = 4;
            this.menuItemFileAge.Text = "Re&gard file age";
            this.menuItemFileAge.Click += new System.EventHandler(this.menuItemFileAge_Click);
            // 
            // menuItemResizeCols
            // 
            this.menuItemResizeCols.Index = 5;
            this.menuItemResizeCols.Text = "Resize &Columns";
            this.menuItemResizeCols.Click += new System.EventHandler(this.menuItemResizeCols_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 8;
            this.menuItem1.Text = "-";
            // 
            // menuItemPrefer
            // 
            this.menuItemPrefer.Index = 9;
            this.menuItemPrefer.Text = "&Preferences...";
            this.menuItemPrefer.Click += new System.EventHandler(this.menuItemPrefer_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 10;
            this.menuItem3.Text = "-";
            // 
            // menuItemHibernate
            // 
            this.menuItemHibernate.Index = 11;
            this.menuItemHibernate.Text = "&Hibernate";
            this.menuItemHibernate.Click += new System.EventHandler(this.menuItemHibernate_Click);
            // 
            // menuItemShutdown
            // 
            this.menuItemShutdown.Index = 12;
            this.menuItemShutdown.Text = "Shut&down";
            this.menuItemShutdown.Click += new System.EventHandler(this.menuItemShutdown_Click);
            // 
            // menuItemOptions
            // 
            this.menuItemOptions.Index = 0;
            this.menuItemOptions.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCopyOnStart,
            this.menuItemCopyAll,
            this.menuItemEvenNewer,
            this.menuItemIgnoreR,
            this.menuItemFileAge,
            this.menuItemResizeCols,
            this.menuItem5,
            this.menuItemShowProt,
            this.menuItem1,
            this.menuItemPrefer,
            this.menuItem3,
            this.menuItemHibernate,
            this.menuItemShutdown});
            this.menuItemOptions.Text = "&Options";
            // 
            // menuItemShowProt
            // 
            this.menuItemShowProt.Index = 7;
            this.menuItemShowProt.Text = "&Show protocol file";
            this.menuItemShowProt.Click += new System.EventHandler(this.menuItemShowProt_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 6;
            this.menuItem5.Text = "-";
            // 
            // comboDest
            // 
            this.comboDest.AllowDrop = true;
            this.comboDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboDest.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboDest.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.comboDest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboDest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboDest.Location = new System.Drawing.Point(16, 53);
            this.comboDest.MaxDropDownItems = 20;
            this.comboDest.Name = "comboDest";
            this.comboDest.ReadOnly = false;
            this.comboDest.Size = new System.Drawing.Size(449, 21);
            this.comboDest.Sorted = true;
            this.comboDest.TabIndex = 2;
            this.comboDest.Text = "Enter destination folder";
            // 
            // listViewFiles
            // 
            this.listViewFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewFiles.CurrDir = null;
            this.listViewFiles.CurrExt = "*.*";
            this.listViewFiles.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewFiles.FullRowSelect = true;
            this.listViewFiles.HideSelection = false;
            this.listViewFiles.KindOfTime = NS_UserList.KIND_OF_TIME.KD_Write;
            this.listViewFiles.Location = new System.Drawing.Point(16, 75);
            this.listViewFiles.Name = "listViewFiles";
            this.listViewFiles.Size = new System.Drawing.Size(449, 89);
            this.listViewFiles.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewFiles.TabIndex = 6;
            this.listViewFiles.UseCompatibleStateImageBehavior = false;
            this.listViewFiles.View = System.Windows.Forms.View.Details;
            this.listViewFiles.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewFiles_ItemSelectionChanged);
            this.listViewFiles.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewFiles_MouseDoubleClick);
            // 
            // comboSrc
            // 
            this.comboSrc.AllowDrop = true;
            this.comboSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSrc.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.comboSrc.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.comboSrc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.comboSrc.Location = new System.Drawing.Point(16, 48);
            this.comboSrc.MaxDropDownItems = 20;
            this.comboSrc.Name = "comboSrc";
            this.comboSrc.ReadOnly = false;
            this.comboSrc.Size = new System.Drawing.Size(449, 21);
            this.comboSrc.Sorted = true;
            this.comboSrc.TabIndex = 1;
            this.comboSrc.Text = "Enter source folder";
            this.comboSrc.SelectedIndexChanged += new System.EventHandler(this.comboSrc_SelectedIndexChanged);
            this.comboSrc.TextUpdate += new System.EventHandler(this.comboSrc_TextUpdate);
            // 
            // Form
            // 
            this.AllowDrop = true;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(511, 484);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkHibernate);
            this.Controls.Add(this.textBoxCounts);
            this.Controls.Add(this.textBox_Timer);
            this.Controls.Add(this.progressBarAll);
            this.Controls.Add(this.checkShutdown);
            this.Controls.Add(this.button_Swap);
            this.Controls.Add(this.progressBarFile);
            this.Controls.Add(this.textBoxFilename);
            this.Controls.Add(this.textBoxActivity);
            this.Controls.Add(this.StartBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Menu = this.mainMenu;
            this.MinimumSize = new System.Drawing.Size(527, 522);
            this.Name = "Form";
            this.Text = "TreeCopy";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.Form_Closing);
            this.Load += new System.EventHandler(this.Form_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            this.Resize += new System.EventHandler(this.Form_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
		#endregion
        
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   24.08.2006
        ***************************************************************************/
        static void Main(string[] args) 
		{
            if(0 != args.Length)    m_sArgument = args[0];
            else                    m_sArgument = "";
			Application.Run(new Form());
		}


        /***************************************************************************
       SPECIFICATION: 
       CREATED:       30.10.2004
       LAST CHANGE:   30.10.2004
       ***************************************************************************/
        private void StartBtn_Click(object sender,System.EventArgs e)
        {
            StartBtn_ClickSub();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        private void StartBtn_ClickSub()
        {
            if (menuItemCopyAll.Checked && ! m_bRunning)
            {
                DialogResult res = MessageBox.Show("      'Copy all' is checked !\n      Is this OK ?","Warning",MessageBoxButtons.YesNo);
                if (res == DialogResult.No) return;
            }

            if (menuItemEvenNewer.Checked && ! m_bRunning)
            {
                DialogResult res = MessageBox.Show("   'Overwrite even newer' is checked !\n   Is this OK ?","Warning",MessageBoxButtons.YesNo);
                if (res == DialogResult.No) return;
            }

            if (null == m_Backup) return;

            if (! ReadExtFile()) return;

            if (m_bRunning)
            {
                m_Backup.Stop();
                Stopped();
                return;
            }

            comboSrc .AddTextEntry();
            comboDest.AddTextEntry();
            m_Pref.comboFileProt.AddTextEntry();
            m_Pref.comboFileMask.AddTextEntry();

            m_Backup.SetSrcPath(comboSrc.Text);
            m_ProtView.Dirs = GetDirs();

            if ( ! checkBoxFolderOnly.Checked )
            {
                m_Backup.SetSrcFiles(listViewFiles.GetFilesAndDirs(true));
            }

            string sDestPath;
            if (checkBoxDrives.Checked)
            {
                string sDrv  = comboBoxDrives.Text.Substring(0,3);
                sDestPath    = comboSrc.Text;
                sDestPath    = sDestPath.Remove(0,1);
                sDestPath    = sDestPath.Insert(0,sDrv[0].ToString());
            }
            else
            {
                sDestPath = comboDest.Text;
            }
            m_Backup.SetDstPath(sDestPath);
            m_Backup.SetProtPath(m_Pref.comboFileProt.Text);

            m_Backup.IgnoreR   = menuItemIgnoreR  .Checked;
            m_Backup.CopyAll   = menuItemCopyAll  .Checked;
            m_Backup.EvenNewer = menuItemEvenNewer.Checked;

            m_Backup.m_dMaxTimeDelta = double.Parse(m_Pref.textTimeDelta.Text);
            if (m_Pref.checkRegardAge.Checked)
            {
                m_Backup.m_iMaxAge = long.Parse(m_Pref.textFileAge.Text);
            }
            else 
            {
                m_Backup.m_iMaxAge = -1;
            }

            m_Backup.m_bSkipCount = m_Pref.checkSkipCount.Checked;

            //m_iOldAProg = 0;

            Started();
            if (! m_Backup.Start())
            {
                Stopped();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.02.2016
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        private DirType GetDirs()
        {
            DirType dirs = new DirType();

            dirs.src = comboSrc.Text;

            if( checkBoxDrives.Checked )
            {
                string p = Utils.GetDriveLetter(comboBoxDrives.Text);
                int idx = dirs.src.IndexOf(":");
                if( idx != -1 )
                {
                    p += dirs.src.Remove(0,idx+1);
                    dirs.dst = p;
                }
            }
            else
            {
                dirs.dst = comboDest.Text;    
            }

            return dirs;
        }

        /***************************************************************************
        * SPECIFICATION: 
        * CREATED:       10.08.2006
        * LAST CHANGE:   10.08.2006
        ***************************************************************************/
        private void Started()
        {
            m_eChangeCursor(Cursors.WaitCursor);
            m_eInscribeStartButton("&Stop");
            m_bRunning    = true;
        }

        /***************************************************************************
        * SPECIFICATION: 
        * CREATED:       10.08.2006
        * LAST CHANGE:   10.08.2006
        ***************************************************************************/
        private void Stopped()
        {
            m_eChangeCursor(Cursors.Default);
            m_eInscribeStartButton("&Copy");
            m_bRunning    = false;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.12.2011
        LAST CHANGE:   07.01.2012
        ***************************************************************************/
        private void Hibernate()
        {
            //System.Diagnostics.Process.Start( "shutdown", "/h" ); 
            System.Diagnostics.Process.Start("rundll32", "powrprof.dll,SetSuspendState" );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.12.2011
        LAST CHANGE:   29.12.2011
        ***************************************************************************/
        private void Shutdown()
        {
            System.Diagnostics.Process.Start( "shutdown", "/s" );

        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        private void ShowReady()
        {
            ShowFile(" (finished)",true);
            m_eChangeCursor(Cursors.Default);
            Stopped();

            if (checkHibernate.Checked ) Hibernate();

            if ( checkShutdown.Checked ) Shutdown();

            m_ProtView.Clear();
            m_Pref.ShowProtView();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   11.11.2007
        ***************************************************************************/
        private void ShowDir(String sDir)
        {
            m_eShowActivity(Utils.LimitPath(sDir,m_iNrChars));
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.08.2006
        LAST CHANGE:   10.08.2006
        ***************************************************************************/
        void ChangeCursor(Cursor a_Curs)
        {
            if (this.InvokeRequired)
            {
                dl_ChangeCursor d = new dl_ChangeCursor(ChangeCursor);
                this.Invoke(d, new object[] { a_Curs });
            }
            else
            {
                this.Cursor = a_Curs;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       27.08.2005
        LAST CHANGE:   11.08.2006
        ***************************************************************************/
        private void ShowFile(String sFname, bool bAppend)
        {
            if (this.InvokeRequired)
            {
                dl_ShowFilename d = new dl_ShowFilename(ShowFile);
                this.Invoke(d, new object[] { sFname, bAppend });
            }
            else
            {
                String s = sFname;

                char[] ca = new char[1];
                ca[0] = '\\';
                String[] a = s.Split(ca);
                s = a[a.Length - 1];

                if (bAppend) textBoxFilename.Text += s;
                else         textBoxFilename.Text  = s;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2005
        LAST CHANGE:   11.05.2009
        ***************************************************************************/
        private void ShowABar( int iCurrFiles, int iAllFiles )
        {
            if ( m_Pref.checkSkipCount.Checked ) return;
            if ( iCurrFiles == m_iOldCurrFiles ) return; 
            if ( iAllFiles  < iCurrFiles )       return;

            textBoxCounts.Text = string.Format("{0}/{1}",iCurrFiles,iAllFiles);

            //bool bCntChanged = (m_iOldAllFiles != iAllFiles);

            m_iOldCurrFiles  = iCurrFiles;
            //m_iOldAllFiles   = iAllFiles;

            int iAProg = 0;

            if ( iAllFiles == 0 ) 
            {
                progressBarAll.Value = 0;
                //m_iOldAProg          = 0;
            }
            else
            {
                iAProg = iCurrFiles * 100 / iAllFiles;
            }

            //m_iOldAProg          = iAProg;
            progressBarAll.Value = iAProg;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.06.2008
        LAST CHANGE:   20.04.2009
        ***************************************************************************/
        private void ShowAllBar( int iCurrFiles, int iAllFiles )
        {
            if (this.InvokeRequired)
            {
                dl_ShowAllProgBar d = new dl_ShowAllProgBar(ShowAllBar);
                this.Invoke(d,new object[] { iCurrFiles, iAllFiles });
            }
            else
            {
                ShowABar( iCurrFiles, iAllFiles );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2005
        LAST CHANGE:   26.06.2008
        ***************************************************************************/
        private void ShowFBar(int iProg)
        {
            if ( iProg == m_iOldFProg ) return;
            m_iOldFProg = iProg;
            progressBarFile.Value = iProg;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.06.2008
        LAST CHANGE:   26.06.2008
        ***************************************************************************/
        private void ShowFilBar(int iProg)
        {
            if (this.InvokeRequired)
            {
                dl_ShowFilProgBar d = new dl_ShowFilProgBar(ShowFilBar);
                this.Invoke(d,new object[] { iProg });
            }
            else
            {
                ShowFBar( iProg );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.07.2008
        LAST CHANGE:   05.07.2008
        ***************************************************************************/
        private void ShowTmSpan ( TimeSpan tSpan )
        {
            textBox_Timer.Text = String.Format("{0:00}:{1:00}:{2:00}",tSpan.Hours,tSpan.Minutes,tSpan.Seconds);
            //textBox_Timer.Text = String.Format("{0:T}",tSpan);
            //textBox_Timer.Text = tSpan.ToString("hh:mm:ss");
        }

        private void ShowTimeDif ( TimeSpan tSpan )
        {
            if ( this.InvokeRequired )
            {
                dl_ShowTimeSpan d = new dl_ShowTimeSpan(ShowTimeDif);
                this.Invoke(d,new object[] { tSpan });
            }
            else
            {
                ShowTmSpan( tSpan );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       24.12.2009
        LAST CHANGE:   10.11.2014
        ***************************************************************************/
        private FavDataSet GiveCurrProps()
        {
            if( this.InvokeRequired )
            {
                dl_GetCurrProperties d = new dl_GetCurrProperties( GiveCurrProps );
                return (FavDataSet) this.Invoke( d, new object[] { } );
            }
            else
            {
                FavDataSet ds = new FavDataSet();

                ds.bCopyAll       = menuItemCopyAll.Checked;
                ds.bIgnoreR       = menuItemIgnoreR.Checked;
                ds.bEvenNewer     = menuItemEvenNewer.Checked;
                ds.bRegardAge     = menuItemFileAge.Checked;
                ds.bFoldersOnly   = checkBoxFolderOnly.Checked;
                ds.bDrive         = checkBoxDrives.Checked;
                ds.sSrc           = comboSrc.Text;
                ds.sDst           = comboDest.Text;
                ds.sDstDrive      = comboBoxDrives.Text; 
                ds.sSrcDrive      = comboSrcDrv.Text;
                ds.tSelected      = m_Selected;
            
                return ds;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        private void Form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            comboSrc .AddTextEntry();
            comboDest.AddTextEntry();

            m_Pref.ClosePrtView();

            m_eShowFilename         -= new dl_ShowFilename(this.ShowFile);
            m_eChangeCursor         -= new dl_ChangeCursor(this.ChangeCursor);
            m_eInscribeStartButton  -= new dl_InscribeStartBtn(this.InscrStartBtn);
     
            m_Backup.m_eBuReady  -= new dl_BuReady(this.ShowReady);
            m_Backup.m_eShowDir  -= new dl_ShowDir(this.ShowDir);
            m_Backup.m_eShowFile -= new dl_ShowFile(this.ShowFile);
            m_Backup.m_eShowABar -= new dl_ShowAllBar(this.ShowAllBar);
            m_Backup.m_eShowFBar -= new dl_ShowFilBar(this.ShowFilBar);
            m_Backup.m_eShowTime -= new dl_ShowTime(this.ShowTimeDif);
            m_Favs.m_eCurrProps  -= new dl_GetCurrProperties( this.GiveCurrProps );

            m_Backup.Stop();

            m_Config.OpenWrite();

            Serialize(ref m_Config);

            m_Config.Close();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       20.12.2014
        LAST CHANGE:   25.03.2016
        ***************************************************************************/
        private void Serialize(ref AppSettings a_Conf)
        {
            if ( a_Conf.IsReading )
            {
                a_Conf.DeserializeDbVersion();
                a_Conf.DeserializeDialog(this);
                menuItemIgnoreR.Checked     = a_Conf.Deserialize<bool>();
                menuItemCopyOnStart.Checked = a_Conf.Deserialize<bool>();
                menuItemCopyAll.Checked     = a_Conf.Deserialize<bool>();
                menuItemEvenNewer.Checked   = a_Conf.Deserialize<bool>();
                checkBoxDrives.Checked      = a_Conf.Deserialize<bool>();
                comboBoxDrives.Text         = a_Conf.Deserialize<string>();
                comboSrcDrv.Text            = a_Conf.Deserialize<string>();
                m_Selected                  = a_Conf.Deserialize<List<string>>();
                checkBoxFolderOnly.Checked  = a_Conf.Deserialize<bool>();

                menuItemFileAge.Checked     = m_Pref.checkRegardAge.Checked;
            }
            else
            {
                a_Conf.Serialize(DB_VERSION);
                a_Conf.SerializeDialog(this);
                a_Conf.Serialize(menuItemIgnoreR.Checked);
                a_Conf.Serialize(menuItemCopyOnStart.Checked);
                a_Conf.Serialize(menuItemCopyAll.Checked);
                a_Conf.Serialize(menuItemEvenNewer.Checked);
                a_Conf.Serialize(checkBoxDrives.Checked);
                a_Conf.Serialize(comboBoxDrives.Text);
                a_Conf.Serialize(comboSrcDrv.Text);
                a_Conf.Serialize(m_Selected);     
                a_Conf.Serialize(checkBoxFolderOnly.Checked);
            }

            comboSrc     .Serialize(ref a_Conf);  
            comboDest    .Serialize(ref a_Conf);  
            m_Pref       .Serialize(ref a_Conf);
            m_Favs       .Serialize(ref a_Conf);
            listViewFiles.Serialize(ref a_Conf);
            
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        private void Form_Load(object sender, System.EventArgs e)
        {
            m_eShowFilename         += new dl_ShowFilename(this.ShowFile);
            m_eChangeCursor         += new dl_ChangeCursor(this.ChangeCursor);
            m_eInscribeStartButton  += new dl_InscribeStartBtn(this.InscrStartBtn);
            m_eShowActivity         += new dl_ShowActivity(this.ShwActivity);

            m_Backup = new Backup();

            m_Backup.m_eBuReady  += new dl_BuReady( this.ShowReady );
            m_Backup.m_eShowDir  += new dl_ShowDir( this.ShowDir );
            m_Backup.m_eShowFile += new dl_ShowFile( this.ShowFile );
            m_Backup.m_eShowABar += new dl_ShowAllBar( this.ShowAllBar );
            m_Backup.m_eShowFBar += new dl_ShowFilBar( this.ShowFilBar );
            m_Backup.m_eShowTime += new dl_ShowTime( this.ShowTimeDif );
            m_Favs.m_eCurrProps  += new dl_GetCurrProperties( this.GiveCurrProps );

            ReadDrives(false);

            if (m_Config.OpenRead()) 
            {
                try
                {
                    Serialize( ref m_Config );

                    if(m_sArgument != "")
                    {
                        string dir = m_sArgument;
                        if(!Directory.Exists(dir))
                        {
                            dir = Directory.GetParent(dir).FullName;
                        }
                        comboSrc.Text = dir;
                    }
                }
                catch (Exception ex) 
                {
                    MessageBox.Show(ex.Message,"Error");
                }
                finally
                {
                    m_Config.Close();
                    string[] sl = comboSrc .CorrectPathByVolName();
                    string[] dl = null;
                    if ( ! checkBoxDrives.Checked ) dl = comboDest.CorrectPathByVolName();

                    if     ( sl != null ) m_Favs.ReplaceDriveLetter( sl[0], sl[1] );
                    else if( dl != null ) m_Favs.ReplaceDriveLetter( dl[0], dl[1] );
                    listViewFiles.ShowFiles(comboSrc.Text);
                    EnterSelected(m_Selected);
                }
            }

            if (menuItemCopyOnStart.Checked)
            {
                StartBtn_ClickSub();
            }

            EnableDest(checkBoxDrives.Checked); 
            ReadDrives( true );
            SyncSrcDriveBox();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.08.2006
        LAST CHANGE:   10.08.2006
        ***************************************************************************/
        private void InscrStartBtn(string sText)
        {
            if (this.InvokeRequired)
            {
                dl_InscribeStartBtn d = new dl_InscribeStartBtn(InscrStartBtn);
                this.Invoke(d, new object[] { sText });
            }
            else
            {
                StartBtn.Text = sText;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.08.2006
        LAST CHANGE:   10.08.2006
        ***************************************************************************/
        private void ShwActivity(string sText)
        {
            if (this.InvokeRequired)
            {
                dl_ShowActivity d = new dl_ShowActivity(ShwActivity);
                this.Invoke(d, new object[] { sText });
            }
            else
            {
                textBoxActivity.Text = sText;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       21.03.2007
        LAST CHANGE:   21.03.2007
        ***************************************************************************/
        private void ClosePrtView()
        {
            if (this.InvokeRequired)
            {
                dl_CloseProtView d = new dl_CloseProtView(ClosePrtView);
                this.Invoke(d, new object[]{ });
            }
            else
            {

            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   11.02.2006
        ***************************************************************************/
        private bool ReadExtFile()
        {
            m_Backup.ClearExts();

            if (menuItemCopyAll.Checked)
            {
                m_Backup.AddExt("*.*");
                return true;
            }

            string path = m_Pref.comboFileMask.Text;

            if (Utils.NoFile(path)) return false;

            StreamReader f = File.OpenText(path);
            while(f.Peek() > 0)
            {
                m_Backup.AddExt(f.ReadLine());
            }

            return true;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   03.12.2014
        ***************************************************************************/
        private void BtnBrowseSrc_Click(object sender, System.EventArgs e)
        {
            comboSrc.BrowseFolder();
            listViewFiles.ShowFiles();
            SyncSrcDriveBox();
        }

        private void BtnBrowseDst_Click(object sender, System.EventArgs e)
        {
            comboDest.BrowseFolder();
        }

        private void menuItemCopyOnStart_Click(object sender, System.EventArgs e)
        {
            if (menuItemCopyOnStart.Checked) menuItemCopyOnStart.Checked = false;
            else                             menuItemCopyOnStart.Checked = true;
        }

        private void menuItemIgnoreR_Click(object sender, System.EventArgs e)
        {
            if (menuItemIgnoreR.Checked) menuItemIgnoreR.Checked = false;
            else                         menuItemIgnoreR.Checked = true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        private void menuItemPrefer_Click(object sender, System.EventArgs e)
        {
            m_Pref.ShowDialog();
            m_Backup.m_dMaxTimeDelta = double.Parse(m_Pref.textTimeDelta.Text);
            menuItemFileAge.Checked  = m_Pref.checkRegardAge.Checked; 
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   17.01.2016
        ***************************************************************************/
        private void menuAbout_Click(object sender, System.EventArgs e)
        {
            About dlg = new About( RELEASE );
            dlg.ShowDialog();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.02.2006
        LAST CHANGE:   11.02.2006
        ***************************************************************************/
        private void comboBoxDrives_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string sSelDrv = (string)comboBoxDrives.SelectedItem;
            comboBoxDrives.Text = sSelDrv;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.02.2006
        LAST CHANGE:   08.03.2007
        ***************************************************************************/
        private void ReadDrives(bool bSrc)
        {
            string[] drv = Directory.GetLogicalDrives(); 

            if (bSrc)   comboSrcDrv   .Items.Clear();
            else        comboBoxDrives.Items.Clear();

            foreach (string d in drv)
            {
                string n;

                if (d[0] == 'A') n = "Floppy";
                else             n = Utils.GetDriveName(d);   

                if (0 == n.Length)  n = d;
                else                n = d + " (" + n + ")";

                if (bSrc)  comboSrcDrv   .Items.Add(n);
                else       comboBoxDrives.Items.Add(n);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.02.2006
        LAST CHANGE:   25.04.2006
        ***************************************************************************/
        void EnableDest(bool bDrives)
        {
            if (bDrives)
            {
                ReadDrives(false); 
                comboDest.Enabled       = false;
                BtnBrowseDst.Enabled    = false;
                comboBoxDrives.Enabled  = true;
                button_Swap.Enabled     = false;
            }
            else
            {
                comboDest.Enabled       = true;
                BtnBrowseDst.Enabled    = true;
                comboBoxDrives.Enabled  = false;
                button_Swap.Enabled     = true;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.02.2006
        LAST CHANGE:   11.02.2006
        ***************************************************************************/
        private void menuItemCopyAll_Click(object sender, System.EventArgs e)
        {
            menuItemCopyAll.Checked = ! menuItemCopyAll.Checked;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.03.2006
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        private void menuItemFileAge_Click(object sender, System.EventArgs e)
        {
           menuItemFileAge.Checked = ! menuItemFileAge.Checked;
           SetRegardFileage(menuItemFileAge.Checked);
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.05.2006
        LAST CHANGE:   07.05.2006
        ***************************************************************************/
        public void SetRegardFileage(bool a_bRegard)
        {
            m_Pref.checkRegardAge.Checked = a_bRegard;
            menuItemFileAge.Checked       = a_bRegard;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.03.2006
        LAST CHANGE:   27.02.2015
        ***************************************************************************/
        public void menuItemFavSave_Click(object sender, System.EventArgs e)
        {
            FavDataSet ds = new FavDataSet();
            
            ds.sSrc          = comboSrc .Text.Substring( 0, 1 ).ToUpper() + comboSrc .Text.Substring( 1 );
            ds.sDst          = comboDest.Text.Substring( 0, 1 ).ToUpper() + comboDest.Text.Substring( 1 );
            ds.sDstDrive     = comboBoxDrives    .Text;
            ds.sSrcDrive     = comboSrcDrv       .Text;
            ds.bDrive        = checkBoxDrives    .Checked;
            ds.bEvenNewer    = menuItemEvenNewer .Checked;
            ds.bCopyAll      = menuItemCopyAll   .Checked;
            ds.bIgnoreR      = menuItemIgnoreR   .Checked;
            ds.bRegardAge    = menuItemFileAge   .Checked;
            ds.bFoldersOnly  = checkBoxFolderOnly.Checked;

            ds.sDstDescr     = "";
            ds.sSrcDescr     = "";
            ds.tSelected     = m_Selected;

            m_Favs.Save(ds);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.04.2006
        LAST CHANGE:   09.04.2006
        ***************************************************************************/
        private void menuItemEvenNewer_Click(object sender, System.EventArgs e)
        {
            menuItemEvenNewer.Checked = ! menuItemEvenNewer.Checked;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.04.2006
        LAST CHANGE:   25.04.2006
        ***************************************************************************/
        private void button_Swap_Click(object sender, System.EventArgs e)
        {
            string hlp     = comboSrc .Text;
            comboSrc .Text = comboDest.Text;
            comboDest.Text = hlp;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.08.2006
        LAST CHANGE:   23.08.2006
        ***************************************************************************/
        private void Form_DragDrop(object sender,DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.FileDrop,false))
            {
                string[] sa = (string[])e.Data.GetData(DataFormats.FileDrop);
                string fn = sa[0];
                string dir = fn;
                if(!Directory.Exists(fn))
                {
                    dir = Directory.GetParent(fn).FullName;
                }
                comboSrc.Text = dir;
                SyncSrcDriveBox();

                if (! checkBoxFolderOnly.Checked) listViewFiles.ShowFiles(sa);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.08.2006
        LAST CHANGE:   23.08.2006
        ***************************************************************************/
        private void Form_DragEnter(object sender,DragEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            e.Effect    = DragDropEffects.Copy;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.02.2006
        LAST CHANGE:   11.02.2006
        ***************************************************************************/
        private void checkBoxDrives_CheckedChanged(object sender, System.EventArgs e)
        {
            EnableDest(checkBoxDrives.Checked);
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.11.2006
        LAST CHANGE:   25.11.2006
        ***************************************************************************/
        private void comboBoxDrives_Click(object sender,EventArgs e)
        {
            ReadDrives(false);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.02.2007
        LAST CHANGE:   10.03.2007
        ***************************************************************************/
        private void comboSrc_SelectedIndexChanged(object sender,EventArgs e)
        {
            listViewFiles.ShowFiles(comboSrc.Text);
            SyncSrcDriveBox();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.03.2007
        LAST CHANGE:   07.03.2007
        ***************************************************************************/
        public void listViewFiles_MouseDoubleClick(object sender,MouseEventArgs e)
        {
            listViewFiles.CurrDir = comboSrc.Text;
            listViewFiles.OnMouseDoubleClick(sender,e);
            comboSrc.Text = listViewFiles.CurrDir;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.03.2007
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        private void checkBoxFolderOnly_CheckedChanged(object sender,EventArgs e)
        {
            listViewFiles.Enabled       = ! checkBoxFolderOnly.Checked;
            button_SelectAll.Enabled    = ! checkBoxFolderOnly.Checked;
            button_SelectInvert.Enabled = ! checkBoxFolderOnly.Checked;

            if ( listViewFiles.Enabled )
            {
                listViewFiles.ShowFiles(comboSrc.Text);
                EnterSelected( m_Selected );
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.03.2007
        LAST CHANGE:   08.03.2007
        ***************************************************************************/
        private void comboSrcDrv_Click(object sender,EventArgs e)
        {
            ReadDrives(true);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.03.2007
        LAST CHANGE:   07.12.2007
        ***************************************************************************/
        private void comboSrcDrv_SelectedIndexChanged(object sender,EventArgs e)
        {
            string sSelDrv = (string)comboSrcDrv.SelectedItem;

            int iIdx = sSelDrv.IndexOf('\\');
            if (iIdx != -1)
            {
                sSelDrv = sSelDrv.Remove(iIdx);
            }
            else
            {
                sSelDrv.Remove(1);
            }

            if ("" == comboSrc.Text) return;

            string sPath = sSelDrv + comboSrc.Text.Remove(0,sSelDrv.Length);

            while(! Directory.Exists(sPath))
            {
                int len = sPath.Length;
                sPath = listViewFiles.Set2ParentDir(sPath);
                if (len == sPath.Length) break;
            }

            comboSrc.Text = sPath;
            listViewFiles.ShowFiles(sPath);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.03.2007
        LAST CHANGE:   08.03.2007
        ***************************************************************************/
        private void comboSrc_TextUpdate(object sender,EventArgs e)
        {
            SyncSrcDriveBox();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.03.2007
        LAST CHANGE:   17.01.2015
        ***************************************************************************/
        public void SyncSrcDriveBox(string srcDrv)
        {
            string sDrv = srcDrv;
            int    iIdx = sDrv.IndexOf(':');
            if (iIdx != -1 && iIdx < sDrv.Length - 1)
            {
                sDrv = sDrv.Remove(iIdx + 1);
            }

            if (! comboSrcDrv.Text.StartsWith(sDrv) ) 
            {
                comboSrcDrv.Text = sDrv;
                comboSrcDrv.Text = sDrv + " (" + Utils.GetDriveName(sDrv) + ")";
                comboSrc.Text    = srcDrv;
            }
        }

        public void SyncSrcDriveBox()
        {
            SyncSrcDriveBox(comboSrc.Text);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       15.03.2007
        LAST CHANGE:   15.03.2007
        ***************************************************************************/
        private void buttonParentDir_Click(object sender,EventArgs e)
        {
            comboSrc.Text = listViewFiles.Set2ParentDir(comboSrc.Text);
            listViewFiles.ShowFiles();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.11.2007
        LAST CHANGE:   24.04.2009
        ***************************************************************************/
        private void Form_Resize(object sender,EventArgs e)
        {
            int tbWitdh     = textBoxActivity.Size.Width;

            int iLetterWdth = (int)textBoxActivity.Font.Size;
 
            m_iNrChars  = tbWitdh / iLetterWdth; // * 6 / 5;
            m_iNrChars *= 11;
            m_iNrChars /= 10;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       04.07.2008
        LAST CHANGE:   04.07.2008
        ***************************************************************************/
        private void button_SelectAll_Click(object sender,EventArgs e)
        {
            foreach ( ListViewItem it in listViewFiles.Items )
            {
                it.Selected = true;
            }

            listViewFiles.Select();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       04.07.2008
        LAST CHANGE:   04.07.2008
        ***************************************************************************/
        private void button_SelectInvert_Click(object sender,EventArgs e)
        {
            foreach ( ListViewItem it in listViewFiles.Items )
            {
                it.Selected = ! it.Selected;
            }

            listViewFiles.Select();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       20.05.2009
        LAST CHANGE:   20.05.2009
        ***************************************************************************/
        private void menuItemResizeCols_Click(object sender,EventArgs e)
        {
            listViewFiles.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       24.12.2009
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        private void listViewFiles_ItemSelectionChanged( object sender, ListViewItemSelectionChangedEventArgs e )
        {
            if (m_bEnteringSelection) return;

            m_Selected.Clear();

            foreach( ListViewItem it in listViewFiles.Items )
            {
                if (it.Selected)
                {
                    m_Selected.Add(it.Text);
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       24.12.2009
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        public void EnterSelected( List<string> aSelected )
        {
            m_Selected = aSelected;
            m_bEnteringSelection = true;

            foreach( ListViewItem it in listViewFiles.Items )
            {
                foreach ( string name in aSelected )
                {
                    if( it.Text == name )
                    {
                        it.Selected = true; 
                    }
                }
            }

            m_bEnteringSelection = false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.12.2011
        LAST CHANGE:   29.12.2011
        ***************************************************************************/
        private void menuItemHibernate_Click( object sender, EventArgs e )
        {
            Hibernate();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.12.2011
        LAST CHANGE:   29.12.2011
        ***************************************************************************/
        private void menuItemShutdown_Click( object sender, EventArgs e )
        {
            Shutdown();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       17.01.2016
        LAST CHANGE:   17.01.2016
        ***************************************************************************/
        private void menuItemShowProt_Click( object sender, EventArgs e )
        {
            m_ProtView.Dirs  = GetDirs();
            m_Pref.ShowProtView();
        }
    }
}


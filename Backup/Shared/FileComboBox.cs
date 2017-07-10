using System;
using System.IO;
using System.Windows.Forms;
using NS_Utilities;
using NS_AppConfig;

namespace NS_UserCombo
{
	/// <summary>
	/// Summary description for FileComboBox.
	/// </summary>
	public class FileComboBox: UserComboBox
	{
        private bool     m_bDragDir;   // if true only directories are dropped, otherwise files
        private string   m_VolumeName;

        public string VolumeName { get { return m_VolumeName; } }

        /***************************************************************************
        SPECIFICATION: C'tors 
        CREATED:       ??.??.2004
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        public FileComboBox()
            : base()
        {
            m_bDragDir   = true;
            SubCtor();
        }

        public FileComboBox(bool bDragDir)
            : base()
		{
            m_bDragDir  = bDragDir;
            SubCtor();
		}

        private void SubCtor()
        {
            this.m_VolumeName = "";
            this.AllowDrop = true;
            this.DragDrop  += new System.Windows.Forms.DragEventHandler(this.FileCombo_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FileCombo_DragEnter);

            this.AutoCompleteSource = AutoCompleteSource.FileSystemDirectories;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       2004
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        public DialogResult BrowseFileRead()
        {
            return BrowseFileRead( "Text files (*.txt)|*.txt|All files (*.*)|*.*" );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       13.12.2012
        MODIFIED:      13.12.2012
        ***************************************************************************/
        public DialogResult BrowseFileRead( string a_sFilter )
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = a_sFilter;
            fd.InitialDirectory = Text;
            DialogResult res = fd.ShowDialog();

            if( res == DialogResult.OK )
            {
                if( !AlreadyIn( Text ) ) Items.Add( Text );
                Text = fd.FileName;
                if( !AlreadyIn( Text ) ) Items.Add( Text );
            }
            return res;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       2004
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        public DialogResult BrowseFolder()
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.ShowNewFolderButton = true;
            fd.SelectedPath = Text;
            DialogResult res = fd.ShowDialog();

            if (res == DialogResult.OK) 
            {
                AddTextEntry();
                Text = fd.SelectedPath;
                AddTextEntry();
            }

            return res;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.03.2006
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        private void FileCombo_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop,false))
            {
                string[] sa = (string[])e.Data.GetData(DataFormats.FileDrop);
                string   fn = sa[0];
                string   dir = fn;
                if (! Directory.Exists(fn) && m_bDragDir)
                {
                    dir = Directory.GetParent(fn).FullName;
                }
                ((FileComboBox)sender).Text = dir;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.03.2006
        LAST CHANGE:   11.03.2006
        ***************************************************************************/
        private void FileCombo_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            this.Cursor = Cursors.Hand;
            e.Effect    = DragDropEffects.Copy;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.04.2009
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        public new void Serialize(ref AppSettings a_Conf)
        {
            base.Serialize(ref a_Conf);

            if ( a_Conf.IsReading )
            {
                if ( a_Conf.DbVersion > a_Conf.m_OldVersion2 )
                {
                    m_VolumeName = a_Conf.Deserialize<string>( );
                }
            }
            else
            {
                m_VolumeName = Utils.GetDriveName(this.Text);   // get volume name of current path
                a_Conf.Serialize( m_VolumeName );
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.04.2009
        LAST CHANGE:   26.11.2012
        ***************************************************************************/
        public string[] CorrectPathByVolName()
        {
            if( m_VolumeName == "" )             return null;
            if ( Directory.Exists( this.Text ) ) return null;     
            if ( File.Exists(this.Text ) )       return null;

            string[] drv = Directory.GetLogicalDrives(); 

            foreach(string d in drv)
            {
                if (d[0]=='A' || d[0]=='B') continue;

                string vn = Utils.GetDriveName(d);

                if (vn == m_VolumeName)
                {
                    string[] ret = new string[2];

                    ret[0]    = Utils.GetDriveLetter(this.Text);
                    this.Text = Utils.ReplaceDriveLetter(this.Text,d);
                    ret[1]    = Utils.GetDriveLetter(this.Text);
                    return ret;
                }
            }

            return null;
        }

	}
}

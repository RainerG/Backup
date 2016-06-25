using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using NS_Utilities;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;
using System.Threading;

//using NS_ID3;

namespace NS_UserList
{
    public enum KIND_OF_TIME
    {
        KD_Write,
        KD_Access,
        KD_Creation,
        KD_FI_LastWrite,
        KD_FI_LastAccess,
        KD_FI_Creation,
        KD_IMG_Taken
    };

    /***************************************************************************
    SPECIFICATION: 
    CREATED:       07.03.2007
    LAST CHANGE:   08.05.2009
    ***************************************************************************/
    public class UserFileListView : UserListView
    {
        const float         FONTSIZE = 9.0f;

        protected string       m_CurrDir;
        private   string       m_CurrExt;
        private   Font         m_DirFont;
        private   Font         m_FileFont;
        private   KIND_OF_TIME m_KindTime;

        public string CurrDir 
        {
            get { return m_CurrDir;  }
            set { m_CurrDir = value; }
        }

        public string CurrExt
        {
            get { return m_CurrExt; }
            set { m_CurrExt = value; }
        }

        public KIND_OF_TIME KindOfTime
        {
            get { return m_KindTime;  }
            set { m_KindTime = value; }
        }

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       07.03.2007
        LAST CHANGE:   07.03.2007
        ***************************************************************************/
        public UserFileListView()
        : base()
        {
            m_DirFont   = new Font("Courier New",FONTSIZE,FontStyle.Bold,   GraphicsUnit.Point);
            m_FileFont  = new Font("Courier New",FONTSIZE,FontStyle.Regular,GraphicsUnit.Point);
            Columns.Add("Name");
            Columns.Add("Ext");
            Columns.Add("DateTime");
            View = View.Details;

            // ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);

            m_CurrExt    = "*.*";
            m_CurrDir    = "";
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       15.03.2007
        LAST CHANGE:   15.03.2007
        ***************************************************************************/
        //private new void listView_ColumnClick(object sender,ColumnClickEventArgs e)
        //{
        //    if (0!= Items.Count) Items.RemoveAt(0);

        //    ListViewItemSorter = new ListViewItemComparer(e.Column,m_bSortAscending);
        //    Sort();
        //    m_bSortAscending = !m_bSortAscending;

        //    Items.Add(FIRST_ELEM);
        //}

        /***************************************************************************
        SPECIFICATION: Has to be hooked into the DoubleClick handler of calling form.
        CREATED:       07.03.2007
        LAST CHANGE:   07.03.2007
        ***************************************************************************/
        public void OnMouseDoubleClick(object sender,MouseEventArgs e)
        {
            UserListView v = (UserListView)sender;

            ListViewHitTestInfo hti = v.HitTest(e.Location);

            string entry = hti.Item.Text;

            string tempDir = Utils.ConcatPaths(m_CurrDir,entry);

            if ( Directory.Exists( tempDir ) )
            {
                //entry = entry.Remove(0,1);
                //int ix = entry.LastIndexOf(']');
                //if ( ix != -1 )
                //{
                //    entry = entry.Remove(ix,1);
                //}
            
                m_CurrDir = tempDir;

                ShowFiles();
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       15.03.2007
        LAST CHANGE:   15.03.2007
        ***************************************************************************/
        public void Set2ParentDir()
        {
            string dir = m_CurrDir;

            int lix = m_CurrDir.LastIndexOf('\\');
            int fix = m_CurrDir.IndexOf('\\');

            if ( lix != -1 )
            {
                dir = m_CurrDir.Remove(lix);

                if ( lix >= dir.Length && lix < 4 )
                {
                    dir += '\\';
                }
            }

            m_CurrDir = dir;
        }

        public string Set2ParentDir(string sCurrPath)
        {
            m_CurrDir = sCurrPath;
            Set2ParentDir();
            return m_CurrDir;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.03.2007
        LAST CHANGE:   07.03.2007
        ***************************************************************************/
        public void ShowFiles()
        {
            ShowFiles ( (string[])null );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.03.2007
        LAST CHANGE:   07.03.2007
        ***************************************************************************/
        public void ShowFiles(string a_CurrDir)
        {
            m_CurrDir = a_CurrDir;
            ShowFiles();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.02.2007
        LAST CHANGE:   01.06.2009
        ***************************************************************************/
        public void ShowFiles( string[] a_Selected, bool a_bIncludeFiles, bool a_bIncludeDirs )
        {
            ShowFiles( a_Selected, a_bIncludeFiles, a_bIncludeDirs, false );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.02.2007
        LAST CHANGE:   01.06.2009
        ***************************************************************************/
        public void ShowFiles(string[] a_Selected, bool a_bIncludeFiles, bool a_bIncludeDirs, bool a_bSelectAll )
        {
            try
            {
                Items.Clear();

                if (a_bIncludeDirs)
                {
                    string[] dirs  = Directory.GetDirectories(m_CurrDir);
                    if ( a_bSelectAll)  EnterItems2List (dirs, dirs      , true);
                    else                EnterItems2List (dirs, a_Selected, true);
                }

                if (a_bIncludeFiles)
                {
                    string[] files = Directory.GetFiles (m_CurrDir,m_CurrExt);

                    if (a_bSelectAll)   EnterItems2List (files, files     , false);
                    else                EnterItems2List (files, a_Selected, false); 
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in retrieving files");
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.05.2009
        LAST CHANGE:   07.05.2009
        ***************************************************************************/
        public void ShowFiles(string[] a_Selected)
        {
            ShowFiles(a_Selected,true,true);
        }



        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2007
        LAST CHANGE:   30.03.2012
        ***************************************************************************/
        public List<string> GetFilesAndDirs(bool bOnlySelected)
        {
            List<ListViewItem> al  = MergeColumns(bOnlySelected,".",2);
            List<string> ret = new List<string>();

            foreach( ListViewItem li in al)
            {
                string fn = li.Text;

                // 09.09.2010
                if ( fn[fn.Length-1] == '.' ) 
                {
                    fn = fn.Substring(0,fn.Length-1);
                }

                if (fn[0] == '[')
                {
                    fn = fn.Remove(0,1);
                    fn = fn.Remove(fn.Length - 2, 2);
                }
                ret.Add(Utils.ConcatPaths(m_CurrDir,fn));
            }

            return ret;
        }

        /***************************************************************************
        SPECIFICATION:
        PARAMETERS:
                       a_Files:    list of files to be added to the list
                       a_Selected: entries which shall appear as selected
                       a_bDirs:    true, if entries are directories
        CREATED:       06.03.2007
        LAST CHANGE:   16.02.2010
        ***************************************************************************/
        private void EnterItems2List(string[] a_Files, string[] a_Selected, bool a_bDirs)
        {
            foreach(string f in a_Files)
            {
                string[] fs   = f.Split('\\');
                string   file;

                bool bSelected = false;

                if (a_Selected != null)
                {
                    foreach(string sel in a_Selected)
                    {
                        if (sel == f) 
                        {
                            bSelected = true;
                            break;
                        }
                    }
                }

                if (fs.Length != 0)
                {
                    file = fs[fs.Length - 1];
                }
                else file = f;

                string[] name = new string[1];

                if (a_bDirs) name[0] = file;
                else         name    = file.Split('.');

                ListViewItem it = new ListViewItem();

                //if (a_bDirs)  // Add brackets in case of directory
                //{
                //    name[0] = name[0].Insert(0,"[");
                //    int iLastIdx = name.Length - 1;
                //    name[iLastIdx] += "]";
                //}

                int i=0;

                while(true)
                {
                    it.Text += name[i++];
                    if ( i < name.Length - 1 )
                    {
                        it.Text += ".";
                    }
                    else break;
                };
                
                if (i < name.Length) it.SubItems.Add(name[i]);

                it.Selected = bSelected;
                if (a_bDirs)  it.Font = m_DirFont;
                else          it.Font = m_FileFont;

                DateTime dt = GetTime( f );

                if ( a_bDirs || name.Length == 1 ) it.SubItems.Add("");   // Dummy extension in case of directory or missing extension
                
                
                it.SubItems.Add(dt.ToString("yyyy-MM-dd HH:mm:ss"));

                Items.Add(it);
            }
            Select();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       02.08.2011
        LAST CHANGE:   02.08.2011
        ***************************************************************************/
        private FileInfo GetFileInfo( string sFilename )
        {
            FileInfo fi = new FileInfo( sFilename );
            return fi;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.02.2010
        LAST CHANGE:   16.02.2010
        ***************************************************************************/
        public DateTime GetTime( string sFilename )
        {
            DateTime ret = DateTime.MinValue;

            try
            {
                switch( m_KindTime )
                {
                    case KIND_OF_TIME.KD_Access: ret = File.GetLastAccessTime( sFilename ); break;
                    case KIND_OF_TIME.KD_Creation: ret = File.GetCreationTime( sFilename ); break;
                    case KIND_OF_TIME.KD_Write: ret = File.GetLastWriteTime( sFilename ); break;

                    case KIND_OF_TIME.KD_FI_LastAccess: ret = GetFileInfo( sFilename ).LastAccessTime; break;
                    case KIND_OF_TIME.KD_FI_LastWrite: ret = GetFileInfo( sFilename ).LastWriteTime; break;
                    case KIND_OF_TIME.KD_FI_Creation: ret = GetFileInfo( sFilename ).CreationTime; break;

                    case KIND_OF_TIME.KD_IMG_Taken:

                        PropertyItem[] pis = Image.FromFile( sFilename ).PropertyItems;

                        foreach( PropertyItem pi in pis )
                        {
                            if( pi.Id == 0x9003 )  // Date taken
                            {
                                byte[] val = pi.Value;
                                string s = System.Text.ASCIIEncoding.ASCII.GetString( val );
                                string v = "";
                                for( int i = 0; i < s.Length - 1; i++ )
                                {
                                    if( s[i] == ':' && i < 8 ) v += '/';
                                    else v += s[i];
                                }

                                ret = DateTime.Parse( v );
                                break;
                            }
                        }

                        Thread.Sleep( 1 );  // in order to give the GUI some time

                        break;
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show( "Wrong kind of time selected !\n Set to Time of Write !\n" + ex.Message, "Error occurred" );
                m_KindTime = KIND_OF_TIME.KD_Write;
            }

            return ret;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       25.06.2010
        LAST CHANGE:   02.08.2011
        ***************************************************************************/
        public bool SetTime ( string sFilename, DateTime tTime, KIND_OF_TIME eKind )
        {
            try 
            {
                switch( eKind )
                {
                    case KIND_OF_TIME.KD_Access:    File.SetLastAccessTime( sFilename, tTime ); break;
                    case KIND_OF_TIME.KD_Creation:  File.SetCreationTime( sFilename, tTime ); break;
                    case KIND_OF_TIME.KD_Write:     File.SetLastWriteTime( sFilename, tTime ); break;
                }
                return true;
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Error setting file time occurred" );
                return false;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.08.2011
        LAST CHANGE:   06.08.2011
        ***************************************************************************/
        public bool SetTime ( string sFilename, DateTime tTime )
        {
            return SetTime ( sFilename, tTime, m_KindTime );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.02.2010
        LAST CHANGE:   16.02.2010
        ***************************************************************************/
        public string GetFullPath( string sFnameBody )
        {
            string ret = Utils.ConcatPaths(CurrDir,sFnameBody);
            ret += CurrExt.Substring(1);

            return ret;
        }

    }  // class
} // namespace

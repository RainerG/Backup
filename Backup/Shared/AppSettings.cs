using System;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using NS_UserCombo;

/***************************************************************************
SPECIFICATION: Contains methods for storing and loading application settings
CREATED:       ??.??.2005
LAST CHANGE:   20.02.2011
***************************************************************************/
namespace NS_AppConfig
{
    #if ! XMLCONFIG
    public class DialogPosSize
    {
        public DialogPosSize()
        {
            Loc      = new Point(-1,-1);
            Siz      = new Size(-1,-1);
            WinStat  = FormWindowState.Normal;
            RestBnds = new Rectangle();
        }

        public DialogPosSize(Form a_rDlg)
        {
            Loc      = a_rDlg.Location;
            Bounds   = a_rDlg.RectangleToScreen(new Rectangle(Loc,a_rDlg.ClientSize));
            Siz      = a_rDlg.Bounds.Size;
            WinStat  = a_rDlg.WindowState; 
            RestBnds = a_rDlg.RestoreBounds;
        }

        public Point           Loc;
        public Size            Siz;
        public FormWindowState WinStat;
        //public Point           RestBndsLc;
        //public Size            RestBndsSz;
        public Rectangle       RestBnds;
        public Rectangle       Bounds;

        public bool IsInit()
        {
            if (-1 != Loc.X)            return false;
            if (-1 != Loc.Y)            return false;
            if (-1 != Siz.Width)        return false;
            if (-1 != Siz.Height)       return false;
            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.04.2006
        LAST CHANGE:   20.02.2011
        ***************************************************************************/
        public void Read(Form a_Dlg)
        {
            Loc        = a_Dlg.Location;
            Bounds     = a_Dlg.RectangleToScreen(new Rectangle(Loc,a_Dlg.ClientSize));       
            Siz        = Bounds.Size;
            WinStat    = a_Dlg.WindowState; 
            RestBnds   = a_Dlg.RestoreBounds;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.04.2006
        LAST CHANGE:   20.02.2011
        ***************************************************************************/
        public void Write(Form a_Dlg)
        {
            a_Dlg.Location    = Loc;
            a_Dlg.Size        = Siz;       // 20.02.2011
            a_Dlg.WindowState = WinStat;
            //a_Dlg.RestoreBounds = rb;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.01.2009
        LAST CHANGE:   18.01.2009
        ***************************************************************************/
        public void WriteLoc( Form a_Dlg )
        {
            a_Dlg.Location = Loc;
        }
    }

    /***************************************************************************
    SPECIFICATION: AppSettings class definition
    CREATED:       2005
    LAST CHANGE:   18.03.2013
    ***************************************************************************/
    public class AppSettings
    {
        const int MAX_COMBO_ENTRIES = 500;

        private int m_iReadDbVersion;
        private int m_iDbVersion;

        public int  DbVersion 
        {
            get { return m_iReadDbVersion; } 
        }

        public int CurrDbVersion
        {
            set { m_iDbVersion = value; }
            get { return m_iDbVersion;  } 
        }

        private bool m_bReading;
        public bool IsReading
        {
            get { return m_bReading;  }
            set { m_bReading = value; }
        }
        public bool IsWriting
        {
            get { return ! m_bReading;  }
            set { m_bReading = ! value; }
        }

        public int m_OldVersion1;   // needed for ReadDlbLocation
        public int m_OldVersion2;   // needed for FileComboBox in Backup

        protected string   m_FileName;
        private FileStream m_Stream;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       2005
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        public AppSettings(string a_sFileName,int a_iDbVersion)
        {
            //
            // TODO: Add constructor logic here
            //
            m_iDbVersion = a_iDbVersion;
            m_FileName   = a_sFileName;
            m_Stream     = null;

            m_OldVersion1 = a_iDbVersion;
            m_OldVersion2 = 0; 
        }


        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       29.04.2006
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        public AppSettings(string a_sFileName)
        {
            //
            // TODO: Add constructor logic here
            //
            m_iDbVersion = 0;
            m_FileName   = a_sFileName;
            m_Stream     = null;

            m_OldVersion1 = 0;
            m_OldVersion2 = 0;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   04.05.2006
        ***************************************************************************/
        public bool OpenRead()
        {
            if (! File.Exists(m_FileName)) return false;
            m_Stream = File.OpenRead(m_FileName);
            m_bReading = true;
            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.04.2013
        LAST CHANGE:   26.04.2013
        ***************************************************************************/
        public bool DeleteFile()
        {
            if ( File.Exists( m_FileName ) )
            {
                Close();
                File.Delete( m_FileName );
                return true;
            }
            return false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   13.02.2007
        ***************************************************************************/
        public void OpenWrite()
        {
            m_bReading = false;
            try
            {
                m_Stream = File.Create(m_FileName);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in writing " + m_FileName);
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   ??.??.2004
        ***************************************************************************/
        public void Close()
        {
            m_Stream.Close();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   13.02.2007
        ***************************************************************************/
        public void Serialize(object iObj)
        {
            try
            {
                BinaryFormatter bf  = new BinaryFormatter();
                bf.Serialize(m_Stream,iObj);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in serialization of " + iObj.ToString());
                throw e;
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        public object Deserialize()
        {
            return Deserialize<object>();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.01.2014
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        public T Deserialize<T>()
        {
            T ret = default(T);

            try
            {
                BinaryFormatter bf  = new BinaryFormatter();
                ret = (T)bf.Deserialize(m_Stream);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in deserialization");
                throw e;
            }

            return ret;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   13.02.2007
        ***************************************************************************/
        public void SerializeComboBox(ComboBox iComboBox)
        {
            try
            {
                BinaryFormatter bf  = new BinaryFormatter();

                bf.Serialize(m_Stream,(string)iComboBox.Text);
                int cnt = iComboBox.Items.Count;

                if (cnt > MAX_COMBO_ENTRIES) cnt = MAX_COMBO_ENTRIES;

                bf.Serialize(m_Stream,cnt);

                IEnumerator en = iComboBox.Items.GetEnumerator();
                en.Reset();
                while(en.MoveNext())
                {
                    string itm = (string)en.Current;
                    bf.Serialize(m_Stream,itm);
                    if (--cnt == 0) break;
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in serialization of " + iComboBox.ToString());
                throw e;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   27.02.2007
        ***************************************************************************/
        public bool DeserializeComboBox(ComboBox oComboBox)
        {
            if (! File.Exists(m_FileName)) return false;

            try
            {
                oComboBox.Items.Clear();  // erase it before

                BinaryFormatter bf  = new BinaryFormatter();
                oComboBox.Text      = (string)bf.Deserialize(m_Stream);
                int cnt             = (int)   bf.Deserialize(m_Stream);

                for (int i=0; i<cnt; i++)
                {
                    string itm = (string)bf.Deserialize(m_Stream);
                    oComboBox.Items.Add((string)itm);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in deserialization of combo box");

                throw e;
            }

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.06.2007
        LAST CHANGE:   23.06.2007
        ***************************************************************************/
        public void SerializeCheckedListBox(CheckedListBox iChListBox)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize( m_Stream, iChListBox.Items.Count );

                //for ( int i=0; i<cnt; i++ )
                //{
                //    bf.Serialize(m_Stream, iChListBox.Items[i].Text);
                //    bf.Serialize(m_Stream, iChListBox.Items[i].checked);
                //}

                foreach ( Object itm in iChListBox.Items )
                {
                    bf.Serialize(m_Stream, itm);
                }

                bf.Serialize( m_Stream, iChListBox.CheckedIndices.Count );

                foreach ( int chked in iChListBox.CheckedIndices )
                {
                    bf.Serialize( m_Stream, chked );
                }
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message,"Error in serialization of " + iChListBox.ToString());
                throw e;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.06.2007
        LAST CHANGE:   23.06.2007
        ***************************************************************************/
        public bool DeserializeCheckedListBox(CheckedListBox oChListBox)
        {
            if ( !File.Exists(m_FileName) ) return false;

            try
            {
                oChListBox.Items.Clear();  // erase it before

                BinaryFormatter bf = new BinaryFormatter();

                int cnt = (int)bf.Deserialize(m_Stream);  // number of entries

                for (int i=0; i<cnt; i++)
                {
                    oChListBox.Items.Add(bf.Deserialize(m_Stream));
                }

                cnt = (int) bf.Deserialize(m_Stream);  // number of checked indices

                for (int i=0; i<cnt; i++)
                {
                    int idx = (int)bf.Deserialize(m_Stream);
                    oChListBox.SetItemChecked(idx,true);
                }
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message,"Error in deserialization of" + oChListBox.ToString());

                throw e;
            }

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   13.02.2007
        ***************************************************************************/
        public bool WriteDlgLocation(DialogPosSize iDLoc)
        {
            try
            {
                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(m_Stream,iDLoc.Loc);
                bf.Serialize(m_Stream,iDLoc.Siz);
                bf.Serialize(m_Stream,iDLoc.WinStat);
                bf.Serialize(m_Stream,iDLoc.RestBnds);
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message,"Error of serialization of " + iDLoc.ToString());
                throw e;
            }

            return true;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   13.02.2007
        ***************************************************************************/
        public bool ReadDlgLocation(DialogPosSize oDLoc)
        {
            if (! File.Exists(m_FileName)) return false;

            try
            {
                BinaryFormatter bf  = new BinaryFormatter();
                oDLoc.Loc           = (Point)          bf.Deserialize(m_Stream);
                oDLoc.Siz           = (Size)           bf.Deserialize(m_Stream);

                if ( m_OldVersion1 < m_iReadDbVersion )
                {
                    oDLoc.WinStat   = (FormWindowState)bf.Deserialize(m_Stream);
                    oDLoc.RestBnds  = (Rectangle)      bf.Deserialize(m_Stream);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in deserialization of dialog");
                throw e;
            }

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.04.2006
        LAST CHANGE:   29.04.2006
        ***************************************************************************/
        public bool SerializeDialog(Form a_rDlg)
        {
           DialogPosSize dps = new DialogPosSize(a_rDlg); 

           return WriteDlgLocation(dps);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.04.2006
        LAST CHANGE:   13.02.2009
        ***************************************************************************/
        public bool DeserializeDialog(Form a_rDlg)
        {
            DialogPosSize dps = new DialogPosSize( a_rDlg ); 

            bool ret = ReadDlgLocation(dps);

            a_rDlg.Location      = dps.Loc;
            a_rDlg.Size          = dps.Siz;
            a_rDlg.WindowState   = dps.WinStat;

            if ( dps.WinStat == FormWindowState.Minimized )
            {
                a_rDlg.Location = dps.RestBnds.Location;
                a_rDlg.Size     = dps.RestBnds.Size;
            }
            
            return ret;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       29.04.2006
        LAST CHANGE:   18.03.2013
        ***************************************************************************/
        public void DeserializeDbVersion()
        {
            try
            {
                m_iDbVersion = m_iReadDbVersion = (int)Deserialize();
                     
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"Error in deserialization of data base version");
                throw e;
            }
        }
    } // class
    #endif // ! XMLCONFIG
} // namespace

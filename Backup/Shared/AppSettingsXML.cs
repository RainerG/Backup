using System;
using System.Xml;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections;
using System.Threading;
using System.Text;
//using System.Runtime.Serialization.Formatters.Binary;
using NS_UserCombo;

/***************************************************************************
SPECIFICATION: Contains methods for storing and loading application settings
CREATED:       28.01.2014
LAST CHANGE:   29.01.2014
***************************************************************************/
namespace NS_AppConfig
{
    #if XMLCONFIG
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
    CREATED:       28.01.2014
    LAST CHANGE:   28.01.2014
    ***************************************************************************/
    public class AppSettings
    {
        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        const int MAX_COMBO_ENTRIES = 500;

        private int m_iReadDbVersion;
        private int m_iDbVersion;
        public  int m_OldVersion1;   // needed for ReadDlbLocation
        public  int m_OldVersion2;   // needed for FileComboBox in Backup

        protected   string          m_sFileName;
        private     TextReader      m_tReader;
        private     TextWriter      m_tWriter;
        private     Encoding        m_tEncoding; 

        private     XmlRootAttribute  m_tRootAttr;
        private     MemoryStream      m_tMemory;

        /***************************************************************************
        SPECIFICATION: Accessors
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
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
        public  bool IsReading
        {
            get { return m_bReading;  }
            set { m_bReading = value; }
        }
        public bool IsWriting
        {
            get { return ! m_bReading;  }
            set { m_bReading = ! value; }
        }


        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public AppSettings( string a_sFileName,int a_iDbVersion )
        : this( a_sFileName )
        {
            m_iDbVersion  = a_iDbVersion;
            m_OldVersion1 = a_iDbVersion;
        }


        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       28.01.2014
        LAST CHANGE:   29.01.2014
        ***************************************************************************/
        public AppSettings(string a_sFileName)
        {
            m_iDbVersion  = 0;
            m_sFileName   = a_sFileName;

            object ob = new object();

            m_tReader     = null;
            m_tWriter     = null;
            m_OldVersion1 = 0;
            m_OldVersion2 = 0;

            m_tEncoding   = new UTF8Encoding();
            m_tRootAttr   = new XmlRootAttribute();

            m_tRootAttr.ElementName = "AppConfig"; 
            m_tRootAttr.IsNullable  = false;

            m_tMemory = new MemoryStream();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool DeleteFile()
        {
            if ( File.Exists( m_sFileName ) )
            {
                Close();
                File.Delete( m_sFileName );
                return true;
            }
            return false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public void OpenWrite()
        {
            m_bReading = false;
            try
            {
                m_tWriter = new StreamWriter( m_sFileName, true, m_tEncoding );
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in writing " + m_sFileName);
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool OpenRead()
        {
            if (! File.Exists(m_sFileName)) return false;

            m_tReader    = new StreamReader(m_sFileName, m_tEncoding );

            m_bReading = true;

            return true;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   ??.??.2004
        ***************************************************************************/
        public void Close()
        {
            if (m_tWriter != null)  
            {
                //XmlSerializer tSerlzr = new XmlSerializer( typeof(string) );
                //XmlSerializer tSerlzrI = new XmlSerializer( typeof(int) );

                //TextWriter sw = new StreamWriter(m_sFileName);

                //tSerlzr.Serialize( sw, "hallo" );
                //tSerlzr.Serialize( sw, "ich" );
                //tSerlzrI.Serialize( sw, 12 );
                //tSerlzr.Serialize( sw, "auch" );

                //byte[] mem = m_tMemory.ToArray();
                //char[] ca = new char[mem.Length];
                //mem.CopyTo(ca,0);
                //string s = new string(ca);

                //tSerlzr.Serialize( sw, s );
                //sw.Close();

                m_tWriter.Close();
            }
            if (m_tReader != null)  m_tReader.Close();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public void Serialize(object iObj)
        {
            try
            {
                XmlSerializer tSerializer = new XmlSerializer( iObj.GetType() ); //, m_tRootAttr );
                tSerializer.Serialize( m_tWriter, iObj );
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in serialization of " + iObj.ToString());
                throw e;
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public T Deserialize<T> ( )
        {
            T ret = default(T);

            try
            {
                XmlSerializer tSerializer = new XmlSerializer( typeof(T) ); //, m_tRootAttr );

                ret = (T)tSerializer.Deserialize(m_tReader);
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public void SerializeComboBox(ComboBox iComboBox)
        {
            try
            {
                XmlSerializer tSerializeInt = new XmlSerializer( iComboBox.Items.Count.GetType() ); 
                XmlSerializer tSerializeStr = new XmlSerializer( iComboBox.Text.GetType() ); 

                tSerializeStr.Serialize(m_tWriter, iComboBox.Text);

                int cnt = iComboBox.Items.Count;

                if (cnt > MAX_COMBO_ENTRIES) cnt = MAX_COMBO_ENTRIES;

                tSerializeInt.Serialize(m_tWriter,cnt);

                IEnumerator en = iComboBox.Items.GetEnumerator();
                en.Reset();
                while(en.MoveNext())
                {
                    string itm = (string)en.Current;
                    tSerializeStr.Serialize(m_tWriter,itm);
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool DeserializeComboBox(ComboBox oComboBox)
        {
            if (! File.Exists(m_sFileName)) return false;

            try
            {
                Type iType = oComboBox.Items.Count.GetType();
                Type sType = oComboBox.Text.GetType();

                XmlSerializer tSerializeInt = new XmlSerializer( iType ); 
                XmlSerializer tSerializeStr = new XmlSerializer( sType ); 

                oComboBox.Items.Clear();  // erase it before

                oComboBox.Text      = (string)tSerializeStr.Deserialize(m_tReader);
                int cnt             = (int)   tSerializeInt.Deserialize(m_tReader);

                for (int i=0; i<cnt; i++)
                {
                    string itm = (string)tSerializeStr.Deserialize(m_tReader);
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public void SerializeCheckedListBox(CheckedListBox iChListBox)
        {
            try
            {
                Type iType = iChListBox.Items.Count.GetType();
                Type oType = iChListBox.Items.GetType();

                XmlSerializer tSerializeInt = new XmlSerializer( iType ); 
                XmlSerializer oSerializeStr = new XmlSerializer( oType ); 

                tSerializeInt.Serialize( m_tWriter, iChListBox.Items.Count );

                foreach ( Object itm in iChListBox.Items )
                {
                    oSerializeStr.Serialize(m_tWriter, itm);
                }

                tSerializeInt.Serialize( m_tWriter, iChListBox.CheckedIndices.Count );

                foreach ( int chked in iChListBox.CheckedIndices )
                {
                    tSerializeInt.Serialize( m_tWriter, chked );
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
            if ( !File.Exists(m_sFileName) ) return false;

            try
            {
                Type iType = oChListBox.Items.Count.GetType();
                Type oType = oChListBox.Items.GetType();

                XmlSerializer tSerializeInt = new XmlSerializer( iType ); 
                XmlSerializer oSerializeStr = new XmlSerializer( oType ); 

                oChListBox.Items.Clear();  // erase it before

                int cnt = (int)tSerializeInt.Deserialize(m_tReader);  // number of entries

                for (int i=0; i<cnt; i++)
                {
                    oChListBox.Items.Add(oSerializeStr.Deserialize(m_tReader));
                }

                cnt = (int) tSerializeInt.Deserialize(m_tReader);  // number of checked indices

                for (int i=0; i<cnt; i++)
                {
                    int idx = (int)tSerializeInt.Deserialize(m_tReader);
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool WriteDlgLocation(DialogPosSize iDLoc)
        {
            try
            {
                XmlSerializer tSerializer = new XmlSerializer( iDLoc.GetType() );

                tSerializer.Serialize( m_tWriter, iDLoc );
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool ReadDlgLocation(DialogPosSize oDLoc)
        {
            if (! File.Exists(m_sFileName)) return false;

            try
            {
                oDLoc = Deserialize<DialogPosSize>( );
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
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
        ***************************************************************************/
        public bool SerializeDialog(Form a_rDlg)
        {
           DialogPosSize dps = new DialogPosSize( a_rDlg ); 

           return WriteDlgLocation(dps);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.01.2014
        LAST CHANGE:   28.01.2014
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
        LAST CHANGE:   29.01.2014
        ***************************************************************************/
        public void DeserializeDbVersion()
        {
            try
            {
                m_iDbVersion = m_iReadDbVersion = Deserialize<int>( );
                     
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,"Error in deserialization of data base version");
                throw e;
            }
        }

    }  // class
    #endif // XMLCONFIG
}  // namespace

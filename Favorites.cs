using System;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Collections.Generic;

using NS_AppConfig;
using NS_Utilities;
using NS_ReplDL;
using NS_FavDescr;

namespace NS_Backup
{
    public delegate FavDataSet dl_GetCurrProperties( );

    /***************************************************************************
    SPECIFICATION: 
    CREATED:       ?
    LAST CHANGE:   12.02.2023
    ***************************************************************************/
    public class FavDataSet
    {
        public bool   bDrive;
        public bool   bEvenNewer;
        public bool   bCopyAll;
        public bool   bIgnoreR;     // added 04.05.2006
        public bool   bRegardAge;   // "
        public bool   bFoldersOnly; // added 22.03.2007
        public string sSrc;
        public string sDst;
        public string sSrcDescr;  // added 21.12.2009
        public string sDstDescr;  // " 
        public string sDstDrive;  // added 10.11.2014
        public string sSrcDrive;  // added 09.12.2014
        public string sFileMsks;  // 12.02.2023
        public List<string> tSelected;

        public FavDataSet()
        {
            tSelected   = new List<string>();
            sSrc        = "";
            sDst        = "";
            sSrcDescr   = "";
            sDstDescr   = "";
            sDstDrive   = "";
            sSrcDrive   = "";
            sFileMsks   = "";
            bDrive      = false;
        }

        public void LoadSelected( List<string> a_Selctd )
        {
            tSelected.Clear();
            tSelected.AddRange( a_Selctd );
        }
    }

	/// <summary>
	/// Summary description for Favorites.
	/// </summary>
	public class Favorites
	{
        public event dl_GetCurrProperties m_eCurrProps;  // 24.12.2009

        private List<FavDataSet>     m_DataList;
        private MenuItem             m_Menu;
        private MainDlg              m_Main;
        private Font                 m_Font;
        private bool                 m_bAscending;
        private int                  m_iStartMenuIdx;
        private ReplaceDL            m_ReplDL;
        private FavDescriptionEditor m_DescrEdit;

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2006
        LAST CHANGE:   22.12.2009
        ***************************************************************************/
        public Favorites(ref MenuItem iMenuItem, MainDlg iMain)
		{
            m_DataList  = new List<FavDataSet>();
            m_ReplDL    = new ReplaceDL();
            m_DescrEdit = new FavDescriptionEditor();

            m_Menu     = iMenuItem;
            m_Main     = iMain;

            m_bAscending   = true;

            m_Font = new Font(FontFamily.GenericMonospace, 10, FontStyle.Regular, GraphicsUnit.Point);
		}

        /***************************************************************************
        SPECIFICATION: Comparer object needed for sorting MenuItems
        CREATED:       05.05.2006
        LAST CHANGE:   17.05.2021
        ***************************************************************************/
        private class MenuItemComparer : IComparer<FavDataSet>
        {
            bool m_bLeft;
            bool m_bAscendSort;

            public MenuItemComparer(bool a_bLeft, bool a_bAscendSort)
            {
                m_bLeft       = a_bLeft;
                m_bAscendSort = a_bAscendSort;
            }

            public int Compare(FavDataSet mi1, FavDataSet mi2)
            {
                string s1;
                string s2;

                if (m_bLeft)
                {
                    if (mi1.sSrcDescr == "")  s1 = mi1.sSrc      + mi1.sDst     ;
                    else                      s1 = mi1.sSrcDescr + mi1.sDstDescr;

                    if( mi2.sSrcDescr == "" ) s2 = mi2.sSrc      + mi2.sDst     ;
                    else                      s2 = mi2.sSrcDescr + mi2.sDstDescr;
                }
                else
                {
                    if( mi1.sDstDescr == "" ) s1 = mi1.sDst      + mi1.sSrc     ;
                    else                      s1 = mi1.sDstDescr + mi1.sSrcDescr;

                    if( mi2.sDstDescr == "" ) s2 = mi2.sDst      + mi2.sSrc     ;
                    else                      s2 = mi2.sDstDescr + mi2.sSrcDescr;
                }

                if (m_bAscendSort) return s1.CompareTo(s2);
                else               return s2.CompareTo(s1);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2006
        LAST CHANGE:   14.03.2006
        ***************************************************************************/
        public void Delete(FavDataSet ds)
        {
            m_DataList.Remove(ds);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2006
        LAST CHANGE:   26.04.2006
        ***************************************************************************/
        public void Save(FavDataSet iDataSet)
        {
            bool add = true;

            foreach(FavDataSet ds in m_DataList)
            {
                if (ds.Equals(iDataSet)) 
                {
                    add = false;
                    break;
                }
            }

            if (add) m_DataList.Add(iDataSet);
            UpdateMenu(ref m_Menu);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2006
        LAST CHANGE:   22.03.2006
        ***************************************************************************/
        public List<FavDataSet> GetDataList()
        {
            return m_DataList;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.03.2006
        LAST CHANGE:   10.10.2016
        ***************************************************************************/
        public void Serialize(ref AppSettings iConfig)
        {
            if( iConfig.IsReading )
            {
                m_DataList.Clear();

                int cnt = (int)iConfig.Deserialize();

                for( int i = 0; i < cnt; i++ )
                {
                    FavDataSet ds = new FavDataSet();

                    ds.sSrc         = (String)iConfig.Deserialize();
                    ds.sDst         = (String)iConfig.Deserialize();
                    ds.bDrive       = (bool)iConfig.Deserialize();
                    ds.bEvenNewer   = (bool)iConfig.Deserialize();
                    ds.bCopyAll     = (bool)iConfig.Deserialize();
                    ds.bIgnoreR     = (bool)iConfig.Deserialize();
                    ds.bRegardAge   = (bool)iConfig.Deserialize();
                    ds.bFoldersOnly = (bool)iConfig.Deserialize();
                    ds.sSrcDescr    = iConfig.Deserialize<String>();
                    ds.sDstDescr    = iConfig.Deserialize<String>();
                    ds.tSelected    = (List<string>)iConfig.Deserialize();
                    ds.sDstDrive    = iConfig.Deserialize<string>();
                    ds.sSrcDrive    = iConfig.Deserialize<string>();
                    if ( iConfig.DbVersion > 310 ) ds.sFileMsks = iConfig.Deserialize<string>();
                    m_DataList.Add( ds );
                }

                UpdateMenu( ref m_Menu );
            }
            else
            {
                int cnt = m_DataList.Count;

                iConfig.Serialize( cnt );

                foreach( FavDataSet ds in m_DataList )
                {
                    iConfig.Serialize( ds.sSrc );
                    iConfig.Serialize( ds.sDst );
                    iConfig.Serialize( ds.bDrive );
                    iConfig.Serialize( ds.bEvenNewer );
                    iConfig.Serialize( ds.bCopyAll );
                    iConfig.Serialize( ds.bIgnoreR );
                    iConfig.Serialize( ds.bRegardAge );
                    iConfig.Serialize( ds.bFoldersOnly );
                    iConfig.Serialize( ds.sSrcDescr );
                    iConfig.Serialize( ds.sDstDescr );
                    iConfig.Serialize( ds.tSelected );
                    iConfig.Serialize( ds.sDstDrive );    
                    iConfig.Serialize( ds.sSrcDrive );    
                    iConfig.Serialize( ds.sFileMsks );
                }
            }

            m_DescrEdit.Serialize( ref iConfig );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2006
        LAST CHANGE:   22.03.2006
        ***************************************************************************/
        public void UpdateMenu(ref MenuItem iMenu)
        {
            const int MAXLEN = 50;

            Menu.MenuItemCollection mc = iMenu.MenuItems;

            mc.Clear();

            mc.Add("&Save");
            mc.Add("Sort &left");
            mc.Add("Sort &right");
            mc.Add("Replace &drive letter");
            mc.Add("-");

            m_iStartMenuIdx = mc.Count;

            mc[0].Click += new EventHandler(m_Main.menuItemFavSave_Click);
            mc[1].Click += new EventHandler(menuItem_FavSortLeft_Click);
            mc[2].Click += new EventHandler(menuItem_FavSortRight_Click);
            mc[3].Click += new EventHandler(menuItem_FavReplDL_Click);

            List<FavDataSet> tFavs = GetDataList();

            int iSMaxLen = 0;
            int iDMaxLen = 0;

            // calculate string widths
            foreach (FavDataSet s in tFavs)
            {
                string src,dst;
                
                if (s.sSrcDescr == "") src = Utils.LimitPath(s.sSrc,MAXLEN);
                else                   src = s.sSrcDescr;

                if (s.sDstDescr == "") dst = Utils.LimitPath(s.sDst,MAXLEN);
                else                   dst = s.sDstDescr;

                if (iSMaxLen < src.Length) iSMaxLen = src.Length;
                if (iDMaxLen < dst.Length) iDMaxLen = dst.Length;
            }

            // create format template
            string sFormat = "{0}{1}{2}{3}{4} | {5,-" + iSMaxLen.ToString() + "} | {6,-" + iDMaxLen.ToString() + "}";

            foreach (FavDataSet s in tFavs)
            {
                string line = string.Format( sFormat,
                                             s.bCopyAll   ?   'C' : '-',
                                             s.bEvenNewer ?   'N' : '-',
                                             s.bIgnoreR   ?   'R' : '-',
                                             s.bRegardAge ?   'A' : '-',
                                             s.bFoldersOnly ? 'F' : '-',
                                             s.sSrcDescr == "" ? Utils.LimitPath(s.sSrc,MAXLEN) : s.sSrcDescr,
                                             s.sDstDescr == "" ? Utils.LimitPath(s.sDst,MAXLEN) : s.sDstDescr);
                
                mc.Add(line);
                MenuItem mi = mc[mc.Count-1];
                
                Menu.MenuItemCollection mcs = new Menu.MenuItemCollection(mi);

                mcs.Add("&Select");
                mcs.Add("&Delete");
                mcs.Add("&Replace");
                mcs.Add("D&escription");

                mi.DrawItem    += new DrawItemEventHandler   (menuItem_DrawItem);
                mi.MeasureItem += new MeasureItemEventHandler(menuItem_MeasureItem);
                mi.OwnerDraw = true;

                mi.MenuItems[0].Click  += new EventHandler(OnSelect);
                mi.MenuItems[1].Click  += new EventHandler(OnDelete);
                mi.MenuItems[2].Click  += new EventHandler(OnReplace);
                mi.MenuItems[3].Click  += new EventHandler(OnDescription);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.05.2006
        LAST CHANGE:   05.05.2006
        ***************************************************************************/
        private void SortFavMenu(bool a_bLeft, bool a_bAscending)
        {
            m_DataList.Sort(new MenuItemComparer(a_bLeft,a_bAscending));
            m_bAscending = ! a_bAscending;
            UpdateMenu(ref m_Menu);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.05.2006
        LAST CHANGE:   05.05.2006
        ***************************************************************************/
        private void menuItem_FavSortLeft_Click(object sender, System.EventArgs e)
        {
            SortFavMenu(true, m_bAscending);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.05.2006
        LAST CHANGE:   05.05.2006
        ***************************************************************************/
        private void menuItem_FavSortRight_Click(object sender, System.EventArgs e)
        {
            SortFavMenu(false, m_bAscending);
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.04.2009
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        public void ReplaceDriveLetter(string sBefore, string sAfter)
        {
            for( int i = 0; i < m_DataList.Count; i++ )
            {
                FavDataSet d = m_DataList[i];

                d.sSrc = d.sSrc.Replace( sBefore, sAfter );
                d.sDst = d.sDst.Replace( sBefore, sAfter );

                d.sSrc = d.sSrc.Replace( sBefore.ToLower(), sAfter );
                d.sDst = d.sDst.Replace( sBefore.ToLower(), sAfter );

                m_DataList[i] = d;
            }

            UpdateMenu( ref m_Menu );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.06.2007
        LAST CHANGE:   23.06.2007
        ***************************************************************************/
        private void menuItem_FavReplDL_Click(object sender, System.EventArgs e)
        {
            if (DialogResult.OK == m_ReplDL.ShowDialog())
            {
                ReplaceDriveLetter(m_ReplDL.sBefore, m_ReplDL.sAfter);
            }
        }
        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.04.2006
        LAST CHANGE:   01.04.2006
        ***************************************************************************/
        private void menuItem_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) 
        { 
            SolidBrush b = new SolidBrush( Color.DarkBlue ); //e.ForeColor); 

            //e.BackColor = SystemColors.Window;
            //e.DrawBackground( ); //Draw the menu item background 

            e.Graphics.DrawString(((MenuItem)sender).Text,m_Font,b,e.Bounds.Left+8,e.Bounds.Top,StringFormat.GenericTypographic); 

            b.Dispose(); 
        } 
  

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       01.04.2006
        LAST CHANGE:   01.04.2006
        ***************************************************************************/
        private void menuItem_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) 
        { 
            System.Drawing.SizeF s = e.Graphics.MeasureString(((MenuItem)sender).Text,m_Font,1024,StringFormat.GenericTypographic); 

            s.Width+=SystemInformation.MenuCheckSize.Width; // for the checkmark if any 

            s.Width+=30; // for the menu glyph if any 

            e.ItemHeight=(int)s.Height + 3; 

            e.ItemWidth=(int)s.Width; 

        } 



        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.03.2006
        LAST CHANGE:   12.02.2023
        ***************************************************************************/
        void OnSelect(Object sender, EventArgs e)
        {
            MenuItem mi   = (MenuItem)sender;
            MenuItem pm   = (MenuItem)mi.Parent;

            int ind = pm.Index - m_iStartMenuIdx;

            FavDataSet ds = (FavDataSet)m_DataList[ind];

            m_Main.checkBoxDrives    .Checked  = ds.bDrive;
            m_Main.menuItemEvenNewer .Checked  = ds.bEvenNewer;
            m_Main.menuItemCopyAll   .Checked  = ds.bCopyAll;
            m_Main.menuItemIgnoreR   .Checked  = ds.bIgnoreR;
            m_Main.checkBoxFolderOnly.Checked  = ds.bFoldersOnly;
            m_Main.comboDest.Text              = ds.sDst;
            m_Main.comboSrc .Text              = ds.sSrc;

            string[] pth = m_Main.comboSrc.CorrectPathByVolName(ds.sSrcDrive);

            string drvn = Utils.GetDriveByVolName(ds.sDstDrive);
            if ( drvn != null )
            {
                string drvo = Utils.GetDriveLetter   (ds.sDstDrive);

                m_Main.comboBoxDrives.Text = ds.sDstDrive.Replace(drvo,drvn);
            }
            else
            {
                m_Main.comboBoxDrives.Text = ds.sDstDrive;
            }
            
            // exchange drive letters
            if( pth != null && pth.Length > 1 )
            {
                if (ds.sSrc.Contains(pth[0])) ds.sSrc = ds.sSrc.Replace(pth[0],pth[1]);
            }

            m_Main.SyncSrcDriveBox(ds.sSrc);

            m_Main.SetRegardFileage( ds.bRegardAge );
            m_Main.EnterSelected   ( ds.tSelected  );
            if ( ds.sFileMsks != "") m_Main.FileExts = ds.sFileMsks;  
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.03.2006
        LAST CHANGE:   06.05.2021
        ***************************************************************************/
        void OnDelete(Object sender, EventArgs e)
        {
            MenuItem mi   = (MenuItem)sender;
            MenuItem pm   = (MenuItem)mi.Parent;
            MenuItem mm   = (MenuItem)pm.Parent;

            int ind = pm.Index - m_iStartMenuIdx;
            FavDataSet mbr = m_DataList[ind];

            if (DialogResult.Yes == MessageBox.Show("Really Delete '" + mbr.sSrc + " - " + mbr.sDst + "' ?", "Warning" , MessageBoxButtons.YesNo) ) 
            {
                m_DataList  .RemoveAt(ind);
                mm.MenuItems.Remove(pm);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       06.05.2006
        LAST CHANGE:   17.02.2023
        ***************************************************************************/
        const string REPL_FORMAT = "{0,-60} => {1}\n";

        void OnReplace(Object sender, EventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            MenuItem pm = (MenuItem)mi.Parent;
            MenuItem mm = (MenuItem)pm.Parent;

            int ind = pm.Index - m_iStartMenuIdx;
            FavDataSet mbr = m_DataList[ind];

            FavDataSet ds;

            if( null != m_eCurrProps )
            {
                ds = m_eCurrProps();
            }
            else
            {
                MessageBox.Show("No properties assigned");
                return;
            }

            if (DialogResult.OK != MessgeBox.Show("Really replace:\n\n" + 
                                                        string.Format(REPL_FORMAT, mbr.sSrcDescr, ds.sSrcDrive ) +
                                                        string.Format(REPL_FORMAT, mbr.sSrc     , ds.sSrc      ) +
                                                        string.Format(REPL_FORMAT, mbr.sDstDrive, ds.sDstDrive ) +
                                                        string.Format(REPL_FORMAT, mbr.sDst     , ds.sDst      ) +
                                                        string.Format(REPL_FORMAT, mbr.sFileMsks, ds.sFileMsks ) +
                                                        "\n?", "Warning" , MessageBoxButtons.YesNo) ) return;
            ds.sDstDescr = mbr.sDstDescr;
            ds.sSrcDescr = mbr.sSrcDescr;

            m_DataList[ind] = ds;
            UpdateMenu( ref m_Menu );
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       21.12.2009
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        void OnDescription( Object sender, EventArgs e )
        {
            MenuItem mi = (MenuItem)sender;
            MenuItem pm = (MenuItem)mi.Parent;
            MenuItem mm = (MenuItem)pm.Parent;

            int ind = pm.Index - m_iStartMenuIdx;
            FavDataSet mbr = m_DataList[ind];

            m_DescrEdit.LoadData(mbr);

            if (m_DescrEdit.ShowDialog() == DialogResult.OK)
            {
                FavDataSet s = new FavDataSet();

                s = mbr;

                s = m_DescrEdit.ReadData(mbr);

                m_DataList[ind] = s;

                UpdateMenu( ref m_Menu );
            }
        }
	}
}

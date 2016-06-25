using System;
using System.Collections.Generic;
using System.Text;
using NS_AppConfig;
using System.Windows.Forms;
using System.Collections;

namespace NS_UserList
{
    /***************************************************************************
    SPECIFICATION: UserListView class definition
    CREATED:       05.03.2007
    LAST CHANGE:   05.03.2007
    ***************************************************************************/
    public class UserListView : System.Windows.Forms.ListView
    {
        protected bool m_bSortAscending;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       05.03.2007
        LAST CHANGE:   07.12.2012
        ***************************************************************************/
        public UserListView()
          :base()
        {
            //this.DoubleBuffered = true;
            this.SetStyle( ControlStyles.EnableNotifyMessage, true );

            m_bSortAscending = true;
            ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);
        }
        
        /***************************************************************************
        SPECIFICATION: Serialization of column header's text and width
        CREATED:       05.03.2007
        LAST CHANGE:   29.01.2014
        ***************************************************************************/
        public void Serialize(ref AppSettings a_rConf)
        {
            if (a_rConf.IsReading)
            {
                Columns.Clear();

                int iNrCols = a_rConf.Deserialize<int>( );

                for (int i=0; i<iNrCols; i++)
                {
                    string sColText = a_rConf.Deserialize<string>( );

                    Columns.Add(sColText);
                }

                foreach (ColumnHeader col in Columns)
                {
                    col.Width = a_rConf.Deserialize<int>( );
                }
            }
            else
            {
                a_rConf.Serialize(Columns.Count);

                foreach (ColumnHeader col in Columns)
                {
                    a_rConf.Serialize(col.Text);
                }

                foreach (ColumnHeader col in Columns)
                {
                    a_rConf.Serialize(col.Width);
                }
            }
        }


        /***************************************************************************
        SPECIFICATION: Implements the manual sorting of items by columns.
        CREATED:       11.05.2006
        LAST CHANGE:   06.03.2007
        ***************************************************************************/
        public class ListViewItemComparer:IComparer
        {
            private int m_Col;
            private bool m_bAsc;
            public ListViewItemComparer()
            {
                m_Col = 0;
                m_bAsc = true;
            }

            public ListViewItemComparer(int a_Column,bool a_bAscending)
            {
                m_Col = a_Column;
                m_bAsc = a_bAscending;
            }

            public int Compare(object x,object y)
            {
                ListViewItem a,b;

                if ( m_bAsc )
                {
                    a = (ListViewItem)x;
                    b = (ListViewItem)y;
                }
                else
                {
                    b = (ListViewItem)x;
                    a = (ListViewItem)y;
                }

                if ( a.SubItems.Count <= m_Col )
                    return -1;
                if ( b.SubItems.Count <= m_Col )
                    return 1;
                return String.Compare(a.SubItems[m_Col].Text,b.SubItems[m_Col].Text);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.05.2006
        LAST CHANGE:   11.05.2006
        ***************************************************************************/
        protected void listView_ColumnClick(object sender,ColumnClickEventArgs e)
        {
            ListViewItemSorter = new ListViewItemComparer(e.Column,m_bSortAscending);
            Sort();
            m_bSortAscending = !m_bSortAscending;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       05.12.2013
        LAST CHANGE:   05.12.2013
        ***************************************************************************/
        public string MergeLine( ListViewItem a_Item )
        {
            string ret = "";

            foreach ( ListViewItem.ListViewSubItem si in a_Item.SubItems )
            {
                ret += si.Text;
            }
            
            return ret;
        }
        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.11.2006
        PARAMS:        a_NrCols:    all columns if -1
        LAST CHANGE:   20.05.2009
        ***************************************************************************/
        public List<ListViewItem> MergeColumns(bool bOnlySelected, string sSeparator, int a_NrCols)
        {
            List<ListViewItem> al = new List<ListViewItem>();

            foreach ( ListViewItem li in Items )
            {
                if ( li.Selected || !bOnlySelected )
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = "";
                    lvi.Selected = li.Selected;

                    int iNrCols = Columns.Count;
                    if (a_NrCols != -1) iNrCols = a_NrCols;
 
                    foreach ( ListViewItem.ListViewSubItem col in li.SubItems )
                    {
                        lvi.Text += col.Text; // + "\r\n";
                        if ( --iNrCols <= 0 ) break;
                        lvi.Text += sSeparator;
                    }
                    al.Add(lvi);
                }
            }
            return al;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.12.2013
        LAST CHANGE:   09.12.2013
        ***************************************************************************/
        public string[] MergeColumns( )
        {
            //List<ListViewItem> al = new List<ListViewItem>();
            ListViewItem[] al;
            string[]       sa;

            sa = new string      [SelectedItems.Count];
            al = new ListViewItem[SelectedItems.Count];
            SelectedItems.CopyTo(al,0);

            int i = 0;
            
            foreach ( ListViewItem lvi in al )
            {
                //sa[i] = lvi.Text;
                
                foreach ( ListViewItem.ListViewSubItem si in lvi.SubItems )
                {
                    sa[i] += si.Text;
                }
                
                i++;
            }
                
            return sa;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2007
        LAST CHANGE:   09.12.2013
        ***************************************************************************/
        //public ListViewItem[] MergeColumns( bool bOnlySelected )
        //{
        //    ListViewItem[] al;

        //    if (bOnlySelected)
        //    {
        //        al = new ListViewItem[SelectedItems.Count];
        //        SelectedItems.CopyTo(al,0);
        //    }
        //    else
        //    {
        //        al = new ListViewItem[Items.Count];
        //        Items.CopyTo(al,0);
        //    }

        //    int i=0;
            
        //    foreach ( ListViewItem lvi in al )
        //    {
        //        foreach ( ListViewItem.ListViewSubItem si in lvi.SubItems )
        //        {
        //            lvi.Text += si.Text;
        //        }
        //    }
                
        //    return al;
        //}


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.12.2012
        LAST CHANGE:   07.12.2012
        ***************************************************************************/
        protected override void OnNotifyMessage( Message m )
        {
            //Filter out the WM_ERASEBKGND message 
            if( m.Msg != 0x14 )
            {
                base.OnNotifyMessage( m );
            }
        }

    }

}

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace NS_UserTabControl
{

    /***************************************************************************
    SPECIFICATION: UserTabPage class
    CREATED:       12.12.2013
    LAST CHANGE:   12.12.2013
    ***************************************************************************/
    public class UserTabPage : TabPage
    {
        public Color TabColor  { get { return m_tTabColor;  } }

        Color m_tTabColor;
        Color m_tTabColorActive;
        Color m_tTabColorInact;
        bool  m_bColorChanged;
        
        private delegate void dl_Activate ( bool a_bActive );

        public UserTabPage()
            :base()
        {
            init();
        }

        public UserTabPage( string a_sTab )
            : base( a_sTab )
        {
            init();
        }

        private void init()
        {
            m_tTabColor     = Color.Black;
            m_bColorChanged = false;
        }

        public void SetColor( Color a_tCol )
        {
            m_tTabColor     = a_tCol;
            m_bColorChanged = true;
            Invalidate();
        }

        public void SetActiveColor  ( Color a_tActCol ) { m_tTabColorActive = a_tActCol; }
        public void SetInactiveColor( Color a_tActCol ) { m_tTabColorInact  = a_tActCol; }

        public bool ColChanged()
        {
            bool ret = m_bColorChanged;
            m_bColorChanged = false;
            return ret;
        }

        public void Activate( bool a_bActive )
        {
            if ( this.InvokeRequired )
            {
                dl_Activate d = new dl_Activate( Activate );
                this.Invoke( d, new object[] { a_bActive } );
            }
            else 
            {
                if (a_bActive ) SetColor( m_tTabColorActive );
                else            SetColor( m_tTabColorInact );
                Invalidate();
            }
        }
    }


    /***************************************************************************
    SPECIFICATION: UserTabControl class
    CREATED:       12.12.2013
    LAST CHANGE:   12.12.2013
    ***************************************************************************/
    public class UserTabControl : TabControl
    {
        //public List<UserTabPage> TabPages { get { return m_tTabPages; } }
        //private List<UserTabPage> m_tTabPages;

        public UserTabControl()
        : base()
        {
            //m_tTabPages = new List<UserTabPage>();
            DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.tabControl_DrawItem );        
        }

        private void tabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            UserTabPage utp = (UserTabPage)TabPages[e.Index];

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;

            // Font fnt = new Font(e.Font,FontStyle.Bold); 
            Font fnt = e.Font; 

            Brush backBr = null;
            Brush foreBr = new SolidBrush ( utp.TabColor );

            if (SelectedIndex == e.Index ) backBr = new SolidBrush ( Color.Transparent );
            else                           backBr = new SolidBrush ( Color.FromArgb(197,192,184) );


            e.Graphics.FillRectangle(backBr, e.Bounds);
            SizeF sz = e.Graphics.MeasureString( TabPages[e.Index].Text, fnt );
            Rectangle rect = e.Bounds;
            rect = new Rectangle(rect.X, rect.Y + 3, rect.Width, rect.Height - 3);

            e.Graphics.DrawString( utp.Text, fnt, foreBr, rect, sf );

            sf.Dispose();
        }
    }
}

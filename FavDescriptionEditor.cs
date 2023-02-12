using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NS_AppConfig;
using NS_Utilities;


namespace NS_FavDescr
{
    /***************************************************************************
    SPECIFICATION: 
    CREATED:       22.12.2009
    LAST CHANGE:   22.12.2009
    ***************************************************************************/
    public partial class FavDescriptionEditor : Form
    {
        private DialogPosSize    m_PosSize;

        /***************************************************************************
        SPECIFICATION: C`tor 
        CREATED:       22.12.2009
        LAST CHANGE:   22.12.2009
        ***************************************************************************/
        public FavDescriptionEditor()
        {
            InitializeComponent();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.12.2009
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if ( a_Conf.IsReading )
            {
                if( a_Conf.DbVersion > 9 )
                {
                    a_Conf.DeserializeDialog( this );
                    m_PosSize = new DialogPosSize( this );
                }
            }
            else
            {
                a_Conf.SerializeDialog(this);
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       22.12.2009
        LAST CHANGE:   22.12.2009
        ***************************************************************************/
        private void FavDescriptionEditor_Load( object sender, EventArgs e )
        {
            if( null != m_PosSize ) m_PosSize.Write( this );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       24.12.2009
        LAST CHANGE:   24.12.2009
        ***************************************************************************/
        private void buttonAutoSet_Click( object sender, EventArgs e )
        {
            string src = textBoxSrcPath.Text;
            string dst = textBoxDstPath.Text;

            string[] hlp = src.Split( "\\".ToCharArray() );
            src = hlp[hlp.Length-1];
                     hlp = dst.Split( "\\".ToCharArray() );
            dst = hlp[hlp.Length-1];

            textBoxSrcDescr.Text = src;
            textBoxDstDescr.Text = dst;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.12.2014
        LAST CHANGE:   09.12.2014
        ***************************************************************************/
        public void LoadData( NS_Backup.FavDataSet data )
        {
            textBoxDstDescr.Text = data.sDstDescr;
            
            textBoxDstDescr.Text = data.sDstDescr;
            textBoxDstPath.Text  = data.sDst;
            textBoxSrcDescr.Text = data.sSrcDescr;
            textBoxSrcPath.Text  = data.sSrc;
            textBoxDstDrive.Text = data.sDstDrive;
            textBoxSrcDrv.Text   = data.sSrcDrive; 
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.12.2014
        LAST CHANGE:   10.12.2014
        ***************************************************************************/
        public NS_Backup.FavDataSet ReadData( NS_Backup.FavDataSet data )
        {
            NS_Backup.FavDataSet ret = data;

            ret.sDstDescr = textBoxDstDescr.Text;
            ret.sDst      = textBoxDstPath .Text;
            ret.sSrcDescr = textBoxSrcDescr.Text;
            ret.sSrc      = textBoxSrcPath .Text;
            ret.sDstDrive = textBoxDstDrive.Text;
            ret.sSrcDrive = textBoxSrcDrv  .Text;

            return ret;
        }
    }
}
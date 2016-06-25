using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NS_Utilities;

namespace NS_ReplDL
{
    public partial class ReplaceDL:Form
    {
        private char m_cBefore = ' ';
        private char m_cAfter  = ' ';

        public char cBefore
        {
            get { return char.ToUpper(m_cBefore); }
        }

        public char cAfter
        {
            get { return char.ToUpper(m_cAfter); }
        }

        public string sBefore
        {
            get { return char.ToUpper(m_cBefore) + ":"; }
        }

        public string sAfter
        {
            get { return char.ToUpper(m_cAfter) + ":"; }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.06.2007
        LAST CHANGE:   19.06.2007
        ***************************************************************************/
        public ReplaceDL()
        {
            InitializeComponent();

            ReadDrives(true);
            ReadDrives(false);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.06.2007
        LAST CHANGE:   19.06.2007
        ***************************************************************************/
        private void ReadDrives(bool bBefore)
        {
            string[] drv = Directory.GetLogicalDrives();

            if ( bBefore ) comboBoxBefore.Items.Clear();
            else           comboBoxAfter .Items.Clear();

            foreach ( string d in drv )
            {
                string n;

                if ( d[0] == 'A' ) n = "Floppy";
                else n = Utils.GetDriveName(d);

                if ( 0 == n.Length ) n = d;
                else n = d + " (" + n + ")";

                if ( bBefore ) comboBoxBefore.Items.Add(n);
                else           comboBoxAfter.Items.Add(n);
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.06.2007
        LAST CHANGE:   19.06.2007
        ***************************************************************************/
        private void comboBoxBefore_Click(object sender,EventArgs e)
        {
            ReadDrives(true);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.06.2007
        LAST CHANGE:   19.06.2007
        ***************************************************************************/
        private void comboBoxAfter_Click(object sender,EventArgs e)
        {
            ReadDrives(false);
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.06.2007
        LAST CHANGE:   23.06.2007
        ***************************************************************************/
        private void buttonReplace_Click(object sender,EventArgs e)
        {
            m_cAfter  = comboBoxAfter .Text[0];
            m_cBefore = comboBoxBefore.Text[0]; 

            DialogResult = DialogResult.OK;

            Close();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.06.2007
        LAST CHANGE:   23.06.2007
        ***************************************************************************/
        private void ReplaceDL_FormClosing(object sender,FormClosingEventArgs e)
        {
            e.Cancel = false;
        }


    }
}
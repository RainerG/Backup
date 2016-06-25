using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using NS_UserTextEditor;
using NS_AppConfig;

namespace NS_UserCombo 
{
    /// <summary>
    /// Summary description for FileComboBox.
    /// </summary>
    /***************************************************************************
    SPECIFICATION: More comfortable ComboBox
    CREATED:       2004
    LAST CHANGE:   07.12.2007
    ***************************************************************************/
    public class UserComboBox: System.Windows.Forms.ComboBox
    {
        private int     m_iMaxNrItems  = -1;  // unlimited;
        private bool    m_bCtrlPressed = false;
        private bool    m_bAltPressed  = false;
        private string  m_sOldText;
        
        private UserTextEditor m_TextEditor;

        public int MaxNrItems { set {  m_iMaxNrItems = value; } }

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       ??.??.2004
        LAST CHANGE:   07.08.2013
        ***************************************************************************/
        public UserComboBox()
            : base()
        {
            KeyUp                += new KeyEventHandler(Combo_KeyUp);
            KeyDown              += new KeyEventHandler(Combo_KeyDown);
            SelectedIndexChanged += new EventHandler(Combo_SelectedIndexChanged);
            Click                += new EventHandler(Combo_Click);

            this.AutoCompleteMode   = AutoCompleteMode.SuggestAppend;
            this.AutoCompleteSource = AutoCompleteSource.CustomSource;
            
            m_sOldText   = "";
            m_TextEditor = new UserTextEditor(this);
            this.Text    = "";
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       10.12.2007
        LAST CHANGE:   10.12.2007
        ***************************************************************************/
        public void Serialize(ref AppSettings a_Conf)
        {
            if (a_Conf.IsReading)
            {
                a_Conf.DeserializeComboBox(this);
            }
            else
            {
                a_Conf.SerializeComboBox(this);
            }

            m_TextEditor.Serialize(ref a_Conf);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   24.02.2007
        ***************************************************************************/
        protected bool AlreadyIn(string sPath)
        {
            if (sPath.Trim() == "") return true;   // white spaces are rejected

            for(int i=0; i<Items.Count; i++)
            {
                if (sPath == (string)Items[i])
                {
                    return true;
                }
            }

            return false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   07.08.2013
        ***************************************************************************/
        public void AddTextEntry()
        {
            if (! AlreadyIn(Text)) 
            {
                Items.Add(Text);
                m_sOldText = Text;
                
                if (m_iMaxNrItems != -1)
                {
                    // all entries over m_iMaxNrItems are cancelled
                    while(Items.Count > m_iMaxNrItems) 
                    {
                        Items.RemoveAt(0);
                    }
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   28.10.2006
        ***************************************************************************/
        private void Combo_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            m_bCtrlPressed = false;
            m_bAltPressed  = false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   07.12.2007
        ***************************************************************************/
        private void Combo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.Control) m_bCtrlPressed = true;
            if (e.Alt)     m_bAltPressed  = true;
            m_sOldText = this.Text;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       28.10.2006
        LAST CHANGE:   12.12.2007
        ***************************************************************************/
        private void Combo_Click(object sender, System.EventArgs e)
        {
            if (m_bCtrlPressed && m_bAltPressed)
            {
                EditItems();
            }
            m_bAltPressed  = false;
            m_bCtrlPressed = false;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ??.??.2004
        LAST CHANGE:   07.12.2007
        ***************************************************************************/
        private void Combo_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (m_bCtrlPressed)
            {
                m_bCtrlPressed = false;
                if (m_bAltPressed)
                {
                    DialogResult r = MessageBox.Show("Delete All ? (Yes)\nEdit list ? (No)\nAbort ? (Cancel)","Warning",MessageBoxButtons.YesNoCancel);
                    switch(r)
                    {
                        case DialogResult.Yes:
                            this.Items.Clear();
                            break;

                        case DialogResult.No:
                            EditItems();
                            break;
                    }
                }
                else
                {
                    int idx  = SelectedIndex;
                    string txt = (string)Items[idx];

                    DialogResult r = MessageBox.Show("Delete \"" + txt + "\" ?","Warning",MessageBoxButtons.YesNo);
                    if (r == DialogResult.Yes)
                    {
                        Items.RemoveAt(idx);
                    }
                }

                this.Text = m_sOldText;
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       12.12.2007
        LAST CHANGE:   12.12.2007
        ***************************************************************************/
        private void EditItems()
        {
            m_TextEditor.GetComboItems();
            if ( DialogResult.OK == m_TextEditor.ShowDialog() )
            {
                this.Items.Clear();
                foreach ( string line in m_TextEditor.m_aOutput )
                {
                    if ( "" != line ) this.Items.Add(line);
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       2004
        LAST CHANGE:   2004
        ***************************************************************************/
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }

    } // class
} // namespace

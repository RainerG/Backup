using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Dotnetrix_Samples
{
    /// <summary>
    /// Summary description for MirroredTabControl.
    /// </summary>
    public class MirroredTabControl : System.Windows.Forms.TabControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public MirroredTabControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitializeComponent call

        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.SuspendLayout();
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Location = new System.Drawing.Point(0, 0);
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(100, 20);
            this.maskedTextBox1.TabIndex = 0;
            this.ResumeLayout(false);

        }
        #endregion

        private MaskedTextBox maskedTextBox1;

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_LAYOUTRTL  = 0x400000;
                const int WS_EX_NOINHERITLAYOUT = 0x100000;
                CreateParams cp = base.CreateParams;
                if (this.Mirror)
                    cp.ExStyle = cp.ExStyle|WS_EX_LAYOUTRTL|WS_EX_NOINHERITLAYOUT;
                return cp;
            }
        }


private bool m_Mirror = false;

        [DefaultValue(false)]
        public bool Mirror
        {
            get
            {
                return m_Mirror;
            }
            set
            {
                if (m_Mirror == value) return;
                m_Mirror = value;
                base.UpdateStyles();
            }
        }


    }
}

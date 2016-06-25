using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using NS_Utilities;
using NS_AppConfig;
using NS_UserOut;
using NS_AppConfig;

namespace NS_Backup
{
	/// <summary>
	/// Summary description for ProtocolView.
	/// </summary>
	public class ProtocolView : UserOutText
	{
        /***************************************************************************
        SPECIFICATION: Accessors 
        CREATED:       09.02.2016
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        public DirType Dirs { set { m_Dirs = value; } }

        /***************************************************************************
        SPECIFICATION: Members 
        CREATED:       09.02.2016
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        private DirType m_Dirs;

        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       ??.??.2004
        LAST CHANGE:   29.03.2006
        ***************************************************************************/
        public ProtocolView()
            :base()
		{
			InitializeComponent();

            ToolStripSeparator tss  = new ToolStripSeparator();
            ToolStripMenuItem  tsm1 = new ToolStripMenuItem("&Delete selection");
            ToolStripMenuItem  tsm2 = new ToolStripMenuItem("&Delete selection and sources");

            optionsToolStripMenuItem.DropDownItems.Add(tss);
            optionsToolStripMenuItem.DropDownItems.Add(tsm1);
            optionsToolStripMenuItem.DropDownItems.Add(tsm2);

            tsm1.Click +=new EventHandler(DeleteSelection_Click);
            tsm2.Click +=new EventHandler(DeleteSelectionNSources_Click);

            TextBox.DetectUrls = false;
		}


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.02.2016
        LAST CHANGE:   09.02.2016
        ***************************************************************************/
        private string[] SelectedFiles()
        {
            string seltxt = TextBox.SelectedText;

            string[] files = seltxt.Split("\n".ToCharArray());

            List<string> delfiles = new List<string>();

            foreach( string f in files )
            {
                string s = f.Replace("new:","");
                       s = s.Replace("old:","");
                       s = s.Trim();
 
                if (File.Exists(s)) delfiles.Add(s);
            }

            ShowOutput("\nFiles to delete:\n",ColHdr,true);
            foreach( string f in delfiles )
            {
                ShowOutput(f,Col2);
                ShowOutput("\n");
            }

            return delfiles.ToArray();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       21.01.2016
        LAST CHANGE:   21.01.2016
        ***************************************************************************/
        private void DeleteSelection_Click( object sender, EventArgs e )
        {
            try
            {
                string[] delfiles = SelectedFiles();

                DialogResult res = MessageBox.Show("Delete ?","Files to delete ...", MessageBoxButtons.YesNo);

                if( res == DialogResult.Yes )
                {  // delete the files
                    ShowOutput("\n");
                    foreach( string f in delfiles )
                    {
                        File.Delete(f);
                        ShowOutput( "Deleted: ",Col1 );
                        ShowOutput( f + "\n"   ,Col2 );
                    }
                }
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Error in file deletion occurred");
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       09.02.2016
        LAST CHANGE:   08.06.2016
        ***************************************************************************/
        private void DeleteSelectionNSources_Click( object sender, EventArgs e )
        {
            try
            {
                string[] delfiles = SelectedFiles();

                List<string> srcfiles = new List<string>();

                foreach( string f in delfiles )
                {
                    string fil = f.Replace( m_Dirs.dst, "" );
                           fil = Utils.ConcatPaths( m_Dirs.src, fil );
                    if( File.Exists( fil ) )
                    {
                        srcfiles.Add( fil );
                        ShowOutput(fil,Col2);
                        ShowOutput("\n");
                    }
                }

                srcfiles.AddRange(delfiles);

                DialogResult res = MessageBox.Show("Delete ?","Files to delete ...", MessageBoxButtons.YesNo);

                if( res == DialogResult.Yes )
                {  // delete the files
                    ShowOutput("\n");
                    foreach( string f in srcfiles )
                    {
                        File.Delete(f);
                        ShowOutput( "Deleted: ",Col1 );
                        ShowOutput( f + "\n"   ,Col2 );
                    }
                    // delete empty directories
                    foreach( string f in srcfiles )
                    {
                        string d = Utils.GetPath(f);
                        if (Directory.Exists(d))
                        {
                            if (Directory.GetDirectories(d).Length > 0) continue;
                            if (Directory.GetFiles      (d).Length > 0) continue;
                        }
                        Directory.Delete(d);
                    }
                }

            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message, "Error in file deletion occurred");
            }
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       17.01.2016
        LAST CHANGE:   17.01.2016
        ***************************************************************************/
        private delegate void dl_ReadFile( string sProtfile );
        public void ReadFile( string sProtFile )
        {
            if( this.InvokeRequired )
            {
                dl_ReadFile d = new dl_ReadFile(ReadFile);
                this.Invoke( d, new object[]{sProtFile} );
            }
            else
            {
                if (! Utils.NoFile(sProtFile))
                {
                    StreamReader f = new StreamReader(sProtFile);

                    while(f.Peek() > 0)
                    {
                        string s = f.ReadLine();
                        if (s.StartsWith("old:")) ShowOutput(s+"\n",Col1);
                        if (s.StartsWith("new:")) ShowOutput(s+"\n",ColErr);
                    }
                    f.Close();
                }
            }
        }


		/***************************************************************************
        SPECIFICATION: 
        CREATED:       17.01.2016
        LAST CHANGE:   17.01.2016
        ***************************************************************************/
        private void InitializeComponent()
		{
            this.Name = "ProtocolView";

        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       17.01.2016
        LAST CHANGE:   17.01.2016
        ***************************************************************************/
        public void Serialize( ref AppSettings a_Conf )
        {
            if (a_Conf.IsReadingAndDBST(14)) return;
            base.Serialize( ref a_Conf );
        }

	} // class
} // namespace


using System;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace NS_Utilities
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
        [DllImport("kernel32.dll")]
        public static extern int GetVolumeInformation(string strPathName,
                                                       StringBuilder strVolumeNameBuffer,
                                                       int lngVolumeNameSize,
                                                       int lngVolumeSerialNumber,
                                                       int lngMaximumComponentLength,
                                                       int lngFileSystemFlags,
                                                       string strFileSystemNameBuffer,
                                                       int lngFileSystemNameSize);

        [DllImport("kernel32.dll")]
        public static extern int GetDriveType(string driveLetter);

        /***************************************************************************
         * SPECIFICATION: Ctor
         * CREATED:       26.04.2006
         * LAST CHANGE:   26.04.2006
         * ***************************************************************************/
        public Utils()
		{
			//
			// TODO: Add constructor logic here
			//
		}

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.04.2006
        LAST CHANGE:   26.04.2006
        ***************************************************************************/
        static public string LimitPath(string a_sPath, int a_iMaxLen)
        {
            const string DOTS   = "...";

            string s;
            int iSubLen = (a_iMaxLen - DOTS.Length) / 2;

            if (a_sPath.Length > a_iMaxLen)
            {
                s =  a_sPath.Substring(0,iSubLen);
                s += DOTS;
                s += a_sPath.Substring(a_sPath.Length-iSubLen,iSubLen);
            }
            else   s = a_sPath;

            return s;
        }

        static public bool NoFile(string sFilePath)
        {
            if (File.Exists(sFilePath)) return false;

            MessageBox.Show("'" + sFilePath + "' not found","Error");

            return true;
        }

        static public bool NoFolder(string sDirectory)
        {
            if (Directory.Exists(sDirectory)) return false;

            MessageBox.Show("'" + sDirectory + "' not found","Error");

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.03.2006
        LAST CHANGE:   06.05.2009
        ***************************************************************************/
        static public string GetDriveName(string drive)
        {
            //receives volume name of drive
            StringBuilder volname = new StringBuilder(256);
            //receives serial number of drive,not in case of network drive(win95/98)
            int sn          = new int();
            int maxcomplen  = new int();//receives maximum component length
            int sysflags    = new int();//receives file system flags
            string sysname  = new string(new char[1]); //receives the file system name
            int retval      = new int();//return value

            if (drive == null) return "";

            string[] fracs = drive.Split(':');  // only the drive letter 

            if (fracs.Length < 1) return "";

            retval = GetVolumeInformation(fracs[0]+":",volname,256,sn,maxcomplen,sysflags,sysname,256);

            if(0 == volname.Length)
            {
                switch(GetDriveType(drive))
                {
                    case 5:     volname.Insert(0,"CD-ROM");         break;
                    case 3:     volname.Insert(0,"Fixed");          break;
                    case 2:     volname.Insert(0,"Removable disc"); break;
                    case 4:     volname.Insert(0,"Remote disc");    break;
                    case 6:     volname.Insert(0,"RAM disc");       break;
                    default:    volname.Insert(0,"");               break;
                }
            }

            return volname.ToString();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       20.04.2006
        LAST CHANGE:   20.04.2006
        ***************************************************************************/
        static public string ConcatPaths(string a,string b)
        {
            if(b.StartsWith("\\"))
            {
                if(a.EndsWith("\\"))
                {
                    return a + b.Remove(0,1);
                }
                else
                {
                    return a + b;
                }
            }
            else
            {
                if(a.EndsWith("\\"))
                {
                    return a + b;
                }
                else
                {
                    return a + "\\" + b;
                }
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.11.2013
        LAST CHANGE:   08.11.2013
        ***************************************************************************/
        static public string GoOneUp( string sPath )
        {
            string[] pa = sPath.Split( '\\' );

            List<string> pth = new List<string>( pa );

            pth.RemoveAt( pth.Count - 2 );
                
            string ret = "";

            foreach( string p in pth )
            {
                ret += p + "\\";
            }

            ret = ret.Remove( ret.Length-1, 1 );

            return ret;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.01.2013
        LAST CHANGE:   30.01.2013
        ***************************************************************************/
        static public string CancelGoUps( string sPath )
        {
            string[] pa = sPath.Split('\\');

            List<string> pth = new List<string>(pa);

            while ( true )
            {
                bool found = false;

                for ( int i=1; i<pth.Count; i++ )
                {
                    if( pth[i] == ".." )
                    {
                        found = true;
                        pth.RemoveAt(i);
                        pth.RemoveAt(i-1);
                    }
                }

                if ( ! found ) break;
            }

            string ret = "";

            foreach ( string p in pth )
            {
                ret += p + "\\";
            }

            ret = ret.Remove(ret.Length-1,1);

            return ret;
        }


        /***************************************************************************
        SPECIFICATION: Retrieves a filename from a fully qualified path
        CREATED:       15.03.2007
        LAST CHANGE:   15.03.2007
        ***************************************************************************/
        static public string GetFilename(string sPath)
        {
            int idx = sPath.LastIndexOf("\\");

            if (-1 == idx)               return sPath;
            if (idx >= sPath.Length - 1) return "";

            return sPath.Substring(idx + 1);
        }


        /***************************************************************************
        SPECIFICATION: Retrieves a filename body from a fully qualified path
        CREATED:       24.05.2009
        LAST CHANGE:   24.05.2009
        ***************************************************************************/
        static public string GetFilenameBody( string sPath )
        {
            string ret= "";

            string fn = GetFilename(sPath);

            string[] hlp = fn.Split('.');

            for( int i = 0; i < hlp.Length - 1; i++ )
            {
                ret += hlp[i];            
            }

            return ret;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.05.2009
        LAST CHANGE:   07.05.2009
        ***************************************************************************/
        static public string GetExtension(string sPath)
        {
            string[] hlp = sPath.Split('.');

            int len = hlp.Length;

            if (len < 2) return "";

            return hlp[len - 1];
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.03.2007
        LAST CHANGE:   23.05.2013
        ***************************************************************************/
        static public string GetPath(string sPathFile)
        {
            int idx = sPathFile.LastIndexOf('\\');

            if (-1 == idx) return "";

            return sPathFile.Substring(0,idx);
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       16.04.2009
        LAST CHANGE:   16.04.2009
        ***************************************************************************/
        static public string ReplaceDriveLetter(string sPath, string sDrive)
        {
            string ret = null;

            string[] hlp = sDrive.Split(':');
            string   ltr = hlp[0];

            string[] fracs = sPath.Split(':');

            if (fracs.Length < 1) return sPath;

            ret  = ltr;
            ret += ":";
 
            for ( int i=1; i<fracs.Length; i++ )
            {
                ret += fracs[i];    
            }

            return ret;
        }

        /***************************************************************************
        SPECIFICATION: returns the drive letter inclusive ':'.
        CREATED:       16.04.2009
        LAST CHANGE:   27.04.2011
        ***************************************************************************/
        static public string GetDriveLetter(string sPath)
        {
            string[] hlp = sPath.Split(':');

            if ( hlp.Length < 2 ) return "";

            return hlp[0] + ":";
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.05.2009
        LAST CHANGE:   08.05.2009
        ***************************************************************************/
        static public int RemoveNumber(ref string a_Name)
        {
            int idx = a_Name.IndexOfAny("0123456789".ToCharArray());

            if ( idx == -1 ) return idx;

            int count = 0;
            for ( int i = idx;i < a_Name.Length;i++ )
            {
                char c = a_Name[i];

                if ( c >= '0' && c <= '9' )
                {
                    count++;
                }
                else break;
            }

            a_Name = a_Name.Remove(idx,count);

            return idx;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       19.05.2009
        LAST CHANGE:   19.05.2009
        ***************************************************************************/
        static public int ExtractNumber(string a_Str)
        {
            int idx = a_Str.IndexOfAny("0123456789".ToCharArray());

            if ( idx == -1 ) return 0;

            int count = 0;
            for ( int i = idx; i < a_Str.Length; i++ )
            {
                char c = a_Str[i];

                if ( c >= '0' && c <= '9' )
                {
                    count++;
                }
                else break;
            }

            return int.Parse(a_Str.Substring(idx,count));
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       07.05.2009
        LAST CHANGE:   08.05.2009
        ***************************************************************************/
        static public string DeleteNumber(string a_Name)
        {
            string ret = a_Name;

            RemoveNumber(ref ret);

            return ret;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.05.2009
        LAST CHANGE:   08.05.2009
        ***************************************************************************/
        static public string ReplaceNumber(string a_Name,string a_Nr)
        {
            string ret = a_Name;

            int idx = RemoveNumber(ref ret);

            if ( idx == -1 ) return a_Name;

            ret = ret.Insert(idx,a_Nr);

            return ret;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.04.2013
        LAST CHANGE:   18.04.2013
        ***************************************************************************/
        static public Color PickColor ( Color inCol )
        {
            Color rCol = inCol;

            ColorDialog dlg = new ColorDialog();
            dlg.Color = inCol;
            DialogResult dres = dlg.ShowDialog();

            if( dres == DialogResult.OK )
            {
                rCol = dlg.Color;
            }

            return rCol;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       08.03.2013
        LAST CHANGE:   11.12.2013
        ***************************************************************************/
        static public Double Str2Double( string a_sArg )
        {
            try
            {
                bool bSign = false;

                int idx = a_sArg.IndexOfAny( "0123456789".ToCharArray() );

                if ( idx == -1 ) return 0.0;

                if ( idx > 0 && a_sArg[idx-1] == '-' ) bSign = true;

                string resp = a_sArg.Remove( 0, idx );

                idx = resp.LastIndexOfAny( "0123456789".ToCharArray() ) + 1;

                if( idx < resp.Length ) resp = resp.Remove( idx );

                if ( resp.IndexOf(".") != -1 && resp.IndexOf(",") != -1 )
                { // , is supposed to be a separator of more numbers
                    string[] nrs = resp.Split(',');
                    resp = nrs[0];
                }

                resp = resp.Replace( '.', ',' );

                Double ret = Double.Parse( resp );

                if ( bSign ) ret *= -1.0;

                return ret;
            }
            catch (System.FormatException)
            {
                return(0.0);                                            	
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Str2Double exception");
                
                return 0.0;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.01.2014
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        static public string GetAppDir()
        {
            return Application.StartupPath + "\\";
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.01.2014
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        static public string GetCurrDir()
        {
            return Directory.GetCurrentDirectory() + "\\";
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.01.2014
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        static public string GetSlnDir()
        {
            string slndir = GetCurrDir();
            slndir = GoOneUp(slndir);
            slndir = GoOneUp(slndir);
            slndir = GoOneUp(slndir);
            return slndir;
        }

    } // class

    public class CriticalSectionLock : IDisposable 
    {
	    private CriticalSection mCriticalSection;

	    // Acquires the lock:

	    public CriticalSectionLock( CriticalSection criticalSection ) 
	    {
		    mCriticalSection = criticalSection;
		    mCriticalSection.Enter();
	    }

	    // Releases the lock:

	    public void Dispose() 
	    {
		    mCriticalSection.Leave();
	    }
    }
 
    /// The C/C++ CRITIAL_SECTION store debug information like which thread holds the lock,
    /// so this class is also named CriticalSection to emphasize the similarity.
    /// See: http://msdn.microsoft.com/en-us/magazine/cc164040.aspx
    /// 
    /// Using the Monitor.Enter/Exit is the same as using the lock statement. See:
    /// http://msdn.microsoft.com/en-us/library/ms173179.aspx
    /// </summary>
    /// <seealso cref="CriticalSectionLock"/>
    public class CriticalSection 
    {
	    private const int UNOWNED_THREAD_ID = -1;

	    // This is not a property just in case the debugger has problems evaluating it
	    // and it is public so that the compiler doesn't try to optimize it away.

	    public int OwnerThreadId = UNOWNED_THREAD_ID;

	    internal void Enter()
	    {
		    // When we were able to lock 'this' then it is safe to store the owning thread id:

		    Monitor.Enter( this );
		    OwnerThreadId = Thread.CurrentThread.ManagedThreadId;
	    }

	    internal void Leave()
	    {
		    // We are still locked so we can safely erase the owner thread id:

		    OwnerThreadId = UNOWNED_THREAD_ID;
		    Monitor.Exit( this );
	    }
    }

} // namespace

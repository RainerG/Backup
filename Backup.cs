//#define NEW_ALG

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using NS_Utilities;
using System.Collections.Generic;
using System.Diagnostics;

namespace NS_Backup
{
	/// <summary>
	/// Summary description for Backup.
	/// </summary>
	
    /***************************************************************************
    SPECIFICATION: Globals and types
    CREATED:       2014
    LAST CHANGE:   28.06.2016
    ***************************************************************************/
    public delegate void dl_BuReady();
    public delegate void dl_ShowDir     ( String sDir );
    public delegate void dl_ShowFile    ( String sFname, bool bAppend );
    public delegate void dl_ShowAllBar  ( int iCurrFiles, int iAllFiles );
    public delegate void dl_ShowFilBar  ( int iProg );
    public delegate void dl_ShowTime    ( TimeSpan tSpan );
    public delegate void dl_IncFileCount( int iCnt );

    public class DirType
    {
        public string src;
        public string dst;
    }

    public class PathType
    {
        public string path;
        public bool   forced;

        public PathType( string a_Path, bool a_Forced )
        {
            path   = a_Path;
            forced = a_Forced;
        }
    }

    public class Backup
    {
        /***************************************************************************
        SPECIFICATION: Members 
        CREATED:       2014
        LAST CHANGE:   28.06.2016
        ***************************************************************************/
        public event dl_BuReady      m_eBuReady;
        public event dl_ShowDir      m_eShowDir;
        public event dl_ShowFile     m_eShowFile;
        public event dl_ShowAllBar   m_eShowABar;
        public event dl_ShowFilBar   m_eShowFBar;
        public event dl_ShowTime     m_eShowTime;
        public event dl_IncFileCount m_eIncFileCount;

        private List<string>    m_tSrcPath;
        private string          m_sSrcPath;
        private string          m_sDstPath;
        private string          m_sProtPath;
        private List<string>    m_aExtSingle;

        private List<string>    m_aExtensions;
        private List<string>    m_aExclusions;
        private List<string>    m_aForced;

        private List<string>    m_aCntExtensions;
        private List<string>    m_aCntExclusions;
        private List<string>    m_aCntForced;

        private List<string>    m_aProt;
        private StreamWriter    m_Prot;
        private bool            m_bIgnoreR;
        public  bool IgnoreR    { set { m_bIgnoreR = value; } }
        private bool            m_bCopyAll;
        public  bool CopyAll    { set { m_bCopyAll = value; } }
        private bool            m_bEvenNewer;
        public  bool EvenNewer  { set { m_bEvenNewer = value; } }
        public  double          m_dMaxTimeDelta;
        private bool            m_bRet;
        private Thread          m_Thrd;
        private bool            m_bRunning;
        public  long            m_iMaxAge;
        private int             m_iNrFiles;
        private int             m_iCurrNrFiles;
        public  bool            m_bSkipCount;
        private Stopwatch       m_tTimer;
        private Mutex           m_IncMutex;

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   28.06.2016
        ***************************************************************************/
        public Backup()
        {
            m_tSrcPath   = new List<string>();

            m_bIgnoreR   = false;
            m_bCopyAll   = false;
            m_bEvenNewer = false;
            m_iNrFiles   = 0;
            m_iCurrNrFiles = 0;

            m_dMaxTimeDelta = 0.0;
            m_iMaxAge       = -1;

            m_IncMutex      = new Mutex();

            m_aExtSingle     = new List<string>();
            m_aExtensions    = new List<string>();
            m_aExclusions    = new List<string>();
            m_aForced        = new List<string>();
            m_aCntExtensions = new List<string>();
            m_aCntExclusions = new List<string>();
            m_aCntForced     = new List<string>();
            m_aProt          = new List<string>();

            m_eIncFileCount += new dl_IncFileCount( IncFileCount );

            m_aExtSingle.Add("*.*");

            m_tTimer = new Stopwatch();
        }

        /***************************************************************************
        SPECIFICATION: D'tor
        CREATED:       20.04.2009
        LAST CHANGE:   20.04.2009
        ***************************************************************************/
        ~Backup()
        {
            m_eIncFileCount -= new dl_IncFileCount( IncFileCount );
        }

        public void SetDstPath (string sPath)  { m_sDstPath  = CompletePath(sPath); }
        public void SetProtPath(string sPath)  { m_sProtPath = sPath; }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2007
        LAST CHANGE:   28.06.2016
        ***************************************************************************/
        public void SetSrcFiles(List<string> tPath) 
        { 
            m_tSrcPath.Clear();
            m_tSrcPath.AddRange( tPath );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.03.2007
        LAST CHANGE:   18.03.2007
        ***************************************************************************/
        public void SetSrcPath(string sPath)
        {
            m_tSrcPath.Clear();
            m_tSrcPath.Add(sPath);
            m_sSrcPath = sPath;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       13.11.2006
        LAST CHANGE:   13.11.2006
        ***************************************************************************/
        private string CompletePath(string aPath)
        {
            int len = aPath.Length; 
            if (0 == len) return aPath;

            if (aPath[len-1] != '\\')
            {
                return aPath + "\\";
            }
            return aPath;
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.04.2009
        LAST CHANGE:   15.11.2010
        ***************************************************************************/
        private void IncFileCount( int iCnt )
        {
            if (0 == iCnt) return;

            m_IncMutex.WaitOne();
            m_iNrFiles += iCnt;
            m_IncMutex.ReleaseMutex();
        }


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       18.04.2009
        LAST CHANGE:   16.11.2010
        ***************************************************************************/
        private void CountFiles(string sPath)
        {
            if( m_bSkipCount ) return;

            Thread CntThrd = new Thread( new ParameterizedThreadStart( CountFilesThread ) );

            CntThrd.Priority = ThreadPriority.Highest; // 16.11.2010
            CntThrd.Start( new PathType( sPath, Forced( sPath )) );
        }

        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   10.07.2017  
        ***************************************************************************/
        private void BuThread()
        {
            m_aProt.Clear();
            m_aProt.Capacity = 50000000;
            bool showed = false;
            m_eShowFile("counting files ...",false);

            foreach ( string path in m_tSrcPath )
            {
                CountFiles(path);
            }

            m_eShowTime(new TimeSpan(0));
            m_tTimer.Reset();
            m_tTimer.Start();

            if ( null != m_eShowFBar ) m_eShowFBar(0);
            if ( null != m_eShowABar ) m_eShowABar(0,0);

            foreach ( string path in m_tSrcPath )
            {
                if (! showed) 
                {
                    m_eShowFile("copying ...",false);
                    showed = true;
                }
                m_bRet = TraverseTree( new PathType( path, Forced( path )) );
            }

            m_eShowTime( m_tTimer.Elapsed );
            m_tTimer.Stop();

            if ( null != m_eShowFBar ) m_eShowFBar(0);
            if ( null != m_eShowABar ) m_eShowABar(0,0);

            try
            {
                m_Prot = new StreamWriter(m_sProtPath);

                foreach (object line in m_aProt)
                {
                    m_Prot.WriteLine((string)line);
                }

                m_Prot.Close();

                m_eBuReady();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error in writing protocol file");
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   21.12.2009
        ***************************************************************************/
        public bool Start()
        {
            //if (Utils.NoFolder(m_sSrcPath)) return false;

            m_Thrd = new Thread(new ThreadStart(BuThread));

            m_bRunning      = true;
            m_iNrFiles      = 0;
            m_iCurrNrFiles  = 0;

            m_Thrd.Start();

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       2005
        LAST CHANGE:   20.04.2009
        ***************************************************************************/
        public void Stop()
        {
            m_bRunning     = false;
            m_iNrFiles     = 0;
            m_iCurrNrFiles = 0;
        }

        public void SetIgnoreR(bool i_bIgnoreR)
        {
            m_bIgnoreR = i_bIgnoreR;
        }

        /***************************************************************************
        SPECIFICATION: Subroutine for TraverseTree
        CREATED:       27.08.2005
        LAST CHANGE:   15.04.2009
        ***************************************************************************/
        private void CopyFile( String sPrefix, String sFile, String sDstFile )
        {
            DateTime ftime = File.GetLastWriteTime(sFile);  // save write time

            if (null != m_eShowFile) m_eShowFile(sFile,false);
            //File.Copy(sFile,sDstFile,true);
            CopyFileProgress( sFile, sDstFile );
            m_aProt.Add( sPrefix + sDstFile );
            m_eShowTime( m_tTimer.Elapsed );

            File.SetLastWriteTime(sDstFile,ftime);  // restore write time of source file
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       26.06.2008
        LAST CHANGE:   15.07.2008
        ***************************************************************************/
        const int BLOCK_SIZE = 0x2FFFF;

        private void CopyFileProgress( String sSrcFile, String sDstFile )
        {
            byte[] buf = new byte[BLOCK_SIZE];

            FileStream rd = new FileStream( sSrcFile, FileMode.Open, FileAccess.Read );
            CreateDirs( sDstFile );
            FileStream wr = new FileStream( sDstFile, FileMode.Create );

            long len   = rd.Length;
            long loops = len / BLOCK_SIZE;

            for ( int i = 0; i < loops; i++ )
            {
                rd.Read ( buf, 0, BLOCK_SIZE );
                m_eShowFBar( (int)(i * 100 / loops) );
                m_eShowTime( m_tTimer.Elapsed );
                wr.Write( buf, 0, BLOCK_SIZE );
            }

            rd.Read  ( buf, 0, (int)len % BLOCK_SIZE );
            wr.Write ( buf, 0, (int)len % BLOCK_SIZE );
            m_eShowFBar ( 100 );

            rd.Close();
            wr.Close();
        }

        /***************************************************************************
        * SPECIFICATION: 
        * CREATED:       11.09.2005
        * LAST CHANGE:   28.06.2016
        ***************************************************************************/
        public void CountFilesThread( object a_PathTyp )
        {
            PathType pt = (PathType)a_PathTyp;

            try
            {
                //Thread.Sleep(0);
                if( Excluded( pt.path ) ) return;

                if ( File.Exists(pt.path) )
                {   // if not a directory but a file just increment and return
                    m_eIncFileCount(1);
                    return;
                }

                #if NEW_ALG

                foreach (string sExt in m_aCntExtensions)
                {
                    string[] sFiles = Directory.GetFiles( sPath, sExt, SearchOption.AllDirectories );
                    m_eIncFileCount(sFiles.Length);
                }

                #else

                if (Directory.GetFiles(pt.path).Length != 0)  // if dir. contains single files
                {
                    string[] files;

                    if (pt.forced)
                    {
                        files = Directory.GetFiles( pt.path );
                        m_eIncFileCount( files.Length );
                    }
                    else 
                    {
                        foreach (string sExt in m_aCntExtensions)
                        {
                            files = Directory.GetFiles( pt.path, sExt );
                            m_eIncFileCount( files.Length );
                        }
                    }
                }

                string[] sDirs = Directory.GetDirectories( pt.path );

                foreach ( string dir in sDirs )
                {
                    CountFilesThread( new PathType( dir, pt.forced || Forced( dir ) ) );
                }

                #endif
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in counting files");
            }
        }


        //private static void CountFiles( string sPath, string sExt )
        //{
        //    string[] sFiles = Directory.GetFiles( sPath, sExt );
        //    m_eIncFileCount( sFiles.Length );
        //}

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       12.11.2006
        LAST CHANGE:   21.06.2016
        ***************************************************************************/
        private bool Excluded(string sFile)
        {
            if (0 == m_aExclusions.Count) return false;

            string fname = Utils.GetFilename(sFile);

            string ex = m_aExclusions.Find( e => e == fname );

            if (ex == null) return false;

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       6/28/2016
        LAST CHANGE:   28.06.2016
        ***************************************************************************/
        private bool Forced(string sDir)
        {
            if (0 == m_aForced.Count) return false;

            string dir = Utils.GetFilename(sDir);

            string fc = m_aForced.Find( f => f == dir );

            if (fc == null) return false;

            return true;
        }

        /***************************************************************************
        SPECIFICATION: Creates all subdirectories in the path
        CREATED:       15.11.2010
        LAST CHANGE:   15.11.2010
        ***************************************************************************/
        private void CreateDirs(string sFilePath)
        {
            try
            {
                if (File.Exists(sFilePath)) return;

                int    iLastIdx  = sFilePath.LastIndexOf("\\");
                string path      = sFilePath.Substring(0,iLastIdx);

                Directory.CreateDirectory(path);
            }
            catch( Exception ex )
            {
                MessageBox.Show( ex.Message,"Error in creating directories in path: " + sFilePath );
            }
                
        }


        /***************************************************************************
        * SPECIFICATION: Recursive call for 
        * CREATED:       30.10.2004
        * LAST CHANGE:   05.06.2016
        * *************************************************************************/
        private bool TraverseTree( PathType a_PT )
        {
            #if ! NEW_ALG

            try
            {
                if (! m_bRunning) return false;

                if( Excluded( a_PT.path ) ) return true;

                if (null != m_eShowDir) m_eShowDir(a_PT.path);

                string[] sDirs;
                bool     bIsDir = Directory.Exists(a_PT.path);

                if ( bIsDir )  sDirs = Directory.GetDirectories(a_PT.path);
                else	       sDirs = new string[0];

                string sNewPath = Utils.ConcatPaths( m_sDstPath, a_PT.path.Substring(m_sSrcPath.Length) );
                string[] sFiles = null;

                if (a_PT.forced)
                {
                    if ( bIsDir )
                    {
                        sFiles = Directory.GetFiles( a_PT.path );
                    }
                    else
                    {
                        sFiles = new string[1];
                        sFiles[0] = a_PT.path;
                    }

                    if (! TraverseFiles( sFiles, sNewPath, bIsDir ) ) return false;
                }
                else
                {
                    foreach ( string sExt in m_aExtensions )
                    {
                        Thread.Sleep(0);

                        if ( bIsDir )
                        {
                            sFiles = Directory.GetFiles( a_PT.path, sExt );
                        }
                        else
                        {
                            sFiles = new string[1];
                            sFiles[0] = a_PT.path;
                        }

                        if ( ! TraverseFiles( sFiles, sNewPath, bIsDir ) ) return false;
                    }
                }


                foreach (string sDir in sDirs)
                {
                    if (! TraverseTree( new PathType ( sDir, Forced( sDir ) || a_PT.forced ) ) ) return false;  // Recursive call
                }

                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Common error");
                return false;
            }

            #else

            try
            {
                if( !m_bRunning ) return false;

                // if( null != m_eShowDir ) m_eShowDir( sPath );

                bool bIsDir = Directory.Exists( sPath );

                string sNewPath = Utils.ConcatPaths( m_sDstPath, sPath.Substring( m_sSrcPath.Length ) );

                foreach( string sExt in m_aExtensions )
                {
                    Thread.Sleep( 0 );

                    string[] sFiles;

                    if( bIsDir )
                    {
                        sFiles = Directory.GetFiles( sPath, sExt, SearchOption.AllDirectories );
                    }
                    else
                    {
                        sFiles = new string[1];
                        sFiles[0] = sPath;
                    }

                    foreach( string sFile in sFiles )
                    {
                        m_IncMutex.WaitOne();
                        int iNrFiles = m_iNrFiles;
                        m_IncMutex.ReleaseMutex();

                        if( !m_bSkipCount && null != m_eShowABar && 0 != iNrFiles )
                        {
                            m_eShowABar( m_iCurrNrFiles++, iNrFiles );
                        }

                        string sFileName = sFile.Substring( m_sSrcPath.Length );  // 09.09.2010

                        if( Excluded( sFileName ) ) continue;

                        string sNewFile = Utils.ConcatPaths( m_sDstPath, sFileName );

                        System.DateTime odt = File.GetLastWriteTime( sFile );
                        System.DateTime crt = System.DateTime.Now;
                        System.TimeSpan age = crt - odt;

                        if( ( m_iMaxAge > -1 ) && ( m_iMaxAge < age.Days ) )
                        {
                            continue;
                        }

                        try
                        {
                            if( File.Exists( sNewFile ) )
                            {
                                System.DateTime ndt = File.GetLastWriteTime( sNewFile );
                                System.TimeSpan ddt = odt - ndt;

                                if( m_bEvenNewer || ( ddt.TotalSeconds > m_dMaxTimeDelta ) )
                                {
                                    if( m_bIgnoreR )
                                    {
                                        File.SetAttributes( sNewFile, FileAttributes.Normal );
                                    }
                                    CopyFile( "old: ", sFile, sNewFile ); 
                                }
                            }
                            else
                            {
                                //string sNewDir;

                                //if( bIsDir ) sNewDir = sNewPath;
                                //else sNewDir = Utils.GetPath( sNewPath );

                                //if( !Directory.Exists( sNewDir ) )
                                //{
                                //    Directory.CreateDirectory( sNewDir );
                                //}
                                CopyFile( "new: ", sFile, sNewFile );
                            }
                        }
                        catch( UnauthorizedAccessException e )
                        {
                            DialogResult dr = MessageBox.Show( e.Message + "\n Possibly the R attribute is set.", "Unauthorized Access", MessageBoxButtons.AbortRetryIgnore );
                            if( DialogResult.Abort == dr ) return false;
                        }
                        catch( Exception e )
                        {
                            DialogResult dr = MessageBox.Show( e.Message, "Common copy error", MessageBoxButtons.AbortRetryIgnore );
                            if( DialogResult.Abort == dr ) return false;
                        }
                    }
                }

                //foreach( string sDir in sDirs )
                //{
                //    if( !TraverseTree( sDir ) ) return false;  // Recursive call
                //}

                return true;
            }
            catch( Exception e )
            {
                MessageBox.Show( e.Message, "Common error" );
                return false;
            }

            #endif
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       6/30/2016
        LAST CHANGE:   6/30/2016
        ***************************************************************************/
        private bool TraverseFiles( string[] sFiles, string sNewPath, bool bIsDir )
        {
            for (int i=0; i<sFiles.Length; i++)
            {
                string sFile = sFiles[i];

                m_IncMutex.WaitOne();
                int iNrFiles = m_iNrFiles;
                m_IncMutex.ReleaseMutex();

                if( !m_bSkipCount && null != m_eShowABar && 0 != iNrFiles ) 
                {
                    m_eShowABar( m_iCurrNrFiles++, iNrFiles );
                }

                if (! CopyPath( sFile, sNewPath, bIsDir, ref i ) ) return false;
            }
            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       6/29/2016
        LAST CHANGE:   6/29/2016
        ***************************************************************************/
        private bool CopyPath( string a_SrcPath, string a_DstPath, bool a_IsDir, ref int a_Idx )
        {
            string sFileName = a_SrcPath.Substring(m_sSrcPath.Length);  // 09.09.2010

            if(Excluded(sFileName)) return true;

            string sNewFile = Utils.ConcatPaths(m_sDstPath,sFileName);

            System.DateTime odt = File.GetLastWriteTime(a_SrcPath); 
            System.DateTime crt = System.DateTime.Now;
            System.TimeSpan age = crt - odt;

            if ((m_iMaxAge > -1) && (m_iMaxAge < age.Days)) 
            {
                return true;
            }

            try
            {
                if (File.Exists(sNewFile))
                {
                    System.DateTime ndt = File.GetLastWriteTime(sNewFile);
                    System.TimeSpan ddt = odt - ndt;

                    bool eq = true;
                    
                    eq = Utils.FilesREqual( a_SrcPath, sNewFile );

                    if ( (!eq && m_bEvenNewer) || (ddt.TotalSeconds > m_dMaxTimeDelta))
                    {
                        if (m_bIgnoreR) 
                        {
                            File.SetAttributes(sNewFile,FileAttributes.Normal);
                        }
                        CopyFile("old: ", a_SrcPath, sNewFile);
                    }
                }
                else
                {
                    string sNewDir;

                    if (a_IsDir)  sNewDir = a_DstPath;
                    else          sNewDir = Utils.GetPath(a_DstPath);

                    if ( !Directory.Exists(sNewDir) ) 
                    {
                        Directory.CreateDirectory(sNewDir);
                    }
                    CopyFile("new: ", a_SrcPath, sNewFile);
                }
            }
            catch(UnauthorizedAccessException e)
            {
                DialogResult dr = MessageBox.Show(e.Message + "\n Possibly the R attribute is set.","Unauthorized Access",MessageBoxButtons.AbortRetryIgnore);
                if (DialogResult.Abort == dr) return false;
            }
            catch( IOException e )
            {
                DialogResult dr = MessageBox.Show( e.Message, "IO exception error", MessageBoxButtons.AbortRetryIgnore );
                switch( dr )
                {
                    case DialogResult.Abort: return false;
                    case DialogResult.Retry: a_Idx--; return true;
                }
            }
            catch(Exception e)
            {
                DialogResult dr = MessageBox.Show(e.Message,"Common copy error",MessageBoxButtons.AbortRetryIgnore);
                if (DialogResult.Abort == dr) return false;
            }

            return true;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   28.06.2016 
        ***************************************************************************/
        public void ClearExts()
        {
            m_aExtensions   .Clear();
            m_aExclusions   .Clear();
            m_aCntExtensions.Clear();
            m_aCntExclusions.Clear();
            m_aCntForced    .Clear();
            m_aForced       .Clear();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   28.06.2016
        ***************************************************************************/
        public void AddExt(string sExt)
        {
            if (null == sExt)    return;
            if (""   == sExt)    return;

            switch( sExt[0] )
            {
                case '!':
                        m_aExclusions   .Add( sExt.Substring( 1 ) );
                        m_aCntExclusions.Add( sExt.Substring( 1 ) );
                        break;

                case '#':
                        m_aForced       .Add( sExt.Substring( 1 ) );
                        m_aCntForced    .Add( sExt.Substring( 1 ) );
                        break;

                default:
                        m_aExtensions   .Add( sExt );
                        m_aCntExtensions.Add( sExt );
                        break;
            }
        }
    } // class    
} // namespace



 
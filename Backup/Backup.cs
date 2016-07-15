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
	
    public delegate void dl_BuReady();
    public delegate void dl_ShowDir     ( String sDir );
    public delegate void dl_ShowFile    ( String sFname, bool bAppend );
    public delegate void dl_ShowAllBar  ( int iCurrFiles, int iAllFiles );
    public delegate void dl_ShowFilBar  ( int iProg );
    public delegate void dl_ShowTime    ( TimeSpan tSpan );
    public delegate void dl_IncFileCount( int iCnt );

    public class Backup
    {
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
        private List<string>    m_aCntExtensions;
        private List<string>    m_aCntExclusions;
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
        LAST CHANGE:   20.04.2009
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
            m_aCntExtensions = new List<string>();
            m_aCntExclusions = new List<string>();
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

        public void SetDstPath(string sPath)  { m_sDstPath  = CompletePath(sPath); }
        public void SetProtPath(string sPath) { m_sProtPath = sPath; }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       14.03.2007
        LAST CHANGE:   14.03.2007
        ***************************************************************************/
        public void SetSrcFiles(List<string> tPath) 
        { 
            m_tSrcPath.Clear();

            foreach (string p in tPath)
            {
                m_tSrcPath.Add(p); 
            }
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
            CntThrd.Start(sPath);
        }

        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       ?
        LAST CHANGE:   24.04.2009  
        ***************************************************************************/
        private void BuThread()
        {
            m_aProt.Clear();
            m_aProt.Capacity = 50000000;
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
                m_eShowFile("copying ...",false);
                m_bRet = TraverseTree(path);
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
        CREATED:       05.07.2008
        LAST CHANGE:   05.07.2008
        ***************************************************************************/
        private void TimerThread()
        {

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
        * LAST CHANGE:   15.11.2010
        ***************************************************************************/
        public void CountFilesThread( object oPath )
        {
            string sPath = (string)oPath;

            try
            {
                if ( File.Exists(sPath) )
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

                if (Directory.GetFiles(sPath).Length != 0)  // if dir. contains single files
                {
                    foreach (string sExt in m_aCntExtensions)
                    {
                        string[] sFiles = Directory.GetFiles( sPath, sExt );
                        m_eIncFileCount(sFiles.Length);
                    }
                }

                string[] sDirs = Directory.GetDirectories(sPath);

                foreach ( string sDir in sDirs )
                {
                    CountFilesThread( sDir );
                }

                #endif
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"Error in counting files");
            }
        }
        
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       12.11.2006
        LAST CHANGE:   18.11.2010
        ***************************************************************************/
        private bool Excluded(string sFile)
        {
            if (0 == m_aExclusions.Count) return false;

            string fname = Utils.GetFilename(sFile);

            foreach ( string f in m_aExclusions )
            {
                if ( f == fname ) return true;
            }

            return false;
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
        * LAST CHANGE:   09.09.2010
        * *************************************************************************/
        private bool TraverseTree(string sPath)
        {
            #if ! NEW_ALG

            try
            {
                if (! m_bRunning) return false;

                if (null != m_eShowDir) m_eShowDir(sPath);

                string[] sDirs;
                bool     bIsDir = Directory.Exists(sPath);

                List<string> aExts;

                if ( bIsDir )
                {
                    sDirs = Directory.GetDirectories(sPath);
                    aExts = m_aExtensions;
                }
                else
                {
                    sDirs = new string[0];
                    aExts = m_aExtensions;
                }

                string sNewPath = Utils.ConcatPaths(m_sDstPath,sPath.Substring(m_sSrcPath.Length));

                foreach ( string sExt in aExts )
                {
                    Thread.Sleep(0);

                    string[] sFiles;

                    if ( bIsDir )
                    {
                        sFiles = Directory.GetFiles(sPath,sExt);
                    }
                    else
                    {
                        sFiles = new string[1];
                        sFiles[0] = sPath;
                    }

                    foreach (string sFile in sFiles)
                    {
                        m_IncMutex.WaitOne();
                        int iNrFiles = m_iNrFiles;
                        m_IncMutex.ReleaseMutex();

                        if( !m_bSkipCount && null != m_eShowABar && 0 != iNrFiles ) 
                        {
                            m_eShowABar( m_iCurrNrFiles++, iNrFiles );
                        }

                        string sFileName = sFile.Substring(m_sSrcPath.Length);  // 09.09.2010

                        if(Excluded(sFileName)) continue;

                        string sNewFile = Utils.ConcatPaths(m_sDstPath,sFileName);

                        System.DateTime odt = File.GetLastWriteTime(sFile); 
                        System.DateTime crt = System.DateTime.Now;
                        System.TimeSpan age = crt - odt;

                        if ((m_iMaxAge > -1) && (m_iMaxAge < age.Days)) 
                        {
                            continue;
                        }

                        try
                        {
                            if (File.Exists(sNewFile))
                            {
                                System.DateTime ndt = File.GetLastWriteTime(sNewFile);
                                System.TimeSpan ddt = odt - ndt;

                                if (m_bEvenNewer || (ddt.TotalSeconds > m_dMaxTimeDelta))
                                {
                                    if (m_bIgnoreR) 
                                    {
                                        File.SetAttributes(sNewFile,FileAttributes.Normal);
                                    }
                                    CopyFile("old: ", sFile, sNewFile);
                                }
                            }
                            else
                            {
                                string sNewDir;

                                if (bIsDir)  sNewDir = sNewPath;
                                else         sNewDir = Utils.GetPath(sNewPath);

                                if ( !Directory.Exists(sNewDir) ) 
                                {
                                    Directory.CreateDirectory(sNewDir);
                                }
                                CopyFile("new: ", sFile, sNewFile);
                            }
                        }
                        catch(UnauthorizedAccessException e)
                        {
                            DialogResult dr = MessageBox.Show(e.Message + "\n Possibly the R attribute is set.","Unauthorized Access",MessageBoxButtons.AbortRetryIgnore);
                            if (DialogResult.Abort == dr) return false;
                        }
                        catch(Exception e)
                        {
                            DialogResult dr = MessageBox.Show(e.Message,"Common copy error",MessageBoxButtons.AbortRetryIgnore);
                            if (DialogResult.Abort == dr) return false;
                        }
                    }
                }

                foreach (string sDir in sDirs)
                {
                    if (! TraverseTree(sDir)) return false;  // Recursive call
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
        CREATED:       30.10.2004
        LAST CHANGE:   20.04.2009 
        ***************************************************************************/
        public void ClearExts()
        {
            m_aExtensions.Clear();
            m_aExclusions.Clear();
            m_aCntExtensions.Clear();
            m_aCntExclusions.Clear();
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       30.10.2004
        LAST CHANGE:   30.10.2004
        ***************************************************************************/
        public void AddExt(string sExt)
        {
            if (null == sExt)    return;
            if (""   == sExt)    return;
            if( sExt[0] == '!' )
            {
                m_aExclusions   .Add( sExt.Substring( 1 ) );
                m_aCntExclusions.Add( sExt.Substring( 1 ) );
            }
            else
            {
                m_aExtensions   .Add( sExt );
                m_aCntExtensions.Add( sExt );
            }
        }
    }    
}



 
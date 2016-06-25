using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace NS_Utilities
{
    public class UserTimer
    {
        /***************************************************************************
        SPECIFICATION: Members
        CREATED:       11.09.2013
        LAST CHANGE:   11.09.2013
        ***************************************************************************/
        public delegate void dl_ExpiredHandler( int iTime );
        public event dl_ExpiredHandler m_eExpiredHandler;

        private System.Threading.Timer m_tTimer;
        private bool                   m_bExpired;
        private bool                   m_bRunning;
        private int                    m_iTime;
        
        /***************************************************************************
        SPECIFICATION: C'tor
        CREATED:       11.09.2013
        LAST CHANGE:   12.09.2013
        ***************************************************************************/
        public UserTimer()
        {
            TimerCallback tcb = new TimerCallback( Timer_Tick );
            m_tTimer = new System.Threading.Timer( tcb );
            m_bExpired = false;
            m_bRunning = false;
        }
    
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2013
        LAST CHANGE:   11.09.2013
        ***************************************************************************/
        private void Timer_Tick( Object tObj )
        {
            bool running;
            lock(this) running = m_bRunning;
            if ( ! running ) return;

            lock(this) 
            {
                m_bExpired = true;
                m_bRunning = false;
            }

            Debug.WriteLine( "Timer expired ({0} ms)", m_iTime );
            if ( m_eExpiredHandler != null ) m_eExpiredHandler( m_iTime );
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2013
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        public void Start( int a_iMilliSec )
        {
            lock(this)
            {
                m_bExpired = false;
                m_bRunning = true;
                m_iTime    = a_iMilliSec;
                m_tTimer.Change( a_iMilliSec, Timeout.Infinite );
            }

            Debug.WriteLine( "Timer started ({0} ms)", m_iTime );
        }   


        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2013
        LAST CHANGE:   30.01.2014
        ***************************************************************************/
        public void Stop()
        {
            Debug.WriteLine( "Timer stopped ({0} ms)", m_iTime );

            lock(this) 
            {
                m_bRunning = false;
                m_bExpired = false;
            }
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       11.09.2013
        LAST CHANGE:   11.09.2013
        ***************************************************************************/
        public bool Expired()
        {
            bool ret;

            lock(this) ret = m_bExpired;

            return ret;
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       12.09.2013
        LAST CHANGE:   12.09.2013
        ***************************************************************************/
        public bool Running()
        {
            bool ret;

            lock( this ) ret = m_bRunning;

            return ret;
        }
    }
}

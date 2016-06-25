using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace NS_UserSocket
{
    public class UserSocket
    {
        /***************************************************************************
        SPECIFICATION: 
        CREATED:       23.03.2009
        LAST CHANGE:   23.03.2009
        ***************************************************************************/
        public UserSocket()
        {
            
        }

        /***************************************************************************
        SPECIFICATION: 
        CREATED:       20.03.2009
        LAST CHANGE:   20.03.2009
        ***************************************************************************/
        public String GetIP()
        {
            String strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry( strHostName );

            // Grab the first IP addresses
            String IPStr = "";
            foreach( IPAddress ipaddress in iphostentry.AddressList )
            {
                IPStr = ipaddress.ToString();
                return IPStr;
            }
            return IPStr;
        }
    }
}

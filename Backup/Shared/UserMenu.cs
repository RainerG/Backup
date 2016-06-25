using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace NS_UserMenu
{
    public partial class UserContextMenu: ContextMenu
    {
        /***************************************************************************
        SPECIFICATION: default c'tor
        CREATED:       22.06.2007
        LAST CHANGE:   22.06.2007
        ***************************************************************************/
        public UserContextMenu()
            :base()
        {
        }

        /***************************************************************************
        SPECIFICATION: c'tor for building up a complete context menu
        CREATED:       22.06.2007
        LAST CHANGE:   22.06.2007
        ***************************************************************************/
        public UserContextMenu(List<string>a_EntryNames,EventHandler a_EvtHandler)
            :base()
        {
            Menu.MenuItemCollection mc = MenuItems;

            foreach( string s in a_EntryNames )
            {
                mc.Add(s);
            }

            foreach ( MenuItem mi in mc )
            {
                mi.Click += new EventHandler(a_EvtHandler);
            }


        }
    }
}

using System;
using System.IO;
using System.Windows.Forms;

namespace Backup
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		public Utils()
		{
			//
			// TODO: Add constructor logic here
			//
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

    }
}



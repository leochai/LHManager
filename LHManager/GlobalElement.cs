using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LHManager
{
    public partial class Form1
    {
        public static DateTime _lastRecallTime = new DateTime();
        public static LHSerialPort RS485 = new LHSerialPort("COM3", 1200, Parity.Even, 8, StopBits.One);
        public static LHUnit[] _unit = new LHUnit[24];
        public static OleDbConnection _DBconn = new OleDbConnection("Provider=Microsoft.Ace.OleDb.12.0;Data Source=" + Application.StartupPath + "/db/老化台.accdb");
        public static byte[] _readBuffer = new byte[32];
    }
}

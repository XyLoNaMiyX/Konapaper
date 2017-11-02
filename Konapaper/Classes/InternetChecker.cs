using System;
using System.Runtime.InteropServices;

namespace Konapaper
{
    public static class InternetChecker
    {
        [DllImport("wininet.dll", SetLastError = true)]
        extern static bool InternetGetConnectedState(out int lpdwFlags, int dwReserved);

        [Flags]
        public enum ConnectionStates
        {
            Modem = 0x1,
            LAN = 0x2,
            Proxy = 0x4,
            RasInstalled = 0x10,
            Offline = 0x20,
            Configured = 0x40,
        }

        public static ConnectionStates GetInternetState()
        {
            int flags;
            bool isConnected = InternetGetConnectedState(out flags, 0);
            return (ConnectionStates)flags;
        }

        public static bool IsConnected()
        {
            int flags;
            return InternetGetConnectedState(out flags, 0);
        }
    }
}

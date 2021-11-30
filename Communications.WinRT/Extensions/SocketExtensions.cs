using Common.Net.Network.Enumerations;
using LogUtils.Net;
using Windows.Networking.Sockets;

namespace Communications.WinRT.Extensions {

    public static class SocketExtensions {

        public static SocketErrCode Convert(this SocketErrorStatus code) {
            // We know that mapping is 100%
            return (SocketErrCode)((int)(code));
        }


        public static SocketErrCode GetSocketCode(this Exception e) {
            try {
                var baseEx = e.GetBaseException();
                if (baseEx != null) {
                    return SocketError.GetStatus(baseEx.HResult).Convert();
                }
            }
            catch (Exception ex) {
                Log.Exception(9999, "Bad Socket Exception", ex);
            }
            return SocketErrCode.Unknown;
        }


    }
}

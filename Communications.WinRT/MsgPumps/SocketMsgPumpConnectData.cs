using Windows.Networking.Sockets;

namespace Communications.WinRT.MsgPumps {

    public class SocketMsgPumpConnectData {
        public string RemoteHostName { get; set; }
        public string ServiceName { get; set; }
        public SocketProtectionLevel ProtectionLevel { get; set; } = SocketProtectionLevel.PlainSocket;
        public uint MaxReadBufferSize { get; set; } = 256;


        public SocketMsgPumpConnectData(
            string host, 
            string service, 
            SocketProtectionLevel level, 
            uint maxBuffSize) { 
        
            this.RemoteHostName = host;
            this.ServiceName = service;
            this.ProtectionLevel = level;
            this.MaxReadBufferSize = maxBuffSize;
        }


    }
}

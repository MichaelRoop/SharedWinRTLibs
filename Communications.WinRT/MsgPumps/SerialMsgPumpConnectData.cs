using Windows.Storage.Streams;

namespace Communications.WinRT.MsgPumps {

    public class SerialMsgPumpConnectData {
        public IInputStream InStream { get; set; }
        public IOutputStream OutStream { get; set; }
        public uint MaxReadBufferSize { get; set; } = 250;

        public SerialMsgPumpConnectData(
            IInputStream inStream,
            IOutputStream outStream,
            uint maxBuffSize) {
            this.InStream = inStream;
            this.OutStream = outStream;
            this.MaxReadBufferSize = maxBuffSize;
        }

    }
}

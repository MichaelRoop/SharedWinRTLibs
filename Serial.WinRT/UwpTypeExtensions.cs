using SerialCommon.Net.Enumerations;
using Windows.Devices.SerialCommunication;

namespace Serial.UWP.Core {

    public static class UwpTypeExtensions {


        public static SerialStopBits Convert(this SerialStopBitCount sb){
            return sb switch {
                SerialStopBitCount.One => SerialStopBits.One,
                SerialStopBitCount.OnePointFive => SerialStopBits.OnePointFive,
                SerialStopBitCount.Two => SerialStopBits.Two,
                _ => SerialStopBits.One,
            };
        }


        public static SerialStopBitCount Convert(this SerialStopBits sb) {
            return sb switch {
                SerialStopBits.One => SerialStopBitCount.One,
                SerialStopBits.OnePointFive => SerialStopBitCount.OnePointFive,
                SerialStopBits.Two => SerialStopBitCount.Two,
                _ => SerialStopBitCount.One,
            };
        }


        public static SerialParityType Convert(this SerialParity sp) {
            return sp switch {
                SerialParity.None => SerialParityType.None,
                SerialParity.Odd => SerialParityType.Odd,
                SerialParity.Even => SerialParityType.Even,
                SerialParity.Mark => SerialParityType.Mark,
                SerialParity.Space => SerialParityType.Space,
                _ => SerialParityType.None,
            };
        }


        public static SerialParity Convert(this SerialParityType sp) {
            return sp switch {
                SerialParityType.None => SerialParity.None,
                SerialParityType.Odd => SerialParity.Odd,
                SerialParityType.Even => SerialParity.Even,
                SerialParityType.Mark => SerialParity.Mark,
                SerialParityType.Space => SerialParity.Space,
                _ => SerialParity.None,
            };
        }


        public static SerialFlowControlHandshake Convert(this SerialHandshake handshake) {
            return handshake switch {
                SerialHandshake.None => SerialFlowControlHandshake.None,
                SerialHandshake.RequestToSend => SerialFlowControlHandshake.RequestToSend,
                SerialHandshake.XOnXOff => SerialFlowControlHandshake.XonXoff,
                SerialHandshake.RequestToSendXOnXOff => SerialFlowControlHandshake.RequestToSendXonXoff,
                _ => SerialFlowControlHandshake.None,
            };
        }


        public static SerialHandshake Convert(this SerialFlowControlHandshake h) {
            return h switch {
                SerialFlowControlHandshake.None => SerialHandshake.None,
                SerialFlowControlHandshake.RequestToSend => SerialHandshake.RequestToSend,
                SerialFlowControlHandshake.XonXoff => SerialHandshake.XOnXOff,
                SerialFlowControlHandshake.RequestToSendXonXoff => SerialHandshake.RequestToSendXOnXOff,
                _ => SerialHandshake.None,
            };
        }

    }
}

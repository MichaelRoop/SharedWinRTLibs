using Communications.WinRT.MsgPumps;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using Ethernet.Common.Net.interfaces;
using MultiCommData.Net.StorageDataModels;
using Windows.Networking.Sockets;

namespace Ethernet.UWP.Core {
    public partial class EthernetImplUwp :IEthernetInterface {

        public void ConnectAsync(EthernetParams dataModel) {
            this.log.InfoEntry("ConnectAsync ***************");

            if (dataModel.EthernetAddress.Length == 0 || dataModel.EthernetServiceName.Length == 0) {
                // TODO - request params event
                this.log.Error(9999, "Params requested");
                this.ParamsRequestedEvent?.Invoke(this, dataModel);
            }

            // Check for erroron data model
            if (dataModel.EthernetAddress.Length == 0 || dataModel.EthernetServiceName.Length == 0) {
                this.log.Error(9999, "ERROR ON Params requested");
                this.OnError?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.EmptyParams));
            }
            else {
                //this.Disconnect();
                Thread.Sleep(500);

                this.log.Error(9999, "Doing connect");


                // Test Arduino: 192.168.1.88:9999
                this.msgPump.ConnectAsync2(new SocketMsgPumpConnectData(
                    dataModel.EthernetAddress,
                    dataModel.EthernetServiceName,
                    // TODO - determine protection level according to connection
                    SocketProtectionLevel.PlainSocket,
                    100));
            }
        }


    }
}

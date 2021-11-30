using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Networking.Sockets;

namespace BluetoothRFCommUWP {

    // To Use UWP libs just include NuGet package Microsoft.Windows.SDK.Contracts(nn.n.nnnn.n)
    //Not sure if you need to add to manifest. If so, would only be in main App

    public partial class BTRfCommUwp : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BTRfCommUwp");
        private readonly string KEY_CAN_PAIR = "System.Devices.Aep.CanPair";
        private readonly string KEY_IS_PAIRED = "System.Devices.Aep.IsPaired";
        //private readonly string KEY_CONTAINER_ID = "System.Devices.Aep.ContainerId";
        private readonly string KEY_SIGNAL_STRENGTH = "System.Devices.Aep.SignalStrength";
        private static uint READ_BUFF_MAX_SIZE = 256;
        IMsgPump<SocketMsgPumpConnectData> msgPump = new SocketMsgPumpBluetooth();

        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        #endregion

        #region Events

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;
        public event EventHandler<BT_PairInfoRequest> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        #endregion

        #region Constructors

        public BTRfCommUwp() {
            this.msgPump.MsgPumpConnectResultEvent += this.MsgPump_ConnectResultEvent;
            this.msgPump.MsgReceivedEvent += this.MsgPump_MsgReceivedEventHandler;
        }

        #endregion

        #region IBTInterface Methods

        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(BTDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    this.msgPump.Disconnect();

                    await this.GetExtraInfo(deviceDataModel, false, false);

                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", deviceDataModel.RemoteHostName, deviceDataModel.RemoteServiceName));

                    this.msgPump.ConnectAsync(new SocketMsgPumpConnectData() {
                        MaxReadBufferSize = READ_BUFF_MAX_SIZE,
                        ProtectionLevel = SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication,
                        RemoteHostName = deviceDataModel.RemoteHostName,
                        ServiceName = deviceDataModel.RemoteServiceName,
                    });
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    this.ConnectionCompleted?.Invoke(this, false);
                }
            });
        }


        public void Disconnect() {
            try {
                this.msgPump.Disconnect();
                // TODO - check if BT has some things to tear down
            }
            catch (Exception e) {
                this.log.Exception(9999, "Disconnect", "", e);
            }
        }

        #endregion

        #region ICommStackChannel methods

        /// <summary>Write message from ICommStackChannel interface</summary>
        /// <param name="msg">The message to write out</param>
        /// <returns>always true</returns>
        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion

        #region Private tools

        /// <summary>Handle the msg pump connect result</summary>
        private void MsgPump_ConnectResultEvent(object sender, MsgPumpResults results) {
            // TODO - make more generic
            this.Connected = results.Code == MsgPumpResultCode.Connected;
            this.ConnectionCompleted?.Invoke(this, this.Connected);
        }


        /// <summary>Handle the msg pump incoming message</summary>
        private void MsgPump_MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("BtWrapper_MsgReceivedEvent", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        /// <summary>Get the boolean value from a Device Information property</summary>
        /// <param name="property">The device information property</param>
        /// <param name="key">The property key to lookup</param>
        /// <param name="defaultValue">Default value on error</param>
        /// <returns></returns>
        private bool GetBoolProperty(IReadOnlyDictionary<string, object> property, string key, bool defaultValue) {
            if (property.ContainsKey(key)) {
                if (property[key] is Boolean) {
                    return (bool)property[key];
                }
                this.log.Error(9999, () => string.Format(
                    "{0} Property is {1} rather than Boolean", key, property[key].GetType().Name));
            }
            return defaultValue;
        }


        private int GetIntProperty(IReadOnlyDictionary<string, object> property, string key, int defaultValue) {
            if (property.ContainsKey(key)) {
                if (property[key] is int) {
                    return (int)property[key];
                }
                this.log.Error(9999, () => string.Format(
                    "{0} Property is {1} rather than int", key, property[key].GetType().Name));
            }
            return defaultValue;
        }


        #endregion

    }
}

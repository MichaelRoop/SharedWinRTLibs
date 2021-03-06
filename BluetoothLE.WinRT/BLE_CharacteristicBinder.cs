using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using VariousUtils.Net;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Bluetooth.UWP.Core {

    /// <summary>
    /// Bind the BLE Characteristic data model with the UWP characteristic
    /// </summary>
    public class BLE_CharacteristicBinder {

        #region Data

        private const int BLE_BLOCK_SIZE = 20;
        private readonly ClassLog log = new("BLE_CharacteristicBinder");
        private readonly bool subscribed = false;

        #endregion

        #region Properties

        /// <summary>The UWP characteristic</summary>
        public GattCharacteristic OSCharacteristic { get; set; }
        
        /// <summary>The cross platform data model</summary>
        public BLE_CharacteristicDataModel DataModel { get; set; }

        #endregion

        #region Constructor and teardown

        /// <summary>Constructor</summary>
        /// <param name="osCharacteristic">The UWP Characteristic</param>
        /// <param name="dataModel">The cross platform data model</param>
        /// <param name="subscribed">if true then subscribe to the UWP characteristic value changes</param>
        public BLE_CharacteristicBinder(GattCharacteristic osCharacteristic, BLE_CharacteristicDataModel dataModel, bool subscribed) {
            this.subscribed = subscribed;
            this.OSCharacteristic = osCharacteristic;
            this.DataModel = dataModel;
            this.log.InfoEntry("BLE_CharacteristicBinder");
            if (this.subscribed) {
                this.OSCharacteristic.ValueChanged += this.OSCharacteristicReadValueChangedHandler;
            }
            this.DataModel.WriteRequestEvent += this.OnDataModelWriteRequestHandler;
            this.DataModel.ReadRequestEvent += this.OnDataModelReadRequestHandler;
        }


        /// <summary>Remove all attached events</summary>
        public void Teardown() {
            this.log.InfoEntry("Teardown");
            if (this.subscribed) {
                this.OSCharacteristic.ValueChanged -= this.OSCharacteristicReadValueChangedHandler;
            }
            this.DataModel.WriteRequestEvent -= this.OnDataModelWriteRequestHandler;
            this.DataModel.ReadRequestEvent -= this.OnDataModelReadRequestHandler;
        }

        #endregion

        #region Event handlers

        /// <summary>Handles read request from the user via the Characteristic Data Model</summary>
        /// <param name="sender">Originator of request</param>
        /// <param name="args">The event args. Has nothing</param>
        private void OnDataModelReadRequestHandler(object? sender, EventArgs args) {
            Task.Run(async () => {
                try {
                    GattReadResult result = await this.OSCharacteristic.ReadValueAsync();
                    if (this.ParseGattStatue(result.Status)) {
                        //this.log.InfoEntry("onDataModelReadRequestHandler");
                        this.PushReadValue(result.Value);
                    }
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "DataModel_ReadRequestEvent", "", ex);
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.UnknownError);
                }
            });
        }


        /// <summary>Write out the incoming message to the OS characteristic</summary>
        private void OnDataModelWriteRequestHandler(object? sender, byte[] data) {
            Task.Run(async () => {
                try {
                    this.log.Info("onDataModelWriteRequestHandler", () => string.Format("{0}", data.ToFormatedByteString()));
                    if (data.Length <= BLE_BLOCK_SIZE) {
                        if (await this.WriteBlock(data, 0, data.Length) != true) {
                            this.log.Error(9999, "onDataModelWriteRequestHandler", "Failed write single block");
                        }
                        return;
                    }
                    else {
                        int count = data.Length / BLE_BLOCK_SIZE;
                        int rest = (data.Length % BLE_BLOCK_SIZE);
                        int lastIndex = 0;
                        for (int i = 0; i < count; i++) {
                            lastIndex = i * BLE_BLOCK_SIZE;
                            if (await this.WriteBlock(data, lastIndex, BLE_BLOCK_SIZE) != true) {
                                this.log.Error(9999, "onDataModelWriteRequestHandler", "Failed write block");
                                return;
                            }
                        }
                        // Last block if partial block
                        if (lastIndex > 0 && rest > 0) {
                            lastIndex += BLE_BLOCK_SIZE;
                            if (!await this.WriteBlock(data, lastIndex, rest)) {
                                this.log.Error(9999, "onDataModelWriteRequestHandler", "Failed write last block");
                            }
                        }
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "onDataModelWriteRequestHandler", "", e);
                }
            });
        }


        /// <summary>Handle data changed events from the UWP characteristic</summary>
        /// <param name="sender">The originator</param>
        /// <param name="args">The event args with the data buffer and timestamp</param>
        private void OSCharacteristicReadValueChangedHandler(
            GattCharacteristic? sender, GattValueChangedEventArgs args) {
            Task.Run(() => {
                try {
                    this.PushReadValue(args.CharacteristicValue);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "OSCharacteristic_ValueChanged", "", e);
                }
            });
        }

        #endregion

        #region Private

        /// <summary>Write a block of data to the UWP characteristic</summary>
        /// <param name="data">Buffer with data to write</param>
        /// <param name="index">Start index of data to read</param>
        /// <param name="size">Number of bytes to read</param>
        /// <returns>true on success, otherwise false with error raised</returns>
        private async Task<bool> WriteBlock(byte[] data, int index, int size) {
            try {
                this.log.Info("WriteBlock", "Write to Gatt");
                using var ms = new DataWriter();
                byte[] part = new byte[size];
                Array.Copy(data, index, part, 0, part.Length);
                ms.WriteBytes(part);
                GattCommunicationStatus result = await
                    this.OSCharacteristic.WriteValueAsync(ms.DetachBuffer());
                this.log.Info("WriteBlock", () => string.Format("WriteValueAsync result:{0}", result.ToString()));

                return this.ParseGattStatue(result);
            }
            catch (Exception e) {
                this.log.Exception(9999, "WriteBlock", "", e);
                return false;
            }
        }


        /// <summary>Parse communications operation status and raise if error</summary>
        /// <param name="gattStatus">The UWP Gatt status</param>
        /// <returns>true on success, otherwise false where an error raised</returns>
        private bool ParseGattStatue(GattCommunicationStatus gattStatus) {
            switch (gattStatus) {
                case GattCommunicationStatus.Success:
                    return true;
                case GattCommunicationStatus.Unreachable:
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.Unreachable);
                    break;
                case GattCommunicationStatus.ProtocolError:
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.ProtocolError);
                    break;
                case GattCommunicationStatus.AccessDenied:
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.AccessDenied);
                    break;
                default:
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.UnknownError);
                    break;
            }
            return false;
        }



        private void PushReadValue(IBuffer buffer) {
            byte[] data = buffer.FromBufferToBytes();
            string str = this.DataModel.Parser.Parse(data);
            this.DataModel.PushReadDataEvent(data, str);
        }


        #endregion

    }
}

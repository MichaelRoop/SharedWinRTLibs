using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using Common.Net.Network;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        #region IBLETInterface Interface events

        public event EventHandler<BluetoothLEDeviceInfo>? DeviceDiscovered;

        public event EventHandler<string>? DeviceRemoved;

        public event EventHandler<bool>? DeviceDiscoveryCompleted;

        public event EventHandler<NetPropertiesUpdateDataModel>? DeviceUpdated;

        public event EventHandler<BLEGetInfoStatus>? DeviceInfoAssembled;

        public event EventHandler<BLE_ConnectStatusChangeInfo>? ConnectionStatusChanged;

        public event EventHandler<BLEOperationStatus>? BLE_Error;

        #endregion

        #region IBLETInterface:ICommStackChannel events

        // TODO - the read thread on the BLE will raise this
        // TODO - update this. Not using this to pass up bytes. See where it is done
        // When bytes come in
        public event EventHandler<byte[]>? MsgReceivedEvent;

        #endregion


        // TODO - see if we can do without these ToStatisfyCompilerWarnings now
#pragma warning disable IDE0051 // Remove unused private members
        private void ToStatisfyCompilerWarnings() {
#pragma warning restore IDE0051 // Remove unused private members
                               // TODO use this later
            this.MsgReceivedEvent?.Invoke(this, Array.Empty<byte>());
        }


        private void RaiseIfError(BLEOperationStatus status) {
            if (status != BLEOperationStatus.Success) {
                Task.Run(() => {
                    try {
                        BLE_Error?.Invoke(this, status);
                    }
                    catch(Exception e) {
                        this.log.Exception(9999, "Raise if error", "", e);
                    }
                });
            }
        }

    }

}

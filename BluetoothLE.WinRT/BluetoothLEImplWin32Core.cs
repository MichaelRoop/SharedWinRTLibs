using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers.Characteristics;
using BluetoothLE.Net.Parsers.Descriptor;
using LogUtils.Net;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        #region data

        private readonly ManualResetEvent stopped = new(false);

        /// <summary>Allows discovery of device list</summary>
        private DeviceWatcher? devWatcher = null;

        private BluetoothLEDevice? currentDevice = null;
        private readonly List<GattDeviceService> currentServices = new();
        private readonly BLE_CharcteristicsBinderSet binderSet = new();
        private readonly IDescParserFactory descParserfactory = new DescParserFactory();
        private readonly ICharParserFactory charParserFactory = new CharParserFactory();
        private readonly ClassLog log = new("BluetoothLEImplWin32Core");

        #endregion

        #region IBLETInterface:ICommStackChannel methods


        public bool SendOutMsg(byte[] msg) {
            // TODO - send out by some kind of stream to BLE device - see classic
            return false;
        }

        #endregion

        #region Interface methods

        /// <summary>Interface call to discover devices</summary>
        /// <returns>A list of discovered devices.  However I am now using events so it is empty</returns>
        public void DiscoverDevices() {
            this.log.Info("DiscoverDevices", "Doing stuff...");
            this.DoLEWatcherSearch();
        }


        public void CancelDiscoverDevices() {
            this.TearDownWatcher();
        }


        public void Disconnect() {
            this.DoDisconnect();
        }


        public void Connect(BluetoothLEDeviceInfo deviceInfo) {
            // TODO - need to have a copy of the BluetoothLEDeviceInfo saved also which subscribes to the BLE OS Device
            //        info and passes those events up to the UI
            this.Disconnect();
            Task.Run(async () => {
                try {
                    await this.ConnectToDeviceAsync(deviceInfo);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "On Task Run ConnectToDevice", e);
                    this.RaiseConnectAttemptResult(BLEOperationStatus.Failed);
                }
            });
        }

        public void GetInfo(BluetoothLEDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    // Not using the get info anymore so make sure the device is nulled
                    this.Disconnect();
                    await this.HarvestDeviceInfo(deviceDataModel);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "On Task Run GetInfo", e);
                }
            });
        }

        #endregion

        #region Connection to device code



        private void CurrentDevice_NameChanged(BluetoothLEDevice sender, object args) {
            this.log.Info("CurrentDevice_NameChanged", () =>
                string.Format("Device '{0}' name has changed", sender.Name));
        }


        private void CurrentDevice_GattServicesChanged(BluetoothLEDevice sender, object args) {
            this.log.Info("CurrentDevice_NameChanged", () =>
                string.Format("Device '{0}' services have changed", sender.Name));
        }


        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args) {
            Task.Run(() => {
                try {
                    this.log.Info("CurrentDevice_ConnectionStatusChanged", () =>
                        string.Format("Device '{0}' Connection status changed to {1}", sender.Name, sender.ConnectionStatus.ToString()));
                    this.ConnectionStatusChanged?.Invoke(sender, new BLE_ConnectStatusChangeInfo() {
                        Status = sender.ConnectionStatus.Convert(),
                        Message = "",
                    });
                }
                catch(Exception ex) {
                    this.log.Exception(9999, "CurrentDevice_ConnectionStatusChanged", "", ex);
                }
            });
        }

        #endregion


    }
}

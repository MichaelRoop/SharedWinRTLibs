using Common.Net.Network;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothCommon.UWP {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BluetoothExtensions {

        #region Data

        private static ClassLog log = new ClassLog("BluetoothExtensions");

        #endregion

        static BluetoothExtensions() {
            NetPropertyHelpers.SetPropertyKeys(new Bluetooth_WinPropertyKeys());
        }


        /// <summary>Convert from Windows IBuffer to properly sized byte array</summary>
        /// <param name="buffer">The buffer containing data</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBufferToBytes(this IBuffer buffer) {
            uint dataLength = buffer.Length;
            byte[] data = new byte[dataLength];
            using (DataReader reader = DataReader.FromBuffer(buffer)) {
                reader.ReadBytes(data);
            }
            return data;
        }



        private static bool HasProperty(this DeviceInformation info, string key) {
            if (info.Properties != null) {
                return info.Properties.ContainsKey(key);
            }
            return false;
        }


        public static Dictionary<string, NetPropertyDataModel> CreatePropertiesDictionary(this DeviceInformation info) {
            if (info != null) {
                return NetPropertyHelpers.CreatePropertiesDictionary(info.Properties);
            }
            return new Dictionary<string, NetPropertyDataModel>();
        }


        public static NetPropertiesUpdateDataModel CreatePropertiesUpdateData(this DeviceInformationUpdate updateInfo) {
            NetPropertiesUpdateDataModel dm = new NetPropertiesUpdateDataModel() {
                Id = updateInfo.Id,
                ServiceProperties = NetPropertyHelpers.CreatePropertiesDictionary(updateInfo.Properties),
            };
            return dm;
        }


        //public static BLE_ConnectStatus Convert(this BluetoothConnectionStatus status) {
        //    switch (status) {
        //        case BluetoothConnectionStatus.Disconnected:
        //            return BLE_ConnectStatus.Disconnected;
        //        case BluetoothConnectionStatus.Connected:
        //            return BLE_ConnectStatus.Connected;
        //        default:
        //            // Just for compiler. Only 2 enums
        //            return BLE_ConnectStatus.Disconnected;
        //    }
        //}

    }
}

using BluetoothLE.Net.Enumerations;
using ChkUtils.Net;
using LogUtils.Net;

namespace Bluetooth.UWP.Core {

    /// <summary>Manage a list of OS to Data model binders</summary>
    public class BLE_CharcteristicsBinderSet {

        private readonly ClassLog log = new("BLE_CharcteristicsBinderSet");
        private readonly List<BLE_CharacteristicBinder> binders = new();


        /// <summary>Event raised when value changes or the result of a read request</summary>
        public event EventHandler<BLE_CharacteristicReadResult>? ReadValueChanged;


        /// <summary>Add a binder to the set</summary>
        /// <param name="binder">The binder to manager</param>
        public void Add(BLE_CharacteristicBinder binder) {
            this.binders.Add(binder);
            binder.DataModel.OnReadValueChanged += OnReadValueChanged;
        }


        /// <summary>Tear down all binders including detaching events</summary>
        public void ClearAll() {
            try {
                foreach (BLE_CharacteristicBinder binder in this.binders) {
                    try {
                        binder.DataModel.OnReadValueChanged -= OnReadValueChanged;
                        binder.Teardown();
                    }
                    catch(Exception e) {
                        this.log.Exception(9998, "ClearAll", "", e);
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "ClearAll", "", e);
            }
            finally {
                WrapErr.SafeAction(this.binders.Clear);
            }
        }


        private void OnReadValueChanged(object? sender, BLE_CharacteristicReadResult result) {
            this.ReadValueChanged?.Invoke(sender, result);
        }
    }

}

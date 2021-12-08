using VariousUtils.Net;
using WifiCommon.Net.Enumerations;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace Wifi.UWP.Core.Helpers {

    public static class EnumerationHelpers {

        public static NetworkKind Convert(this WiFiNetworkKind kind) {
            return EnumHelpers.ToEnum<NetworkKind>(
                EnumHelpers.ToInt(kind));
        }


        public static WifiPhyPhysicalLayerKind Convert(this WiFiPhyKind phyKind) {
            return EnumHelpers.ToEnum<WifiPhyPhysicalLayerKind>(
                EnumHelpers.ToInt(phyKind));
        }


        public static NetAuthenticationType Convert (this NetworkAuthenticationType authenticationType) {
            return EnumHelpers.ToEnum<NetAuthenticationType>(
                EnumHelpers.ToInt(authenticationType));
        }


        public static NetEncryptionType Convert(this NetworkEncryptionType encryptionType) {
            return EnumHelpers.ToEnum<NetEncryptionType>(
                EnumHelpers.ToInt(encryptionType));
        }


        public static WifiErrorCode Convert (this WiFiConnectionStatus connectionStatus) {
            return connectionStatus switch {
                WiFiConnectionStatus.UnspecifiedFailure => WifiErrorCode.Unknown,
                WiFiConnectionStatus.Success => WifiErrorCode.Success,
                WiFiConnectionStatus.AccessRevoked => WifiErrorCode.AccessRevoked,
                WiFiConnectionStatus.InvalidCredential => WifiErrorCode.InvalidCredentials,
                WiFiConnectionStatus.NetworkNotAvailable => WifiErrorCode.NetworkNotAvailable,
                WiFiConnectionStatus.Timeout => WifiErrorCode.Timeout,
                WiFiConnectionStatus.UnsupportedAuthenticationProtocol => WifiErrorCode.UnsupportedAuthenticationProtocol,
                _ => WifiErrorCode.Unknown,
            };
        }



    }
}

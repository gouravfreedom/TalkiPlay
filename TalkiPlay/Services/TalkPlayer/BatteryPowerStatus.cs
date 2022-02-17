using System.ComponentModel;

namespace TalkiPlay.Shared
{
    public enum BatteryPowerStatus
    {
        [Description("Status unknown")]
        Unknown,
        [Description("Not chargable")]
        NotChargeable,
        [Description("Not charging")]
        NotCharging,
        [Description("Charging")]
        Charging,
    }
}
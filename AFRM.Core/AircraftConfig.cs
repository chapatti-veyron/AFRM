using System.Text.Json.Serialization;

namespace AFRM.Core
{
    public class AircraftConfig
    {
        [JsonRequired]
        public decimal SpeedKt { get; set; }

        [JsonRequired]
        public decimal FuelCapacityKg { get; set; }

        [JsonRequired]
        public decimal GroundFuelKgph { get; set; }

        [JsonRequired]
        public decimal FuelKgph { get; set; }

        [JsonRequired]
        public decimal MinSpareMins { get; set; }

        [JsonRequired]
        public decimal WaitingFuelKgph { get; set; }

        [JsonRequired]
        public decimal SparePerc { get; set; }

        [JsonRequired]
        public decimal ReserveMins { get; set; }

        [JsonRequired]
        public decimal WarningMins { get; set; }
    }
}
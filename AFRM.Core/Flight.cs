using System.Text.Json.Serialization;

namespace AFRM.Core
{
    public class Flight
    {
        [JsonRequired]
        public string Id { get; set; } = "";

        [JsonRequired]
        public decimal DistanceNm { get; set; }

        [JsonRequired]
        public decimal WindKt { get; set; }

        [JsonRequired]
        public decimal GroundTimeMins { get; set; }

        [JsonRequired]
        public decimal BackupDistanceNm { get; set; }

        [JsonRequired]
        public decimal ExtraFuelKg { get; set; }

        [JsonRequired]
        public decimal FuelLoadedKg { get; set; }
    }
}
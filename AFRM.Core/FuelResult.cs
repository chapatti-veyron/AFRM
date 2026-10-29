namespace AFRM.Core
{
    public class FuelResult
    {
        public decimal GroundFuelKg { get; set; }

        public decimal TripFuelKg { get; set; }

        public decimal SpareFuelKg { get; set; }

        public decimal BackupFuelKg { get; set; }

        public decimal ReserveFuelKg { get; set; }

        public decimal ExtraFuelKg { get; set; }

        public decimal RequiredFuelKg { get; set; }

        public decimal LeftoverFuelKg { get; set; }

        public decimal EstimatedRangeNm { get; set; }

        public FuelStatus Status { get; set; }
    }
}
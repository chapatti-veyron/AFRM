using System;

namespace AFRM.Core
{
    public static class FuelCalculator
    {
        public static FuelResult Calculate(Flight flight, AircraftConfig aircraft)
        {
            Validate(flight, aircraft);

            decimal groundSpeed =
                aircraft.SpeedKt + flight.WindKt;

            decimal tripHours = flight.DistanceNm / groundSpeed;

            FuelResult result = new FuelResult();

            result.GroundFuelKg =
                flight.GroundTimeMins / 60 * aircraft.GroundFuelKgph;

            result.TripFuelKg =
                tripHours * aircraft.FuelKgph;

            decimal percentageSpare =
                result.TripFuelKg * aircraft.SparePerc / 100;

            decimal minimumSpare =
                aircraft.WaitingFuelKgph
                * aircraft.MinSpareMins / 60;

            result.SpareFuelKg =
                Math.Max(percentageSpare, minimumSpare);

            decimal backupHours =
                flight.BackupDistanceNm / groundSpeed;

            result.BackupFuelKg =
                backupHours * aircraft.FuelKgph;

            result.ReserveFuelKg =
                aircraft.WaitingFuelKgph
                * aircraft.ReserveMins / 60;

            result.ExtraFuelKg = flight.ExtraFuelKg;

            result.RequiredFuelKg =
                result.GroundFuelKg
                + result.TripFuelKg
                + result.SpareFuelKg
                + result.BackupFuelKg
                + result.ReserveFuelKg
                + result.ExtraFuelKg;

            result.LeftoverFuelKg =
                flight.FuelLoadedKg - result.RequiredFuelKg;

            decimal bufferKg =
                aircraft.WaitingFuelKgph
                * aircraft.WarningMins / 60;

            result.Status =
                StatusRules.GetStatus(result.LeftoverFuelKg, bufferKg);

            decimal fixedAllowances =
                result.RequiredFuelKg - result.TripFuelKg;

            decimal fuelForRange =
                Math.Max(0, flight.FuelLoadedKg - fixedAllowances);

            decimal hoursForRange =
                fuelForRange / aircraft.FuelKgph;

            result.EstimatedRangeNm = hoursForRange * groundSpeed;

            return result;
        }

        private static void Validate(Flight flight, AircraftConfig aircraft)
        {
            if (flight == null || aircraft == null)
            {
                throw new ArgumentException(
                    "Flight and aircraft settings required.");
            }

            if (aircraft.FuelCapacityKg <= 0)
            {
                throw new ArgumentException(
                    "Fuel capacity must be greater than zero.");
            }

            if (aircraft.SparePerc < 0
                || aircraft.SparePerc > 100)
            {
                throw new ArgumentException(
                    "Spare percentage must be between 0 and 100.");
            }

            if (aircraft.MinSpareMins < 0)
            {
                throw new ArgumentException(
                    "Minimum spare minutes cannot be negative.");
            }

            if (aircraft.ReserveMins <= 0)
            {
                throw new ArgumentException(
                    "Reserve minutes must be greater than zero.");
            }

            if (aircraft.WarningMins <= 0)
            {
                throw new ArgumentException(
                    "Warning minutes must be greater than zero.");
            }

            if (aircraft.SpeedKt <= 0)
            {
                throw new ArgumentException(
                    "Speed must be greater than zero.");
            }

            if (aircraft.FuelKgph <= 0)
            {
                throw new ArgumentException(
                    "Fuel/hour must be greater than zero.");
            }

            if (aircraft.WaitingFuelKgph <= 0)
            {
                throw new ArgumentException(
                    "Waiting fuel/hour must be greater than zero.");
            }

            if (aircraft.GroundFuelKgph < 0)
            {
                throw new ArgumentException(
                    "Ground fuel/hour cannot be negative.");
            }

            if (string.IsNullOrWhiteSpace(flight.Id))
            {
                throw new ArgumentException("ID required.");
            }

            if (flight.DistanceNm <= 0)
            {
                throw new ArgumentException(
                    "Flight distance must be greater than zero.");
            }

            if (aircraft.SpeedKt + flight.WindKt <= 0)
            {
                throw new ArgumentException(
                    "Wind results in ground speed of zero or less.");
            }

            if (flight.GroundTimeMins < 0)
            {
                throw new ArgumentException(
                    "Ground time cannot be negative.");
            }

            if (flight.GroundTimeMins > 0 && aircraft.GroundFuelKgph == 0)
            {
                throw new ArgumentException(
                    "Ground fuel/hour must be positive when ground time is positive.");
            }

            if (flight.BackupDistanceNm < 0)
            {
                throw new ArgumentException(
                    "Backup distance cannot be negative.");
            }

            if (flight.ExtraFuelKg < 0)
            {
                throw new ArgumentException(
                    "Extra fuel cannot be negative.");
            }

            if (flight.FuelLoadedKg < 0
                || flight.FuelLoadedKg > aircraft.FuelCapacityKg)
            {
                throw new ArgumentException(
                    "Fuel loaded must be between zero and aircraft capacity.");
            }
        }
    }
}
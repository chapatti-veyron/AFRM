using System;
using AFRM.Core;
using Xunit;

namespace AFRM.Tests
{
    public class FuelCalculatorTests
    {
        [Fact]
        public void FR03_WorkedExampleComponents()
        {
            FuelResult result =
                FuelCalculator.Calculate(CreateFlight(), CreateAircraft());

            Assert.Equal(150m, result.GroundFuelKg);
            Assert.Equal(4800m, result.TripFuelKg);
            Assert.Equal(240m, result.SpareFuelKg);
            Assert.Equal(480m, result.BackupFuelKg);
            Assert.Equal(1000m, result.ReserveFuelKg);
            Assert.Equal(100m, result.ExtraFuelKg);
            Assert.Equal(6770m, result.RequiredFuelKg);
            Assert.Equal(500m, result.LeftoverFuelKg);
            Assert.Equal(993.75m, result.EstimatedRangeNm);
            Assert.Equal(FuelStatus.OK, result.Status);
        }

        [Fact]
        public void FR03_ShortFlightUsesSpareMinimum()
        {
            AircraftConfig aircraft = CreateAircraft();
            Flight flight = CreateFlight();
            flight.DistanceNm = 225;
            aircraft.WaitingFuelKgph = 2400;
            flight.FuelLoadedKg = 3330;

            FuelResult result =
                FuelCalculator.Calculate(flight, aircraft);

            Assert.Equal(1200m, result.TripFuelKg);
            Assert.Equal(200m, result.SpareFuelKg);
            Assert.Equal(3330m, result.RequiredFuelKg);
            Assert.Equal(225m, result.EstimatedRangeNm);
        }

        [Fact]
        public void FR03_UsesConfiguredSpareMinimum()
        {
            AircraftConfig aircraft = CreateAircraft();
            Flight flight = CreateFlight();
            flight.DistanceNm = 225;
            aircraft.WaitingFuelKgph = 2400;

            aircraft.MinSpareMins = 10;

            FuelResult result = FuelCalculator.Calculate(flight, aircraft);

            Assert.Equal(400m, result.SpareFuelKg);
        }

        [Fact]
        public void FR03_HeadwindIncreasesTripFuel()
        {
            Flight flight = CreateFlight();
            flight.WindKt = -90;

            FuelResult result =
                FuelCalculator.Calculate(flight, CreateAircraft());

            Assert.Equal(6000m, result.TripFuelKg);
        }

        [Fact]
        public void FR03_BackupUsesMainLegGroundSpeed()
        {
            Flight flight = CreateFlight();
            flight.WindKt = -90;

            FuelResult result =
                FuelCalculator.Calculate(flight, CreateAircraft());

            Assert.Equal(600m, result.BackupFuelKg);
        }

        [Fact]
        public void FR03_RangeNeverNegative()
        {
            Flight flight = CreateFlight();
            flight.FuelLoadedKg = 0;

            FuelResult result =
                FuelCalculator.Calculate(flight, CreateAircraft());

            Assert.Equal(0m, result.EstimatedRangeNm);
            Assert.Equal(FuelStatus.INSUFFICIENT_FUEL, result.Status);
        }

        [Theory]
        [InlineData(6769, FuelStatus.INSUFFICIENT_FUEL)]
        [InlineData(6770, FuelStatus.MARGINAL)]
        [InlineData(7269, FuelStatus.MARGINAL)]
        [InlineData(7270, FuelStatus.OK)]
        public void FR04_StatusBoundaries(int fuel, FuelStatus expected)
        {
            Flight flight = CreateFlight();
            flight.FuelLoadedKg = fuel;

            FuelResult result =
                FuelCalculator.Calculate(flight, CreateAircraft());

            Assert.Equal(expected, result.Status);
        }

        [Fact]
        public void FR04_UsesConfiguredBufferMinutes()
        {
            AircraftConfig aircraft = CreateAircraft();
            aircraft.WarningMins = 30;

            FuelResult result =
                FuelCalculator.Calculate(CreateFlight(), aircraft);

            Assert.Equal(500m, result.LeftoverFuelKg);
            Assert.Equal(FuelStatus.MARGINAL, result.Status);
        }

        [Theory]
        [InlineData(2000, 7270, FuelStatus.OK)]
        [InlineData(2400, 7470, FuelStatus.MARGINAL)]
        public void FR04_BufferUsesWaitingFuel(
            int waitingFuel, int fuel, FuelStatus expected)
        {
            AircraftConfig aircraft = CreateAircraft();
            Flight flight = CreateFlight();
            aircraft.WaitingFuelKgph = waitingFuel;
            flight.FuelLoadedKg = fuel;

            FuelResult result =
                FuelCalculator.Calculate(flight, aircraft);

            Assert.Equal(500m, result.LeftoverFuelKg);
            Assert.Equal(expected, result.Status);
        }

        [Theory]
        [InlineData("blank ID")]
        [InlineData("zero distance")]
        [InlineData("zero ground speed")]
        [InlineData("negative ground speed")]
        [InlineData("negative ground time")]
        [InlineData("negative backup distance")]
        [InlineData("negative extra fuel")]
        [InlineData("negative fuel")]
        [InlineData("over capacity")]
        public void FR02_RejectsInvalidFlight(string invalidCase)
        {
            Flight flight = CreateFlight();

            switch (invalidCase)
            {
                case "blank ID":
                    flight.Id = " ";
                    break;
                case "zero distance":
                    flight.DistanceNm = 0;
                    break;
                case "zero ground speed":
                    flight.WindKt = -450;
                    break;
                case "negative ground speed":
                    flight.WindKt = -451;
                    break;
                case "negative ground time":
                    flight.GroundTimeMins = -1;
                    break;
                case "negative backup distance":
                    flight.BackupDistanceNm = -1;
                    break;
                case "negative extra fuel":
                    flight.ExtraFuelKg = -1;
                    break;
                case "negative fuel":
                    flight.FuelLoadedKg = -1;
                    break;
                case "over capacity":
                    flight.FuelLoadedKg = 20001;
                    break;
                default:
                    throw new ArgumentException("Unknown test case.");
            }

            Assert.Throws<ArgumentException>(() =>
                FuelCalculator.Calculate(flight, CreateAircraft()));
        }

        [Theory]
        [InlineData("zero speed")]
        [InlineData("zero fuel per hour")]
        [InlineData("zero waiting fuel")]
        [InlineData("negative ground fuel")]
        [InlineData("missing ground fuel")]
        [InlineData("zero capacity")]
        [InlineData("negative percentage")]
        [InlineData("percentage above 100")]
        [InlineData("negative minimum minutes")]
        [InlineData("zero reserve")]
        [InlineData("zero buffer")]
        public void FR02_RejectsInvalidAircraft(string invalidCase)
        {
            AircraftConfig aircraft = CreateAircraft();

            switch (invalidCase)
            {
                case "zero speed":
                    aircraft.SpeedKt = 0;
                    break;
                case "zero fuel per hour":
                    aircraft.FuelKgph = 0;
                    break;
                case "zero waiting fuel":
                    aircraft.WaitingFuelKgph = 0;
                    break;
                case "negative ground fuel":
                    aircraft.GroundFuelKgph = -1;
                    break;
                case "missing ground fuel":
                    aircraft.GroundFuelKgph = 0;
                    break;
                case "zero capacity":
                    aircraft.FuelCapacityKg = 0;
                    break;
                case "negative percentage":
                    aircraft.SparePerc = -1;
                    break;
                case "percentage above 100":
                    aircraft.SparePerc = 101;
                    break;
                case "negative minimum minutes":
                    aircraft.MinSpareMins = -1;
                    break;
                case "zero reserve":
                    aircraft.ReserveMins = 0;
                    break;
                case "zero buffer":
                    aircraft.WarningMins = 0;
                    break;
                default:
                    throw new ArgumentException("Unknown test case.");
            }

            Assert.Throws<ArgumentException>(() =>
                FuelCalculator.Calculate(CreateFlight(), aircraft));
        }

        [Fact]
        public void FR02_RejectsNullInputs()
        {
            Assert.Throws<ArgumentException>(() =>
                FuelCalculator.Calculate(null!, CreateAircraft()));

            Assert.Throws<ArgumentException>(() =>
                FuelCalculator.Calculate(CreateFlight(), null!));
        }

        [Fact]
        public void NFR01_SameInputSameResult()
        {
            Flight flight = CreateFlight();
            AircraftConfig aircraft = CreateAircraft();

            FuelResult first = FuelCalculator.Calculate(flight, aircraft);
            FuelResult second = FuelCalculator.Calculate(flight, aircraft);

            Assert.Equal(first.GroundFuelKg, second.GroundFuelKg);
            Assert.Equal(first.TripFuelKg, second.TripFuelKg);
            Assert.Equal(first.SpareFuelKg, second.SpareFuelKg);
            Assert.Equal(first.BackupFuelKg, second.BackupFuelKg);
            Assert.Equal(first.ReserveFuelKg, second.ReserveFuelKg);
            Assert.Equal(first.ExtraFuelKg, second.ExtraFuelKg);
            Assert.Equal(first.RequiredFuelKg, second.RequiredFuelKg);
            Assert.Equal(first.LeftoverFuelKg, second.LeftoverFuelKg);
            Assert.Equal(first.EstimatedRangeNm, second.EstimatedRangeNm);
            Assert.Equal(first.Status, second.Status);
        }

        private static AircraftConfig CreateAircraft()
        {
            return new AircraftConfig
            {
                SpeedKt = 450,
                FuelKgph = 2400,
                GroundFuelKgph = 600,
                WaitingFuelKgph = 2000,
                FuelCapacityKg = 20000,
                SparePerc = 5,
                MinSpareMins = 5,
                ReserveMins = 30,
                WarningMins = 15
            };
        }

        private static Flight CreateFlight()
        {
            return new Flight
            {
                Id = "TEST",
                DistanceNm = 900,
                WindKt = 0,
                GroundTimeMins = 15,
                BackupDistanceNm = 90,
                ExtraFuelKg = 100,
                FuelLoadedKg = 7270
            };
        }
    }
}
using System.Globalization;
using System.Text.Json;
using AFRM.Core;

namespace AFRM.App
{
    public class Program
    {
        public static int Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            Console.WriteLine(
                "AFRM");

            try
            {
                string dataFolder =
                    Path.Combine(AppContext.BaseDirectory, "data");

                string aircraftJson = File.ReadAllText(
                    Path.Combine(dataFolder, "aircraft.json"));

                string flightsJson = File.ReadAllText(
                    Path.Combine(dataFolder, "flights.json"));

                AircraftConfig? aircraft =
                    JsonSerializer.Deserialize<AircraftConfig>(aircraftJson);

                List<Flight>? flights =
                    JsonSerializer.Deserialize<List<Flight>>(flightsJson);

                if (aircraft == null || flights == null || flights.Count == 0)
                {
                    throw new ArgumentException(
                        "Missing data. Flight and aircraft data required");
                }

                List<string> flightIds = new List<string>();

                foreach (Flight flight in flights)
                {
                    if (flight == null
                        || string.IsNullOrWhiteSpace(flight.Id))
                    {
                        throw new ArgumentException(
                            "Flight ID required");
                    }

                    string id = flight.Id.Trim().ToUpperInvariant();

                    if (flightIds.Contains(id))
                    {
                        throw new ArgumentException(
                            "Duplicate flight ID: " + flight.Id);
                    }

                    flightIds.Add(id);
                }

                foreach (Flight flight in flights)
                {
                    FuelResult result =
                        FuelCalculator.Calculate(flight, aircraft);

                    string statusText;

                    if (result.Status == FuelStatus.OK)
                    {
                        statusText = "OK";
                    }
                    else if (result.Status == FuelStatus.MARGINAL)
                    {
                        statusText = "MARGINAL";
                    }
                    else
                    {
                        statusText = "INSUFFICIENT_FUEL";
                    }

                    Console.WriteLine();
                    Console.WriteLine($"{flight.Id}: {statusText}");
                    Console.WriteLine(
                        $"  Ground fuel: {result.GroundFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Trip fuel: {result.TripFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Spare fuel: {result.SpareFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Backup fuel: {result.BackupFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Reserve fuel: {result.ReserveFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Extra fuel: {result.ExtraFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Required fuel: {result.RequiredFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Leftover fuel: {result.LeftoverFuelKg:F1} kg");
                    Console.WriteLine(
                        $"  Estimated range: {result.EstimatedRangeNm:F1} NM");
                }

                return 0;
            }
            catch (IOException error)
            {
                Console.Error.WriteLine("File error: " + error.Message);
                return 1;
            }
            catch (JsonException error)
            {
                Console.Error.WriteLine("JSON error: " + error.Message);
                return 1;
            }
            catch (ArgumentException error)
            {
                Console.Error.WriteLine("Input error: " + error.Message);
                return 1;
            }
        }
    }
}
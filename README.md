## AFRM
The AFRM calculates fuel requirements and estimated range, then classifies flights using  available fuel amounts.
## Requirements

| ID     | Requirement                                                                | Verification Method      |
| ------ | -------------------------------------------------------------------------- | ------------------------ |
| FR-01  | Load only valid aircraft and flight data                                   | Manual Check             |
| FR-02  | Reject invalid input values                                                | Automated Tests          |
| FR-03  | Calculate fuel components, total required fuel and estimated range         | Automated Tests          |
| FR-04  | Classify flights as OK, MARGINAL or INSUFFICIENT_FUEL                      | Automated Tests          |
| FR-05  | Output results for each flight                                             | Manual Check             |
| NFR-01 | Isolate calculation logic from input and output                            | Automated Tests          |
| NFR-02 | Support reproducible builds using shared settings and locked dependencies. | CI Pipeline              |
| NFR-03 | Process the  dataset in under one second                                   | Performance Measurements |

## Prerequisites
- .NET SDK 10.0.401, specified in global.json
- Windows x64
- Visual Studio 2026 (development)
## Build, Run and Test
Commands to restore, build, run and test:

```
dotnet restore AFRM.slnx --locked-mode
dotnet build AFRM.slnx -c Release --no-restore
dotnet run --project AFRM.App -c Release --no-build --no-restore
dotnet test AFRM.slnx -c Release --no-build --no-restore
```
## Project Structure
- AFRM.Core: aircraft settings, input validation, fuel calculations and status rules.
- AFRM.App: loads JSON data and prints results.
- AFRM.Tests: boundary checks and invalid-input tests.
## Status Rules
Required fuel includes the 30min reserve and spare fuel, based on CAA regulations.

MARGINAL means the leftover fuel is below 15mins of waiting fuel
## Input Data
flights.json settings: flight IDs, distances in nautical miles, wind in knots, ground time in minutes and fuel quantities in kilograms. Positive wind represents a tailwind and negative wind represents a headwind.

Flight data values were generated using AI.

aircraft.json settings: speed in knots, capacity in kg, fuel consumption in kg/hour, contingency percentage and reserve and warning times in minutes.
## Assumptions
- Fuel consumption and wind are constant.
- The backup route uses the main route’s wind and fuel usage rate.
## Verification
The test suite contains 35 automated test cases. Data loading and console output are checked manually.

GitHub Actions is configured to restore, build and test pull requests targeting main, and pushes to main. Merges are blocked for PRs failing the check

The recorded runtime is approximately 108 ms on the development machine. 
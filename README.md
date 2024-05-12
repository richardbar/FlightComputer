# FlightComputer

### Prerequisites
Make sure you have the following prerequisites installed on your machine:
- [.NET SDK](https://dotnet.microsoft.com/download) (version 8.x)

### Wiring
Connect the BME280 sensor to the Raspberry Pi's I2C 1 bus using the following pins:

- VCC: Connect to the 3.3V power supply pin on the Raspberry Pi.
- GND: Connect to the ground pin on the Raspberry Pi.
- SDA: Connect to the SDA (Serial Data) pin on the Raspberry Pi's I2C 1 bus.
- SCL: Connect to the SCL (Serial Clock) pin on the Raspberry Pi's I2C 1 bus.
- SDO: Connect to the 3.3V power supply pin on the Raspberry Pi.

Make sure to enable I2C on the Raspberry Pi by following the appropriate setup instructions for your operating system.

### Restore the Project dependencies
Use the following command to restore the project depedencies:
```bash
dotnet restore
```

### Build the Project
Use the following command to build the project:
```bash
dotnet build --no-restore
```

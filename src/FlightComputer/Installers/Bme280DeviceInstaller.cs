// MIT License
// 
// Copyright (c) 2024 Rikarnto Bariampa
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Device.I2c;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using FlightComputer.Devices;
using FlightComputer.Devices.Abstractions;
using FlightComputer.Devices.Mocking;
using FlightComputer.Installers.Abstractions;
using Iot.Device.Bmxx80;

namespace FlightComputer.Installers;

// ReSharper disable once UnusedType.Global
public sealed class Bme280DeviceInstaller : IInstaller
{
    public bool Install(IServiceCollection serviceCollection)
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? LinuxInstall(serviceCollection)
            : MockInstall(serviceCollection);
    }

    private static bool LinuxInstall(IServiceCollection serviceCollection)
    {
        bool bme280Detected;
        {
            // ReSharper disable InconsistentNaming
            var bme280I2cConnectionSettings = new I2cConnectionSettings(1, Bmx280Base.DefaultI2cAddress);
            using var i2cDevice = I2cDevice.Create(bme280I2cConnectionSettings);
            // ReSharper restore InconsistentNaming

            // Chip ID register is 0xD0
            Span<byte> writeBuffer = stackalloc byte[1] { 0xD0 };
            Span<byte> readBuffer = stackalloc byte[1];

            i2cDevice.WriteRead(writeBuffer, readBuffer);

            // The BME280 has a chip ID 0x60
            bme280Detected = (readBuffer[0] == 0x60);
        }

        if (!bme280Detected)
        {
            return MockInstall(serviceCollection);
        }

        // First, register the BME280 device
        serviceCollection.AddSingleton<Bme280Device>();
        
        // Then, register the interfaces as a BME280 device
        serviceCollection.AddSingleton<IPressureDevice>(serviceProvider =>
            serviceProvider.GetRequiredService<Bme280Device>());
        serviceCollection.AddSingleton<ITemperatureDevice>(serviceProvider =>
            serviceProvider.GetRequiredService<Bme280Device>());

        return true;
    }

    private static bool MockInstall(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IPressureDevice, RandomPressureMockingDevice>();
        serviceCollection.AddSingleton<ITemperatureDevice, RandomTemperatureMockingDevice>();

        return true;
    }
}

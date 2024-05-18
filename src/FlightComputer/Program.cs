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

using System.Text.Json;
using FlightComputer.Data;
using FlightComputer.Devices;
using FlightComputer.Devices.Abstractions;
using FlightComputer.Devices.Mocking;

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    // ReSharper disable once AccessToDisposedClosure
    cancellationTokenSource.Cancel();
    e.Cancel = true; // Prevent the process from terminating.
};

Bme280Device? bme280Device = null;
try
{
    bme280Device = new Bme280Device();
}
catch (Exception)
{
    // ignored
}

var pressureDevice = (IPressureDevice?)bme280Device ?? new RandomPressureMockingDevice();
var temperatureDevice = (ITemperatureDevice?)bme280Device ?? new RandomTemperatureMockingDevice();

try
{
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        var data = new FlightComputerData
        {
            Pressure = await pressureDevice.ReadPressureAsync(cancellationTokenSource.Token),
            Temperature = await temperatureDevice.ReadTemperatureAsync(cancellationTokenSource.Token)
        };

        Console.WriteLine(JsonSerializer.Serialize(data));

        await Task.Delay(1000, cancellationTokenSource.Token);
    }
}
catch (TaskCanceledException)
{
    // ignored
}
finally
{
    bme280Device?.Dispose();
}

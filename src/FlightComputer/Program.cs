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
using Iot.Device.Bmxx80;
using Iot.Device.Common;

using var cancellationTokenSource = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    // ReSharper disable once AccessToDisposedClosure
    cancellationTokenSource.Cancel();
    e.Cancel = true; // Prevent the process from terminating.
};

using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

// ReSharper disable once InconsistentNaming
using var i2cBus1 = I2cBus.Create(1);

// ReSharper disable once InconsistentNaming
using var bme280I2cDevice = i2cBus1.CreateDevice(Bmx280Base.SecondaryI2cAddress);
using var bme280 = new Bme280(bme280I2cDevice);

try
{
    while (await timer.WaitForNextTickAsync(cancellationTokenSource.Token))
    {
        var bme280Data = await bme280.ReadAsync();
        var altitude = WeatherHelper.CalculateAltitude(bme280Data.Pressure!.Value);

        Console.WriteLine($"Temperature: {bme280Data.Temperature}");
        Console.WriteLine($"Pressure: {bme280Data.Pressure}");
        Console.WriteLine($"Altitude: {altitude}");
    }
}
catch (OperationCanceledException) { }

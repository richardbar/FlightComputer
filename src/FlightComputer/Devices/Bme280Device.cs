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
using Iot.Device.Bmxx80.ReadResult;
using UnitsNet;
using FlightComputer.Devices.Abstractions;

namespace FlightComputer.Devices;

public sealed class Bme280Device : IPressureDevice, ITemperatureDevice, IDisposable
{
    private static readonly I2cConnectionSettings I2CConnectionSettings = new(1, Bmx280Base.DefaultI2cAddress);
    private readonly I2cDevice _i2CDevice = I2cDevice.Create(I2CConnectionSettings);
    private readonly Bme280 _bme280;
    private readonly TimeSpan _measurementDuration;
    private DateTime _lastMeasurementTime = DateTime.MinValue;
    private Bme280ReadResult _lastReadResult = new(null, null, null);

    public Bme280Device()
    {
        _bme280 = new Bme280(_i2CDevice); 
        
        var bme280MeasurementDurationInMs = _bme280.GetMeasurementDuration();
        _measurementDuration = TimeSpan.FromMilliseconds(bme280MeasurementDurationInMs);
    }

    public async Task<Pressure?> ReadPressureAsync(CancellationToken cancellationToken = default)
    {
        await EnsureMeasurementIsRecent();
        return _lastReadResult.Pressure;
    }

    public async Task<Temperature?> ReadTemperatureAsync(CancellationToken cancellationToken = default)
    {
        await EnsureMeasurementIsRecent();
        return _lastReadResult.Temperature;
    }

    public void Dispose()
    {
        _bme280.Dispose();
        _i2CDevice.Dispose();
    }
    
    private async Task EnsureMeasurementIsRecent()
    {
        var timeSinceLastMeasurement = DateTime.UtcNow - _lastMeasurementTime;
        var doNotNeedToRead = timeSinceLastMeasurement < _measurementDuration;
        
        if (doNotNeedToRead)
        {
            return;
        }
        
        _lastReadResult = await _bme280.ReadAsync();
        _lastMeasurementTime = DateTime.UtcNow;
    }
}

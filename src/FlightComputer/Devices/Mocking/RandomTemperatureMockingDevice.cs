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

using UnitsNet;
using FlightComputer.Devices.Abstractions;

namespace FlightComputer.Devices.Mocking;

public sealed class RandomTemperatureMockingDevice : ITemperatureDevice
{
    private static readonly Temperature MinTemperature = Temperature.FromDegreesCelsius(-5);
    private static readonly Temperature MaxTemperature = Temperature.FromDegreesCelsius(90);
    private readonly Random _random = new();
    private DateTime _lastMeasurementTime = DateTime.MinValue;
    
    public Task<Temperature?> ReadTemperatureAsync(CancellationToken cancellationToken = default)
    {
        var temperature = _random.NextDouble() * (MaxTemperature - MinTemperature) + MinTemperature;
        _lastMeasurementTime = DateTime.UtcNow;
        return Task.FromResult<Temperature?>(temperature);
    }

    public DateTime GetLastMeasurementTime()
    {
        return _lastMeasurementTime.ToUniversalTime();
    }
}

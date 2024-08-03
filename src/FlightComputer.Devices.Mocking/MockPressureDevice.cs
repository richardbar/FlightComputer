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

namespace FlightComputer.Devices.Mocking;

public sealed class MockPressureDevice : IPressureDevice
{
    private readonly Pressure MaxPressure = Pressure.FromMetersOfElevation(0).ToUnit(PressureUnit.Pascal);
    private readonly Pressure MinPressure = Pressure.FromMetersOfElevation(1000).ToUnit(PressureUnit.Pascal);
    private readonly long IntervalTicks = TimeSpan.FromMinutes(1).Ticks;

    private Pressure Amplitude => (MaxPressure - MinPressure) / 2;
    private Pressure Offset => (MaxPressure + MinPressure) / 2;

    public Task<Pressure> GetPressureAsync(CancellationToken cancellationToken = default)
    {
        var pressureValue = Math.Sin(DateTime.UtcNow.Ticks % IntervalTicks / (double)IntervalTicks * 2 * Math.PI) * Amplitude + Offset;

        return Task.FromResult(pressureValue);
    }
}

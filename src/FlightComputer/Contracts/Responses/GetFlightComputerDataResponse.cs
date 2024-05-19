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

using System.Text.Json.Serialization;
using Iot.Device.Common;
using UnitsNet;

namespace FlightComputer.Contracts.Responses;

public sealed class GetFlightComputerDataResponse
{
    [JsonIgnore]
    public required Pressure? Pressure { get; init; }

    [JsonIgnore]
    public Length? Altitude
    {
        get
        {
            if (Pressure is null)
            {
                return null;
            }

            return WeatherHelper.CalculateAltitude(Pressure!.Value);
        }
    }
    
    [JsonIgnore]
    public required Temperature? Temperature { get; init; }
    
    [JsonInclude]
    [JsonPropertyName("Pressure")]
    private double? PressureValue => (Pressure is not null) ? Math.Round(Pressure.Value.Pascals, 2) : null;
    
    [JsonInclude]
    [JsonPropertyName("Altitude")]
    private double? AltitudeValue => (Altitude is not null) ? Math.Round(Altitude.Value.Meters, 2) : null;

    [JsonInclude]
    [JsonPropertyName("Temperature")]
    private double? TemperatureValue => (Temperature is not null) ? Math.Round(Temperature.Value.Kelvins, 2) : null;
}

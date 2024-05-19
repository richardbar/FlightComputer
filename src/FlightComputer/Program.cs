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

using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FlightComputer.Devices;
using FlightComputer.Devices.Abstractions;
using FlightComputer.Devices.Mocking;
using FlightComputer.Services;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(serviceCollection =>
    {
        serviceCollection.AddHostedService<FlightComputerService>();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            serviceCollection.AddSingleton<Bme280Device>();
            serviceCollection.AddSingleton<IPressureDevice>(serviceProvider =>
                serviceProvider.GetRequiredService<Bme280Device>());
            serviceCollection.AddSingleton<ITemperatureDevice>(serviceProvider =>
                serviceProvider.GetRequiredService<Bme280Device>());
        }
        else
        {
            serviceCollection.AddSingleton<IPressureDevice, RandomPressureMockingDevice>();
            serviceCollection.AddSingleton<ITemperatureDevice, RandomTemperatureMockingDevice>();
        }
    });

var app = builder.Build();

await app.RunAsync();

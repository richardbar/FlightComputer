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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using FlightComputer.Data;
using FlightComputer.Devices.Abstractions;

namespace FlightComputer.Services;

public sealed class FlightComputerService(
    ILogger<FlightComputerService> logger,
    IPressureDevice pressureDevice,
    ITemperatureDevice temperatureDevice) : IHostedService
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _backgroundTask = Task.CompletedTask;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Service} is starting.", nameof(FlightComputerService));
        _backgroundTask = Run(_cancellationTokenSource.Token);
        logger.LogInformation("{Service} has started.", nameof(FlightComputerService));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("${Service} is stopping.", nameof(FlightComputerService));
        await _cancellationTokenSource.CancelAsync();

        try
        {
            await Task.WhenAny(_backgroundTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("{Service} stop was cancelled.", nameof(FlightComputerService));
        }

        logger.LogInformation("{Service} is stopped.", nameof(FlightComputerService));
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(1000));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken))
            {
                var data = new FlightComputerData
                {
                    Pressure = await pressureDevice.ReadPressureAsync(cancellationToken),
                    Temperature = await temperatureDevice.ReadTemperatureAsync(cancellationToken)
                };

                logger.LogInformation("Data: {Data}", JsonSerializer.Serialize(data));
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("{Service} cancellation was requested.", nameof(FlightComputerService));
        }

        logger.LogInformation("{Service} cleanup is completed.", nameof(FlightComputerService));
    }
}

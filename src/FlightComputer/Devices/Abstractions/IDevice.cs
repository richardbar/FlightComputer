namespace FlightComputer.Devices.Abstractions;

public interface IDevice
{
    /// <summary>
    /// Return UTC time of the last measurement.
    /// </summary>
    /// <result>UTC time of the last measurement.</result>
    public DateTime GetLastMeasurementTime();
}

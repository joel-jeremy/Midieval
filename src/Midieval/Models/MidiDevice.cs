namespace Midieval.Models;

/// <summary>
/// Represents a discovered BLE MIDI device.
/// </summary>
public class MidiDevice
{
    /// <summary>Gets or sets the device's unique identifier.</summary>
    public Guid Id { get; init; }

    /// <summary>Gets or sets the device's display name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Gets or sets the signal strength in dBm.</summary>
    public int Rssi { get; init; }

    /// <summary>Gets or sets whether the device is currently connected.</summary>
    public bool IsConnected { get; set; }

    /// <inheritdoc/>
    public override string ToString() => $"{Name} ({Id}) RSSI={Rssi}";
}

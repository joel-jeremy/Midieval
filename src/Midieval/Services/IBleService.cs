using Midieval.Models;

namespace Midieval.Services;

/// <summary>
/// Abstraction over Bluetooth Low Energy operations required for BLE-MIDI.
/// </summary>
public interface IBleService
{
    /// <summary>Gets whether Bluetooth is currently available and powered on.</summary>
    bool IsBluetoothAvailable { get; }

    /// <summary>Gets whether a BLE-MIDI device is currently connected.</summary>
    bool IsConnected { get; }

    /// <summary>Gets the currently connected device, or <c>null</c> if not connected.</summary>
    MidiDevice? ConnectedDevice { get; }

    /// <summary>Raised when a new device is discovered during a scan.</summary>
    event EventHandler<MidiDevice> DeviceDiscovered;

    /// <summary>Raised when the connection state changes.</summary>
    event EventHandler<bool> ConnectionStateChanged;

    /// <summary>
    /// Starts scanning for BLE-MIDI peripherals.
    /// Only devices advertising the BLE-MIDI service UUID are surfaced.
    /// </summary>
    /// <param name="cancellationToken">Token to stop the scan.</param>
    Task StartScanAsync(CancellationToken cancellationToken = default);

    /// <summary>Stops an ongoing BLE scan.</summary>
    Task StopScanAsync();

    /// <summary>
    /// Connects to the given MIDI device and resolves the BLE-MIDI characteristic.
    /// </summary>
    Task ConnectAsync(MidiDevice device, CancellationToken cancellationToken = default);

    /// <summary>Disconnects from the currently connected device.</summary>
    Task DisconnectAsync();

    /// <summary>
    /// Writes raw bytes to the BLE-MIDI characteristic.
    /// </summary>
    Task WriteAsync(byte[] data, CancellationToken cancellationToken = default);
}

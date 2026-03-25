using Midieval.Models;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace Midieval.Services;

/// <summary>
/// BLE-MIDI service implementation using Plugin.BLE.
/// Handles device scanning, connection, and raw data transmission over the
/// BLE-MIDI GATT characteristic.
/// </summary>
public class BleService : IBleService, IDisposable
{
    // BLE-MIDI GATT identifiers as defined in the
    // "MIDI over Bluetooth Low Energy (BLE-MIDI)" specification.
    private static readonly Guid BleMidiServiceUuid =
        new("03B80E5A-EDE8-4B33-A751-6CE34EC4C700");

    private static readonly Guid BleMidiCharacteristicUuid =
        new("7772E5DB-3868-4112-A1A9-F2669D106BF3");

    private readonly IBluetoothLE _ble;
    private readonly IAdapter _adapter;

    private IDevice? _connectedBleDevice;
    private ICharacteristic? _midiCharacteristic;
    private MidiDevice? _connectedDevice;

    public BleService(IBluetoothLE ble, IAdapter adapter)
    {
        _ble = ble;
        _adapter = adapter;

        _adapter.DeviceDiscovered += OnDeviceDiscovered;
        _adapter.DeviceConnected += OnDeviceConnected;
        _adapter.DeviceDisconnected += OnDeviceDisconnected;
        _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
    }

    /// <inheritdoc/>
    public bool IsBluetoothAvailable =>
        _ble.State == Plugin.BLE.Abstractions.BluetoothState.On;

    /// <inheritdoc/>
    public bool IsConnected => _connectedBleDevice is not null && _midiCharacteristic is not null;

    /// <inheritdoc/>
    public MidiDevice? ConnectedDevice => _connectedDevice;

    /// <inheritdoc/>
    public event EventHandler<MidiDevice>? DeviceDiscovered;

    /// <inheritdoc/>
    public event EventHandler<bool>? ConnectionStateChanged;

    /// <inheritdoc/>
    public async Task StartScanAsync(CancellationToken cancellationToken = default)
    {
        if (!IsBluetoothAvailable)
        {
            throw new InvalidOperationException("Bluetooth is not available or not powered on.");
        }

        _adapter.ScanMode = Plugin.BLE.Abstractions.ScanMode.LowLatency;

        // Filter to only BLE-MIDI devices to avoid flooding the list
        await _adapter.StartScanningForDevicesAsync(
            serviceUuids: [BleMidiServiceUuid],
            cancellationToken: cancellationToken);
    }

    /// <inheritdoc/>
    public async Task StopScanAsync()
    {
        if (_adapter.IsScanning)
        {
            await _adapter.StopScanningForDevicesAsync();
        }
    }

    /// <inheritdoc/>
    public async Task ConnectAsync(MidiDevice device, CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            await DisconnectAsync();
        }

        var bleDevice = await _adapter.ConnectToKnownDeviceAsync(device.Id, cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"Could not connect to device '{device.Name}'.");

        var service = await bleDevice.GetServiceAsync(BleMidiServiceUuid, cancellationToken)
            ?? throw new InvalidOperationException("BLE-MIDI service not found on the connected device.");

        var characteristic = await service.GetCharacteristicAsync(BleMidiCharacteristicUuid)
            ?? throw new InvalidOperationException("BLE-MIDI characteristic not found on the connected device.");

        _connectedBleDevice = bleDevice;
        _midiCharacteristic = characteristic;
        _connectedDevice = device;
        _connectedDevice.IsConnected = true;
    }

    /// <inheritdoc/>
    public async Task DisconnectAsync()
    {
        if (_connectedBleDevice is not null)
        {
            await _adapter.DisconnectDeviceAsync(_connectedBleDevice);
        }

        _connectedBleDevice = null;
        _midiCharacteristic = null;

        if (_connectedDevice is not null)
        {
            _connectedDevice.IsConnected = false;
            _connectedDevice = null;
        }
    }

    /// <inheritdoc/>
    public async Task WriteAsync(byte[] data, CancellationToken cancellationToken = default)
    {
        if (_midiCharacteristic is null)
        {
            throw new InvalidOperationException("Not connected to a BLE-MIDI device.");
        }

        await _midiCharacteristic.WriteAsync(data, cancellationToken);
    }

    private void OnDeviceDiscovered(object? sender, DeviceEventArgs e)
    {
        var device = new MidiDevice
        {
            Id = e.Device.Id,
            Name = string.IsNullOrWhiteSpace(e.Device.Name) ? "Unknown Device" : e.Device.Name,
            Rssi = e.Device.Rssi,
        };

        DeviceDiscovered?.Invoke(this, device);
    }

    private void OnDeviceConnected(object? sender, DeviceEventArgs e)
    {
        ConnectionStateChanged?.Invoke(this, true);
    }

    private void OnDeviceDisconnected(object? sender, DeviceEventArgs e)
    {
        if (_connectedDevice is not null)
        {
            _connectedDevice.IsConnected = false;
            _connectedDevice = null;
        }

        _connectedBleDevice = null;
        _midiCharacteristic = null;
        ConnectionStateChanged?.Invoke(this, false);
    }

    private void OnDeviceConnectionLost(object? sender, DeviceErrorEventArgs e)
    {
        if (_connectedDevice is not null)
        {
            _connectedDevice.IsConnected = false;
            _connectedDevice = null;
        }

        _connectedBleDevice = null;
        _midiCharacteristic = null;
        ConnectionStateChanged?.Invoke(this, false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _adapter.DeviceDiscovered -= OnDeviceDiscovered;
        _adapter.DeviceConnected -= OnDeviceConnected;
        _adapter.DeviceDisconnected -= OnDeviceDisconnected;
        _adapter.DeviceConnectionLost -= OnDeviceConnectionLost;
    }
}

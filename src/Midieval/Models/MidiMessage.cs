namespace Midieval.Models;

/// <summary>
/// Represents a MIDI message with its type, channel, and data bytes.
/// </summary>
public class MidiMessage
{
    /// <summary>Gets or sets the MIDI message type (status nibble).</summary>
    public MidiMessageType Type { get; init; }

    /// <summary>Gets or sets the MIDI channel (0–15).</summary>
    public int Channel { get; init; }

    /// <summary>Gets or sets the first data byte (e.g. note number, controller number, program number).</summary>
    public int Data1 { get; init; }

    /// <summary>Gets or sets the second data byte (e.g. velocity, controller value). Not used for Program Change.</summary>
    public int Data2 { get; init; }

    /// <summary>
    /// Creates a Control Change (CC) message.
    /// </summary>
    /// <param name="channel">MIDI channel (0–15).</param>
    /// <param name="controller">Controller number (0–127).</param>
    /// <param name="value">Controller value (0–127).</param>
    public static MidiMessage ControlChange(int channel, int controller, int value) =>
        new()
        {
            Type = MidiMessageType.ControlChange,
            Channel = Clamp(channel, 0, 15),
            Data1 = Clamp(controller, 0, 127),
            Data2 = Clamp(value, 0, 127),
        };

    /// <summary>
    /// Creates a Program Change (PC) message.
    /// </summary>
    /// <param name="channel">MIDI channel (0–15).</param>
    /// <param name="program">Program number (0–127).</param>
    public static MidiMessage ProgramChange(int channel, int program) =>
        new()
        {
            Type = MidiMessageType.ProgramChange,
            Channel = Clamp(channel, 0, 15),
            Data1 = Clamp(program, 0, 127),
        };

    /// <summary>
    /// Creates a Note On message.
    /// </summary>
    /// <param name="channel">MIDI channel (0–15).</param>
    /// <param name="note">MIDI note number (0–127).</param>
    /// <param name="velocity">Note velocity (0–127).</param>
    public static MidiMessage NoteOn(int channel, int note, int velocity) =>
        new()
        {
            Type = MidiMessageType.NoteOn,
            Channel = Clamp(channel, 0, 15),
            Data1 = Clamp(note, 0, 127),
            Data2 = Clamp(velocity, 0, 127),
        };

    /// <summary>
    /// Creates a Note Off message.
    /// </summary>
    /// <param name="channel">MIDI channel (0–15).</param>
    /// <param name="note">MIDI note number (0–127).</param>
    /// <param name="velocity">Release velocity (0–127).</param>
    public static MidiMessage NoteOff(int channel, int note, int velocity) =>
        new()
        {
            Type = MidiMessageType.NoteOff,
            Channel = Clamp(channel, 0, 15),
            Data1 = Clamp(note, 0, 127),
            Data2 = Clamp(velocity, 0, 127),
        };

    /// <summary>
    /// Creates a Pitch Bend message.
    /// </summary>
    /// <param name="channel">MIDI channel (0–15).</param>
    /// <param name="value">Pitch bend value (0–16383, center = 8192).</param>
    public static MidiMessage PitchBend(int channel, int value) =>
        new()
        {
            Type = MidiMessageType.PitchBend,
            Channel = Clamp(channel, 0, 15),
            Data1 = value & 0x7F,
            Data2 = (value >> 7) & 0x7F,
        };

    /// <summary>
    /// Returns the raw MIDI status byte (type | channel).
    /// </summary>
    public byte StatusByte => (byte)((int)Type | (Channel & 0x0F));

    /// <summary>
    /// Returns the raw MIDI data bytes for this message.
    /// </summary>
    public byte[] DataBytes =>
        Type switch
        {
            MidiMessageType.ProgramChange or MidiMessageType.ChannelAftertouch =>
                [(byte)Data1],
            _ =>
                [(byte)Data1, (byte)Data2],
        };

    /// <inheritdoc/>
    public override string ToString() =>
        Type switch
        {
            MidiMessageType.ControlChange => $"CC Ch{Channel + 1} #{Data1} = {Data2}",
            MidiMessageType.ProgramChange => $"PC Ch{Channel + 1} #{Data1}",
            MidiMessageType.NoteOn => $"Note On Ch{Channel + 1} Note={Data1} Vel={Data2}",
            MidiMessageType.NoteOff => $"Note Off Ch{Channel + 1} Note={Data1} Vel={Data2}",
            MidiMessageType.PitchBend => $"Pitch Bend Ch{Channel + 1} Val={(Data2 << 7) | Data1}",
            _ => $"MIDI {Type} Ch{Channel + 1} {Data1} {Data2}",
        };

    private static int Clamp(int value, int min, int max) =>
        value < min ? min : value > max ? max : value;
}

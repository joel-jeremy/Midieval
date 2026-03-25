namespace Midieval.Models;

/// <summary>
/// Types of MIDI messages supported by the app.
/// </summary>
public enum MidiMessageType
{
    NoteOff = 0x80,
    NoteOn = 0x90,
    PolyphonicAftertouch = 0xA0,
    ControlChange = 0xB0,
    ProgramChange = 0xC0,
    ChannelAftertouch = 0xD0,
    PitchBend = 0xE0,
}

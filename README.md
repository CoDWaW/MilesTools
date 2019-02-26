# MilesTools
MilesTools is a set of utilities to help analyzing the audio archive of game Titanfall 2 and Apex Legends.

*This documentation is work in progress.*

## Miles Sound System 10
Titanfall 2 and Apex Legends use Miles Sound System 10 as their audio system. MSS 10 is originally developed for TF|2, while previous versions of MSS is widely used in other games.

MSS 10 uses a new audio archive format that consists of multiple files:
- `.mprj` - Project file. Loaded by MSS 10 library at game initialization.
- `.mbnk` - Bank file. (?)
- `.mbnk_digest` - Bank digest file. (?)
- `.mstr` - Stream banks. Multiple stream banks exist; voice banks and sfx banks are separated.
- `*_patch_*.mstr` - Stream bank patches. Patches are presumably used for adding (or modifying?) audio assets in the primary stream bank, and are applied one by one after their respective main stream bank is loaded.

## Bink Audio
Sound files (actually fragments) can be extracted from stream banks directly by looking up their header, `1FCB`, or `{ 0x31, 0x46, 0x43, 0x42 }`, for Bink Audio files.

Each piece of audio can be converted to `.wav` with Binkalore (a rewrite of BinkA2Wav), but the pieces sound like fragments at the beginning of actual audio files. In fact, these fragments only comprise a small portion of a stream bank.

Following the last identifiable Bink Audio header in a stream bank is a large blob with no Bink Audio headers inside (the "tail blob"). The majority of audio data is stored here. Binkalore or BinkA2Wav is unable to convert this piece in its entirety.
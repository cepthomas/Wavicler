# Wavicler

Play tool for editing audio waves.

- Using int for sample index. This gives us a max time of about 1.3 hours. Plenty for the purpose of this application.
- Everything internal is 32bit fp 44100Hz.
- 44.1 sample/msec <-> 0.0226757369614512 msec/sample



## UI/Tools
- Mode: bars/beats (BB) or time/samples (TS).

- Main
  - standard file, about stuff
  - If stereo file ask to open as mono/L/R/both
  - Play/loop/rewind

- BB mode
  - Select two sample indexes (SI)
  - identify as number of bars/beats - show in UI
  - Select two BB. Uses snap mode: bars or beats

- TS mode
  - Select two samples
  - Show num and time in UI
  - Select two TS. Uses snap mode: numsamples or 

- Context menu?
  ? cut/copy/paste/insert
  - Set gain in selection
  ? show/select altered gain.
  - Save selection as...

- Tools
  - split stereo m4a, save as wavs, convert to mono.
  - resample.
  - detect tempo.


## Navigation
- Wheel:
  - none - x shift
  - ctrl - x zoom
  - shift - y zoom
  - L click sels mark1, ctrl-L selects mark2
  - R click opens context menu


## Work flow
  - Open an audio file. This is a now a "clip", ClipEditor.
  - Select a part of a clip. Snap to bar/beat/sample/time/???
  ? Gain envelope. Simple only for now.
  ? Stretch/fit etc.
  - Export selection to a new clip.
  - Clip play, select start, loop on/off/, auto-start, rewind

## ????
  - New empty clip.
  - Combine multiple clips, incl blank.
  - Render to new clip/file.


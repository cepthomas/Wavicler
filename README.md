# Wavicler

Play tool for manipulating and slicing audio waves. The general idea is to use it to
chop up pieces of audio into chunks that can be used in a music production DAW. The
selection can be by sample, time, or beats with user supplied BPM.

Caveats:
- Everything internal is 32 bit fp 44100Hz.
- Uses int for sample index. This gives us a theoretical maximum clip time of about 1.3 hours.
  In actuality it uses a setting from AudioLib to limit this (default is 5 minutes).
  This gives plenty of time for the purpose of this application.

# UI

## Main
- Standard file menu.
- Open an audio file. This is a now a `clip` in a clip editor tab page.
- Double click the tab to close the clip.
- If it's a stereo file the user is asked to open as left, right, mono, or both.
- Selection play, rewind, loop on/off, auto-start on/off.

## Selection
- Beats mode:
  - Establish timing by select two samples and identify corresponding number of beats.
  - Show in waveform.
  - Subsequent selections are by beat using snap.

- Time mode:
  - Select two times using ?? resolution.
  - Shows number of samples and time in UI.

- Sample mode:
  - Select two samples using ?? resolution.
  - Shows number of samples and time in UI.

## Mouse
- Wheel alone is time shift (x shift).
- Wheel + ctrl is time zoom (x zoom).
- Wheel + shift is gain (y zoom).
- Left click is selection start.
- Left click + ctrl is selection length.
- Right click is context menu.

## Context Menu
- Copy selection to new clip.
- Close.
- Set gain to fill or default.

## Tools
- Stereo file: split into two wavs, combine to mono wav.
- Resample file.
- ? Detect tempo.
- Edit settings.
- About.


## Future
  - Combine multiple clips, incl blank.
  - Edit - cut/copy/paste/insert.
  - Gain envelope.
  - Stretch/fit etc.

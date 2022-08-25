# Wavicler

Play tool for editing audio waves.

- Everything internal is 32bit fp 44100Hz.
- Using int for sample index. This gives us a max time of about 1.3 hours. Plenty for the purpose of this application.


## UI/Tools

- Main
  - standard file, about/settings stuff
  - Open an audio file. This is a now a `clip` in a ClipEditor tab page.
  - If stereo file ask to open as mono/L/R/both
  - Clip play, loop on/off, auto-start on/off, rewind

- bars/beats mode
  - Select two samples
  - identify as number of bars/beats - show in UI
  - Select two bars/beats. Uses snap mode: bars or beats

- time/samples mode
  - Select two samples
  - Show num and time in UI
  - Select two time/samples. Uses snap mode: numsamples or time

- Context or edit menu
  - Copy selection to new clip

- Tools
  - stereo file: split into two wavs, combine to mono wav. Does resampling.
  - detect tempo.

- Navigation :
  - wheel no mods is x shift
  - wheel + ctrl is x zoom
  - wheel + shift is gain (y zoom)
  - left click is SelStart
  - left click + ctrl is SelLength


## TODO-future
  - New empty clip.
  - Combine multiple clips, incl blank.
  - Render to new clip/file.
  - cut/copy/paste/insert
  - Set gain in selection
  - show/select altered gain.
  - Gain envelope. Simple only for now.
  - Stretch/fit etc.
  - R click opens context menu

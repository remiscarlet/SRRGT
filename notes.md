# Notes

- Something reminiscent of flash game "Knights of Rock" - Has some progress animation with notes affecting actions?
- 7-key? 5-key? 4-key?
    - Switch between two positions? wasd+arrows vs sdfjkl+space
- Rhythm-action rpg or pure rhythm? Choose?
- Procedurally generated charts?


## Todos


## "Roadmap"/Priorities


## Random Design Thoughts
- Sample rate/count vs dsptime for time tracking?
    - Sample rate is easier for keeping "true time" and syncing everything to audio - accounts for fps lag nicely
    - dspTime or other form of actual "time" is easier for formats and accounts for edge case/non-beat conforming music
- What about for chart data?
    - By "measures/beat" or "pure time"?


## Chart format
<eventtype:int>,<eventdata:data>
    where
    <eventtype> == 0:
        type: note
        <keydata>: <keynum>[,<keynum>...]
        <eventdata>: <keydata:data>,<isByBeat:bool>,<beatdata:data>
            where
            <isByBeat> == true:
                <beatdata>: <beatnum:int>
            <isByBeat> == false:
                <beatdata>: <playTime:double> 
    <eventtype> == 1:
        type: bpm change
        <eventdata>: <newBPM:int><isByBeat:bool>,<timedata:data>
            where
            <isByBeat> == true:
                <timedata>: <beatnum:int>
            <isByBeat> == false;
                <timedata>: <playTime:double>
    <eventtype> == 2:
        type: numkeys change
        <eventdata>: <numKeys:int>,<isByBeat:bool>,<timedata:data>,<transitionDurInBeats:int>
        duration of 0 => instantaneous
    <eventtype> == 3:
        type: "bonus points time" toggle? start? Prob want explicit start/stops
        <NotSupportedYet>

<eventtype:int=0>,<keynum>[-<keynum>...],<isByBeat:bool=1>,<beatnum:int>
<eventtype:int=0>,<keynum>[-<keynum>...],<isByBeat:bool=0>,<playTime:double>
<eventtype:int=1>,<newBPM:int>,<isByBeat:bool=1>,<beatnum:int>
<eventtype:int=1>,<newBPM:int>,<isByBeat:bool=0>,<playTime:double>

### Theme


### Story/Lore


### Levels


### UI/UX

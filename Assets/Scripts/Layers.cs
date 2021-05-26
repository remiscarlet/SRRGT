using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layers {

    public static int Default = 0;
    public static LayerMask DefaultMask = Default;

    public static int NonJudgeableNotes = 16;
    public static LayerMask NonJudgeableNotesMask = NonJudgeableNotes;
    public static int JudgeableNotes = 17;
    public static LayerMask JudgeableNotesMask = JudgeableNotes;
    public static int MissedNotes = 18;
    public static LayerMask MissedNotesMask = MissedNotes;
}

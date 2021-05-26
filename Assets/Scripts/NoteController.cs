using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class NoteController : MonoBehaviour {
    private bool noteAlreadyMissed = false;

    public int Pos {
        get;
        set;
    } = 0;

    private ScoreManager ScoreManager;
    private NotesManager NotesManager;

    // Start is called before the first frame update
    void Start() {
        ScoreManager = ReferenceManager.ScoreManager;
        NotesManager = ReferenceManager.NotesManagerComponent;
    }

    public void RegisterHitMiss() {
        ScoreManager.RegisterMiss(transform);
        SelfDestroy();
    }

    public void RegisterHitGood() {
        ScoreManager.RegisterGood(transform);
        SelfDestroy();
    }

    public void RegisterHitPerfect() {
        ScoreManager.RegisterPerfect(transform);
        SelfDestroy();
    }

    // Update is called once per frame
    void Update() {
        UpdatePosition();

        if (NoteWasMissed() && ! noteAlreadyMissed) {
            // Point at which note is missed is, ie spawning hitresult text, is not the exact
            // same as when the object should be deleted. Deletion comes a little later.

            noteAlreadyMissed = true;
            RegisterHitMiss();
        } else if (IsOutOfBounds()) {
            // Shouldn't hit If not miss is at earlier z?
            SelfDestroy();
        }
    }

    bool NoteWasMissed() {
        // TODO: Make this timing based eventually. For now, naive position based.
        return gameObject.layer == Layers.MissedNotes;
    }

    public int minZPos = -3;

    bool IsOutOfBounds() {
        return transform.position.z <= minZPos;
    }

    void SelfDestroy() {
        NotesManager.DestroyNote(this);
    }

    private int noteFallSpeed = 10;

    void UpdatePosition() {
        transform.Translate(Vector3.back * noteFallSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HitOuter")) {
            gameObject.layer = Layers.JudgeableNotes;
        } else if (other.CompareTag("MissedNoteLine")) {
            gameObject.layer = Layers.MissedNotes;
        }
    }
}

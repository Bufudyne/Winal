using System;
using System.Collections;
using System.Collections.Generic;
using MidiPlayerTK;
using UnityEngine;

public class CharacterSoundPlayer : MonoBehaviour
{
// MidiPlayerGlobal is a singleton: only one instance can be created. Making static to have only one reference.

    MidiFilePlayer midiFilePlayer;

    /*private void OnEnable()
    {
        EventManager.StartListening(On.PointObtained, PlayOnCollision);
    }

    private void OnDisable()
    {
        EventManager.StopListening(On.PointObtained, PlayOnCollision);
    }*/

    private void Awake()
    {
        Debug.Log("Awake: dynamically add MidiFilePlayer component");

        // MidiPlayerGlobal is a singleton: only one instance can be created. 
        if (MidiPlayerGlobal.Instance == null)
            gameObject.AddComponent<MidiPlayerGlobal>();

        // When running, this component will be added to this gameObject. Set essential parameters.
        midiFilePlayer = gameObject.AddComponent<MidiFilePlayer>();
        midiFilePlayer.MPTK_CorePlayer = true;
        midiFilePlayer.MPTK_DirectSendToPlayer = true;
    }

    public void Start()
    {
        Debug.Log("Start: select a MIDI file from the MPTK DB and play");

        // Select a MIDI from the MIDI DB (with exact name)
        midiFilePlayer.MPTK_MidiName = "Yves Montand - A bicyclette";
        // Play the MIDI file
        midiFilePlayer.MPTK_Play();
    }
}


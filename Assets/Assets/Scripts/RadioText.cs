using System.Collections.Generic;
using UnityEngine;

public class RadioText : MonoBehaviour
{
    public List<SO_DialogueTemplate> RadioReactionLines;
    public AudioManager AudioManager;
    public SO_DialogueTemplate BabyBeaverLine;

    private DialogueManager _dialogueManager;

    private void Start()
    {
        _dialogueManager = GetComponent<DialogueManager>();
    }

    private void Update()
    {
        //If I had to redo that, I'd clearly use a delegate here instead of checking that every frame...!
        if (AudioManager.ChangedSong)
        {
            ChangeSongReactionLine();
        }
    }

    /// <summary>
    /// Changes the song reaction line depending on the name of the currently played song.
    /// </summary>
    private void ChangeSongReactionLine()
    {
        if (_dialogueManager.SO_DialogueEvents.Count > 1)
        {
            _dialogueManager.SO_DialogueEvents.Remove(_dialogueManager.SO_DialogueEvents[1]);
        }

        if (AudioManager.CurrentRadioSong.name == "01_Oscar Peterson")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[0]);
        }

        if (AudioManager.CurrentRadioSong.name == "02_Wilf Carter")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[1]);
        }

        if (AudioManager.CurrentRadioSong.name == "03_William Eckstein")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[2]);
        }

        if (AudioManager.CurrentRadioSong.name == "04_Girl Guides")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[3]);
        }

        if (AudioManager.CurrentRadioSong.name == "05_Dame Emma Albani")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[4]);
        }

        if (AudioManager.CurrentRadioSong.name == "06_Michael Mitchell")
        {
            _dialogueManager.SO_DialogueEvents.Add(RadioReactionLines[5]);
        }
    }
}

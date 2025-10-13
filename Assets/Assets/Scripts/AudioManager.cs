using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip CurrentRadioSong;
    public List<AudioClip> RadioSongsList;
    public bool ChangedSong;

    private AudioSource _audioSource;
    [SerializeField] private List<AudioClip> _playedSongsList = new List<AudioClip>();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (RadioSongsList.Count == 0)
        {
            RepopulateSongsList();
        }

        if (!_audioSource.isPlaying)
        {
            RandomizeNextSong();
        }
    }

    /// <summary>
    /// Adds back the songs from the played songs list to the main songs list and clears the played songs list.
    /// </summary>
    private void RepopulateSongsList()
    {
        for (int i = 0; i < _playedSongsList.Count; i++)
        {
            RadioSongsList.Add(_playedSongsList[i]);
        }
        _playedSongsList.Clear();
    }

    /// <summary>
    /// Picks a random song, removes it from the songs list and adds played songs to a different list, waiting for the songs list to be empty.
    /// </summary>
    private void RandomizeNextSong()
    {
        int randomIndex = Random.Range(0, RadioSongsList.Count - 1);

        _audioSource.clip = RadioSongsList[randomIndex];
        CurrentRadioSong = _audioSource.clip;

        ChangedSong = true;

        _playedSongsList.Add(CurrentRadioSong);
        RadioSongsList.Remove(CurrentRadioSong);

        _audioSource.Play();
    }
}

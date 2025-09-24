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

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
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

    private void RepopulateSongsList()
    {
        for (int i = 0; i < _playedSongsList.Count; i++)
        {
            RadioSongsList.Add(_playedSongsList[i]);
            _playedSongsList.Remove(_playedSongsList[i]);
        }
    }

    private void RandomizeNextSong()
    {
        int randomIndex = Random.Range(0, RadioSongsList.Count);

        _audioSource.clip = RadioSongsList[randomIndex];
        CurrentRadioSong = _audioSource.clip;

        ChangedSong = true;

        _playedSongsList.Add(CurrentRadioSong);
        RadioSongsList.Remove(CurrentRadioSong);

        _audioSource.Play();
    }
}

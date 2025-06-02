using Godot;
using System;

public partial class MusicManager : Node
{
    private float LastPosition = 0;
    private AudioStreamPlayer _musicPlayer;
    public override void _Ready()
    {
        _musicPlayer = new AudioStreamPlayer();
        AddChild(_musicPlayer);

        _musicPlayer.Stream = GD.Load<AudioStream>("res://Resources/Music/fantasyorchestralthememix2.ogg");
        _musicPlayer.Play();
        _musicPlayer.Finished+= Repeat;
    }
    private void Repeat()
    {
        _musicPlayer.Stream = GD.Load<AudioStream>("res://Resources/Music/fantasyorchestralthememix2.ogg");
        _musicPlayer.Play();
    }
    public bool IsPlaying()=>_musicPlayer.Playing;
    public bool StopPlay()
    {
        if(_musicPlayer.Playing)
        {
            LastPosition =_musicPlayer.GetPlaybackPosition();
            _musicPlayer.Stop();
        }
        else
        {
            _musicPlayer.Play(LastPosition);
        }
        return _musicPlayer.Playing;
    }
}

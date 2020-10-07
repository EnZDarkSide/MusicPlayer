using DevExpress.Mvvm;
using LibVLCSharp.Shared;
using MusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MusicPlayer.ViewModels
{
    class GeneralViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public LibVLC LibVLC { get; private set; }
        public MediaPlayer Player { get; set; }

        private Track _CurrentTrack;
        public Track CurrentTrack { get => _CurrentTrack; set {
                _CurrentTrack = value;
                Player.Media = GetMedia(CurrentTrack);
            } 
        }

        public float CurrentPosition
        {
            get => Player.Position;
            set => Player.Position = value;
        }

        public string TrackTime { get; set; }
        public string TrackDuration { get; set; }

        public int CurrentVolume
        {
            get => Player.Volume;
            set => Player.Volume = value;
        }

        private Timer _PositionTimer;

        public List<Track> Playlist { get; set; }
        public string CurrentPlaylistDirectory { get; set; }

        private int CurrentTrackIndex { get; set; }

        public GeneralViewModel()
        {
            LibVLCSharp.Shared.Core.Initialize();

            LibVLC = new LibVLC();
            Player = new MediaPlayer(LibVLC);

            Player.VolumeChanged += Player_VolumeChanged;

            _PositionTimer = new Timer()
            {
                Interval = 1000,
            };

            _PositionTimer.Tick += UpdatePosition;
            _PositionTimer.Tick += UpdateTrackTime;
        }

        private void UpdatePosition(object sender, EventArgs e)
        {
            CurrentPosition = Player.Position;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentPosition"));
        }

        private void UpdateTrackTime(object sender, EventArgs e)
        {
            var d_time = Player.Time;
            var minutes = TimeSpan.FromMilliseconds(d_time).Minutes;
            var seconds = TimeSpan.FromMilliseconds(d_time).Seconds;

            TrackTime = $"{minutes}:{seconds:00}";
        }

        private void Player_VolumeChanged(object sender, MediaPlayerVolumeChangedEventArgs e)
        {
            CurrentVolume = Player.Volume;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentVolume"));
        }

        void GetPlaylist()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "MP3 files(*.mp3)|*.mp3";
                ofd.Multiselect = true;
                DialogResult result = ofd.ShowDialog();
                if (result == DialogResult.OK && ofd.FileNames.Any())
                {
                    Playlist = ofd.FileNames.Select(file => new Track(file)).ToList();

                    CurrentTrackIndex = 0;
                    CurrentTrack = Playlist[CurrentTrackIndex];
                    TrackDuration = CurrentTrack.DurationToString();


                    PlayPause();
                }
            }
        }

        void PlayPause()
        {
            if (CurrentTrack == null)
                return;

            if (Player.Media == null)
                Player.Media = GetMedia(CurrentTrack);

            if (Player.State != VLCState.Playing)
            {
                Player.Play();
                _PositionTimer.Start();
            }
            else
                Player.Pause();
        }

        void NextTrack()
        {
            Player.Stop();

            if (Playlist.Count() == 0)
                return;

            if (++CurrentTrackIndex >= Playlist.Count())
                CurrentTrackIndex = 0;

            CurrentTrack = Playlist[CurrentTrackIndex];
            TrackDuration = CurrentTrack.DurationToString();
            Player.Play();
        }

        void PreviousTrack()
        {
            Player.Stop();

            if (Playlist.Count() == 0)
                return;

            if(--CurrentTrackIndex == -1)
                CurrentTrackIndex = Playlist.Count() - 1;

            CurrentTrack = Playlist[CurrentTrackIndex];
            TrackDuration = CurrentTrack.DurationToString();
            Player.Play();
        }

        private Media GetMedia(Track track)
        {
            return new Media(LibVLC, new Uri(track.Path));
        }

        public DelegateCommand cmdOpenPlaylistFiles {
            get => new DelegateCommand(() => GetPlaylist());
        }
        public DelegateCommand cmdPlayPause {
            get => new DelegateCommand(() => PlayPause());
        }
        public DelegateCommand cmdNextTrack{
            get => new DelegateCommand(() => NextTrack());
        }
        public DelegateCommand cmdPreviousTrack
        {
            get => new DelegateCommand(() => PreviousTrack());
        }

    }
}

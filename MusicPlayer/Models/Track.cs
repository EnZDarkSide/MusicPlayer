using MusicPlayer.DataHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MusicPlayer.Models
{
    class Track
    {
        public string Title { get; set; }
        public double Duration { get; set; }
        public BitmapImage ImageSource { get; set; }
        public string[] Performers { get; set; }
        public string Path { get; set; }

        public Track(string path)
        {
            var defaultCover = (BitmapImage)App.Current.Resources["DefaultAlbumCover"];
            var result = TagLib.File.Create(path);

            Title = result.Tag.Title;
            Performers = result.Tag.Performers;
            Duration = result.Properties.Duration.TotalSeconds;

            Path = path;
            ImageSource = result.Tag.Pictures.Length != 0 ? ImageHelper.GetImage(result.Tag.Pictures[0].Data.Data) : defaultCover;
        }

        public string DurationToString()
        {
            var minutes = TimeSpan.FromSeconds(Duration).Minutes;
            var seconds = TimeSpan.FromSeconds(Duration).Seconds;

            return $"{minutes}:{seconds:00}";
        }
    }
}

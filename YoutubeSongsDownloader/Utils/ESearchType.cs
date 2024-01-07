using System.ComponentModel.DataAnnotations;

namespace YoutubeSongsDownloader.Utils
{
    public enum ESearchType
    {
        [Display(Name = "Playlist")]
        Playlist,
        [Display(Name = "Single Videos")]
        Video
    }
}

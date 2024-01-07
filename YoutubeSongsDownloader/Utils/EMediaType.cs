using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace YoutubeSongsDownloader.Utils
{
    public enum EMediaType
    {
        [Display(Name = "Video")]
        Video,
        [Display(Name = "Audio")]
        Audio
    }
}

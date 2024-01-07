using YoutubeExplode.Search;

namespace YoutubeSongsDownloader.Utils.Classes
{
    public class Playlist
    {
        public int Id { get; set; }
        public PlaylistSearchResult Entity { get; set; }


        public Playlist(int Id, PlaylistSearchResult entity)
        {
            this.Id = Id;
            this.Entity = entity;
        }
    }
}

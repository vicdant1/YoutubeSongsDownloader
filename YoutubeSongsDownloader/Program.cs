using MediaToolkit;
using MediaToolkit.Model;
using System.ComponentModel.DataAnnotations;
using VideoLibrary;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeSongsDownloader.Utils;
using YoutubeSongsDownloader.Utils.Classes;
using YoutubeSongsDownloader.Utils.Extensions;

Console.WriteLine("Youtube Video Download");

ESearchType searchType;
EMediaType mediaType = EMediaType.Audio;
List<string> searchList = new List<string>()
{
    "Angra",
    "Nirvana",
    "Beatles"
};
string currentUserInput = string.Empty;


Console.WriteLine("Choose what search you are going to do: ");
Console.WriteLine($"0 - Playlist\n1 - Video");


while (true)
{
    try
    {
        int userChoice = Convert.ToInt16(Console.ReadLine());
        if (userChoice < 0 || userChoice > 1)
            throw new Exception();

        searchType = (ESearchType)userChoice;

        Console.WriteLine($"'{searchType.GetAttribute<DisplayAttribute>().Name}' Mode Selected");

        break;
    }
    catch
    {
        Console.WriteLine("Invalid input, try again.");
    }
}

Console.WriteLine("Enter terms to search: (type 'End Search' to finish searching)");
while (true)
{
    currentUserInput = Console.ReadLine();
    if (currentUserInput.ToLower().Contains("end search"))
        break;

    if (searchList.Any(x => x.Contains(currentUserInput)))
    {
        Console.WriteLine("Term already added");
        continue;
    }

    searchList.Add(currentUserInput);
}

var youtubeClient = new YoutubeClient();
var youtubeDownloader = new YouTube();
const int PL_MAX_RESULT_LIMIT = 10;
int playlistCounter = 0;
int playlistIterator = 0;
foreach (string term in searchList)
{
    List<Playlist> playlistsResults = new List<Playlist>();
    await foreach (var playlist in youtubeClient.Search.GetPlaylistsAsync(term))
    {
        playlistsResults.Add(new Playlist(playlistCounter, playlist));
        playlistCounter++;
        playlistIterator++;

        if (playlistIterator == PL_MAX_RESULT_LIMIT)
        {
            while (true)
            {
                Console.WriteLine("Localized playlists: ");
                foreach (var result in playlistsResults)
                    Console.WriteLine($"{result.Id + 1} - {result.Entity.Title} ({result.Entity.Author})");

                Console.Write("Show more playlists? [y/n]");
                currentUserInput = Console.ReadLine();

                if (currentUserInput.ToLower().Equals("y"))
                {
                    playlistIterator = 0;
                    break;
                }

                if (currentUserInput.ToLower().Equals("n"))
                    break;

                Console.WriteLine("Invalid response, try 'y' or 'n'");
            }

            if (playlistIterator != 0)
                break;
        }
    }

    Console.WriteLine("Localized playlists: ");
    foreach (var playlist in playlistsResults)
        Console.WriteLine($"{playlist.Id + 1} - {playlist.Entity.Title}");

    Console.Write("Please, select the playlists you want to download by comma separated index (1,3,4,10...): ");

    currentUserInput = Console.ReadLine();
    var indexes = currentUserInput.Split(',').Select(index => Convert.ToInt16(index) - 1);

    Console.WriteLine(indexes);

    Console.WriteLine("Selected playlists: ");
    var selectedPlaylists = playlistsResults.Where(result => indexes.Contains(result.Id)).ToList();


    string downloadDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
    foreach (var playlist in selectedPlaylists)
    {
        if (!Directory.Exists($"{downloadDirectoryPath}\\{playlist.Entity.Title}\\"))
            Directory.CreateDirectory($"{downloadDirectoryPath}\\{playlist.Entity.Title}\\");

        var playlistVideosUrls = new List<string>();

        await foreach (var item in youtubeClient.Playlists.GetVideosAsync(playlist.Entity.Id))
            playlistVideosUrls.Add(item.Url);



        foreach (var videoUrl in playlistVideosUrls)
        {
            var video = await youtubeDownloader.GetVideoAsync(videoUrl);
            
            File.WriteAllBytes($"{downloadDirectoryPath}\\{playlist.Entity.Title}\\{video.FullName}", video.GetBytes());

            var inputFile = new MediaFile
            {
                Filename = $"{downloadDirectoryPath}\\{playlist.Entity.Title}\\{video.FullName}",
            };

            var outputFile = new MediaFile
            {
                Filename = $"{downloadDirectoryPath}\\{playlist.Entity.Title}\\{video.FullName}".Replace("mp4", "mp3"),
            };

            using(var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }

            File.Delete(inputFile.Filename);
        }
    }
}


Console.ReadLine();
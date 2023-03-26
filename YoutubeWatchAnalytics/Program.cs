using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Xml;
using YoutubeWatchAnalytics;

bool run = true;

while (run)
{
    try
    {
        Console.Clear();
        Console.WriteLine(@"__   __          _         _        __        __    _       _        _                _       _   _          ");
        Console.WriteLine(@"\ \ / /__  _   _| |_ _   _| |__   __\ \      / /_ _| |_ ___| |__    / \   _ __   __ _| |_   _| |_(_) ___ ___ ");
        Console.WriteLine(@" \ V / _ \| | | | __| | | | '_ \ / _ \ \ /\ / / _` | __/ __| '_ \  / _ \ | '_ \ / _` | | | | | __| |/ __/ __|");
        Console.WriteLine(@"  | | (_) | |_| | |_| |_| | |_) |  __/\ V  V / (_| | || (__| | | |/ ___ \| | | | (_| | | |_| | |_| | (__\__ \");
        Console.WriteLine(@"  |_|\___/ \__,_|\__|\__,_|_.__/ \___| \_/\_/ \__,_|\__\___|_| |_/_/   \_\_| |_|\__,_|_|\__, |\__|_|\___|___/");
        Console.WriteLine(@"                                                                                        |___/                ");
        Console.WriteLine("                                               by eugene :)");
        Console.WriteLine();
        Console.WriteLine("Request Packages [1]");
        Console.WriteLine("Merge Packages Files [2]");
        Console.WriteLine("Exit [0]");
        int key = Convert.ToInt32(Console.ReadLine());
        switch (key)
        {
            case 0:
                run = false;
                break;
            case 1:
                RequestPackage();
                break;
            case 2:
                MergePackages();
                break;
            default:
                break;
        }
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
        Console.WriteLine("Press any button...");
        Console.ReadKey();
    }
}
void RequestPackage()
{
    Console.Clear();
    Console.Write("Json-Path: ");
    string jsonpath = Console.ReadLine();
    string watchJson = string.Empty;
    using (StreamReader reader = new StreamReader(jsonpath))
    {
        watchJson = reader.ReadToEnd();
    }
    Console.Write("Youtube API-Key: ");
    string apiKey = Console.ReadLine();
    Console.Write("Application Name: ");
    string applicationName = Console.ReadLine();
    Console.WriteLine("Loading. . .");
    List<string> videoURLs = new List<string>();
    using (JsonTextReader jsonTextReader = new JsonTextReader(new StringReader(watchJson)))
    {
        while (jsonTextReader.Read())
        {
            if (jsonTextReader.Value != null)
            {
                if (jsonTextReader.Value.ToString() == "titleUrl")
                {
                    jsonTextReader.Read();
                    videoURLs.Add(jsonTextReader.Value.ToString());
                }
            }
        }
    }
    Console.WriteLine("Loaded Videos into List. . .");
    Console.WriteLine($"Found {videoURLs.Count} Videos.");
    Console.WriteLine($"Needs to be requested in {Math.Ceiling((double)videoURLs.Count / (double)10000)} Packages from Youtubes Free API (10k max per day)");
    Console.WriteLine();
    Console.Write("What Package would you like to request?: ");
    int package = Convert.ToInt32(Console.ReadLine());

    List<VideoData> videoList = new List<VideoData>();
    for (int i = (package - 1) * 10000; i < 10000 * package; i++)
    {
        GetYoutubeData(videoURLs[i], videoList, apiKey, applicationName);
    }
    Console.WriteLine("Loaded Videos from API. . .");
    using (StreamWriter streamWriter = new StreamWriter($"package{package}.txt"))
    {
        foreach (VideoData item in videoList)
        {
            streamWriter.WriteLine(item.URL);
            streamWriter.WriteLine(item.Title);
            streamWriter.WriteLine(item.ChannelName);
            streamWriter.WriteLine(item.Duration);
        }
    }

    Console.WriteLine($"Package {package}");
    Console.WriteLine($"Estimated time spend on Youtube: {GetTotalDuration(videoList)}");
    Console.WriteLine($"Creators watched: {GetTotalCreator(videoList)}");
    Console.WriteLine();
    Console.WriteLine("Press any button...");
    Console.ReadKey();
}
void MergePackages()
{
    Console.Clear();
    Console.Write("How many Packages would you like to merge?: ");
    int paketCount = Convert.ToInt32(Console.ReadLine());
    List<string> packagePaths = new List<string>();
    for (int i = 0; i < paketCount; i++)
    {
        Console.Write($"Path-Package {i + 1}: ");
        packagePaths.Add(Console.ReadLine());
    }

    List<VideoData> videoList = new List<VideoData>();
    foreach (string package in packagePaths)
    {
        using (StreamReader sr = new StreamReader(package))
        {
            while (!sr.EndOfStream)
            {
                string url = sr.ReadLine();
                string title = sr.ReadLine();
                string channelName = sr.ReadLine();
                TimeSpan duration = TimeSpan.Parse(sr.ReadLine());
                videoList.Add(new VideoData(url, title, channelName, duration));
            }
        }
    }
    Dictionary<string, int> creatorList = GetTotalCreator(videoList);
    TimeSpan totalTime = GetTotalDuration(videoList);
    Console.WriteLine();
    Console.WriteLine("Merged Packages");
    if (totalTime.Days > 0) Console.WriteLine($"Estimated time spend on Youtube: {totalTime.Days} Days and {totalTime.Hours}h");
    else Console.WriteLine($"Estimated time spend on Youtube: {totalTime.Hours}h");

    Console.WriteLine($"Videos watched: {videoList.Count}");
    Console.WriteLine($"Creators watched: {creatorList.Count}");
    Console.WriteLine();
    Console.WriteLine("Your Top 10 Creators:");
    var topCreators = creatorList.OrderByDescending(x => x.Value).Take(10);
    foreach (var creator in topCreators)
    {
        Console.WriteLine($"{creator.Key}: {creator.Value} Videos");
    }
    Console.WriteLine();
    Console.WriteLine("Your Top 10 Creators in time:");
    Dictionary<string, TimeSpan> creatorListTime = GetTotalCreatorTime(videoList);
    var topCreatorsTime = creatorListTime.OrderByDescending(x => x.Value).Take(10);
    foreach (var creator in topCreatorsTime)
    {
        Console.WriteLine($"{creator.Key}: {creator.Value.Days} Days and {creator.Value.Hours}h");
    }
    Console.WriteLine();

    Console.Write("Press any button...");
    Console.ReadKey();
}

TimeSpan GetTotalDuration(List<VideoData> videoList)
{
    TimeSpan timeSpan = new TimeSpan();
    foreach (VideoData video in videoList)
    {
        //Sorting out extramly (Livesteams, 24h Videos...)
        if (video.Duration <= new TimeSpan(5, 0, 0) && video.Duration.Days == 0)
        {
            timeSpan += video.Duration;
        }
        else
        {
            Console.WriteLine();
        }
    }
    return timeSpan;
}
void GetYoutubeData(string videoLink, List<VideoData> videoList, string apiKey, string applicationName)
{
    // Extract the video ID from the link
    string videoId = ExtractVideoId(videoLink);

    // Use the YouTube API to fetch the video information
    var youtubeService = new YouTubeService(new BaseClientService.Initializer()
    {
        ApiKey = apiKey,
        ApplicationName = applicationName
    });

    try
    {
        var videoRequest = youtubeService.Videos.List("snippet,contentDetails");
        videoRequest.Id = videoId;

        var videoResponse = videoRequest.Execute();
        var video = videoResponse.Items[0];

        // Extract the title, duration, and creator from the video information
        string title = video.Snippet.Title;
        string duration = video.ContentDetails.Duration;
        string creator = video.Snippet.ChannelTitle;

        // Convert the duration string into a TimeSpan object
        TimeSpan time = XmlConvert.ToTimeSpan(duration);

        // Print the video information
        Console.WriteLine($"Video title: {title}");
        Console.WriteLine($"Video duration: {time}");
        Console.WriteLine($"Video creator: {creator}");

        videoList.Add(new VideoData(videoLink, title, creator, time));
    }
    catch (Exception ex)
    {

    }
}
string ExtractVideoId(string videoLink)
{
    // Use a regular expression to extract the video ID
    Regex regex = new Regex(@"v=([^&]*)");
    Match match = regex.Match(videoLink);

    if (match.Success)
    {
        return match.Groups[1].Value;
    }
    else
    {
        return null;
    }
}
Dictionary<string, int> GetTotalCreator(List<VideoData> videoList)
{
    Dictionary<string, int> creatorCount = new Dictionary<string, int>();

    foreach (VideoData video in videoList)
    {
        string creatorName = video.ChannelName;

        if (creatorCount.ContainsKey(creatorName))
        {
            creatorCount[creatorName]++;
        }
        else
        {
            creatorCount[creatorName] = 1;
        }
    }
    return creatorCount;
}

Dictionary<string, TimeSpan> GetTotalCreatorTime(List<VideoData> videoList)
{
    Dictionary<string, TimeSpan> creatorCount = new Dictionary<string, TimeSpan>();

    foreach (VideoData video in videoList)
    {
        string creatorName = video.ChannelName;

        if (creatorCount.ContainsKey(creatorName))
        {
            if(video.Duration <= new TimeSpan(5, 0, 0) && video.Duration.Days == 0)
                creatorCount[creatorName] += video.Duration;
        }
        else
        {
            if (video.Duration <= new TimeSpan(5, 0, 0) && video.Duration.Days == 0)
                creatorCount[creatorName] = video.Duration;
        }
    }
    return creatorCount;
}
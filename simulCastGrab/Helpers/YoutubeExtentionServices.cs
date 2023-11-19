using System.Text.Json;

namespace simulCastGrab.Helpers
{
    /// <summary>
    /// A list of helper functions for using the YoutubePlatformService.
    /// </summary>
    public static class YoutubeExtentionServices
    {
        /// <summary>
        /// Returns the channel id of a directly matching channel name.
        /// </summary>
        /// <param name="search">The channel name to search for.</param>
        /// <param name="apikey">The youtube api key.</param>
        /// <returns>The channel id, or null if none found.</returns>
        public static string? YoutubeChannelIDSearch(string search, string apikey)
        {
            string url = $"https://youtube.googleapis.com/youtube/v3/search?part=snippet&q={search}&key={apikey}&type=channel";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "SimulCastGrab");
            var stream = client.GetStreamAsync(url).Result;
            var response = JsonSerializer.Deserialize<GenericYoutubeResponse>(stream);
            if (response == null) return null;
            client.Dispose();

            return response.items.Where(x => x.snippet.channelTitle == search).FirstOrDefault()?.id.channelId;
        }

        /// <summary>
        /// Returns the first active youtube video id of a given channel id.
        /// </summary>
        /// <param name="channelId">The to be searched channel id.</param>
        /// <param name="apikey">The youtube api key.</param>
        /// <returns>The videoid, or null if none found.</returns>
        public static string? YoutubeLivestreamVideoIDSearch(string channelId, string apikey)
        {
            string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&channelId={channelId}&order=date&type=video&key={apikey}";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "SimulCastGrab");
            var stream = client.GetStreamAsync(url).Result;
            var response = JsonSerializer.Deserialize<GenericYoutubeResponse>(stream);
            if (response == null) return null;
            client.Dispose();

            return response.items.Where(x => (x.snippet.liveBroadcastContent != null) && (x.snippet.liveBroadcastContent != "none")).FirstOrDefault()?.id.videoId;
        }

        /// <summary>
        /// Returns the livestream chat id of a given video id.
        /// </summary>
        /// <param name="liveStreamId">The youtube video id.</param>
        /// <param name="apikey">The youtube api key.</param>
        /// <returns>The livestreamchatid, or null if none found.</returns>
        public static string? YoutubeLivestreamChatIDSearch(string liveStreamId, string apikey)
        {
            string url = $"https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails,snippet&id={liveStreamId}&key={apikey}";
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "SimulCastGrab");
            var stream = client.GetStreamAsync(url).Result;
            var response = JsonSerializer.Deserialize<LiveSreamYoutubeResponse>(stream);
            if (response == null) return null;
            client.Dispose();

            return response.items[0]?.liveStreamingDetails?.activeLiveChatId ?? null;
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class GenericYoutubeResponse
    {
        public List<GenericSearchItem> items { get; set; }
    }

    public class LiveSreamYoutubeResponse
    {
        public List<LivestreamSearchItem> items { get; set; }
    }

    public class GenericSearchItem
    {
        public GenericID id { get; set; }
        public GenericSnippet snippet { get; set; }
        public LiveStreamingDetails liveStreamingDetails { get; set; }
    }

    public class LivestreamSearchItem
    {
        public string id { get; set; }
        public GenericSnippet snippet { get; set; }
        public LiveStreamingDetails liveStreamingDetails { get; set; }
    }

    public class GenericID
    {
        public string channelId { get; set; }
        public string videoId { get; set; }
    }

    public class GenericSnippet
    {
        public string title { get; set; }
        public string channelTitle { get; set; }
        public string liveBroadcastContent { get; set; }
    }

    public class LiveStreamingDetails
    {
        public string activeLiveChatId { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

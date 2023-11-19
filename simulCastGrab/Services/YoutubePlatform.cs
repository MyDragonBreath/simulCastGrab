using System.Text.Json;

namespace simulCastGrab.Services
{
    /// <summary>
    /// The Youtube platform service.
    /// </summary>
    public class YoutubePlatform : Platform
    {
        private string ApiKey { get; }

        // The below can be found by polling https://www.googleapis.com/youtube/v3/videos?part=liveStreamingDetails,snippet&id=[LIVESTREAMID]

        /// <summary>
        /// Gets the stream's liveChatId.
        /// </summary>
        public string StreamId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubePlatform"/> class.
        /// </summary>
        /// <param name="streamId">The liveChatId of the stream.</param>
        /// <param name="apikey">The API key used to access the Youtube Data Api v3.</param>
        /// <param name="resetToStart">Determines if the service should get all messages, or just the new ones.</param>
        public YoutubePlatform(string streamId, string apikey, bool resetToStart = false)
        {
            StreamId = streamId;
            ApiKey = apikey;
            if (!resetToStart) lastChecked = DateTimeOffset.UtcNow;
            PollService();
        }

        /// <inheritdoc/>
        public override Platforms platformE
        {
            get { return Platforms.Youtube; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a connection to the api can be made.
        /// </summary>
        public bool IsConnected { get; set; } = true;

        /// <summary>
        /// Disconnects the service.
        /// </summary>
        public void Disconnect()
        {
            IsConnected = false;
        }

        private string currentPageId = string.Empty;
        private int currentPageIndex = 1;

        /// <summary>
        /// Gets or sets the Results Per Request.
        /// </summary>
        public int ResultsPerPage { get; set; } = 20;

        /// <summary>
        /// Gets or sets the delay between api calls.
        /// </summary>
        public int PullDelay { get; set; } = 10000;

        /// <summary>
        /// Gets or sets a value indicating whether the reset to the page index should occur.<br/>
        /// If you don't know what this does, ignore it.
        /// </summary>
        public bool ResetPageIndex { get; set; }

        private string liveChatUrl => $"https://www.googleapis.com/youtube/v3/liveChat/messages?liveChatId={StreamId}&part=snippet,authorDetails&maxResults={ResultsPerPage}&key={ApiKey}"
            +(currentPageId == string.Empty ? string.Empty : $"&pageToken={currentPageId}");

        private DateTimeOffset lastChecked = DateTimeOffset.MinValue;
        private DateTimeOffset delayCheck = DateTimeOffset.MinValue;

        /// <summary>
        /// Poll the youtube api.
        /// </summary>
        public async void PollService()
        {
            while (IsConnected)
            {
                while ((DateTimeOffset.UtcNow - delayCheck).TotalMilliseconds < PullDelay) await Task.Delay(PullDelay);
                if (!IsConnected) break;
                Console.WriteLine("pol");
                delayCheck = await RequestLiveChat() ?? DateTimeOffset.UtcNow;
            }
        }

        /// <summary>
        /// Directly request the live chat.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> determining time to wait.</returns>
        private async Task<DateTimeOffset?> RequestLiveChat()
        {
            using HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "SimulCastGrab");
            var stream = await client.GetStreamAsync(liveChatUrl);
            var response = await JsonSerializer.DeserializeAsync<LiveChatResponse>(stream);
            if (response == null) return DateTimeOffset.MaxValue;
            client.Dispose();

            var diff = 0;
            if (response.pageInfo.totalResults > response.pageInfo.resultsPerPage)
            {
                diff = response.pageInfo.totalResults;
                Console.WriteLine($"Exceeded snippet size: {response.pageInfo.resultsPerPage}/page" +
                    $"\n\tOn page: {currentPageIndex}" +
                    $"\n\tResponses Left {diff}");
                currentPageIndex += 1;
                currentPageId = response.nextPageToken;
            }
            else if (ResetPageIndex)
            {
                currentPageId = string.Empty;
                currentPageIndex = 1;
            }

            foreach (var item in response.items.Where(x => x.snippet.publishedAt > lastChecked).OrderBy(x=>x.snippet.publishedAt))
            {
                InvokeOnMessage(new(item.authorDetails.displayName, item.snippet.displayMessage, item.authorDetails.displayName, platformE));
            }

            lastChecked = response.items.Last().snippet.publishedAt > lastChecked ? response.items.Last().snippet.publishedAt : lastChecked;
            return (diff == 0) ? null : lastChecked;
        }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public class LiveChatResponse
    {
        public int pollingIntervalMillis { get; set; }
        public string nextPageToken { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<LiveMessage> items { get; set; }
    }

    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class LiveMessage
    {
        public AuthorDetails authorDetails { get; set; }
        public Snippet snippet { get; set; }
    }

    public class AuthorDetails
    {
        public string displayName { get; set; }
    }

    public class Snippet
    {
        public string displayMessage { get; set; }
        public DateTimeOffset publishedAt { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

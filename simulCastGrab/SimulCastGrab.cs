using simulCastGrab.Events;
using simulCastGrab.Events.Arguments;
using simulCastGrab.Helpers;
using simulCastGrab.Services;
using System.Globalization;
using static simulCastGrab.Events.EventHook;

namespace simulCastGrab
{
    /// <summary>
    /// The simultaneous casting chat grabber.
    /// </summary>
    public class SimulCastGrab
    {
        /// <summary>
        /// Gets or sets the onMessage event.
        /// </summary>
        public CEvent<MessageArgs>? onMessage { get; set; }

        /// <summary>
        /// Gets the <see cref="TwitchPlatform"/> instance.
        /// </summary>
        public TwitchPlatform? Twitch { get; }

        /// <summary>
        /// Gets the TwitchChannel string.
        /// </summary>
        public string? TwitchChannel { get; }

        private TwitchAuthentication? twitchAuthentication;

        /// <summary>
        /// Gets the <see cref="YoutubePlatform"/> Instance.
        /// </summary>
        public YoutubePlatform? Youtube { get; }

        /// <summary>
        /// Gets the <see cref="YoutubeSearchInformation"/> instance.
        /// </summary>
        public YoutubeSearchInformation? YoutubeInfo { get; }
        private string? youtubeAuthenticaion;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulCastGrab"/> class.
        /// </summary>
        /// <param name="twitch">The twitch channel.</param>
        /// <param name="twitchAuth">The <see cref="TwitchAuthentication"/> instance.</param>
        /// <exception cref="ServiceConnectionException">An error in connecting to the service.</exception>
        public SimulCastGrab(string twitch, TwitchAuthentication twitchAuth)
        {
            if (twitchAuth.Username == null || twitchAuth.Auth == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "Not Enough Twitch Login Information.");
            twitchAuthentication = twitchAuth;
            TwitchChannel = twitch;

            Twitch = AttemptTwitchConnection();
            Twitch.OnMessage += OnMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulCastGrab"/> class.
        /// </summary>
        /// <param name="youtubeInfo">The <see cref="YoutubeSearchInformation"/> instance.</param>
        /// <param name="youtubeApiKey">The youtube api key, from the google cloud console.</param>
        /// <exception cref="ServiceConnectionException">An error in connecting to the service.</exception>
        public SimulCastGrab(YoutubeSearchInformation youtubeInfo, string youtubeApiKey)
        {
            if (youtubeInfo.YoutubeChatID == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "No Youtube Chat Id.");
            YoutubeInfo = youtubeInfo;
            youtubeAuthenticaion = youtubeApiKey;

            Youtube = AttemptYoutubeConnection();
            Youtube.OnMessage += OnMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulCastGrab"/> class.
        /// </summary>
        /// <param name="twitch">The twitch channel.</param>
        /// <param name="twitchAuth">The <see cref="TwitchAuthentication"/> instance.</param>
        /// <param name="youtubeInfo">The <see cref="YoutubeSearchInformation"/> instance.</param>
        /// <param name="youtubeApiKey">The youtube api key, from the google cloud console.</param>
        /// <exception cref="ServiceConnectionException">An error in connecting to the service.</exception>
        public SimulCastGrab(string twitch, TwitchAuthentication twitchAuth, YoutubeSearchInformation youtubeInfo, string youtubeApiKey)
        {
            if (twitchAuth.Username == null || twitchAuth.Auth == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "Not Enough Twitch Login Information.");
            twitchAuthentication = twitchAuth;
            TwitchChannel = twitch;

            Twitch = AttemptTwitchConnection();
            Twitch.OnMessage += OnMessage;

            if (youtubeInfo.YoutubeChatID == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "No Youtube Chat Id.");
            YoutubeInfo = youtubeInfo;
            youtubeAuthenticaion = youtubeApiKey;

            Youtube = AttemptYoutubeConnection();
            Youtube.OnMessage += OnMessage;
        }

        /// <summary>
        /// Attempts a twitch connection.
        /// </summary>
        /// <returns>A <see cref="TwitchPlatform"/> instance.</returns>
        /// <exception cref="ServiceConnectionException">An error in connecting to the service.</exception>
        public TwitchPlatform AttemptTwitchConnection()
        {
            if (twitchAuthentication == null || twitchAuthentication.Auth == null || twitchAuthentication.Username == null || TwitchChannel == null)
                throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "Need twitch auth info.");
            try
            {
                var tw = new TwitchPlatform(twitchAuthentication.Auth, TwitchChannel, twitchAuthentication.Username);
                return tw;
            }
            catch (TimeoutException ex)
            {
                throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.IncorrectAuthenticationDetails, ex.Message);
            }
        }

        /// <summary>
        /// Attempts a youtube connection.
        /// </summary>
        /// <returns>A <see cref="YoutubePlatform"/> instance.</returns>
        /// <exception cref="ServiceConnectionException">An error in connecting to the service.</exception>
        public YoutubePlatform AttemptYoutubeConnection()
        {
            if (YoutubeInfo == null || YoutubeInfo.YoutubeChatID == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.NotEnoughInformation, "No youtube chat id.");
            if (youtubeAuthenticaion == null) throw new ServiceConnectionException(ServiceConnectionException.ConnectionFailureType.IncorrectAuthenticationDetails, "No Api Key");
            return new YoutubePlatform(YoutubeInfo.YoutubeChatID, youtubeAuthenticaion);
        }

        private void OnMessage(MessageArgs args)
        {
            onMessage?.SafeInvoke(args);
        }

        /// <summary>
        /// Safely dispose of the connection streams.
        /// </summary>
        public void Dispose()
        {
            Twitch?.Disconnect();
            Youtube?.Disconnect();
        }
    }

    /// <summary>
    /// Twitch Authentication holder class.
    /// </summary>
    public class TwitchAuthentication
    {
        /// <summary>
        /// Gets the twitch user's name.
        /// </summary>
        public string Username { get; }
        internal string Auth { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchAuthentication"/> class.
        /// </summary>
        /// <param name="username">The twitch user's name.</param>
        /// <param name="auth">The associated auth token (with or without the oauth: prefix).</param>
        public TwitchAuthentication(string username, string auth)
        {
            Username = username;
            Auth = auth.Trim().ToLower(new CultureInfo("en-US"));
            if (!Auth.StartsWith("oauth:", StringComparison.OrdinalIgnoreCase)) Auth = "oauth:" + Auth;
        }
    }

    /// <summary>
    /// Youtube search information for finding the live chat id.
    /// </summary>
    public class YoutubeSearchInformation
    {
        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public string? YoutubeName { get; }

        /// <summary>
        /// Gets the channel id.
        /// </summary>
        public string? YoutubeChannelID { get; }

        /// <summary>
        /// Gets the video id.
        /// </summary>
        public string? YoutubeVideoID { get; }

        /// <summary>
        /// Gets the live chat id.
        /// </summary>
        public string? YoutubeChatID { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="YoutubeSearchInformation"/> class.
        /// </summary>
        /// <param name="apiKey">The youtube api key.</param>
        /// <param name="name">The channel name.</param>
        /// <param name="channelid">The channel id.</param>
        /// <param name="videoid">The video id.</param>
        /// <param name="chatid">The live chat id.</param>
        public YoutubeSearchInformation(string apiKey, string? name = null, string? channelid = null, string? videoid = null, string? chatid = null)
        {
            YoutubeName = name;
            YoutubeChannelID = channelid;
            YoutubeVideoID = videoid;
            YoutubeChatID = chatid;

            if (YoutubeChannelID == null && YoutubeName != null)
            {
                var sChId = YoutubeExtentionServices.YoutubeChannelIDSearch(YoutubeName, apiKey);
                if (sChId != null)
                {
                    YoutubeChannelID = sChId;
                }
            }

            if (YoutubeVideoID == null && YoutubeChannelID != null)
            {
                var sVId = YoutubeExtentionServices.YoutubeLivestreamVideoIDSearch(YoutubeChannelID, apiKey);
                if (sVId != null)
                {
                    YoutubeVideoID = sVId;
                }
            }

            if (YoutubeChatID == null && YoutubeVideoID != null)
            {
                var sChat = YoutubeExtentionServices.YoutubeLivestreamChatIDSearch(YoutubeVideoID, apiKey);
                if (sChat != null)
                {
                    YoutubeChatID = sChat;
                }
            }
        }
    }

    /// <summary>
    /// A Service Connection error.
    /// </summary>
    public class ServiceConnectionException : Exception
    {
        /// <summary>
        /// A dedicated kind of connection error.
        /// </summary>
        public enum ConnectionFailureType
        {
            /// <summary>
            /// Not enough provided info.
            /// </summary>
            NotEnoughInformation,

            /// <summary>
            /// Wrong authentication info.
            /// </summary>
            IncorrectAuthenticationDetails,
        }

        /// <summary>
        /// Gets the <see cref="ConnectionFailureType"/> type.
        /// </summary>
        public ConnectionFailureType FailureType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConnectionException"/> class.
        /// </summary>
        /// <param name="failure">The <see cref="ConnectionFailureType"/> type.</param>
        /// <param name="message">The associated message.</param>
        public ServiceConnectionException(ConnectionFailureType failure, string message) : base(message)
        {
            FailureType = failure;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{FailureType} : " + base.ToString();
        }
    }
}

using IrcDotNet;

namespace simulCastGrab.Services
{
    /// <summary>
    /// A Twitch platform service.
    /// </summary>
    public class TwitchPlatform : Platform
    {
        private string OAuthToken { get; }

        /// <summary>
        /// Gets or Sets the watched Channel.
        /// </summary>
        public string Channel { get; set; }

        /// <summary>
        /// Gets or Sets the signed in User.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Gets or Sets the TwitchIrcClient.
        /// </summary>
        public TwitchIrcClient Client { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitchPlatform"/> class.
        /// </summary>
        /// <param name="auth">OAuth Token, Example: oauth:xxxxxxx .</param>
        /// <param name="channel">The watched channel.</param>
        /// <param name="user">The associated user to the token.</param>
        public TwitchPlatform(string auth, string channel, string user)
        {
            OAuthToken = auth;
            Channel = channel;
            User = user;
            Client = CreateTwitchIRC();
        }

        /// <inheritdoc/>
        public override Platforms platformE { get { return Platforms.Twitch; } }

        /// <summary>
        /// Disconnects the service.
        /// </summary>
        public void Disconnect()
        {
            Client.Disconnect();
            Client.Dispose();
        }

        /// <summary>
        /// Sends a message to the channel, if the OAuth token has the privellege.
        /// </summary>
        /// <param name="message">The string chat message.</param>
        public void SendMessage(string message)
        {
            Client.SendRawMessage($"PRIVMSG #{Channel} {message}");
        }

        /// <summary>
        /// Initializes a TwitchIRCClient.
        /// </summary>
        /// <returns>An <see cref="IrcClient"/> instance.</returns>
        /// <exception cref="TimeoutException">In the event authentication is wrong, this exception will occur.</exception>
        public TwitchIrcClient CreateTwitchIRC()
        {
            TwitchIrcClient client = new();
            client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            client.Connected += IrcClient_Connected;
            client.Registered += IrcClient_Registered;

            using ManualResetEventSlim registeredEvent = new(false);
            using ManualResetEventSlim connectedEvent = new(false);
            client.Connected += (sender2, e2) => connectedEvent.Set();
            client.Registered += (sender2, e2) => registeredEvent.Set();
            client.Connect("irc.twitch.tv", false, new IrcUserRegistrationInfo()
            {
                NickName = User,
                UserName = User,
                Password = OAuthToken,
            }
            );
            Console.Out.WriteLine("Connecting to twitch");
            if (!connectedEvent.Wait(10000)) throw new TimeoutException("Timed out: Connecting");
            Console.Out.WriteLine("Now connected to twitch");
            if (!registeredEvent.Wait(10000)) throw new TimeoutException("Timed out: Registering");
            Console.Out.WriteLine("Now registered to twitch");
            client.SendRawMessage($"join #{Channel}");
            return client;
        }

        private void IrcClient_Registered(object? sender, EventArgs e)
        {
            if (sender is not IrcClient client) return;
            client.RawMessageReceived += RawMessageReceived;
        }

        private void RawMessageReceived(object? sender, IrcRawMessageEventArgs e)
        {
            if (e.Message.Command != "PRIVMSG") return;
            InvokeOnMessage(new(e.Message.Source.Name, e.Message.Parameters[1], e.Message.Source.Name, platformE));
        }

        private void IrcClient_Connected(object? sender, EventArgs e)
        {
        }
    }
}

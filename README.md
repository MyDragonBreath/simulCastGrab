# Simultaneous streamCasting services chat Grabber

This is an api designed to help csharp modders with streamer integration.
A sample program is provided, and the api is fully documented within the source.

The api is designed to use a robus event system, and whilst async events may cause synchronisation issues within your modding environment,
(or any other csharp environment), working with IRC and REST requires it - lest the frame rate drops significantly.
If you must, the code is modifiable to become synchronous.

How do I get a youtube api key?
Goto https://console.cloud.google.com/, start a new project.
Goto the api section, and enable the youtube data api v3.
Goto the credentials section in api's, and make a new api key.
You may also need to create a oauth client id.

How do I get a twitch oauth key?
Goto here https://twitchapps.com/tmi/, and follow the instructions.
You can also make your own twitch developer app,
add a localhost:8080 redirect uri,
and start a local python server with python -m http.server 8080
then navigate here on your browser, and read the auth key from the top of the 404 page at the end.
https://id.twitch.tv/oauth2/authorize?client_id=[TWITCHCLIENTID]&response_type=token&redirect_uri=http://localhost:8080/&scope=chat%3Aread+chat%3Aedit
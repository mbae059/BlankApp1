Blank
---------

- Trying to make an App using WPF, Prism
- Streaming will also be implemented (1080p60)


Projects
---------
- BlankApp1 
-- The main project, which will be the WPF application using Prism
-- Uses .NET 8.0, C# 12.0, Prism 9.0.537
-- Be able to send messages and stream videos
-- The resolution shall be chosen as the user desire

- BlankApp1.Server
-- The server project, which will handle the backend logic and communication for the chat program
-- Uses .NET 8.0, C# 12.0, ASP.NET Core, SignalR
-- This project will be responsible for managing chat, videos, and other real-time communication features
-- The server keeps track of the data transfered on the console

Problems to solve
----
- Use AV1 codec to stream and play for efficiency
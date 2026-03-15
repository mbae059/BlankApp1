Blank
---------

- Trying to make an App using WPF, Prism
- Streaming will also be implemented (1080p60)


Projects
---------
- BlankApp1 
-- The main project, which will be the WPF application using Prism
-- Uses .NET 8.0, C# 12.0, Prism 9.0.537

- BlankApp1.Server
-- The server project, which will handle the backend logic and communication for the chat program
-- Uses .NET 8.0, C# 12.0, ASP.NET Core, SignalR
-- This project will be responsible for managing chat, videos, and other real-time communication features


Problems to solve
----
- When pressing the start button in MainWindowDMListView, starts to stream the desktop to the other user in the chat
- This is fine but when the user clicks the stop button, the stream should stop.
- streamRegion is replaced so the user cannot see the screen but the communication is still active, wasting resources and bandwidth. The stream should be properly stopped to free up resources and reduce bandwidth usage.

Crewly



Telegram bot for connecting clients with creative freelancers.

Clients can browse freelancer profiles by specialization, and freelancers can receive project requests directly through the bot.

The system includes registration, profile management, filtering, favorites, and client-side task management.



Tech Stack



.NET 8 (C#) — core framework.



Telegram.Bot API (v22.x) — Telegram integration.



Entity Framework Core — ORM for SQL Server.



Redis — session hash and cache storage.



Layered design: data managers, command handlers, and Telegram interaction separated by responsibility



New Update: Client Task Editing \& Deletion



This update introduces full task management capabilities on the client side:



✏️ Clients can now edit their existing tasks



❌ Clients can now delete tasks



This allows clients to manage projects more flexibly without needing admin assistance.



Future Improvements



Expanded management panel for clients



Advanced analytics for freelancers


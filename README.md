Crewly

Telegram bot for connecting clients with creative freelancers.
Clients can browse freelancer profiles by specialization, and freelancers can receive project requests directly through the bot.
Includes registration, profile management, filtering, favorites.

Features

Registration flow for both clients and freelancers — including specialization and contact details.

Clients: browse and filter freelancers, add profiles to favorites.

Freelancers: receive client requests, manage availability status.

Modular architecture separating message handling, business logic, and data layers.

Tech Stack

.NET 8 (C#) — core framework.

Telegram.Bot API (v22.x) — integration with Telegram.

Entity Framework Core — ORM for SQL Server database operations.

Redis — used for session hash and cache storage (updated system — see below).

Layered design: data managers, command handlers, and Telegram interaction separated by responsibility.

Update: Hash Storage Migration to Redis

Previously, hash data and temporary user states were stored using an in-memory or SQL-based mechanism.
In this update, all user session hashes and temporary state data are now stored in Redis.

Benefits of this change:

⚡ High performance and low latency data access.

⏳ Built-in TTL management — automatic expiration of session keys.

☁️ Scalable architecture — supports multiple bot instances with a shared cache.

🧩 Cleaner design — separates persistent data (SQL) and volatile data (Redis).

Future Improvements

Client Management Panel Enhancements
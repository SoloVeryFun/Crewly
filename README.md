🤖 Client & Executor Registration — Telegram Bot Module
📖 Overview

This module powers user onboarding, state-driven message handling, and database interaction for a Telegram bot.
It enables users to register as Clients or Executors, manage their profiles, and navigate through a clear, interactive menu system.

📌 Key Features
🔹 State-Based Message Handling

Each message is processed according to the user’s current state, ensuring:

Smooth, step-by-step registration and interaction flow;

Reliable data persistence in the database;

Predictable and stable logic during onboarding and menu navigation.

🔹 Optimized Database Integration

Built on Entity Framework Core with SQL Server backend;

Fully optimized CRUD operations (create, update, delete);

Includes session caching with auto-cleanup for inactive users;

Clean separation between data access and bot logic.

🔹 Dynamic Navigation Menu

The user-friendly navigation system includes:

📄 View Profile — quickly access personal data;

⚙ Settings — manage account preferences and actions:

🗑 Delete Account;

(More options coming soon!)

⚙️ Technical Stack

Telegram.Bot 22.x — seamless Telegram Bot API integration;

Entity Framework Core — robust ORM for SQL Server;

C# / .NET 8 — clean, scalable backend foundation;

🚀 Future Improvements

✏️ Extended Profile Editing — update bio, skills, and contact info;

🔄 Improved Session Management — Redis-based cache for better scalability;

🧩 Modular Handlers — easier maintenance and feature expansion.
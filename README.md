🤖 Client and Executor Registration — Telegram Bot Module
📖 Overview

This module manages user onboarding, state-based message handling, and database operations in a Telegram bot.
It allows users to register as either Client or Executor and manage their profiles through an intuitive menu system.

📌 Key Features
🔹 State-Based Message Handling

Every incoming message is processed according to the user’s current state.
This enables:

Smooth, step-by-step interaction flow;

Proper data persistence in the database;

Stable and predictable registration logic.

🔹 Optimized Database Integration

Built on Entity Framework Core (SQL Server);

Fully optimized CRUD operations (create, update, delete);

Includes session caching and automatic cleanup of inactive sessions.

🔹 Navigation Menu

📄 View your profile;

A new ⚙ Settings menu has been added, featuring:
🗑 Delete account;

⚙️  Technical Details

Telegram.Bot 22.x — for Telegram API integration;

Entity Framework Core — for SQL Server data handling;

🚀 Future Improvements

Extended profile editing;

Job posting and response management between Clients and Executors.
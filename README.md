📌 Client and Executor Registration in Telegram Bot
📖 Description

This module handles onboarding (first-time entry) and user registration in a Telegram bot.
When a user starts the bot, they choose a role:

👤 Client — searches for executors and posts job requests.

🎨 Executor — creates a freelancer profile and receives job requests.

After choosing a role, the user goes through a step-by-step registration process with required and optional fields.

⚠️ Important

Users can edit their profiles after registration.

🔑 Registration Flow
1. General Onboarding

/start → welcome message.

Role selection: [I’m a Client] / [I’m an Executor].

2. Executor Registration

The following data is collected:

📷 Avatar

👤 Name / Nickname

📝 About (up to 400–600 characters)

🏷 Specializations/Tags (up to 5)

💲 Rate (hourly or fixed)

🧑‍💻 Experience (years/level)

🔗 Portfolio links (Behance, Dribbble, Google Drive, etc.)

⏳ Availability (free / busy / partially available)

Completion: confirmation of profile publication → profile goes to moderation.

3. Client Registration

The following data is collected:

📷 Avatar or Logo

👤 Name / Company name

📝 Description (up to 400–600 characters)

🏢 Type (individual / studio / company)

💲 Budget range

🔗 Links / Brand guide (optional)

🌍 Location

🌐 Language(s)

✅ Verification request

Completion: job posting becomes available only after profile moderation.

⚙️ Technical Notes

Implemented using NuGet package Telegram.Bot 22.x.

Uses Microsoft.EntityFrameworkCore for database operations with SQL Server.

All collected data is stored in database tables (Clients, Executors, Orders)

Future updates will include menu navigation for easier bot usage.
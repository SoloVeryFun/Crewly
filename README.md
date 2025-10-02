📌 Client and Executor Registration in Telegram Bot
📖 Description

This module handles onboarding (first-time entry) and user registration.
When a user starts the bot, they choose a role:

👤 Client — searches for executors and posts job requests.

🎨 Executor — creates a freelancer profile and receives job requests.

After choosing a role, the user goes through a step-by-step registration process with required and optional fields.

⚠️ Important: At this stage, the bot is not yet connected to the database. All collected data is stored in memory (temporary session storage) and can be extended to database integration later.

🔑 Registration Flow
1. General Onboarding

/start → welcome message.

Role selection: [I’m a Client] / [I’m an Executor].

2. Executor Registration

The following data is collected:

📷 Avatar

👤 Name/Nickname

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

At this stage, no database integration is implemented.

User data is stored in session state during the registration flow.

A UserState mechanism is used for step-by-step data collection.

Future updates will include persistent storage in database tables (Clients, Executors, Orders).

Users can edit their profile after registration.

♾️ Implemented using NuGet package Telegram.Bot 22.x
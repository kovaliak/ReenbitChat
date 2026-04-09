# ReenbitChat - Real-time Chat Application

A real-time chat application built as a test task for the **Reenbit Trainee Camp: Back-End Development (.NET)**. 

The application allows users to register, create chat rooms, and exchange messages in real-time. It also includes an integration with **Azure Cognitive Services** to analyze the sentiment of each message (Positive, Negative, Neutral) and displays it directly in the UI.

## Live Demo
* **Frontend (Blazor WebAssembly):** https://reenbitchat-ui-a6e8dmg0akhvafb4.polandcentral-01.azurewebsites.net
* **Backend API:** https://reenbitchat-api-cshre2g8d8c3fueg.polandcentral-01.azurewebsites.net

## ✨ Features
* **Real-time Communication:** Powered by Azure SignalR Service for instant message delivery without page reloads.
* **Sentiment Analysis:** Automatically analyzes the emotional tone of messages using Azure AI Text Analytics and visualizes it in the chat.
* **User Authentication:** Secure registration and login using ASP.NET Core Identity with JWT-like token handling via local storage.
* **Chat Rooms:** Users can create, join, edit, and delete their own chat rooms.
* **Message Management:** Users can edit or delete their own messages in real-time.
* **Cloud Hosted:** Fully deployed on Microsoft Azure App Services.

## 🛠️ Tech Stack
* **Backend:** ASP.NET Core Web API (.NET 8), Minimal APIs.
* **Frontend:** Blazor WebAssembly.
* **Database:** Azure SQL Database, Entity Framework Core.
* **Real-time:** Azure SignalR Service.
* **AI/Cognitive Services:** Azure Text Analytics API.

## 📂 Project Structure
* `ReenbitChat.Api` - Backend project containing Minimal APIs, SignalR Hub, and authentication logic.
* `ReenbitChat.Client` - Frontend Blazor WebAssembly project containing the UI and client-side services.
* `ReenbitChat.Shared` - Class library containing shared DTOs, Enums, and Interfaces used by both projects.
* `ReenbitChat.Services` - Class library containing business logic to our database.
* `ReenbitChat.Data` - Class library containing Entity Framework DbContext and Entity models.

## 💻 How to Run Locally

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/download) installed.
* SQL Server (local or cloud) for the database.
* Azure SignalR Service Connection String.
* Azure Cognitive Services Text Analytics Endpoint and Key.

### Setup Instructions

1. **Clone the repository:**
   ```bash
   git clone [https://github.com/YOUR_GITHUB_USERNAME/ReenbitChat.git](https://github.com/YOUR_GITHUB_USERNAME/ReenbitChat.git)
   cd ReenbitChat
# 🛡️ Ruby - Cybersecurity Awareness Chatbot

## Project Overview

Ruby is a comprehensive Cybersecurity Awareness Chatbot developed in C# using Windows Presentation Foundation (WPF). This application is designed to educate South African citizens about online safety through interactive conversation, task management, a cybersecurity quiz, sentiment detection, and natural language processing simulation.

---

## 📌 Project Information

| **Detail** | **Information** |
|------------|-----------------|
| **Project Name** | Cybersecurity Awareness Chatbot - Part 3/POE |
| **Chatbot Name** | Ruby |
| **Platform** | WPF (Windows Presentation Foundation) |
| **Language** | C# |
| **Database** | SQL Server (LocalDB) |
| **Framework** | .NET Framework / .NET Core |
| **Version** | 3.0 (Final) |

---

## 🎯 Features Implemented

### 1. Voice Greeting
- Plays a recorded WAV audio file when the application launches
- Welcomes users with a friendly message
- Automatic path resolution for deployment

### 2. GUI Design
- Three-screen navigation: Home → Username → Chat
- Modern design with rounded corners and professional colour scheme
- Quick action buttons for common commands
- Chat bubbles with distinct styling for user and bot messages

### 3. Keyword Recognition
- Detects cybersecurity topics:
  - **Password** - Provides password safety tips
  - **Scam/Fraud** - Provides scam awareness tips
  - **Privacy** - Provides privacy protection tips
  - **Phishing** - Provides phishing detection tips
  - **Cybersecurity** - Provides general cybersecurity information

### 4. Random Responses
- Multiple predefined responses for each topic
- Random selection creates varied and natural conversations
- Prevents repetitive or robotic responses

### 5. Conversation Flow
- Handles follow-up questions like "tell me more"
- Tracks the last discussed topic for continuity
- Creates a natural, seamless conversation experience

### 6. Memory and Recall
- Stores user names in `user_names.txt`
- Stores user interests in `user_interests.txt`
- Personalises welcome messages for returning users
- Recalls user interests for personalised conversations

### 7. Sentiment Detection
- Detects user emotions:
  - **Worried/Concerned** - Provides reassuring response with safety tips
  - **Frustrated/Annoyed** - Offers support and step-by-step assistance
  - **Confused/Unsure** - Provides clear explanations
  - **Happy/Great** - Responds with positivity
  - **Sad/Upset** - Offers emotional support
  - **Angry/Mad** - Provides calming assistance
- Adjusts responses based on user's emotional state

### 8. Task Assistant (with Reminders)
- Add tasks with descriptions
- Set reminders with specific dates
- View all tasks with status
- View pending tasks only
- Mark tasks as completed
- Delete tasks
- All changes reflected in SQL Server database

### 9. Cybersecurity Quiz
- 12 questions covering:
  - Phishing detection
  - Password safety
  - Safe browsing
  - Social engineering
  - General cybersecurity
- Mix of Multiple Choice and True/False questions
- Immediate feedback with explanations
- Score tracking with percentage
- Feedback based on performance:
  - 10-12 correct: "Outstanding! You're a cybersecurity pro!"
  - 8-9 correct: "Great job! Excellent cybersecurity awareness!"
  - 6-7 correct: "Good effort! Keep learning!"
  - 4-5 correct: "Keep learning! Review the topics and try again!"
  - 0-3 correct: "Don't give up! Cybersecurity is a learning journey!"

### 10. NLP Simulation
- Recognises varied user phrasings
- Detects user intent using keyword patterns
- Handles different ways to say the same thing:
  - "Add task" / "Create task" / "New task"
  - "Show tasks" / "View tasks" / "List tasks"
  - "Start quiz" / "Play quiz" / "Take quiz"
  - "Remind me" / "Set reminder" / "Notify me"
  - "Show activity log" / "What have you done for me?"

### 11. Activity Log
- Records all user actions with timestamps
- Tracks:
  - User login
  - Tasks added, completed, deleted
  - Reminders set
  - Quiz started and completed
  - NLP interpretations
  - Sentiment detection
  - Follow-up responses
  - Activity log views
- Displays last 10 actions
- Shows total count of activities

---

## 🗂️ Project Structure
cybersecurity_chatbot_p2/
├── MainWindow.xaml # GUI design
├── MainWindow.xaml.cs # Main application logic
├── quiz_manager.cs # Quiz functionality
├── task_manager.cs # Database operations
├── nlp_processor.cs # NLP intent detection
├── sentiment_detector.cs # Emotion detection
├── message_displayer.cs # Chat display formatting
├── topic_detector.cs # Topic identification
├── response_finder.cs # Response lookup
├── response_handler.cs # Default responses
├── respond.cs # Response data storage
├── voice_greeting.cs # Audio playback
├── logo.jpeg # Application logo
├── greet.wav # Voice greeting audio
├── user_names.txt # User name storage (auto-generated)
├── user_interests.txt # User interests (auto-generated)
├── App.config # Application configuration
└── README.md # Project documentation


---

## 📊 Database Schema

### Tasks Table

| Column | Data Type | Description |
|--------|-----------|-------------|
| `task_id` | INT (Primary Key, Identity) | Unique task identifier |
| `username` | VARCHAR(50) | User's name |
| `task_name` | VARCHAR(100) | Task title |
| `task_description` | VARCHAR(200) | Task description |
| `task_due_date` | VARCHAR(20) | Due date for reminder |
| `task_status` | VARCHAR(20) | Pending / Completed |
| `reminder_date` | VARCHAR(20) | Reminder date |
| `date_created` | DATETIME | Date task was created |

---

## 🚀 How to Run the Application

### Prerequisites
- Windows Operating System
- .NET Framework 4.7.2 or higher
- Visual Studio 2019/2022 or any C# IDE
- SQL Server LocalDB (included with Visual Studio)

🎬 Video Presentation
An unlisted YouTube video has been created demonstrating:

Full explanation of code structure and logic

Demonstration of the GUI application running

Walkthrough of all features:

Task Assistant with database

Cybersecurity Quiz with score tracking

Sentiment Detection with empathetic responses

NLP Simulation with varied phrasings

Activity Log with timestamps

https://youtu.be/H4PdzL7VuHw

👨‍💻 Author
Name: [Lufuno Ramasuvha]

Project: Cybersecurity Awareness Chatbot - Part 3/POE

Course: [Programming6221]

Date: June 2026

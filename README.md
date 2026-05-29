# Cybersecurity Awareness Chatbot - Part 2

## Project Overview

This is a **Graphical User Interface (GUI)** cybersecurity awareness chatbot developed for South African citizens as part of a national cybersecurity education campaign. The chatbot helps users learn about online safety practices including password safety, phishing awareness, scam detection, privacy protection, and safe browsing habits.

**Chatbot Name:** Ruby

**Technology:** WPF (Windows Presentation Foundation) with C#

---

## Part 2 Enhancements

Part 2 builds upon Part 1 by introducing:

| Feature | Description |
|---------|-------------|
| **GUI Interface** | Professional WPF interface with 3-screen navigation |
| **Keyword Recognition** | Detects cybersecurity topics (password, scam, privacy, phishing) |
| **Random Responses** | Multiple responses for each topic using arrays/lists |
| **Conversation Flow** | Handles follow-up questions like "tell me more" |
| **Memory & Recall** | Stores user name and interests in text files |
| **Sentiment Detection** | Detects emotions (worried, frustrated, confused, happy, sad, angry) |
| **Voice Greeting** | Plays welcome audio on application launch |
| **Error Handling** | Graceful handling of empty inputs and unrecognised questions |

---

## Features Implemented

### 1. GUI Design (Requirement 1)

- **Three-screen navigation:**
  - Home Grid: Welcome screen with logo and continue button
  - Username Grid: Name collection with validation
  - Chat Grid: Main conversation interface

- **Color Scheme:** White and light green theme
  - Background: White (#FFFFFF)
  - Accent Color: Light Green (#4CAF50)
  - Header Background: Light Green (#E8F5E9)
  - User Messages: Light green bubbles aligned right
  - Bot Messages: White bubbles with green border aligned left

### 2. Voice Greeting (Requirement 1a)

- Plays `greet.wav` audio file when application starts
- Uses `System.Media.SoundPlayer` class
- Automatic path resolution for deployment

### 3. Keyword Recognition (Requirement 2)

The chatbot recognises the following cybersecurity keywords:

| Keyword | Example Response |
|---------|------------------|
| "password" | "Make sure to use strong, unique passwords for each account..." |
| "scam" | "Scammers often create urgency to trick you. Take your time..." |
| "privacy" | "Protect your privacy by reviewing app permissions regularly..." |
| "phishing" | "Be cautious of emails asking for personal information..." |
| "cybersecurity" | "Cybersecurity is about protecting systems from digital threats..." |

### 4. Random Responses (Requirement 3)

- Each topic has multiple predefined responses stored in a `Dictionary<string, List<string>>`
- Responses are randomly selected using `Random` class
- Example: Phishing tips have 5 different responses

### 5. Conversation Flow (Requirement 4)

- Tracks the last discussed topic using `lastTopic` variable
- Handles follow-up phrases:
  - "tell me more"
  - "another tip"
  - "explain more"
  - "more information"
- Provides additional responses without requiring the user to re-ask

### 6. Memory and Recall (Requirement 5)

**User Name Storage:**
- Stores names in `user_names.txt`
- Welcomes new users with "Hey [name]! Welcome to Ruby!"
- Welcomes returning users with "Hey [name]! Welcome back to Ruby!"

**User Interests Storage:**
- Stores interests when user says "I'm interested in [topic]"
- Saves to `user_interests.txt` in format: `username|interest1,interest2`
- Recalls interests for personalised conversations

### 7. Sentiment Detection (Requirement 6)

Detects the following sentiments and responds empathetically:

| Sentiment | Detection Keywords | Response Example |
|-----------|-------------------|------------------|
| Worried | worried, concerned, nervous | "It's completely understandable to feel that way..." + tip |
| Frustrated | frustrated, annoyed | "I understand you're frustrated. Let's work through this..." |
| Confused | confused, unsure, don't understand | "That's okay, confusion is normal..." + explanation |
| Happy | happy, great, awesome | "That's great to hear! I'm glad you're having a good day..." |
| Sad | sad, upset, depressed | "I'm sorry you're feeling this way. I'm here for you..." |
| Angry | angry, mad, furious | "I understand you're angry. Let's solve this together..." |

### 8. Error Handling (Requirement 7)

- Empty input validation with friendly error messages
- Default responses for unrecognised questions
- File existence checks before reading/writing
- Null checks for all user inputs

### 9. Code Optimisation (Requirement 8)

**Data Structures Used:**
- `Dictionary<string, List<string>>` for topic-response storage
- `ArrayList` for reply storage and ignore words
- `Random` class for response selection

**Object-Oriented Design:**
- `response_finder` - Finds responses by topic
- `response_handler` - Handles default responses and input cleaning
- `topic_detector` - Detects topics from user input
- `message_displayer` - Manages chat display formatting
- `sentiment_detector` - Detects and responds to user emotions
- `voice_greeting` - Plays audio greeting
- `respond` - Stores all response data

---

## Project Structure

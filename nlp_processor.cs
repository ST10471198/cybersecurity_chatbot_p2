using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace cybersecurity_chatbot_p2
{//start of namespace
    public class nlp_processor
    {//start of class

        private Dictionary<string, List<string>> intentPatterns;
        private Dictionary<string, string> intentActions;

        public nlp_processor()
        {//start of constructor
            InitializeIntentPatterns();
        }//end of constructor

        private void InitializeIntentPatterns()
        {//start of method

            intentPatterns = new Dictionary<string, List<string>>();
            intentActions = new Dictionary<string, string>();

            // TASK INTENTS
            intentPatterns["add_task"] = new List<string>
            {
                "add task", "create task", "new task", "add a task",
                "i want to add", "i need to add", "please add",
                "can you add", "could you add", "would you add",
                "set up", "setup", "configure", "enable",
                "i need to set up", "i want to set up"
            };
            intentActions["add_task"] = "add_task";

            intentPatterns["show_tasks"] = new List<string>
            {
                "show tasks", "view tasks", "list tasks", "see tasks",
                "show me tasks", "view my tasks", "list my tasks",
                "what are my tasks", "display tasks", "get tasks",
                "show me my tasks", "let me see my tasks"
            };
            intentActions["show_tasks"] = "show_tasks";

            intentPatterns["complete_task"] = new List<string>
            {
                "mark complete", "mark done", "finish task", "complete task",
                "task done", "task complete", "mark as complete",
                "mark as done", "finish", "completed", "done"
            };
            intentActions["complete_task"] = "complete_task";

            intentPatterns["delete_task"] = new List<string>
            {
                "delete task", "remove task", "delete", "remove",
                "delete this task", "remove this task", "get rid of",
                "cancel task", "erase task", "forget task"
            };
            intentActions["delete_task"] = "delete_task";

            // REMINDER INTENTS
            intentPatterns["set_reminder"] = new List<string>
            {
                "remind me", "set reminder", "create reminder",
                "i need a reminder", "can you remind me",
                "please remind me", "reminder for", "set a reminder",
                "i want to be reminded", "notify me", "alert me"
            };
            intentActions["set_reminder"] = "set_reminder";

            // QUIZ INTENTS
            intentPatterns["start_quiz"] = new List<string>
            {
                "start quiz", "play quiz", "take quiz", "do quiz",
                "i want to take quiz", "let's do quiz", "quiz me",
                "test me", "challenge me", "i want to play",
                "let's play quiz", "begin quiz", "start the quiz"
            };
            intentActions["start_quiz"] = "start_quiz";

            // ACTIVITY LOG INTENTS
            intentPatterns["show_log"] = new List<string>
            {
                "show log", "view log", "activity log", "show activity",
                "what have you done", "show me what you did",
                "display log", "recent actions", "action log",
                "show history", "view history", "what did you do",
                "tell me what you've done", "show me the log"
            };
            intentActions["show_log"] = "show_log";

            // HELP INTENT
            intentPatterns["help"] = new List<string>
            {
                "help", "help me", "i need help", "can you help",
                "what can you do", "how do you work", "tell me what you can do",
                "options", "commands", "what can i ask", "guide me",
                "show help", "get help", "i don't know what to ask"
            };
            intentActions["help"] = "help";

            // GREETING INTENTS
            intentPatterns["greeting"] = new List<string>
            {
                "hello", "hi", "hey", "greetings", "good morning",
                "good afternoon", "good evening", "howdy",
                "hey there", "hi there", "hello there", "yo"
            };
            intentActions["greeting"] = "greeting";

            // SENTIMENT INTENTS
            intentPatterns["worried"] = new List<string>
            {
                "worried", "concerned", "nervous", "anxious",
                "i'm scared", "i'm afraid", "i'm worried about",
                "i'm concerned about", "i'm nervous about",
                "i feel unsafe", "i'm not sure", "i'm uncertain"
            };
            intentActions["worried"] = "worried";

            intentPatterns["frustrated"] = new List<string>
            {
                "frustrated", "annoyed", "irritated", "upset",
                "i'm tired of", "i can't figure out", "i don't get it",
                "this is confusing", "i'm confused", "i'm lost"
            };
            intentActions["frustrated"] = "frustrated";

            intentPatterns["happy"] = new List<string>
            {
                "happy", "great", "awesome", "excellent", "wonderful",
                "i'm glad", "i'm happy", "that's great", "i feel good",
                "fantastic", "amazing", "terrific", "outstanding"
            };
            intentActions["happy"] = "happy";

            intentPatterns["sad"] = new List<string>
            {
                "sad", "upset", "depressed", "down", "feeling bad",
                "i'm sad", "i'm down", "i feel terrible", "i'm not good",
                "i'm feeling down", "i'm blue", "i'm depressed"
            };
            intentActions["sad"] = "sad";

        }//end of method

        public string DetectIntent(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower().Trim();

            foreach (KeyValuePair<string, List<string>> intent in intentPatterns)
            {//start of foreach
                string intentKey = intent.Key;
                List<string> patterns = intent.Value;

                foreach (string pattern in patterns)
                {//start of inner foreach
                    if (lowerInput.Contains(pattern))
                    {//start of if
                        return intentKey;
                    }//end of if
                }//end of inner foreach
            }//end of foreach

            if (IsTopicQuestion(lowerInput))
            {//start of if
                return "topic_question";
            }//end of if

            return "unknown";
        }//end of method

        public string ExtractTaskDetails(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower();

            string[] patterns = {
                "add task", "create task", "new task", "add a task",
                "i want to add", "i need to add", "please add",
                "set up", "setup", "configure", "enable",
                "i need to set up", "i want to set up",
                "remind me to", "reminder for", "can you remind me to"
            };

            foreach (string pattern in patterns)
            {//start of foreach
                int index = lowerInput.IndexOf(pattern);
                if (index >= 0)
                {//start of if
                    string taskText = userInput.Substring(index + pattern.Length).Trim();
                    taskText = taskText.TrimStart('-', ':', ' ', 't', 'o');
                    taskText = taskText.Trim();

                    string[] removeWords = { "tomorrow", "today", "next week", "in", "days", "day" };
                    foreach (string word in removeWords)
                    {//start of inner foreach
                        if (taskText.ToLower().Contains(" " + word))
                        {//start of if
                            int lastIndex = taskText.ToLower().LastIndexOf(" " + word);
                            if (lastIndex > 0)
                            {//start of if
                                taskText = taskText.Substring(0, lastIndex);
                            }//end of if
                        }//end of if
                    }//end of inner foreach

                    if (!string.IsNullOrEmpty(taskText))
                    {//start of if
                        return taskText;
                    }//end of if
                }//end of if
            }//end of foreach

            if (lowerInput.Contains(" to "))
            {//start of if
                int toIndex = lowerInput.IndexOf(" to ");
                if (toIndex >= 0 && toIndex + 4 < lowerInput.Length)
                {//start of if
                    string taskText = userInput.Substring(toIndex + 4).Trim();
                    taskText = taskText.TrimStart('-', ':', ' ');
                    if (!string.IsNullOrEmpty(taskText))
                    {//start of if
                        return taskText;
                    }//end of if
                }//end of if
            }//end of if

            return "";
        }//end of method

        public string ExtractReminderTime(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower();

            if (lowerInput.Contains("tomorrow"))
            {//start of if
                return "tomorrow";
            }//end of if

            if (lowerInput.Contains("today"))
            {//start of if
                return "today";
            }//end of if

            if (lowerInput.Contains("next week"))
            {//start of if
                return "next week";
            }//end of if

            Match match = Regex.Match(lowerInput, @"(\d+)\s*days?");
            if (match.Success)
            {//start of if
                return match.Value;
            }//end of if

            match = Regex.Match(lowerInput, @"(\d+)\s*day");
            if (match.Success)
            {//start of if
                return match.Value;
            }//end of if

            return "";
        }//end of method

        public int ExtractNumberOfDays(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower();

            if (lowerInput.Contains("tomorrow"))
            {//start of if
                return 1;
            }//end of if

            if (lowerInput.Contains("today"))
            {//start of if
                return 0;
            }//end of if

            if (lowerInput.Contains("next week"))
            {//start of if
                return 7;
            }//end of if

            Match match = Regex.Match(lowerInput, @"(\d+)\s*days?");
            if (match.Success)
            {//start of if
                return int.Parse(match.Groups[1].Value);
            }//end of if

            match = Regex.Match(lowerInput, @"(\d+)\s*day");
            if (match.Success)
            {//start of if
                return int.Parse(match.Groups[1].Value);
            }//end of if

            return -1;
        }//end of method

        public string ExtractTaskNameForAction(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower();

            string[] actionWords = { "mark", "complete", "finish", "done", "delete", "remove", "cancel", "erase", "forget" };
            string[] taskWords = { "task", "this" };

            string result = userInput;

            foreach (string action in actionWords)
            {//start of foreach
                if (lowerInput.Contains(action))
                {//start of if
                    result = result.Replace(action, "").Replace(action.ToUpper(), "").Replace(action.ToLower(), "");
                }//end of if
            }//end of foreach

            foreach (string task in taskWords)
            {//start of foreach
                result = result.Replace(task, "").Replace(task.ToUpper(), "").Replace(task.ToLower(), "");
            }//end of foreach

            result = result.Replace("as", "").Replace("complete", "").Replace("done", "");
            result = Regex.Replace(result, @"\s+", " ").Trim();
            result = result.TrimStart('-', ':', ' ', '.').TrimEnd('.', '!', '?');

            return result;
        }//end of method

        public bool IsTopicQuestion(string input)
        {//start of method
            string lowerInput = input.ToLower();
            string[] topicKeywords = { "password", "phishing", "scam", "privacy", "cybersecurity", "security", "vpn", "firewall", "hack", "malware", "virus" };

            foreach (string keyword in topicKeywords)
            {//start of foreach
                if (lowerInput.Contains(keyword))
                {//start of if
                    return true;
                }//end of if
            }//end of foreach

            return false;
        }//end of method

    }//end of class
}//end of namespace
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace cybersecurity_chatbot_p2
{//start of namespace

    public partial class MainWindow : Window
    {//start of class

        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();

        private response_finder finder;
        private response_handler handler;
        private topic_detector detector;
        private message_displayer displayer;
        private sentiment_detector sentimentDetector;
        private task_manager taskManager;
        private quiz_manager quizManager;
        private nlp_processor nlpProcessor;

        private string username = string.Empty;
        private string lastTopic = string.Empty;

        private bool awaitingReminderResponse = false;
        private string pendingTaskName = "";
        private string pendingTaskDescription = "";

        // ============================================================
        // ACTIVITY LOG - TASK 4
        // ============================================================
        private List<string> activityLog = new List<string>();

        public MainWindow()
        {//start of constructor
            InitializeComponent();

            new voice_greeting();
            new respond(reply, ignore);

            finder = new response_finder(reply);
            handler = new response_handler(reply, ignore);
            detector = new topic_detector();
            displayer = new message_displayer(chats);
            sentimentDetector = new sentiment_detector(reply, finder, displayer);
            quizManager = new quiz_manager();
            nlpProcessor = new nlp_processor();

            AddToActivityLog("Application started");
        }//end of constructor

        private void proceed(object sender, RoutedEventArgs e)
        {//start of method
            home_grid.Visibility = Visibility.Hidden;
            username_grid.Visibility = Visibility.Visible;
            usernames_input.Focus();
        }//end of method

        private void submit_name(object sender, RoutedEventArgs e)
        {//start of method

            username = usernames_input.Text.Trim();

            if (string.IsNullOrWhiteSpace(username))
            {//start of if
                errorMessage.Visibility = Visibility.Visible;
                usernames_input.Focus();
                return;
            }//end of if

            errorMessage.Visibility = Visibility.Collapsed;

            string filename = "user_names.txt";

            if (!File.Exists(filename))
            {//start of if
                File.AppendAllText(filename, username + "\n");
                displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome to Ruby, your Cybersecurity Assistant!");
            }//end of if
            else
            {//start of else
                string[] names = File.ReadAllLines(filename);
                bool exists = names.Any(n => n.ToLower() == username.ToLower());

                if (exists)
                {//start of if
                    displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome back to Ruby!");
                }//end of if
                else
                {//start of else
                    File.AppendAllText(filename, username + "\n");
                    displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome to Ruby, your Cybersecurity Assistant!");
                }//end of else
            }//end of else

            taskManager = new task_manager(username);

            AddToActivityLog("User logged in: " + username);

            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;
        }//end of method

        private void send(object sender, RoutedEventArgs e)
        {//start of send method

            string userInput = question.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {//start of if
                displayer.ShowMessage("Ruby", "Please enter a question.");
                question.Clear();
                return;
            }//end of if

            displayer.ShowMessage(username, userInput);
            AddToActivityLog("User: " + userInput);

            // PRIORITY 1: Reminder response
            if (awaitingReminderResponse)
            {//start of if
                string lowerInput = userInput.ToLower().Trim();

                if (lowerInput.Contains("yes") || lowerInput.Contains("yeah") || lowerInput.Contains("yep") ||
                    lowerInput.Contains("sure") || lowerInput.Contains("ok") || lowerInput.Contains("okay"))
                {//start of if
                    displayer.ShowMessage("Ruby", "Great! How many days from now would you like the reminder? (Enter a number)");
                    awaitingReminderResponse = false;
                    question.Clear();
                    return;
                }//end of if
                else if (lowerInput.Contains("no") || lowerInput.Contains("nope") || lowerInput.Contains("nah"))
                {//start of else if
                    string result = taskManager.add_task(pendingTaskName, pendingTaskDescription, "");
                    displayer.ShowMessage("Ruby", result);
                    AddToActivityLog("Task added: " + pendingTaskName + " (No reminder)");
                    awaitingReminderResponse = false;
                    pendingTaskName = "";
                    pendingTaskDescription = "";
                    question.Clear();
                    return;
                }//end of else if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please answer Yes or No. Would you like a reminder for this task?");
                    question.Clear();
                    return;
                }//end of else
            }//end of if

            // PRIORITY 2: Days input
            if (!string.IsNullOrEmpty(pendingTaskName) && IsNumeric(userInput))
            {//start of if
                int days = int.Parse(userInput);
                if (days > 0)
                {//start of if
                    string reminderDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(pendingTaskName, pendingTaskDescription, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
                    AddToActivityLog("Task added: " + pendingTaskName + " (Reminder in " + days + " days)");
                    pendingTaskName = "";
                    pendingTaskDescription = "";
                    question.Clear();
                    return;
                }//end of if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please enter a valid number of days (e.g., 5).");
                    question.Clear();
                    return;
                }//end of else
            }//end of if

            // PRIORITY 3: Task Assistant Commands
            if (taskManager != null && handle_task_assistant_command(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // PRIORITY 4: NLP Processing
            if (process_with_nlp(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // PRIORITY 5: Quiz Commands
            if (handle_quiz_command(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // PRIORITY 6: Sentiment Detection
            if (sentimentDetector.DetectSentiment(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // PRIORITY 7: Follow-up Questions
            string fullInput = userInput.ToLower();
            if (fullInput.Contains("more") || fullInput.Contains("another tip") ||
                fullInput.Contains("tell me more") || fullInput.Contains("explain more"))
            {//start of if
                if (!string.IsNullOrEmpty(lastTopic))
                {//start of if
                    string response = finder.FindResponseByTopic(lastTopic);
                    displayer.ShowMessage("Ruby", response);
                    AddToActivityLog("Follow-up response given for: " + lastTopic);
                    question.Clear();
                    return;
                }//end of if
            }//end of if

            // PRIORITY 8: Interest Statements
            if (fullInput.Contains("interested in"))
            {//start of if
                string interest = extract_interest(userInput);
                if (!string.IsNullOrEmpty(interest))
                {//start of if
                    store_interest(username, interest);
                    displayer.ShowMessage("Ruby", "Great! I'll remember that you're interested in " + interest +
                                          ". It's a crucial part of staying safe online.");
                    AddToActivityLog("User interest stored: " + interest);
                    question.Clear();
                    return;
                }//end of if
            }//end of if

            // PRIORITY 9: Activity Log Command
            if (fullInput.Contains("activity log") || fullInput.Contains("show log") ||
                fullInput.Contains("what have you done") || fullInput.Contains("show activity") ||
                fullInput.Contains("show me the log") || fullInput.Contains("recent actions"))
            {//start of if
                string log = GetActivityLog();
                displayer.ShowMessage("Ruby", log);
                AddToActivityLog("Activity log viewed");
                question.Clear();
                return;
            }//end of if

            // PRIORITY 10: Topic Detection
            string[] words = userInput.ToLower().Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '-' },
                                                       StringSplitOptions.RemoveEmptyEntries);

            string topic = detector.DetectTopic(words);
            if (!string.IsNullOrEmpty(topic))
            {//start of if
                lastTopic = topic;
                string response = finder.FindResponseByTopic(topic);
                displayer.ShowMessage("Ruby", response);
                AddToActivityLog("Response given for topic: " + topic);
            }//end of if
            else
            {//start of else
                string response = handler.GetDefaultResponse();
                displayer.ShowMessage("Ruby", response);
            }//end of else

            question.Clear();
        }//end of send method

        // ============================================================
        // TASK ASSISTANT METHODS
        // ============================================================

        private bool handle_task_assistant_command(string userInput)
        {//start of method

            string lowerInput = userInput.ToLower().Trim();

            if ((lowerInput.Contains("show") || lowerInput.Contains("view") || lowerInput.Contains("list")) &&
                lowerInput.Contains("task"))
            {//start of if
                string tasks = taskManager.view_tasks();
                displayer.ShowMessage("Ruby", tasks);
                AddToActivityLog("Viewed tasks");
                return true;
            }//end of if

            if (lowerInput.Contains("pending") && lowerInput.Contains("task"))
            {//start of if
                string pending = taskManager.get_pending_task_names();
                displayer.ShowMessage("Ruby", pending);
                return true;
            }//end of if

            if (lowerInput.Contains("add task") || lowerInput.Contains("create task") ||
                lowerInput.Contains("new task") || lowerInput.Contains("add a task"))
            {//start of if
                string taskInfo = extract_task_info(userInput);
                if (!string.IsNullOrEmpty(taskInfo))
                {//start of if
                    pendingTaskName = taskInfo;
                    pendingTaskDescription = "Task: " + taskInfo;

                    displayer.ShowMessage("Ruby", "Task added with description \"" + taskInfo +
                                          "\" Would you like a reminder? (Yes/No)");
                    awaitingReminderResponse = true;
                    AddToActivityLog("Task pending: " + taskInfo + " (Awaiting reminder)");
                    return true;
                }//end of if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please specify what task you want to add. For example: 'Add task - Review privacy settings'");
                    return true;
                }//end of else
            }//end of if

            if ((lowerInput.Contains("mark") || lowerInput.Contains("complete")) &&
                (lowerInput.Contains("complete") || lowerInput.Contains("done") || lowerInput.Contains("task")))
            {//start of if
                string taskName = extract_task_name_for_action(userInput);
                if (!string.IsNullOrEmpty(taskName))
                {//start of if
                    int taskId = taskManager.get_task_id_from_name(taskName);
                    if (taskId > 0)
                    {//start of if
                        string result = taskManager.complete_task(taskId);
                        displayer.ShowMessage("Ruby", result);
                        AddToActivityLog("Task completed: " + taskName);
                        return true;
                    }//end of if
                    else
                    {//start of else
                        displayer.ShowMessage("Ruby", "Task not found. Please check the task name.");
                        return true;
                    }//end of else
                }//end of if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please specify which task to complete. For example: 'Mark Review privacy settings as complete'");
                    return true;
                }//end of else
            }//end of if

            if ((lowerInput.Contains("delete") || lowerInput.Contains("remove")) && lowerInput.Contains("task"))
            {//start of if
                string taskName = extract_task_name_for_action(userInput);
                if (!string.IsNullOrEmpty(taskName))
                {//start of if
                    int taskId = taskManager.get_task_id_from_name(taskName);
                    if (taskId > 0)
                    {//start of if
                        string result = taskManager.delete_task(taskId);
                        displayer.ShowMessage("Ruby", result);
                        AddToActivityLog("Task deleted: " + taskName);
                        return true;
                    }//end of if
                    else
                    {//start of else
                        displayer.ShowMessage("Ruby", "Task not found. Please check the task name.");
                        return true;
                    }//end of else
                }//end of if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please specify which task to delete. For example: 'Delete task Enable 2FA'");
                    return true;
                }//end of else
            }//end of if

            return false;
        }//end of method

        private string extract_task_info(string input)
        {//start of method

            string[] prefixes = { "add task", "create task", "new task", "add a task" };

            string lowerInput = input.ToLower();
            foreach (string prefix in prefixes)
            {//start of foreach
                int index = lowerInput.IndexOf(prefix);
                if (index >= 0)
                {//start of if
                    string taskInfo = input.Substring(index + prefix.Length).Trim();
                    taskInfo = taskInfo.TrimStart('-', ':', ' ', 't', 'o');
                    taskInfo = taskInfo.Trim();
                    if (!string.IsNullOrEmpty(taskInfo))
                    {//start of if
                        return taskInfo;
                    }//end of if
                }//end of if
            }//end of foreach

            if (lowerInput.Contains(" to ") || lowerInput.Contains(" about "))
            {//start of if
                string[] parts = input.Split(new string[] { " to ", " about " }, StringSplitOptions.None);
                if (parts.Length > 1)
                {//start of if
                    return parts[1].Trim();
                }//end of if
            }//end of if

            return "";
        }//end of method

        private int extract_days(string input)
        {//start of method
            Match match = Regex.Match(input, @"\d+");
            if (match.Success)
            {//start of if
                return int.Parse(match.Value);
            }//end of if
            return 0;
        }//end of method

        private string extract_task_name_for_action(string input)
        {//start of method

            string[] actionWords = { "mark", "complete", "delete", "remove" };
            string[] words = input.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {//start of for
                if (actionWords.Any(w => w == words[i].ToLower()))
                {//start of if
                    string remaining = string.Join(" ", words.Skip(i + 1));
                    remaining = remaining.Replace("task", "").Replace("as", "").Replace("complete", "").Replace("done", "");
                    remaining = remaining.Trim();

                    if (!string.IsNullOrEmpty(remaining))
                    {//start of if
                        return remaining;
                    }//end of if
                }//end of if
            }//end of for

            return "";
        }//end of method

        private bool IsNumeric(string input)
        {//start of method
            if (string.IsNullOrWhiteSpace(input))
            {//start of if
                return false;
            }//end of if

            foreach (char c in input)
            {//start of foreach
                if (!char.IsDigit(c))
                {//start of if
                    return false;
                }//end of if
            }//end of foreach

            return true;
        }//end of method

        // ============================================================
        // NLP PROCESSING METHODS - TASK 3
        // ============================================================

        private bool process_with_nlp(string userInput)
        {//start of method

            string intent = nlpProcessor.DetectIntent(userInput);

            if (intent == "unknown")
            {//start of if
                return false;
            }//end of if

            switch (intent)
            {//start of switch
                case "add_task":
                    return handle_nlp_add_task(userInput);

                case "show_tasks":
                    return handle_nlp_show_tasks(userInput);

                case "complete_task":
                    return handle_nlp_complete_task(userInput);

                case "delete_task":
                    return handle_nlp_delete_task(userInput);

                case "set_reminder":
                    return handle_nlp_set_reminder(userInput);

                case "start_quiz":
                    return handle_nlp_start_quiz(userInput);

                case "show_log":
                    return handle_nlp_show_log(userInput);

                case "help":
                    return handle_nlp_help(userInput);

                case "greeting":
                    return handle_nlp_greeting(userInput);

                case "worried":
                    return handle_nlp_worried(userInput);

                case "frustrated":
                    return handle_nlp_frustrated(userInput);

                case "happy":
                    return handle_nlp_happy(userInput);

                case "sad":
                    return handle_nlp_sad(userInput);

                case "topic_question":
                    return false;

                default:
                    return false;
            }//end of switch
        }//end of method

        private bool handle_nlp_add_task(string userInput)
        {//start of method
            string taskInfo = nlpProcessor.ExtractTaskDetails(userInput);

            if (string.IsNullOrEmpty(taskInfo))
            {//start of if
                displayer.ShowMessage("Ruby", "I'd be happy to add that task. What task would you like me to add?");
                AddToActivityLog("NLP: Asked for task description");
                return true;
            }//end of if

            string reminderTime = nlpProcessor.ExtractReminderTime(userInput);
            int days = nlpProcessor.ExtractNumberOfDays(userInput);

            if (!string.IsNullOrEmpty(reminderTime) && days >= 0)
            {//start of if
                if (days == 0)
                {//start of if
                    string reminderDate = DateTime.Now.ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(taskInfo, "Task: " + taskInfo, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
                    AddToActivityLog("NLP: Task added with reminder - " + taskInfo + " (Today)");
                }//end of if
                else if (days == 1 && reminderTime.Contains("tomorrow"))
                {//start of else if
                    string reminderDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(taskInfo, "Task: " + taskInfo, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
                    AddToActivityLog("NLP: Task added with reminder - " + taskInfo + " (Tomorrow)");
                }//end of else if
                else if (days == 7 && reminderTime.Contains("next week"))
                {//start of else if
                    string reminderDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(taskInfo, "Task: " + taskInfo, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
                    AddToActivityLog("NLP: Task added with reminder - " + taskInfo + " (Next week)");
                }//end of else if
                else if (days > 0)
                {//start of else if
                    string reminderDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(taskInfo, "Task: " + taskInfo, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
                    AddToActivityLog("NLP: Task added with reminder - " + taskInfo + " (in " + days + " days)");
                }//end of else if
                else
                {//start of else
                    string result = taskManager.add_task(taskInfo, "Task: " + taskInfo, "");
                    displayer.ShowMessage("Ruby", "Task added: '" + taskInfo + "' Would you like to set a reminder for this task?");
                    AddToActivityLog("NLP: Task added - " + taskInfo + " (No reminder)");
                    awaitingReminderResponse = true;
                    pendingTaskName = taskInfo;
                    pendingTaskDescription = "Task: " + taskInfo;
                }//end of else
            }//end of if
            else
            {//start of else
                pendingTaskName = taskInfo;
                pendingTaskDescription = "Task: " + taskInfo;
                displayer.ShowMessage("Ruby", "Task added: '" + taskInfo + "' Would you like to set a reminder for this task?");
                AddToActivityLog("NLP: Task pending - " + taskInfo + " (Awaiting reminder)");
                awaitingReminderResponse = true;
            }//end of else

            return true;
        }//end of method

        private bool handle_nlp_show_tasks(string userInput)
        {//start of method
            string tasks = taskManager.view_tasks();
            displayer.ShowMessage("Ruby", tasks);
            AddToActivityLog("NLP: Viewed tasks");
            return true;
        }//end of method

        private bool handle_nlp_complete_task(string userInput)
        {//start of method
            string taskName = nlpProcessor.ExtractTaskNameForAction(userInput);

            if (string.IsNullOrEmpty(taskName))
            {//start of if
                displayer.ShowMessage("Ruby", "Which task would you like to mark as complete? Please specify the task name.");
                AddToActivityLog("NLP: Asked for task to complete");
                return true;
            }//end of if

            int taskId = taskManager.get_task_id_from_name(taskName);
            if (taskId > 0)
            {//start of if
                string result = taskManager.complete_task(taskId);
                displayer.ShowMessage("Ruby", result);
                AddToActivityLog("NLP: Task completed - " + taskName);
            }//end of if
            else
            {//start of else
                displayer.ShowMessage("Ruby", "Task not found. Please check the task name.");
                AddToActivityLog("NLP: Task not found - " + taskName);
            }//end of else

            return true;
        }//end of method

        private bool handle_nlp_delete_task(string userInput)
        {//start of method
            string taskName = nlpProcessor.ExtractTaskNameForAction(userInput);

            if (string.IsNullOrEmpty(taskName))
            {//start of if
                displayer.ShowMessage("Ruby", "Which task would you like to delete? Please specify the task name.");
                AddToActivityLog("NLP: Asked for task to delete");
                return true;
            }//end of if

            int taskId = taskManager.get_task_id_from_name(taskName);
            if (taskId > 0)
            {//start of if
                string result = taskManager.delete_task(taskId);
                displayer.ShowMessage("Ruby", result);
                AddToActivityLog("NLP: Task deleted - " + taskName);
            }//end of if
            else
            {//start of else
                displayer.ShowMessage("Ruby", "Task not found. Please check the task name.");
                AddToActivityLog("NLP: Task not found - " + taskName);
            }//end of else

            return true;
        }//end of method

        private bool handle_nlp_set_reminder(string userInput)
        {//start of method
            string taskInfo = nlpProcessor.ExtractTaskDetails(userInput);

            if (string.IsNullOrEmpty(taskInfo))
            {//start of if
                displayer.ShowMessage("Ruby", "What would you like me to remind you about?");
                AddToActivityLog("NLP: Asked for reminder description");
                return true;
            }//end of if

            string reminderTime = nlpProcessor.ExtractReminderTime(userInput);
            int days = nlpProcessor.ExtractNumberOfDays(userInput);

            if (days < 0)
            {//start of if
                pendingTaskName = taskInfo;
                pendingTaskDescription = "Reminder: " + taskInfo;
                displayer.ShowMessage("Ruby", "Reminder set for '" + taskInfo + "'. How many days from now would you like the reminder?");
                AddToActivityLog("NLP: Asked for reminder days");
                awaitingReminderResponse = true;
                return true;
            }//end of if

            string reminderDate = "";
            if (days == 1 && reminderTime.Contains("tomorrow"))
            {//start of if
                reminderDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                displayer.ShowMessage("Ruby", "Reminder set for '" + taskInfo + "' on tomorrow's date.");
                AddToActivityLog("NLP: Reminder set - " + taskInfo + " (Tomorrow)");
            }//end of if
            else if (days == 7 && reminderTime.Contains("next week"))
            {//start of else if
                reminderDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd");
                displayer.ShowMessage("Ruby", "Reminder set for '" + taskInfo + "' on next week's date.");
                AddToActivityLog("NLP: Reminder set - " + taskInfo + " (Next week)");
            }//end of else if
            else if (days == 0 && reminderTime.Contains("today"))
            {//start of else if
                reminderDate = DateTime.Now.ToString("yyyy-MM-dd");
                displayer.ShowMessage("Ruby", "Reminder set for '" + taskInfo + "' on today's date.");
                AddToActivityLog("NLP: Reminder set - " + taskInfo + " (Today)");
            }//end of else if
            else if (days > 0)
            {//start of else if
                reminderDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                displayer.ShowMessage("Ruby", "Reminder set for '" + taskInfo + "' on " + reminderDate + ".");
                AddToActivityLog("NLP: Reminder set - " + taskInfo + " (in " + days + " days)");
            }//end of else if

            taskManager.add_task(taskInfo, "Reminder: " + taskInfo, reminderDate);
            return true;
        }//end of method

        private bool handle_nlp_start_quiz(string userInput)
        {//start of method
            string result = quizManager.StartQuiz();
            displayer.ShowMessage("Ruby", result);
            AddToActivityLog("NLP: Quiz started");
            return true;
        }//end of method

        private bool handle_nlp_show_log(string userInput)
        {//start of method
            string log = GetActivityLog();
            displayer.ShowMessage("Ruby", log);
            AddToActivityLog("NLP: Activity log viewed");
            return true;
        }//end of method

        private bool handle_nlp_help(string userInput)
        {//start of method
            quick_action_help(null, null);
            return true;
        }//end of method

        private bool handle_nlp_greeting(string userInput)
        {//start of method
            string[] greetings = {
                "Hello " + username + "! How can I help you with cybersecurity today?",
                "Hi " + username + "! I'm here to help you stay safe online.",
                "Hey " + username + "! What cybersecurity topic would you like to learn about?",
                "Greetings " + username + "! Ready to learn about online safety?"
            };
            Random random = new Random();
            displayer.ShowMessage("Ruby", greetings[random.Next(greetings.Length)]);
            AddToActivityLog("NLP: Greeting response");
            return true;
        }//end of method

        private bool handle_nlp_worried(string userInput)
        {//start of method
            string response = "It's completely understandable to feel that way. Cybersecurity can be overwhelming.\n\n" +
                             "Here's a tip to help you stay safe:\n" +
                             finder.FindResponseByTopic("scam");
            displayer.ShowMessage("Ruby", response);
            AddToActivityLog("NLP: Worried sentiment detected");
            return true;
        }//end of method

        private bool handle_nlp_frustrated(string userInput)
        {//start of method
            string response = "I understand you're frustrated. Let's work through the issue step by step.\n\n" +
                             "Tell me what specific cybersecurity concern you have, and I'll help you understand it.";
            displayer.ShowMessage("Ruby", response);
            AddToActivityLog("NLP: Frustrated sentiment detected");
            return true;
        }//end of method

        private bool handle_nlp_happy(string userInput)
        {//start of method
            string[] responses = {
                "That's great to hear! I'm glad you're having a good day.",
                "Awesome! Positivity is always welcome here.",
                "I'm happy for you! Let me know if you need any cybersecurity tips."
            };
            Random random = new Random();
            displayer.ShowMessage("Ruby", responses[random.Next(responses.Length)]);
            AddToActivityLog("NLP: Happy sentiment detected");
            return true;
        }//end of method

        private bool handle_nlp_sad(string userInput)
        {//start of method
            string response = "I'm sorry you're feeling this way. I'm here to support you.\n\n" +
                             "When you're ready, I can help you with any cybersecurity questions you have.";
            displayer.ShowMessage("Ruby", response);
            AddToActivityLog("NLP: Sad sentiment detected");
            return true;
        }//end of method

        // ============================================================
        // QUIZ METHODS
        // ============================================================

        private bool handle_quiz_command(string userInput)
        {//start of method
            string lowerInput = userInput.ToLower().Trim();

            if (quizManager.IsQuizActive())
            {//start of if
                if (lowerInput == "quit" || lowerInput == "exit")
                {//start of if
                    string results = quizManager.SubmitAnswer("quit");
                    displayer.ShowMessage("Ruby", results);
                    AddToActivityLog("Quiz ended early");
                    return true;
                }//end of if

                string result = quizManager.SubmitAnswer(userInput);
                displayer.ShowMessage("Ruby", result);

                if (!quizManager.IsQuizActive())
                {//start of if
                    AddToActivityLog("Quiz completed - Score: " + quizManager.GetScore() + "/" + quizManager.GetTotalQuestions());
                }//end of if

                return true;
            }//end of if

            if (lowerInput.Contains("start quiz") || lowerInput.Contains("play quiz") ||
                lowerInput.Contains("take quiz") || lowerInput.Contains("quiz me") ||
                lowerInput.Contains("test me"))
            {//start of if
                string result = quizManager.StartQuiz();
                displayer.ShowMessage("Ruby", result);
                AddToActivityLog("Quiz started");
                return true;
            }//end of if

            return false;
        }//end of method

        // ============================================================
        // MEMORY / INTEREST METHODS
        // ============================================================

        private string extract_interest(string input)
        {//start of method
            string lower = input.ToLower();
            int index = lower.IndexOf("interested in");
            if (index >= 0)
            {//start of if
                string interest = input.Substring(index + 13).Trim();
                string[] words = interest.Split(' ');
                return words[0].Trim('.', '!', '?');
            }//end of if
            return string.Empty;
        }//end of method

        private void store_interest(string username, string interest)
        {//start of method

            string filename = "user_interests.txt";
            string line = username + "|" + interest;

            if (File.Exists(filename))
            {//start of if
                string[] lines = File.ReadAllLines(filename);
                bool userFound = false;

                for (int i = 0; i < lines.Length; i++)
                {//start of for
                    if (lines[i].StartsWith(username + "|"))
                    {//start of if
                        if (!lines[i].Contains(interest))
                        {//start of if
                            lines[i] = lines[i] + "," + interest;
                        }//end of if
                        userFound = true;
                        break;
                    }//end of if
                }//end of for

                if (!userFound)
                {//start of if
                    File.AppendAllText(filename, line + "\n");
                }//end of if
                else
                {//start of else
                    File.WriteAllLines(filename, lines);
                }//end of else
            }//end of if
            else
            {//start of else
                File.AppendAllText(filename, line + "\n");
            }//end of else
        }//end of method

        // ============================================================
        // ACTIVITY LOG METHODS - TASK 4
        // ============================================================

        private void AddToActivityLog(string action)
        {//start of method
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            activityLog.Add("[" + timestamp + "] " + action);

            if (activityLog.Count > 50)
            {//start of if
                activityLog.RemoveAt(0);
            }//end of if
        }//end of method

        public string GetActivityLog()
        {//start of method
            if (activityLog.Count == 0)
            {//start of if
                return "No activities have been logged yet.";
            }//end of if

            string result = "Recent Activity Log:\n\n";

            // Show last 10 entries
            int startIndex = Math.Max(0, activityLog.Count - 10);

            for (int i = startIndex; i < activityLog.Count; i++)
            {//start of for
                result += (i - startIndex + 1) + ". " + activityLog[i] + "\n";
            }//end of for

            if (activityLog.Count > 10)
            {//start of if
                result += "\nShowing last 10 of " + activityLog.Count + " activities. Use 'Show full log' to see all.";
            }//end of if

            return result;
        }//end of method

        // ============================================================
        // QUICK ACTION METHODS
        // ============================================================

        private void quick_action_tasks(object sender, RoutedEventArgs e)
        {//start of method
            question.Text = "Show my tasks";
            send(sender, e);
        }//end of method

        private void quick_action_quiz(object sender, RoutedEventArgs e)
        {//start of method
            question.Text = "Start quiz";
            send(sender, e);
        }//end of method

        private void quick_action_log(object sender, RoutedEventArgs e)
        {//start of method
            question.Text = "Show activity log";
            send(sender, e);
        }//end of method

        private void quick_action_help(object sender, RoutedEventArgs e)
        {//start of method
            string helpMessage = "Here's what I can help you with:\n\n" +
                                 "TASKS:\n" +
                                 "  - 'Add task - [description]' - Add a new task\n" +
                                 "  - 'Show my tasks' - View all tasks\n" +
                                 "  - 'Mark [task] as complete' - Complete a task\n" +
                                 "  - 'Delete [task]' - Delete a task\n\n" +
                                 "QUIZ:\n" +
                                 "  - 'Start quiz' - Begin the cybersecurity quiz\n" +
                                 "  - Answer with A, B, C, or D during the quiz\n" +
                                 "  - 'quit' during quiz to exit\n\n" +
                                 "ACTIVITY LOG:\n" +
                                 "  - 'Show activity log' - View recent actions\n" +
                                 "  - 'Show full log' - View all activities\n\n" +
                                 "SECURITY TOPICS:\n" +
                                 "  - Ask me about: passwords, scams, privacy, phishing\n" +
                                 "  - 'tell me more' for additional tips\n\n" +
                                 "GENERAL:\n" +
                                 "  - 'How are you?' - Check in with me\n" +
                                 "  - 'What's your purpose?' - Learn about me\n" +
                                 "  - 'I'm interested in [topic]' - I'll remember your interests";

            displayer.ShowMessage("Ruby", helpMessage);
            AddToActivityLog("Help menu displayed");
            question.Clear();
        }//end of method

        private void quick_action_clear(object sender, RoutedEventArgs e)
        {//start of method
            chats.Items.Clear();
            displayer.ShowMessage("Ruby", "Chat cleared. How can I help you today?");
            AddToActivityLog("Chat cleared");
            question.Clear();
        }//end of method

        private void question_KeyDown(object sender, KeyEventArgs e)
        {//start of method
            if (e.Key == Key.Enter)
            {//start of if
                send(sender, e);
            }//end of if
        }//end of method

    }//end of class

}//end of namespace
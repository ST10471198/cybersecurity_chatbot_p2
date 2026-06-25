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

        //creating an instance of ArrayList
        ArrayList reply = new ArrayList();
        ArrayList ignore = new ArrayList();

        //declaring all class instances
        private response_finder finder;
        private response_handler handler;
        private topic_detector detector;
        private message_displayer displayer;
        private sentiment_detector sentimentDetector;
        private task_manager taskManager;

        //variables to store the last detected topic for follow-up questions
        private string username = string.Empty;
        private string lastTopic = string.Empty;

        // Task Assistant variables
        private bool awaitingReminderResponse = false;
        private string pendingTaskName = "";
        private string pendingTaskDescription = "";

        public MainWindow()
        {//start of constructor
            InitializeComponent();

            //creating an instance for the class voice_greeting
            new voice_greeting();

            //creating an instance of the respond class with a constructor
            new respond(reply, ignore);

            //initializing all the class instances
            finder = new response_finder(reply);
            handler = new response_handler(reply, ignore);
            detector = new topic_detector();
            displayer = new message_displayer(chats);
            sentimentDetector = new sentiment_detector(reply, finder, displayer);

        }//end of constructor

        // ============================================================
        // NAVIGATION METHODS
        // ============================================================

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

            // Initialize TaskManager after username is set
            taskManager = new task_manager(username);

            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;

        }//end of method

        // ============================================================
        // MAIN SEND METHOD
        // ============================================================

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

            // ============================================================
            // CHECK: Are we waiting for a reminder response? (PRIORITY 1)
            // ============================================================
            if (awaitingReminderResponse)
            {//start of if
                string lowerInput = userInput.ToLower().Trim();

                // Check if user said yes
                if (lowerInput.Contains("yes") || lowerInput.Contains("yeah") || lowerInput.Contains("yep") ||
                    lowerInput.Contains("sure") || lowerInput.Contains("ok") || lowerInput.Contains("okay"))
                {//start of if
                    displayer.ShowMessage("Ruby", "Great! How many days from now would you like the reminder? (Enter a number)");
                    awaitingReminderResponse = false;
                    question.Clear();
                    return;
                }//end of if
                // Check if user said no
                else if (lowerInput.Contains("no") || lowerInput.Contains("nope") || lowerInput.Contains("nah"))
                {//start of else if
                    string result = taskManager.add_task(pendingTaskName, pendingTaskDescription, "");
                    displayer.ShowMessage("Ruby", result);
                    awaitingReminderResponse = false;
                    pendingTaskName = "";
                    pendingTaskDescription = "";
                    question.Clear();
                    return;
                }//end of else if
                else
                {//start of else
                    // If user entered something else, ask again
                    displayer.ShowMessage("Ruby", "Please answer Yes or No. Would you like a reminder for this task?");
                    question.Clear();
                    return;
                }//end of else
            }//end of if

            // ============================================================
            // CHECK: Are we waiting for days input? (PRIORITY 2)
            // ============================================================
            // Check if the input is a number (days) and we have a pending task
            if (!string.IsNullOrEmpty(pendingTaskName) && IsNumeric(userInput))
            {//start of if
                int days = int.Parse(userInput);
                if (days > 0)
                {//start of if
                    string reminderDate = DateTime.Now.AddDays(days).ToString("yyyy-MM-dd");
                    string result = taskManager.add_task(pendingTaskName, pendingTaskDescription, reminderDate);
                    displayer.ShowMessage("Ruby", "Got it! " + result);
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

            // ============================================================
            // CHECK: Task Assistant Commands (PRIORITY 3)
            // ============================================================
            if (taskManager != null && handle_task_assistant_command(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // ============================================================
            // CHECK: Sentiment Detection (PRIORITY 4)
            // ============================================================
            if (sentimentDetector.DetectSentiment(userInput))
            {//start of if
                question.Clear();
                return;
            }//end of if

            // ============================================================
            // CHECK: Follow-up Questions (PRIORITY 5)
            // ============================================================
            string fullInput = userInput.ToLower();
            if (fullInput.Contains("more") || fullInput.Contains("another tip") ||
                fullInput.Contains("tell me more") || fullInput.Contains("explain more"))
            {//start of if
                if (!string.IsNullOrEmpty(lastTopic))
                {//start of if
                    string response = finder.FindResponseByTopic(lastTopic);
                    displayer.ShowMessage("Ruby", response);
                    question.Clear();
                    return;
                }//end of if
            }//end of if

            // ============================================================
            // CHECK: Interest Statements (Memory Feature) (PRIORITY 6)
            // ============================================================
            if (fullInput.Contains("interested in"))
            {//start of if
                string interest = extract_interest(userInput);
                if (!string.IsNullOrEmpty(interest))
                {//start of if
                    store_interest(username, interest);
                    displayer.ShowMessage("Ruby", "Great! I'll remember that you're interested in " + interest +
                                          ". It's a crucial part of staying safe online.");
                    question.Clear();
                    return;
                }//end of if
            }//end of if

            // ============================================================
            // CHECK: Topic Detection (PRIORITY 7)
            // ============================================================
            string[] words = userInput.ToLower().Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '-' },
                                                       StringSplitOptions.RemoveEmptyEntries);

            // Find topic and response
            string topic = detector.DetectTopic(words);
            if (!string.IsNullOrEmpty(topic))
            {//start of if
                lastTopic = topic;
                string response = finder.FindResponseByTopic(topic);
                displayer.ShowMessage("Ruby", response);
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

            // Check for "show tasks" or "view tasks"
            if ((lowerInput.Contains("show") || lowerInput.Contains("view") || lowerInput.Contains("list")) &&
                lowerInput.Contains("task"))
            {//start of if
                string tasks = taskManager.view_tasks();
                displayer.ShowMessage("Ruby", tasks);
                return true;
            }//end of if

            // Check for "show pending tasks"
            if (lowerInput.Contains("pending") && lowerInput.Contains("task"))
            {//start of if
                string pending = taskManager.get_pending_task_names();
                displayer.ShowMessage("Ruby", pending);
                return true;
            }//end of if

            // Check for "add task" or "create task"
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
                    return true;
                }//end of if
                else
                {//start of else
                    displayer.ShowMessage("Ruby", "Please specify what task you want to add. For example: 'Add task - Review privacy settings'");
                    return true;
                }//end of else
            }//end of if

            // Check for "mark task as complete"
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

            // Check for "delete task"
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

        private void question_KeyDown(object sender, KeyEventArgs e)
        {//start of method
            if (e.Key == Key.Enter)
            {//start of if
                send(sender, e);
            }//end of if
        }//end of method

    }//end of class

}//end of namespace
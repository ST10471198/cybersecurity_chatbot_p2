using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
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

        private string username = string.Empty;
        private string lastTopic = string.Empty;

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
            {
                File.AppendAllText(filename, username + "\n");
                displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome to Ruby, your Cybersecurity Assistant!");
            }
            else
            {
                string[] names = File.ReadAllLines(filename);
                bool exists = names.Any(n => n.ToLower() == username.ToLower());

                if (exists)
                    displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome back to Ruby!");
                else
                {
                    File.AppendAllText(filename, username + "\n");
                    displayer.ShowMessage("Ruby", "Hey " + username + "! Welcome to Ruby, your Cybersecurity Assistant!");
                }
            }

            username_grid.Visibility = Visibility.Hidden;
            chat_grid.Visibility = Visibility.Visible;

        }//end of method

        private void send(object sender, RoutedEventArgs e)
        {//start of send method

            string userInput = question.Text.Trim();

            if (string.IsNullOrWhiteSpace(userInput))
            {
                displayer.ShowMessage("Ruby", "Please enter a question.");
                question.Clear();
                return;
            }

            displayer.ShowMessage(username, userInput);

            // Check for sentiment first
            if (sentimentDetector.DetectSentiment(userInput))
            {
                question.Clear();
                return;
            }

            string[] words = userInput.ToLower().Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '-' }, StringSplitOptions.RemoveEmptyEntries);

            // Check for follow-up questions
            string fullInput = userInput.ToLower();
            if (fullInput.Contains("more") || fullInput.Contains("another tip") || fullInput.Contains("tell me more") || fullInput.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(lastTopic))
                {
                    string response = finder.FindResponseByTopic(lastTopic);
                    displayer.ShowMessage("Ruby", response);
                    question.Clear();
                    return;
                }
            }

            // Check for interest statements (Memory feature)
            if (fullInput.Contains("interested in"))
            {
                string interest = ExtractInterest(userInput);
                if (!string.IsNullOrEmpty(interest))
                {
                    StoreInterest(username, interest);
                    displayer.ShowMessage("Ruby", "Great! I'll remember that you're interested in " + interest +
                                          ". It's a crucial part of staying safe online.");
                    question.Clear();
                    return;
                }
            }

            // Find topic and response
            string topic = detector.DetectTopic(words);
            if (!string.IsNullOrEmpty(topic))
            {
                lastTopic = topic;
                string response = finder.FindResponseByTopic(topic);
                displayer.ShowMessage("Ruby", response);
            }
            else
            {
                string response = handler.GetDefaultResponse();
                displayer.ShowMessage("Ruby", response);
            }

            question.Clear();

        }//end of send method

        private string ExtractInterest(string input)
        {
            string lower = input.ToLower();
            int index = lower.IndexOf("interested in");
            if (index >= 0)
            {
                string interest = input.Substring(index + 13).Trim();
                string[] words = interest.Split(' ');
                return words[0].Trim('.', '!', '?');
            }
            return string.Empty;
        }

        private void StoreInterest(string username, string interest)
        {
            string filename = "user_interests.txt";
            string line = username + "|" + interest;

            if (File.Exists(filename))
            {
                string[] lines = File.ReadAllLines(filename);
                bool userFound = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith(username + "|"))
                    {
                        if (!lines[i].Contains(interest))
                        {
                            lines[i] = lines[i] + "," + interest;
                        }
                        userFound = true;
                        break;
                    }
                }

                if (!userFound)
                {
                    File.AppendAllText(filename, line + "\n");
                }
                else
                {
                    File.WriteAllLines(filename, lines);
                }
            }
            else
            {
                File.AppendAllText(filename, line + "\n");
            }
        }

    }//end of class

}//end of namespace
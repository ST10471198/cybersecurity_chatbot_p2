using System;
using System.Collections;
using System.Collections.Generic;

namespace cybersecurity_chatbot_p2
{
    public class response_finder
    {
        private Dictionary<string, List<string>> topicResponses;
        private Random random;

        public response_finder(ArrayList replies)
        {
            random = new Random();
            BuildTopicDictionary(replies);
        }

        private void BuildTopicDictionary(ArrayList replies)
        {
            topicResponses = new Dictionary<string, List<string>>();

            foreach (string item in replies)
            {
                string lowerItem = item.ToLower();
                string topic = ExtractTopic(lowerItem);

                if (!string.IsNullOrEmpty(topic))
                {
                    string response = ExtractResponse(item);

                    if (!topicResponses.ContainsKey(topic))
                        topicResponses[topic] = new List<string>();

                    topicResponses[topic].Add(response);
                }
            }
        }

        private string ExtractTopic(string item)
        {
            if (item.StartsWith("password")) return "password";
            if (item.StartsWith("scam")) return "scam";
            if (item.StartsWith("privacy")) return "privacy";
            if (item.StartsWith("phishing")) return "phishing";
            if (item.StartsWith("cybersecurity")) return "cybersecurity";
            if (item.StartsWith("firewall")) return "firewall";
            if (item.StartsWith("vpn")) return "vpn";
            if (item.StartsWith("fraud")) return "fraud";
            if (item.StartsWith("greeting")) return "greeting";
            if (item.StartsWith("purpose")) return "purpose";
            if (item.StartsWith("frustrated")) return "frustrated";
            if (item.StartsWith("confused")) return "confused";
            if (item.StartsWith("worried")) return "worried";
            if (item.StartsWith("happy")) return "happy";
            if (item.StartsWith("sad")) return "sad";
            if (item.StartsWith("angry")) return "angry";
            if (item.StartsWith("hacked account")) return "hacked account";

            return string.Empty;
        }

        private string ExtractResponse(string item)
        {
            int spaceIndex = item.IndexOf(' ');
            if (spaceIndex > 0 && spaceIndex < item.Length - 1)
                return item.Substring(spaceIndex + 1).Trim();
            return item;
        }

        public string FindResponseByTopic(string topic)
        {
            if (topicResponses.ContainsKey(topic) && topicResponses[topic].Count > 0)
            {
                int index = random.Next(topicResponses[topic].Count);
                return topicResponses[topic][index];
            }

            return "I have information on that topic. What specific aspect would you like to know about?";
        }
    }
}
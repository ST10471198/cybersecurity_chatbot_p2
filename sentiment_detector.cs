using System.Collections;

namespace cybersecurity_chatbot_p2
{
    public class sentiment_detector
    {
        private response_finder finder;
        private message_displayer displayer;

        public sentiment_detector(ArrayList responses, response_finder responseFinder, message_displayer messageDisplayer)
        {
            finder = responseFinder;
            displayer = messageDisplayer;
        }

        public bool DetectSentiment(string userInput)
        {
            string lowerInput = userInput.ToLower();

            // Check for worried/concerned (Requirement 6)
            if (lowerInput.Contains("worried") || lowerInput.Contains("concerned") || lowerInput.Contains("nervous"))
            {
                string response = "It's completely understandable to feel that way. Scammers can be very convincing.\n\n" +
                                 "Here's a tip to help you stay safe:\n" + finder.FindResponseByTopic("scam");
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            // Check for frustration
            if (lowerInput.Contains("frustrated") || lowerInput.Contains("annoyed"))
            {
                string response = "I understand you're frustrated. Let's work through the issue step by step.\n\n" +
                                 "What specific cybersecurity concern do you have?";
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            // Check for confusion
            if (lowerInput.Contains("confused") || lowerInput.Contains("unsure") || lowerInput.Contains("don't understand"))
            {
                string response = "That's okay, confusion is normal with cybersecurity topics.\n\n" +
                                 "Let me explain it clearly:\n" + finder.FindResponseByTopic("cybersecurity");
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            // Check for sadness
            if (lowerInput.Contains("sad") || lowerInput.Contains("upset") || lowerInput.Contains("depressed"))
            {
                string response = "I'm sorry you're feeling this way. I'm here for you.\n\n" +
                                 "When you're ready, I can help you with any cybersecurity questions.";
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            // Check for happiness
            if (lowerInput.Contains("happy") || lowerInput.Contains("great") || lowerInput.Contains("awesome"))
            {
                string response = "That's great to hear! I'm glad you're having a good day.\n\n" +
                                 "Let me know if you need any cybersecurity tips!";
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            // Check for anger
            if (lowerInput.Contains("angry") || lowerInput.Contains("mad") || lowerInput.Contains("furious"))
            {
                string response = "I understand you're angry. Let's try to solve the issue together.\n\n" +
                                 "Tell me what's bothering you and I'll help.";
                displayer.ShowMessage("Ruby", response);
                return true;
            }

            return false;
        }
    }
}
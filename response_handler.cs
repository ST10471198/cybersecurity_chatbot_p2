using System;
using System.Collections;

namespace cybersecurity_chatbot_p2
{
    public class response_handler
    {
        private string[] fallbackMessages;
        private Random random;

        public response_handler(ArrayList replies, ArrayList ignoredWords)
        {
            random = new Random();

            fallbackMessages = new string[]
            {
                "I'm sorry, I don't understand that. Could you rephrase your question?",
                "I didn't quite get that. Try asking about passwords, scams, or privacy.",
                "Hmm, I'm not sure how to respond to that. Can you ask something else?",
                "I couldn't find an answer for that. Please ask about cybersecurity topics.",
                "My apologies, I don't have information on that topic yet."
            };
        }

        public string GetDefaultResponse()
        {
            int index = random.Next(fallbackMessages.Length);
            return fallbackMessages[index];
        }
    }
}
namespace cybersecurity_chatbot_p2
{
    public class topic_detector
    {
        public string DetectTopic(string[] words)
        {
            foreach (string word in words)
            {
                // Password topic
                if (word.Contains("password") || word.Contains("passphrase"))
                    return "password";

                // Scam/Fraud topic
                if (word.Contains("scam") || word.Contains("fraud") || word.Contains("fake"))
                    return "scam";

                // Privacy topic
                if (word.Contains("privacy") || word.Contains("private"))
                    return "privacy";

                // Phishing topic
                if (word.Contains("phish") || word.Contains("phishing"))
                    return "phishing";

                // Cybersecurity topic
                if (word.Contains("cyber") || word.Contains("security"))
                    return "cybersecurity";

                // Firewall topic
                if (word.Contains("firewall"))
                    return "firewall";

                // VPN topic
                if (word.Contains("vpn"))
                    return "vpn";

                // Hacked account topic
                if (word.Contains("hack") || word.Contains("compromised"))
                    return "hacked account";

                // Greeting
                if (word.Contains("hello") || word.Contains("hi") || word.Contains("hey"))
                    return "greeting";

                // Purpose
                if (word.Contains("purpose"))
                    return "purpose";
            }

            return string.Empty;
        }
    }
}
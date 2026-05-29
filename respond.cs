using System.Collections;

namespace cybersecurity_chatbot_p2
{//start of namespace
    public class respond
    {//start of class
        public respond(ArrayList reply, ArrayList ignore)
        {//start of constructor
            answers(reply);
            words(ignore);
        }//end of constructor

        private void words(ArrayList ignoring)
        {
            ignoring.Add("a");
            ignoring.Add("about");
            ignoring.Add("above");
            ignoring.Add("after");
            ignoring.Add("again");
            ignoring.Add("all");
            ignoring.Add("am");
            ignoring.Add("an");
            ignoring.Add("and");
            ignoring.Add("are");
            ignoring.Add("as");
            ignoring.Add("at");
            ignoring.Add("be");
            ignoring.Add("been");
            ignoring.Add("but");
            ignoring.Add("by");
            ignoring.Add("can");
            ignoring.Add("did");
            ignoring.Add("do");
            ignoring.Add("does");
            ignoring.Add("for");
            ignoring.Add("from");
            ignoring.Add("had");
            ignoring.Add("has");
            ignoring.Add("have");
            ignoring.Add("he");
            ignoring.Add("her");
            ignoring.Add("him");
            ignoring.Add("his");
            ignoring.Add("how");
            ignoring.Add("i");
            ignoring.Add("if");
            ignoring.Add("in");
            ignoring.Add("into");
            ignoring.Add("is");
            ignoring.Add("it");
            ignoring.Add("me");
            ignoring.Add("more");
            ignoring.Add("my");
            ignoring.Add("no");
            ignoring.Add("not");
            ignoring.Add("of");
            ignoring.Add("on");
            ignoring.Add("or");
            ignoring.Add("so");
            ignoring.Add("some");
            ignoring.Add("such");
            ignoring.Add("than");
            ignoring.Add("that");
            ignoring.Add("the");
            ignoring.Add("their");
            ignoring.Add("them");
            ignoring.Add("then");
            ignoring.Add("there");
            ignoring.Add("these");
            ignoring.Add("they");
            ignoring.Add("this");
            ignoring.Add("those");
            ignoring.Add("to");
            ignoring.Add("was");
            ignoring.Add("we");
            ignoring.Add("were");
            ignoring.Add("what");
            ignoring.Add("when");
            ignoring.Add("where");
            ignoring.Add("which");
            ignoring.Add("who");
            ignoring.Add("will");
            ignoring.Add("with");
            ignoring.Add("you");
            ignoring.Add("your");
        }

        public void answers(ArrayList add_answers)
        {
            // Greeting responses
            add_answers.Add("greeting I'm doing well, thanks for asking! How are you doing today?");
            add_answers.Add("greeting I'm great today! How can I help you?");
            add_answers.Add("greeting Doing good! Hope you are also doing well today.");

            // Purpose responses
            add_answers.Add("purpose My purpose is to educate you on how to stay safe online and guide your cybersecurity questions.");
            add_answers.Add("purpose I help users understand online safety and digital protection.");
            add_answers.Add("purpose I assist with cybersecurity awareness and safety guidance.");

            // Cybersecurity responses
            add_answers.Add("cybersecurity Cybersecurity is about protecting systems and networks from digital threats.");
            add_answers.Add("cybersecurity It involves protecting devices and online accounts from attacks.");
            add_answers.Add("cybersecurity It focuses on securing digital information and systems.");

            // Password responses (Requirement 2)
            add_answers.Add("password Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.");
            add_answers.Add("password Use at least 12 characters with a mix of letters, numbers, and symbols.");
            add_answers.Add("password Never reuse the same password across multiple accounts.");
            add_answers.Add("password Consider using a password manager to generate and store strong passwords.");

            // Scam responses (Requirement 2)
            add_answers.Add("scam Scammers often create urgency to trick you. Take your time to verify before acting.");
            add_answers.Add("scam Never share personal information or send money to unverified sources.");
            add_answers.Add("scam If something sounds too good to be true, it probably is a scam.");
            add_answers.Add("scam Always verify the identity of the person or organization contacting you.");

            // Privacy responses (Requirement 2)
            add_answers.Add("privacy Protect your privacy by reviewing app permissions regularly.");
            add_answers.Add("privacy Limit the personal information you share on social media.");
            add_answers.Add("privacy Use a VPN when connecting to public Wi-Fi networks.");
            add_answers.Add("privacy Clear your browser cookies and cache periodically.");

            // Phishing responses (Requirement 3 - Random Responses)
            add_answers.Add("phishing Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.");
            add_answers.Add("phishing Check the sender's email address carefully - scammers use addresses that look similar to real ones.");
            add_answers.Add("phishing Never click on suspicious links or attachments. Hover over links to see the actual URL.");
            add_answers.Add("phishing Look for spelling and grammar mistakes - these are common in phishing emails.");
            add_answers.Add("phishing If an offer seems too good to be true, it probably is a phishing attempt.");

            // Firewall responses
            add_answers.Add("firewall A firewall controls network traffic based on security rules.");
            add_answers.Add("firewall It helps block unwanted access to your device or network.");
            add_answers.Add("firewall It acts as a protective barrier between trusted and untrusted networks.");

            // Hacked account responses
            add_answers.Add("hacked account Immediately secure your account and log out of all devices.");
            add_answers.Add("hacked account Contact support if your account has been compromised.");
            add_answers.Add("hacked account Enable extra security like two-factor authentication.");

            // Fraud responses
            add_answers.Add("fraud Contact your bank immediately if fraud is detected.");
            add_answers.Add("fraud Report suspicious financial activity to the authorities.");
            add_answers.Add("fraud Monitor your accounts for unusual activity.");

            // VPN responses
            add_answers.Add("vpn A VPN helps protect your privacy on public Wi-Fi.");
            add_answers.Add("vpn It encrypts your internet traffic for safety.");
            add_answers.Add("vpn It improves security when using public networks.");

            // Sentiment responses (Requirement 6)
            add_answers.Add("frustrated I understand you're frustrated. Let's work through the issue step by step. I'm here to help.");
            add_answers.Add("frustrated It's okay to feel frustrated when things aren't working. Let me help you solve this.");
            add_answers.Add("frustrated Take a breath, we'll fix this together. What specific issue are you facing?");

            add_answers.Add("confused That's okay, confusion is normal with cybersecurity. I'll explain it clearly for you.");
            add_answers.Add("confused Let me break it down step by step so it makes sense.");
            add_answers.Add("confused No worries, I'll help you understand it better.");

            add_answers.Add("worried It's okay to feel worried. I'm here to help you stay safe online.");
            add_answers.Add("worried Don't panic, most cybersecurity issues can be fixed quickly.");
            add_answers.Add("worried I understand your concern. Let's make sure your information is safe.");

            add_answers.Add("happy That's great to hear! I'm glad things are going well.");
            add_answers.Add("happy Awesome! Positivity is always good.");
            add_answers.Add("happy I'm happy for you! Let me know if you need anything.");

            add_answers.Add("sad I'm sorry you're feeling this way. I'm here for you.");
            add_answers.Add("sad That sounds tough, take things one step at a time.");
            add_answers.Add("sad I hope things improve soon. You can talk to me anytime.");

            add_answers.Add("angry I understand you're angry. Let's try solve the issue together.");
            add_answers.Add("angry It's okay to feel angry, but I'll help you fix the problem.");
            add_answers.Add("angry Take your time, I'm here to help you sort it out.");
        }
    }
}
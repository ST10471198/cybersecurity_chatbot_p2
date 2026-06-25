using System;
using System.Collections.Generic;

namespace cybersecurity_chatbot_p2
{//start of namespace
    public class quiz_manager
    {//start of class

        private List<QuizQuestion> questions;
        private int currentQuestionIndex;
        private int score;
        private bool quizActive;

        public quiz_manager()
        {//start of constructor
            questions = new List<QuizQuestion>();
            currentQuestionIndex = 0;
            score = 0;
            quizActive = false;
            InitializeQuestions();
        }//end of constructor

        private void InitializeQuestions()
        {//start of method

            // Question 1 - Multiple Choice (Phishing)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What should you do if you receive an email asking for your password?",
                Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                CorrectAnswer = "C",
                Explanation = "Reporting phishing emails helps prevent scams and protects others from falling victim."
            });

            // Question 2 - True/False (Password Safety)
            questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: Using the same password for multiple accounts is safe.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "B",
                Explanation = "Using the same password across multiple accounts is dangerous. If one account is compromised, all your accounts are at risk."
            });

            // Question 3 - Multiple Choice (Password Safety)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What is the recommended minimum length for a strong password?",
                Options = new List<string> { "4 characters", "8 characters", "12 characters", "16 characters" },
                CorrectAnswer = "C",
                Explanation = "A strong password should be at least 12 characters long with a mix of letters, numbers, and symbols."
            });

            // Question 4 - Multiple Choice (Social Engineering)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What is social engineering in cybersecurity?",
                Options = new List<string> { "A type of computer virus", "Manipulating people to reveal confidential information", "Building social media networks", "Engineering social platforms" },
                CorrectAnswer = "B",
                Explanation = "Social engineering is a tactic used by attackers to manipulate people into revealing confidential information or performing actions."
            });

            // Question 5 - True/False (Safe Browsing)
            questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: You should always check for 'https://' in the URL before entering personal information.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "A",
                Explanation = "HTTPS indicates a secure connection. Always look for it before entering personal or financial information online."
            });

            // Question 6 - Multiple Choice (Phishing)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What is a common sign of a phishing email?",
                Options = new List<string> { "Professional language", "Urgent or threatening tone", "Correct spelling and grammar", "Official company logo" },
                CorrectAnswer = "B",
                Explanation = "Phishing emails often create a sense of urgency or fear to pressure you into acting without thinking."
            });

            // Question 7 - True/False (Password Safety)
            questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: A password manager is a safe way to store and manage your passwords.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "A",
                Explanation = "Password managers encrypt your passwords and help you generate strong, unique passwords for each account."
            });

            // Question 8 - Multiple Choice (Safe Browsing)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What should you do when using public Wi-Fi?",
                Options = new List<string> { "Use a VPN", "Disable your firewall", "Share your password with others", "Ignore security settings" },
                CorrectAnswer = "A",
                Explanation = "A VPN (Virtual Private Network) encrypts your internet traffic and protects your data on public Wi-Fi networks."
            });

            // Question 9 - Multiple Choice (Social Engineering)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What is a common social engineering attack?",
                Options = new List<string> { "Phishing emails", "Malware downloads", "Firewall bypass", "Password cracking" },
                CorrectAnswer = "A",
                Explanation = "Phishing emails are a common form of social engineering where attackers impersonate trusted sources to steal information."
            });

            // Question 10 - True/False (Cybersecurity)
            questions.Add(new QuizQuestion
            {
                QuestionText = "True or False: Two-factor authentication (2FA) adds an extra layer of security to your accounts.",
                Options = new List<string> { "True", "False" },
                CorrectAnswer = "A",
                Explanation = "Two-factor authentication requires a second form of verification, making it much harder for attackers to access your accounts."
            });

            // Question 11 - Multiple Choice (Cybersecurity)
            questions.Add(new QuizQuestion
            {
                QuestionText = "What is the first step you should take if you suspect your account has been hacked?",
                Options = new List<string> { "Share the news on social media", "Change your password immediately", "Ignore it and hope it goes away", "Send money to the hacker" },
                CorrectAnswer = "B",
                Explanation = "Immediately change your password and enable two-factor authentication if your account is compromised."
            });

            // Question 12 - Multiple Choice (Safe Browsing)
            questions.Add(new QuizQuestion
            {
                QuestionText = "Which of the following is a safe browsing habit?",
                Options = new List<string> { "Clicking on pop-up ads", "Downloading from untrusted websites", "Keeping your browser updated", "Disabling antivirus software" },
                CorrectAnswer = "C",
                Explanation = "Keeping your browser updated ensures you have the latest security patches to protect against vulnerabilities."
            });

        }//end of method

        public string StartQuiz()
        {//start of method
            currentQuestionIndex = 0;
            score = 0;
            quizActive = true;
            return get_current_question();
        }//end of method

        public string get_current_question()
        {//start of method
            if (!quizActive || currentQuestionIndex >= questions.Count)
            {//start of if
                return get_quiz_results();
            }//end of if

            QuizQuestion q = questions[currentQuestionIndex];
            string result = "Question " + (currentQuestionIndex + 1) + " of " + questions.Count + ":\n\n";
            result += q.QuestionText + "\n\n";

            char optionLabel = 'A';
            foreach (string option in q.Options)
            {//start of foreach
                result += optionLabel + ") " + option + "\n";
                optionLabel++;
            }//end of foreach

            result += "\nType your answer (A/B/C/D) or type 'quit' to exit the quiz.";
            return result;
        }//end of method

        public string SubmitAnswer(string userAnswer)
        {//start of method
            if (!quizActive)
            {//start of if
                return "The quiz is not active. Type 'Start quiz' to begin.";
            }//end of if

            if (currentQuestionIndex >= questions.Count)
            {//start of if
                return get_quiz_results();
            }//end of if

            QuizQuestion q = questions[currentQuestionIndex];
            string userInput = userAnswer.ToUpper().Trim();

            // Check if user wants to quit
            if (userInput == "QUIT" || userInput == "EXIT")
            {//start of if
                quizActive = false;
                return "Quiz ended. You answered " + score + " out of " + currentQuestionIndex + " questions correctly.";
            }//end of if

            // Validate answer format (A, B, C, D)
            if (string.IsNullOrEmpty(userInput) || userInput.Length > 1 || !"ABCD".Contains(userInput))
            {//start of if
                return "Please enter a valid answer (A, B, C, or D).\n\n" + get_current_question();
            }//end of if

            string feedback = "";
            bool isCorrect = userInput == q.CorrectAnswer;

            if (isCorrect)
            {//start of if
                score++;
                feedback = "Correct! " + q.Explanation;
            }//end of if
            else
            {//start of else
                string correctOption = get_option_text(q.Options, q.CorrectAnswer);
                feedback = "Incorrect. The correct answer was " + q.CorrectAnswer + ": " + correctOption + "\n" + q.Explanation;
            }//end of else

            currentQuestionIndex++;

            // Check if quiz is complete
            if (currentQuestionIndex >= questions.Count)
            {//start of if
                quizActive = false;
                return feedback + "\n\n" + get_quiz_results();
            }//end of if

            return feedback + "\n\n" + get_current_question();
        }//end of method

        private string get_option_text(List<string> options, string correctAnswer)
        {//start of method
            int index = correctAnswer[0] - 'A';
            if (index >= 0 && index < options.Count)
            {//start of if
                return options[index];
            }//end of if
            return "";
        }//end of method

        public string get_quiz_results()
        {//start of method
            quizActive = false;
            int totalQuestions = questions.Count;
            string result = "Quiz Complete!\n\n";
            result += "You got " + score + " out of " + totalQuestions + " questions correct.\n";
            result += "Score: " + Math.Round((double)score / totalQuestions * 100) + "%\n\n";

            // Feedback based on score
            if (score >= 10)
            {//start of if
                result += "Outstanding! You're a cybersecurity pro! Your knowledge is impressive!";
            }//end of if
            else if (score >= 8)
            {//start of else if
                result += "Great job! You have excellent cybersecurity awareness!";
            }//end of else if
            else if (score >= 6)
            {//start of else if
                result += "Good effort! Keep learning to strengthen your cybersecurity knowledge.";
            }//end of else if
            else if (score >= 4)
            {//start of else if
                result += "Keep learning! Cybersecurity is important for everyone. Review the topics and try again!";
            }//end of else if
            else
            {//start of else
                result += "Don't give up! Cybersecurity is a learning journey. Try the quiz again to improve your knowledge!";
            }//end of else

            result += "\n\nType 'Start quiz' to play again or ask me about cybersecurity topics.";
            return result;
        }//end of method

        public bool IsQuizActive()
        {//start of method
            return quizActive;
        }//end of method

        public int GetCurrentQuestionNumber()
        {//start of method
            return currentQuestionIndex + 1;
        }//end of method

        public int GetTotalQuestions()
        {//start of method
            return questions.Count;
        }//end of method

        public int GetScore()
        {//start of method
            return score;
        }//end of method

    }//end of class

    // Quiz Question Class
    public class QuizQuestion
    {//start of class
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public string Explanation { get; set; }
    }//end of class

}//end of namespace
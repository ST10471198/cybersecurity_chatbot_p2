using System;
using System.Media;

namespace cybersecurity_chatbot_p2
{//start of namespace
    public class voice_greeting
    {//start of class
        public voice_greeting()
        {//start of constructor

            string auto_path = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", @"\greet.wav");

            //create an instance for the soundPlayer class
            SoundPlayer greetMe = new SoundPlayer(auto_path);
            //then greet
            greetMe.Play();

        }//end of constructor

    }//end of class

}//end of namespace
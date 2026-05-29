using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace cybersecurity_chatbot_p2
{
    public class message_displayer
    {
        private ListView chatListView;

        public message_displayer(ListView chats)
        {
            chatListView = chats;
        }

        public void ShowMessage(string senderName, string message)
        {
            Border messageBorder = new Border
            {
                Margin = new Thickness(5, 2, 5, 2),
                Padding = new Thickness(10, 5, 10, 5),
                CornerRadius = new CornerRadius(8)
            };

            if (senderName.ToLower().Contains("ruby"))
            {
                messageBorder.Background = Brushes.White;
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(76, 175, 80));
                messageBorder.BorderThickness = new Thickness(1);
                messageBorder.HorizontalAlignment = HorizontalAlignment.Left;
                messageBorder.MaxWidth = 400;
            }
            else
            {
                messageBorder.Background = new SolidColorBrush(Color.FromRgb(232, 245, 233));
                messageBorder.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 230, 201));
                messageBorder.BorderThickness = new Thickness(1);
                messageBorder.HorizontalAlignment = HorizontalAlignment.Right;
                messageBorder.MaxWidth = 400;
            }

            TextBlock messageText = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(5)
            };

            Brush nameColor = senderName.ToLower().Contains("ruby") ?
                              new SolidColorBrush(Color.FromRgb(76, 175, 80)) :
                              new SolidColorBrush(Color.FromRgb(46, 125, 50));

            messageText.Inlines.Add(new Run
            {
                Text = senderName + ": ",
                Foreground = nameColor,
                FontWeight = FontWeights.Bold
            });

            messageText.Inlines.Add(new Run
            {
                Text = message,
                Foreground = new SolidColorBrush(Color.FromRgb(51, 51, 51))
            });

            messageBorder.Child = messageText;
            chatListView.Items.Add(messageBorder);
            chatListView.ScrollIntoView(chatListView.Items[chatListView.Items.Count - 1]);
        }
    }
}
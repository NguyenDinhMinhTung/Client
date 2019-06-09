using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// ChatWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ChatWindow : Window
    {
        private Action<String> sendMessage;
        public ChatWindow(Action<String> sendMessage)
        {
            InitializeComponent();

            this.sendMessage = sendMessage;

            txtMessage.Focus();
        }

        public void SetCloseAction(Action closeAction)
        {
            //this.closeAction = closeAction;
        }

        public void PushMessage(String message, Boolean isMe)
        {
            Grid grid = new Grid()
            {
                Margin = new Thickness(10, 2, 10, 10)
            };

            if (isMe)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Auto) });
            }
            else
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            Border border = new Border()
            {
                CornerRadius = isMe ? new CornerRadius(5, 5, 1, 5) : new CornerRadius(5, 5, 5, 1),
                Background = isMe ? new SolidColorBrush(Colors.BlueViolet) : new SolidColorBrush(Colors.SeaGreen),
            };

            DockPanel dockPanel = new DockPanel();

            TextBlock txtMess = new TextBlock()
            {
                Text = message,
                Margin = new Thickness(5, 5, 5, 5),
                Foreground = new SolidColorBrush(Colors.White),
                FontSize = 15,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 150
            };

            DockPanel.SetDock(txtMess, Dock.Right);
            dockPanel.Children.Add(txtMess);

            border.Child = dockPanel;

            if (isMe)
            {
                Grid.SetColumn(border, 1);
            }
            else
            {
                Grid.SetColumn(border, 0);
            }

            grid.Children.Add(border);

            pnlChatBox.Children.Add(grid);
        }

        private void SendMessByMe()
        {
            if (String.IsNullOrWhiteSpace(txtMessage.Text)) return;

            sendMessage(txtMessage.Text);
            PushMessage(txtMessage.Text, true);
            txtMessage.Text = "";
            txtMessage.Focus();
            ScrollViewer.ScrollToEnd();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessByMe();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessByMe();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

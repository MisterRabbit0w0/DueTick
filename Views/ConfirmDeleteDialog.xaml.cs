using System.Windows;

namespace DueTick.Views
{
    public partial class ConfirmDeleteDialog : Window
    {
        public ConfirmDeleteDialog(string message)
        {
            InitializeComponent();
            MessageTextBlock.Text = message;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}

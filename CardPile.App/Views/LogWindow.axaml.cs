using Avalonia.Controls;

namespace CardPile.App.Views
{
    public partial class LogWindow : Window
    {
        public LogWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}

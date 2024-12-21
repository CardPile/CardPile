using Avalonia.Controls;

namespace CardPile.Application.Views
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

using System.Windows;
using System.Windows.Controls.Ribbon;
using Microsoft.Extensions.Logging;
using Postman.ViewModels;

namespace Postman
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        private readonly ILogger<MainWindow> _logger;

        public MainWindow(MainWindowViewModel model, ILogger<MainWindow> logger)
        {
            InitializeComponent();

            DataContext = model;
            _logger = logger;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged)
            {
                this.UrlRibbonTextBox.TextBoxWidth = e.NewSize.Width - 380;
            }
        }
    }
}

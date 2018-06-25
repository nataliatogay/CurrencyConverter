using Converter.ViewModel;
using System.Windows;

namespace Converter
{
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(this);
        }

        public void Alert(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

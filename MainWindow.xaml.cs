using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using SciChart.Examples.Examples.CreateRealtimeChart;
using SciChart_RealChart;

namespace SciChartExport
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HighFrequencyButton_Click(object sender, RoutedEventArgs e)
        {
            SetContent(new HighFreqChartView());
        }

        private void LotsOfSeriesButton_Click(object sender, RoutedEventArgs e)
        {
            SetContent(new LotsOfSeriesChartView());
        }

        private void FiftySeries(object sender, RoutedEventArgs e)
        {
            SetContent(new FiftySeries());
        }

        private void SetContent(UserControl control)
        {
            if (content.Content != null)
            {
                content.Content = null;
            }

            content.Content = control;
        }
    }
}

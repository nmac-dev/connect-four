/** MainWindow.xaml.cs */
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace connect_four {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        /** Variables */
        private readonly List<List<Border>> lsBorders;
        private readonly List<StackPanel> lsStackPanels;
        private readonly List<Border> lsTemp;

        /** Init */
        public MainWindow() {
            InitializeComponent();
            lsBorders = new List<List<Border>>();
            lsStackPanels = new List<StackPanel>();
            lsTemp = new List<Border>();
            foreach (StackPanel stackPanel in windowGrid.Children.OfType<StackPanel>()) {
                lsTemp = stackPanel.Children.OfType<Border>().ToList();
                lsTemp.Reverse();
                lsBorders.Add(lsTemp);
                lsStackPanels.Add(stackPanel);
            }
            ConnectFour.setupUI(lsBorders, lsStackPanels, brdCurrentPlayer, txbWinner);
        }

        /** Events */
        // Restart
        private void btnRestart_Click(object sender, RoutedEventArgs e) {
            ConnectFour.restartGame();
        }
        // A
        private void btnA_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(0);
        }
        // B
        private void btnB_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(1);
        }
        // C
        private void btnC_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(2);
        }
        // D
        private void btnD_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(3);
        }
        // E
        private void btnE_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(4);
        }
        // F
        private void btnF_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(5);
        }
        // G
        private void btnG_Click(object sender, RoutedEventArgs e) {
            ConnectFour.selectColumn(6);
        }
    }
}

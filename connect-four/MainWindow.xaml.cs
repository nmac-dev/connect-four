using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace connect_four {
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        /**      Const       */
        private const int 
            COLUMN_A = 0,
            COLUMN_B = 1,
            COLUMN_C = 2,
            COLUMN_D = 3,
            COLUMN_E = 4,
            COLUMN_F = 5,
            COLUMN_G = 6;

        private CFourLogic connectFour;     // Instantiated for each game

        /* Constructor */
        public MainWindow() {

            InitializeComponent();          
            List<Border>       lsTemp;
            List<List<Border>> lsBorders     = new List<List<Border>>();    // The borders within each stackpanel to display player selection
            List<StackPanel>   lsStackPanels = new List<StackPanel>();      // All stackpanels representing borders

            // Populates the lists with UI elements
            foreach (StackPanel stackPanel in windowGrid.Children.OfType<StackPanel>()) {

                lsTemp = stackPanel.Children.OfType<Border>().ToList();
                lsTemp.Reverse();
                lsBorders.Add(lsTemp);
                lsStackPanels.Add(stackPanel);
            }
            CFourLogic.setUIElements(lsBorders, lsStackPanels, brdCurrentPlayer, txbWinner);

            // Create new game
            connectFour = new CFourLogic();
        }

        /**      Events      */
        // Restart
        private void btnRestart_Click(object sender, RoutedEventArgs e) {

            // Instantiate new game
            connectFour = new CFourLogic();
            connectFour.resetUI();
        }
        // A
        private void btnA_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_A);
        }
        // B
        private void btnB_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_B);
        }
        // C
        private void btnC_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_C);
        }
        // D
        private void btnD_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_D);
        }
        // E
        private void btnE_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_E);
        }
        // F
        private void btnF_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_F);
        }
        // G
        private void btnG_Click(object sender, RoutedEventArgs e) {
            connectFour.checkUserInput(COLUMN_G);
        }
    }
}
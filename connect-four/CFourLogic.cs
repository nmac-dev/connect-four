/** ConnectFour.cs */
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace connect_four {
    /** Defines the UI manipulation and game logic */
    class CFourLogic {

        /*      Const       */
        private const int COLUMN_MAX = 7;
        private const int ROW_MAX = 6;
        private static readonly SolidColorBrush RGB_RED = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
        private static readonly SolidColorBrush RGB_YELLOW = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x00));
        private static readonly SolidColorBrush RGB_DARK = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        /*      UI Elements     */
        private static List<List<Border>> lsBorders;
        private static List<StackPanel> lsStackPanels;
        private static Border brdCurrentPlayer;
        private static TextBlock txbWinner;

        /*      Game Variables      */
        private bool currentPlayer = true;   // true = Red team, false = Yellow team
        
        /** Game grid */
        private readonly int[] columnCounter;      // Increments a column upon player selection
        private readonly int[,] arGameGrid;     // Tracks each teams selection
        /*   -----------------------
         * 5 | 05 15 25 35 45 55 65 |
         * 4 | 04 14 24 34 44 54 64 |
         * 3 | 03 13 23 33 43 53 63 | Rotated 90* anti-clockwise
         * 2 | 02 12 22 32 42 52 62 |
         * 1 | 01 11 21 31 41 51 61 |
         * 0 | 00 10 20 30 40 50 60 |
         *   -----------------------
         *     A  B  C  D  E  F  G
         */

        /** Constructor */
        public CFourLogic() {
            currentPlayer = true;
            columnCounter = new int[COLUMN_MAX];
            arGameGrid = new int[COLUMN_MAX, ROW_MAX];
        }

        /** Updates UI & arGameGrid */
        public void checkUserInput(int column) {
            int row = columnCounter[column];
            // Validate row is under max index
            if (row != ROW_MAX) {
                // Mark Red team selection on UI
                if (currentPlayer) {
                    arGameGrid[column, row] = 1;
                    updateUIGrid(column, columnCounter[column], RGB_RED);
                    brdCurrentPlayer.Background = RGB_YELLOW;
                // Mark Yellow team selection on UI
                } else {
                    arGameGrid[column, row] = 3;
                    updateUIGrid(column, columnCounter[column], RGB_YELLOW);
                    brdCurrentPlayer.Background = RGB_RED;
                }
                // Invoke game logic
                runGameLogic(column);
                columnCounter[column]++;
                toggleCurrentPlayer();
            // Column is full (Play error sound)
            } else {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        private void toggleCurrentPlayer() {
            currentPlayer = !currentPlayer;
        }

        /** Check each direction on the player's selection for a victory (4 in a line) */
        private void runGameLogic(int column) {

            int posX,                               // Position: X (column)
                posY,                               //           Y (row)
                sumScore,                           // adds up the current players score (4 = Red win, 12 = Yellow win)
                row = columnCounter[column];        // Marks the row the player selected

            bool player = currentPlayer;            // Compared to check if a grid placement belongs to the current player

            // Horizontal ( - )
            sumScore = 0;
            for (posX = 0; posX < COLUMN_MAX; posX++) {
                compareScore(arGameGrid[posX, row]);
            }
            // Vertical ( | )
            sumScore = 0;
            for (posY = 0; posY < ROW_MAX; posY++) {
                compareScore(arGameGrid[column, posY]);
            }
            // Diagonal Right ( / )
            sumScore = 0;
            for (findPositionLimit(false); posY < ROW_MAX; posY++, posX++) {
                if (posX < COLUMN_MAX && posY < ROW_MAX) {
                    compareScore(arGameGrid[posX, posY]);
                }
            }
            // Diagonal Left ( \ )
            sumScore = 0;
            for (findPositionLimit(true); posY < ROW_MAX; posY++, posX--) {
                if (posX > 0 && posY < ROW_MAX) {
                    compareScore(arGameGrid[posX, posY]);
                }
            }

            /** (Nested) Finds the lowest possible postion index from the game grid */
            void findPositionLimit(bool invert) {
                posX = column;
                posY = row;
                // Diagonal Right
                if (!invert) {
                    while (posX > 0 && posY > 0) {
                        posX--;
                        posY--;
                    }
                    // Diagonal Left
                } else {
                    while (posX < COLUMN_MAX - 1 && posY > 0) {
                        posX++;
                        posY--;
                    }
                }
            }

            /** (Nested) Compare the score against the index */
            void compareScore(int gridIndex) {
                bool identifyPlayer = gridIndex switch {
                    1 => true,      // Red
                    3 => false,     // Yellow
                    _ => !player    // Empty
                };
                // Validate current index is the same player
                player = identifyPlayer;
                if (player != currentPlayer) {
                    sumScore = 0;
                } else {
                    sumScore = sumScore + gridIndex;
                }
                // Check for a victory condition
                if (sumScore == 4) {
                    txbWinner.Text = "Red Team Wins!";
                    txbWinner.Visibility = Visibility.Visible;
                    enableUIButtons(false);
                } else if (sumScore == 12) {
                    txbWinner.Text = "Yellow Team Wins!";
                    txbWinner.Visibility = Visibility.Visible;
                    enableUIButtons(false);
                }
            }
        }

        /*      UIControls      */

        /** Brings the UI elements to the ConnectFour class */
        public static void getUIElements(List<List<Border>> lsBrd, List<StackPanel> lsBtn, Border brd, TextBlock txb) {
            lsBorders = lsBrd;
            lsStackPanels = lsBtn;
            brdCurrentPlayer = brd;
            txbWinner = txb;
        }
        
        /** Resets the main window (.xaml) elements */
        public void resetUI() {
            // Reset grid
            for (int i = 0; i < COLUMN_MAX; i++) {
                for (int j = 0; j < ROW_MAX; j++) {
                    updateUIGrid(i, j, RGB_DARK);
                }
            }
            // Reset player to default
            brdCurrentPlayer.Background = RGB_RED;
            txbWinner.Visibility = Visibility.Collapsed;
            enableUIButtons(true);
        }

        /** Sets the selected grid element to the specified color */
        public static void updateUIGrid(int innerIndex, int outerIndex, SolidColorBrush colorBrush) {
            lsBorders[innerIndex][outerIndex].Background = colorBrush;
        }

        /** Enables/disables UI buttons for team input */
        private static void enableUIButtons(bool isEnabled) {
            foreach(StackPanel stackPanel in lsStackPanels) {
                stackPanel.IsEnabled = isEnabled;
            }
        }
    }
}
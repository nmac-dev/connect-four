/** ConnectFour.cs */
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace connect_four {
    /**
     * Defines the UI manipulation and game logic
     */
    static class ConnectFour {

        /** Const */
        private static readonly int COLUMN_MAX = 7;
        private static readonly int ROW_MAX = 6;
        private static readonly SolidColorBrush RGB_RED = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00));
        private static readonly SolidColorBrush RGB_YELLOW = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x00));
        private static readonly SolidColorBrush RGB_DARK = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));

        /** UI Elements */
        private static List<List<Border>> lsBorders;
        private static List<StackPanel> lsStackPanels;
        private static Border brdCurrentPlayer;
        private static TextBlock txbWinner;

        /** Game Variables */
        private static int score, index;            // Arithmatic for game logic
        private static int posX, posY;              // Position: X (column) & Y (row)
        private static bool currentPlayer = true;   // true = Red team, false = Yellow team
        
        /** Game grid */
        private static readonly int[] columnCounter = new int[7];      // Increments a column upon player selection
        private static readonly int[,] arGameGrid = new int[7, 6];     // Tracks each teams selection
        /**
         *   -----------------------
         * 5 | 05 15 25 35 45 55 65 |
         * 4 | 04 14 24 34 44 54 64 |
         * 3 | 03 13 23 33 43 53 63 | Rotated 90* anti-clockwise
         * 2 | 02 12 22 32 42 52 62 |
         * 1 | 01 11 21 31 41 51 61 |
         * 0 | 00 10 20 30 40 50 60 |
         *   -----------------------
         *     A  B  C  D  E  F  G
         */

        /** Functions (#) */

        // # Brings the UI elements to the class
        public static void setupUI(List<List<Border>> lsBrd, List<StackPanel> lsBtn, Border brd, TextBlock txb) {
            lsBorders = lsBrd;
            lsStackPanels = lsBtn;
            brdCurrentPlayer = brd;
            txbWinner = txb;
        }

        // # Resets arGameGrid & UI Elements
        public static void restartGame() {
            // Reset grid
            for (posX = 0; posX < COLUMN_MAX; posX++) {
                for (posY = 0; posY < ROW_MAX; posY++) {
                    arGameGrid[posX, posY] = 0;
                    lsBorders[posX][posY].Background = RGB_DARK;
                }
                columnCounter[posX] = 0;
            }
            // Reset player to default
            currentPlayer = true;
            brdCurrentPlayer.Background = RGB_RED;
            txbWinner.Visibility = Visibility.Collapsed;
            enableButtons(true);
        }

        // # Main Window class -> ConnectFour class
        public static void playerSelected(int column) {
            updateUI(column, columnCounter[column]);
        }

        // # Update arGameGrid & UI borders
        private static void updateUI(int column, int row) {
            // Validate row is under max index
            if (columnCounter[column] != ROW_MAX) {
                // Mark Red team selection on UI
                if (currentPlayer) {
                    arGameGrid[column, row] = 1;
                    lsBorders[column][columnCounter[column]].Background = RGB_RED;
                    brdCurrentPlayer.Background = RGB_YELLOW;
                // Mark Yellow team selection on UI
                } else {
                    arGameGrid[column, row] = 3;
                    lsBorders[column][columnCounter[column]].Background = RGB_YELLOW;
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

        private static void toggleCurrentPlayer() {
            currentPlayer = !currentPlayer;
        }

        // # Check each direction on the player's selection for a victory (4 in a line)
        private static void runGameLogic(int column) {
            bool player = currentPlayer;
            int row = columnCounter[column];
            // Horizontal ( - )
            score = 0;
            for (posX = 0; posX < COLUMN_MAX; posX++) {
                index = arGameGrid[posX, row];
                compareScore();
            }
            // Vertical ( | )
            score = 0;
            for (posY = 0; posY < ROW_MAX; posY++) {
                index = arGameGrid[column, posY];
                compareScore();
            }
            // Diagonal Right ( / )
            score = 0;
            for (findPositionLimit(false); posY < ROW_MAX; posY++, posX++) {
                if (posX < COLUMN_MAX && posY < ROW_MAX) {
                    index = arGameGrid[posX, posY];
                    compareScore();
                }
            }
            // Diagonal Left ( \ )
            score = 0;
            for (findPositionLimit(true); posY < ROW_MAX; posY++, posX--) {
                if (posX > 0 && posY < ROW_MAX) {
                    index = arGameGrid[posX, posY];
                    compareScore();
                }
            }

            // # (Nested) Finds the lowest possible postion index from the game grid
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

            // # (Nested) Compare the score against the index
            void compareScore() {
                bool identifyPlayer = index switch {
                    1 => true,      // Red
                    3 => false,     // Yellow
                    _ => !player    // Empty
                };
                // Validate current index is the same player
                player = identifyPlayer;
                if (player != currentPlayer) {
                    score = 0;
                } else {
                    score = score + index;
                }
                // Check for a victory condition
                if (score == 4) {
                    txbWinner.Text = "Red Team Wins!";
                    txbWinner.Visibility = Visibility.Visible;
                    enableButtons(false);
                } else if (score == 12) {
                    txbWinner.Text = "Yellow Team Wins!";
                    txbWinner.Visibility = Visibility.Visible;
                    enableButtons(false);
                }
            }
        }

        // # Enables/disables UI buttons for team input
        private static void enableButtons(bool isEnabled) {
            foreach(StackPanel stackPanel in lsStackPanels) {
                stackPanel.IsEnabled = isEnabled;
            }
        }
    }
}
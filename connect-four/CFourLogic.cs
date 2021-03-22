using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace connect_four {
    /// <summary>
    ///     Defines the UI controls and game logic for a connect four game
    /// </summary>
    public class CFourLogic {

        /**     Const       */
        private const int 
            COLUMN_MAX = 7,
            ROW_MAX    = 6;

        private static readonly SolidColorBrush 
            RGB_RED    = new SolidColorBrush(Color.FromRgb(0xff, 0x00, 0x00)),
            RGB_YELLOW = new SolidColorBrush(Color.FromRgb(0xff, 0xff, 0x00)),
            RGB_DARK   = new SolidColorBrush(Color.FromRgb(0x33, 0x33, 0x33));


        /**     UI Elements     */
        private static List<List<Border>> lsBorders;
        private static List<StackPanel>   lsStackPanels;
        private static Border             brdCurrentPlayer;
        private static TextBlock          txbWinner;

        /**     Game Variables      */
        private enum Player {
            NONE,                                   // NONE must be first enum (defaults for grid population)
            RED,
            YELLOW
        }
        private Player             currentPlayer;   // Tracks the current player for each turn
        private readonly int[]     columnCounter;   // Increments a column upon player selection
        private readonly Player[,] arGameGrid;      // Tracks each teams selection
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

        /* Constructor */
        public CFourLogic() {

            currentPlayer = Player.RED;
            columnCounter = new int[COLUMN_MAX];
            arGameGrid    = new Player[COLUMN_MAX, ROW_MAX];
        }

        /* Updates UI & arGameGrid */
        public void checkUserInput(int column) {

            int row = columnCounter[column];
            // Validate row is under max index
            if (row != ROW_MAX) {

                // Update grid and UI with users input
                arGameGrid[column, row] = currentPlayer;
                updateUIGrid(column, row, currentPlayer);

                // Invoke game logic
                runGameLogic(column);
                columnCounter[column]++;
                currentPlayer = toggleCurrentPlayer(currentPlayer);
            }
            // Column is full (Play error sound)
            else
            {
                System.Media.SystemSounds.Beep.Play();
            }
        }

        /* Toggle UI to show which team is due to play next */
        private Player toggleCurrentPlayer(Player team) {
            
            if (team.Equals(Player.RED)) {
                team = Player.YELLOW;
                brdCurrentPlayer.Background = RGB_YELLOW;
            } 
            else {
                team = Player.RED;
                brdCurrentPlayer.Background = RGB_RED;
            }
            return team;
        }

        /* Check each direction on the player's selection for a victory (4 in a line) */
        private void runGameLogic(int column) {

            int posX,                               // Position: X (column)
                posY,                               //           Y (row)
                sumScore,                           // adds up the current players score (4 = Red win, 12 = Yellow win)
                row = columnCounter[column];        // Marks the row the player selected

            // Horizontal ( - )
            for (posX = resetScore(); posX < COLUMN_MAX; posX++)
                compareScore(arGameGrid[posX, row]);

            // Vertical ( | )
            for (posY = resetScore(); posY < ROW_MAX; posY++)
                compareScore(arGameGrid[column, posY]);

            // Diagonal Right ( / )
            for (findPositionLimit(false); posY < ROW_MAX; posY++, posX++) 
                
                if (posX < COLUMN_MAX && posY < ROW_MAX)
                    compareScore(arGameGrid[posX, posY]);

            // Diagonal Left ( \ )
            for (findPositionLimit(true); posY < ROW_MAX; posY++, posX--)

                if (posX > -1 && posY < ROW_MAX)
                    compareScore(arGameGrid[posX, posY]);

            int resetScore() {
                return sumScore = 0;
            }

            /** (Nested) Compare the score against the index */
            void compareScore(Player gridIndex)
            {

                // Increment score
                if (gridIndex.Equals(currentPlayer))
                    sumScore++;
                else
                    sumScore = 0;

                // Check for a victory condition
                if (sumScore == 4)
                    showUIWinner(currentPlayer);
            }

            /** (Nested) Finds the lowest possible postion index from the game grid */
            void findPositionLimit(bool invert) {

                resetScore();
                posX = column;
                posY = row;

                // Diagonal Right ( / )
                if (!invert)
                    while (posX > 0 && posY > 0) {
                        posX--;
                        posY--;
                    }

                // Diagonal Left ( \ )
                else
                    while (posX < COLUMN_MAX - 1 && posY > 0) {
                        posX++;
                        posY--;
                    }
            }
        }

        /**     UIControls      */

        /* Sets the UI elements for the UI control methods */
        public static void setUIElements(List<List<Border>> lsBrd, List<StackPanel> lsBtn, Border brd, TextBlock txb) {

            lsBorders        = lsBrd;
            lsStackPanels    = lsBtn;
            brdCurrentPlayer = brd;
            txbWinner        = txb;
        }
        
        /* Resets the main window (.xaml) elements */
        public void resetUI() {

            // Reset UI grid
            for (int i = 0; i < COLUMN_MAX; i++)
                for (int j = 0; j < ROW_MAX; j++)
                    updateUIGrid(i, j, Player.NONE);

            // Reset current player to default (red)
            brdCurrentPlayer.Background = RGB_RED;
            txbWinner.Visibility        = Visibility.Collapsed;
            enableUIButtons(true);
        }

        /* Sets the selected grid element to the specified color */
        private static void updateUIGrid(int innerIndex, int outerIndex, Player team) {
            
            SolidColorBrush selectColorBrush = team switch {
                Player.RED    => RGB_RED,
                Player.YELLOW => RGB_YELLOW,
                _             => RGB_DARK    // Default
            };

            lsBorders[innerIndex][outerIndex].Background = selectColorBrush;
        }

        /* Disables user input and updates UI with the games winner */
        private static void showUIWinner(Player team) {

            txbWinner.Text       = $"{team} Team Wins!";
            txbWinner.Visibility = Visibility.Visible;
            enableUIButtons(false);
        }

        /* Enables/disables UI buttons for team input */
        private static void enableUIButtons(bool isEnabled) {

            foreach(StackPanel stackPanel in lsStackPanels)
                stackPanel.IsEnabled = isEnabled;
        }
    }
}
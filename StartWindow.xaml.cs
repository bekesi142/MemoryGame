using System;
using System.Windows;
using System.Windows.Controls;

namespace MemoryGame
{
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Nehézségi szint kiválasztása
            string difficulty = (DifficultyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (difficulty == null)
            {
                MessageBox.Show("Kérlek válassz egy nehézséget!");
                return;
            }

            // Az új ablak indítása a kiválasztott nehézségi szinttel
            MainWindow mainWindow = new MainWindow(difficulty); // Kiválasztott nehézség átadása a MainWindow-nak
            mainWindow.Show();

            // Az aktuális ablak bezárása
            this.Close();
        }
    }
}

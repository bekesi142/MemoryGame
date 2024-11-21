using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MemoryGame
{
    public partial class MainWindow : Window
    {
        private List<int> cards = new List<int>(); // Kártyák listája
        private Button firstClicked = null; // Első kártya, amit felfordítunk
        private Button secondClicked = null; // Második kártya, amit felfordítunk
        
        private List<Button> cardButtons = new List<Button>(); // Minden kártya gombja
        private DispatcherTimer timer = new DispatcherTimer(); // Időzítő
        private int seconds = 0; // Másodpercek száma
        private bool isProcessing = false; // Segít elkerülni az újabb kattintásokat, amíg az előző választott kártyák nem kerülnek feldolgozásra

        public MainWindow(string difficulty)
        {
            InitializeComponent();
            SetDifficulty(difficulty);
            timer.Interval = TimeSpan.FromSeconds(1); // 1 másodpercenként frissít
            timer.Tick += Timer_Tick; // Időzítő esemény kezelőjének hozzárendelése
            
        }

        private void SetDifficulty(string difficulty)
        {
            int numPairs;

            switch (difficulty)
            {
                case "Könnyű":
                    numPairs = 4; // 4 pár kártya
                    break;
                case "Közepes":
                    numPairs = 6; // 6 pár kártya
                    break;
                case "Nehéz":
                    numPairs = 10; // 10 pár kártya
                    break;
                case "Borzalmasan Nehéz":
                    numPairs = 16; // 16 pár kártya
                    break;
                default:
                    MessageBox.Show("Kérlek válassz nehézséget!");
                    return;
            }

            // Új játék indítása
            NewGame(numPairs);
        }

        // Új játék inicializálása
        private void NewGame(int numPairs)
        {
            // Idő visszaállítása
            seconds = 0;
            TimerText.Text = "Idő: 0 másodperc";

            // Kártyák előkészítése
            cards.Clear();
            cardButtons.Clear();
            CardGrid.Children.Clear();

            // Hozz létre párokat a kiválasztott nehézségi szintnek megfelelően
            for (int i = 1; i <= numPairs; i++)
            {
                cards.Add(i);
                cards.Add(i);
            }

            // Keverd meg a kártyákat
            Random rng = new Random();
            cards = cards.OrderBy(card => rng.Next()).ToList();

            // Kártyák elhelyezése a Grid-ben
            int gridSize = (int)Math.Sqrt(cards.Count);
            CardGrid.Rows = gridSize;
            CardGrid.Columns = gridSize;

            for (int i = 0; i < cards.Count; i++)
            {
                Button cardButton = new Button
                {
                    Name = "Card" + i,
                    Content = "?",
                    FontSize = 24,
                    Width = 50,
                    Height = 50,
                    Margin = new Thickness(5),
                    Background = System.Windows.Media.Brushes.LightSeaGreen
                };

                cardButton.Click += CardButton_Click; // Kattintás esemény hozzárendelése
                cardButtons.Add(cardButton);
                CardGrid.Children.Add(cardButton);
            }

            // Indítsd el az időzítőt
            timer.Start();
        }

        // Kattintás a kártyákra
        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (isProcessing) return; // Ha éppen feldolgozunk két kártyát, ne engedjünk újabb kattintásokat.

            Button clickedButton = sender as Button;

            // Ne csinálj semmit, ha már meg van találva a pár
            if (clickedButton.Content.ToString() != "?")
                return;

            int cardIndex = cardButtons.IndexOf(clickedButton);
            int cardValue = cards[cardIndex];

            // Kattintás hatására felfordítjuk a kártyát
            clickedButton.Content = cardValue.ToString();

            if (firstClicked == null)
            {
                firstClicked = clickedButton; // Első kártya
            }
            else if (secondClicked == null)
            {
                secondClicked = clickedButton; // Második kártya
                isProcessing = true; // Megkezdjük a párok ellenőrzését
                CheckForMatch();
            }
        }

        // Két kártya összehasonlítása
        private void CheckForMatch()
        {
            int firstValue = int.Parse(firstClicked.Content.ToString());
            int secondValue = int.Parse(secondClicked.Content.ToString());

            if (firstValue == secondValue)
            {
                // Ha megegyeznek, akkor ne csinálj semmit, a kártyák felfordítva maradnak
                firstClicked = null;
                secondClicked = null;
                isProcessing = false;
                CheckForWin();
            }
            else
            {
                // Ha nem egyeznek, akkor 1 másodperc múlva visszafordítjuk őket
                DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
                timer.Tick += (sender, e) =>
                {
                    firstClicked.Content = "?";
                    secondClicked.Content = "?";
                    firstClicked = null;
                    secondClicked = null;
                    isProcessing = false;
                    timer.Stop();
                };
                timer.Start();
            }
        }

        // Nyertél-e?
        private void CheckForWin()
        {
            if (cardButtons.All(button => button.Content.ToString() != "?"))
            {
                timer.Stop();
                MessageBox.Show($"Gratulálok! Kész vagy! {seconds} másodperc alatt.");
            }
        }

        // Időzítő frissítése
        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            TimerText.Text = $"Idő: {seconds} másodperc";
        }

        // Vissza a kezdőképernyőre
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            StartWindow startWindow = new StartWindow();
            startWindow.Show();
            this.Close();
        }
    }
}

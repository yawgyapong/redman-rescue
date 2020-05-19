using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;             // With this statement, we can use anmation code from the .NET framework to move the aliens on the screen
using System.Windows.Threading;


namespace Redman_Rescue
{
    ///  Main aim -- Drag Redman to safety in the bluish-black portal
    ///  Dragging Redman against an alien or into the black zones means Game Over
    ///  You must move when you click on Start because the timer begins right away
    ///  Wasting time also means Game Over

    public partial class MainWindow : Window
    {
        Random random = new Random();     
        DispatcherTimer alienTimer = new DispatcherTimer();   // Here is our alien timer. Using DispatcherTimer() class
        DispatcherTimer progressTimer = new DispatcherTimer();
        bool redmanCaptured = false;      // At the begining of game, human is not captured yet
     

        public MainWindow()
        {
            this.InitializeComponent();

            alienTimer.Tick += alienTimer_Tick;           //  the number of aliens every two seconds 
            alienTimer.Interval = TimeSpan.FromSeconds(2);

            progressTimer.Tick += progressTimer_Tick;
            progressTimer.Interval = TimeSpan.FromSeconds(.1); // Increasing the progress bar count every 0.1th second
            gameOverText.Visibility = Visibility.Collapsed;   // Hiding Game Over when the game starts initially
        }

        private void progressTimer_Tick(object sender, EventArgs e)
        {
            progressBar.Value += 1;

            if (progressBar.Value >= progressBar.Maximum) //Ending the game if player uses up his time and the progress bar is full
            {
                EndTheGame();
            }
        }

        private void alienTimer_Tick(object sender, EventArgs e)
        {
            Addalien();      // Function to add more aliens
        }

        private void EndTheGame()
        {
            if (!playArea.Children.Contains(gameOverText))   // Stopping the game when Game over shows up, Giving the user a chance to start again
            {
                alienTimer.Stop();
                progressTimer.Stop();
                redmanCaptured = false;
                startButton.Visibility = Visibility.Visible;
                playArea.Children.Add(gameOverText);
                gameOverText.Visibility = Visibility.Visible;     // Revealing Game Over when the user loses
            }
        }

        private void startButton_Click(object sender, RoutedEventArgs e) // Starting the game when the player presses the start button
        {
            StartGame();
        }

        private void StartGame()
        {
            redman.IsHitTestVisible = true;                
            redmanCaptured = false;
            progressBar.Value = 0;                                    // Empty the progress bar
            startButton.Visibility = Visibility.Collapsed;            // Hide the Start button when the game starts
            playArea.Children.Clear();                                // Clearing the canvas
            playArea.Children.Add(target);
            playArea.Children.Add(redman);
            alienTimer.Start();            // Start the alien timer and add more aliens according to the specified time
            progressTimer.Start();         // Start the progress bar and increase the value according to the specified time
        }

        private void Addalien()
        {
            ContentControl alien = new ContentControl();
            alien.Template = Resources["alienTemplate"] as ControlTemplate;                   

            AnimateAlien(alien, 0, playArea.ActualWidth - 100, "(Canvas.Left)");

            AnimateAlien(alien, random.Next((int)playArea.ActualHeight) - 100,                 
                random.Next((int)playArea.ActualHeight) - 100, "(Canvas.Top)");

            playArea.Children.Add(alien);

            alien.MouseEnter += Alien_MouseEnter;
        }

        private void Alien_MouseEnter(object sender, MouseEventArgs e)
        {
            if (redmanCaptured)
                EndTheGame();
        }

        private void AnimateAlien(ContentControl alien, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard() { AutoReverse = true, RepeatBehavior = RepeatBehavior.Forever };   // Creating storyboard object from the Storyboard class
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6)))
            };
            Storyboard.SetTarget(animation, alien);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();
        }

        private void redman_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (alienTimer.IsEnabled)
            {
                redmanCaptured = true;
                redman.IsHitTestVisible = false;
            }
        }

        private void target_MouseEnter(object sender, MouseEventArgs e)
        {
            if (progressTimer.IsEnabled && redmanCaptured)
            {
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)playArea.ActualWidth - 100));    //Returns a non-negative random integer
                Canvas.SetTop(target, random.Next(100, (int)playArea.ActualHeight - 100));
                Canvas.SetLeft(redman, random.Next(100, (int)playArea.ActualWidth - 100));
                Canvas.SetTop(redman, random.Next(100, (int)playArea.ActualHeight - 100));
                redmanCaptured = false;
                redman.IsHitTestVisible = true;
            }
        }

        private void playArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (redmanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArea).Transform(pointerPosition);

                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(redman)) > redman.ActualWidth * 3)
                    || (Math.Abs(relativePosition.Y - Canvas.GetTop(redman)) > redman.ActualHeight * 3))
                {
                    redmanCaptured = false;
                    redman.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(redman, relativePosition.X - redman.ActualWidth / 2);
                    Canvas.SetTop(redman, relativePosition.Y - redman.ActualHeight / 2);
                }
            }
        }

        private void playArea_MouseLeave(object sender, MouseEventArgs e)
        {
            if (redmanCaptured)
            {
                EndTheGame();
            }
        }
    }
}
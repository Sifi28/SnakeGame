using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;


namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Brush customColor;
        Brush FoodColour;
        bool goRight, goUp,goDown = false;
        bool goLeft = true;
        int SnakeSpeed = 8;
        List<Rectangle> FoodToRemove = new List<Rectangle>();
        List<Rectangle> BodyParts = new List<Rectangle>();
        int count=0;
        Random RandomFood = new Random();
        DispatcherTimer GameTimer = new DispatcherTimer();
        
        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus();

            GameTimer.Tick += GameTimer_Event;
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);
            GameTimer.Start();
        }

        /// <summary>
        /// ReStart the game again 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Game(object sender, RoutedEventArgs e)
        {
            GameTimer.Start();
            status.Content = "Game Start";
            Canvas.SetLeft(snakeHead, 200);
            Canvas.SetTop(snakeHead, 300);
        }

        /// <summary>
        /// Starting the Game 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameTimer_Event(object? sender, EventArgs e)
        {
            double headX = Canvas.GetLeft(snakeHead);
            double headY = Canvas.GetTop(snakeHead);

            #region Stoping Condition for Game
            if (Canvas.GetLeft(snakeHead) == 0 || Canvas.GetTop(snakeHead) < 0 || 
                Canvas.GetLeft(snakeHead) + (snakeHead.Width) == MyCanvas.ActualWidth ||
                Canvas.GetTop(snakeHead) + snakeHead.Height > MyCanvas.ActualHeight)
            {
                status.Content = "Game Over";
                GameTimer.Stop();
            }
            #endregion

            #region Moving Condition for Snake
            if (goLeft && Canvas.GetLeft(snakeHead) > 0)
            {
                Canvas.SetLeft(snakeHead, Canvas.GetLeft(snakeHead) - SnakeSpeed);
            }
            else if (goRight && Canvas.GetLeft(snakeHead) + snakeHead.Width < MyCanvas.ActualWidth)
            {
                //snakeHead = snakeTail;
                Canvas.SetLeft(snakeHead, Canvas.GetLeft(snakeHead) + SnakeSpeed);
            }
            else if (goUp && Canvas.GetTop(snakeHead) > 0)
            {
                Canvas.SetTop(snakeHead, Canvas.GetTop(snakeHead) - SnakeSpeed);
            }
            else if (goDown && Canvas.GetTop(snakeHead) + snakeHead.Height < MyCanvas.ActualHeight)
            {
                Canvas.SetTop(snakeHead, Canvas.GetTop(snakeHead) + SnakeSpeed);
            }
            #endregion

            double Prevx = headX;
            double Prevy = headY;

            #region Counting Body Parts

            List<Rectangle> BodyForinteracion = new List<Rectangle>();

            foreach (var b in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)b.Tag == "body")
                {
                    count++;
                    BodyForinteracion.Add(b);
                }
            }

            #endregion

            #region Condition For Body Parts of Snake

            foreach (var b in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)b.Tag == "body")
                {
                    Rect Snake = new Rect(Canvas.GetLeft(snakeHead), Canvas.GetTop(snakeHead), snakeHead.Width, snakeHead.Height);
                    Rect OwnBody = new Rect(Canvas.GetLeft(BodyForinteracion[count-1]), Canvas.GetTop(BodyForinteracion[count - 1]), BodyForinteracion[count - 1].Width, BodyForinteracion[count - 1].Height);
                    if(Snake.IntersectsWith(OwnBody) && MyCanvas.Children.Count > 30)
                    {
                        status.Content = "Game Over";
                        GameTimer.Stop();
                    }
                    double Currentx = Canvas.GetLeft(b);
                    double Currenty = Canvas.GetTop(b);

                    Canvas.SetLeft(b, Prevx);
                    Canvas.SetTop(b, Prevy);

                    Prevx = Currentx;
                    Prevy = Currenty;
                }
            }

            #endregion

            List<Rectangle> newBodyParts = new List<Rectangle>();
            List<Rectangle> NewFood = new List<Rectangle>();

            #region Food Condition 
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if((string)x.Tag == "food")
                {
                    x.Stroke = Brushes.Black;
                    Rect Snake = new Rect(Canvas.GetLeft(snakeHead),Canvas.GetTop(snakeHead), snakeHead.Width, snakeHead.Height);
                    Rect foods = new Rect(Canvas.GetLeft(x),Canvas.GetTop(x),x.Width,x.Height);
                    if(Snake.IntersectsWith(foods))
                    {
                        FoodToRemove.Add(x);
                        customColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF22A10A"));
                        FoodColour = new SolidColorBrush (Colors.Red);
                        Rectangle NewRactangle = new Rectangle
                        {
                            Tag = "body",
                            Fill = customColor,
                            Width = 20,
                            Height =20
                        };
                        Canvas.SetLeft(NewRactangle, 0);
                        Canvas.SetTop(NewRactangle, 0);

                        newBodyParts.Add(NewRactangle);

                        Rectangle newFoodItems = new Rectangle
                        {
                            Tag = "food",
                            Fill = FoodColour,
                            Width = 20,
                            Height = 20
                        };
                        Canvas.SetLeft(newFoodItems, RandomFood.Next(20,558));
                        Canvas.SetTop(newFoodItems, RandomFood.Next(20,578));

                        NewFood.Add(newFoodItems);
                        
                    }
                }
            }
            #endregion

            foreach (var item in FoodToRemove)
            {
                MyCanvas.Children.Remove(item);
            }

            foreach (var parts in newBodyParts)
            {
               MyCanvas.Children.Add(parts);
            }
            foreach (var food in NewFood)
            {
                MyCanvas.Children.Add(food);
            }
            count = 0;
        }
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
                goRight = false;
                goUp = false;
                goDown = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
                goLeft = false;
                goUp = false;
                goDown = false;
            }
            if (e.Key == Key.Up)
            {
                goUp = true;
                goDown = false;
                goLeft = false;
                goRight = false;
            }
            if (e.Key == Key.Down)
            {
                goDown = true;
                goLeft = false;
                goRight = false;
                goUp = false;
            }
        }
    } 
}
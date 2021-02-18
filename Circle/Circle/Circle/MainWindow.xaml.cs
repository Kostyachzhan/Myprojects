using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Circle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// Масив списків обєктів квадратів
        private List<List<Rectangle>> pixels;
        private List<Tuple<int,int>> circle_points;
        /// координати кола 
        private int circle_center_x;
        private int circle_center_y;
        private int circle_radius;
        /// координати лінії 
        private int line_start_x;
        private int line_start_y;
        private int line_end_x;
        private int line_end_y;

        private object sync_root = new object();

        public MainWindow()
        {
            InitializeComponent();
            InitializeView();
        }
        /// функция ініціалізації  View
        private void InitializeView()
        { 
            /// якщо пікселі дорівнюють нулю
            if(pixels != null)
            {
                ///назначаємо їх 0
                pixels = null;
                ///очищуємо ввсе View від пікселів які добавляли
                main_view.Children.Clear();
            }
            pixels = new List<List<Rectangle>>();
            for(int i = 0; i < 32; ++i)
            {
                var tmp_list = new List<Rectangle>();
                for(int j = 0; j < 32; ++j)
                {
                    Rectangle rectangle = new Rectangle();
                    rectangle.Fill = new SolidColorBrush(Colors.Green);
                    rectangle.Tag = "<blank>";
                    main_view.Children.Add(rectangle);
                    tmp_list.Add(rectangle);
                }
                ///добавляємо в пікселі
                pixels.Add(tmp_list);
            }
        }
        ///обробник подіії який вікликається при натиску на кнопку
        private void clear_Click(object sender, RoutedEventArgs e)
        {
            InitializeView();
            circle_center_x_input.Text = "Enter X of circle center";
            circle_center_y_input.Text = "Enter Y of circle center";
            demo_mode.IsEnabled = false;
            main_mode.IsEnabled = false;
            status.Text += "\r\nSystem status...CLEARED\r\nInput parameters...CLEARED\r\n\r\nAwaiting input parameters...";
            start_x.Text = "Enter X of starting point";
            start_y.Text = "Enter Y of starting point";
            end_x.Text = "Enter X of ending point";
            end_y.Text = "Enter Y of ending point";
            clear.IsEnabled = false;
        }
        ///функція потрібна для того що перевірити чи не виходить x,y за рамки
        private bool CheckCircleCoords(int x, int y)
        {
            if(x > 5 && x < 27)
            {
                if(y > 5 && y < 27)
                {
                    return true;
                }
            }
            return false;
        }

        private void GenerateCircle(int center_x, int center_y)
        {
            status.Text += $"\r\nGenerating circle data...";
            ///вибираємо центр кола 
            int radius = -1;
            int from_top = center_y;
            status.Text += $"\r\nRadius from top of view -> {from_top}";
            int from_bottom = 31 - center_y;
            status.Text += $"\r\nRadius from bottom of view -> {from_bottom}";
            int from_left = center_x;
            status.Text += $"\r\nRadius from left of view -> {from_left}";
            int from_right = 31 - center_y;
            status.Text += $"\r\nRadius from right of view -> {from_right}";
            ///зрівнюємо 2 Math.Min, вибираємо найменше, для того щоб круг не виходив за межі гріду
            radius = Math.Min(Math.Min(from_top, from_bottom), Math.Min(from_left, from_right))-1; //TODO: bottom border isn't working correctly
            status.Text += $"\r\nComputed acceptable radius -> {radius}";
            ///вибираємо як радіус
            circle_radius = radius;

            circle_points = new List<Tuple<int,int>>();
            int x = radius, y = 0;

            ///алгоритм, який перевіряє точки які генеруються, щоб вони були в рамках  кола
            // Printing the initial point on the 
            // axes after translation 
            status.Text += $"\r\nPlacing point at coordiantes -> ({x + center_x}, {y + center_y})";
            pixels[x+center_x][y+center_y].Fill = new SolidColorBrush(Colors.White);
            pixels[x + center_x][y + center_y].Tag = "<circle>";
            circle_points.Add(new Tuple<int, int>(x + center_x, y + center_y));

            status.Text += $"\r\nPlacing point at coordiantes -> ({center_x - radius}, {center_y})";
            pixels[center_x - radius][center_y].Fill = new SolidColorBrush(Colors.White);
            pixels[center_x - radius][center_y].Tag = "<circle>";
            circle_points.Add(new Tuple<int, int>(center_x - radius, center_y));

            status.Text += $"\r\nPlacing point at coordiantes -> ({center_x + radius}, {center_y})";
            pixels[center_x + radius][center_y].Fill = new SolidColorBrush(Colors.White);
            pixels[center_x + radius][center_y].Tag = "<circle>";
            circle_points.Add(new Tuple<int, int>(center_x + radius, center_y));

            status.Text += $"\r\nPlacing point at coordiantes -> ({center_x}, {center_y-radius})";
            pixels[center_x][center_y - radius].Fill = new SolidColorBrush(Colors.White);
            pixels[center_x][center_y - radius].Tag = "<circle>";
            circle_points.Add(new Tuple<int, int>(center_x, center_y-radius));

            status.Text += $"\r\nPlacing point at coordiantes -> ({center_x}, {center_y + radius})";
            pixels[center_x][center_y + radius].Fill = new SolidColorBrush(Colors.White);
            pixels[center_x][center_y + radius].Tag = "<circle>";
            circle_points.Add(new Tuple<int, int>(center_x, center_y + radius));


            // When radius is zero only a single 
            // point will be printed 
            if (radius > 0)
            {
                status.Text += $"\r\nPlacing point at coordiantes -> ({x + center_x}, {-y + center_y})";
                pixels[x + center_x][-y + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[x + center_x][-y + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(x + center_x, -y + center_y));

                status.Text += $"\r\nPlacing point at coordiantes -> ({y + center_x}, {x + center_y})";
                pixels[y + center_x][x + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[y + center_x][x + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(y + center_x, x + center_y));

                status.Text += $"\r\nPlacing point at coordiantes -> ({-y + center_x}, {x + center_y})";
                pixels[-y + center_x][x + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[-y + center_x][x + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(-y + center_x, x + center_y));
            }

            // Initialising the value of P 
            int P = 1 - radius;
            while (x > y)
            {

                y++;

                // Mid-point is inside or on the perimeter 
                if (P <= 0)
                    P = P + 2 * y + 1;

                // Mid-point is outside the perimeter 
                else
                {
                    x--;
                    P = P + 2 * y - 2 * x + 1;
                }

                // All the perimeter points have already  
                // been printed 
                if (x < y)
                    break;

                // Printing the generated point and its  
                // reflection in the other octants after 
                // translation 
                status.Text += $"\r\nPlacing point at coordiantes -> ({x + center_x}, {y + center_y})";
                pixels[x + center_x][y + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[x + center_x][y + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(x + center_x, y + center_y));

                status.Text += $"\r\nPlacing point at coordiantes -> ({-x + center_x}, {y + center_y})";
                pixels[-x + center_x][y + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[-x + center_x][y + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(-x + center_x, y + center_y));

                status.Text += $"\r\nPlacing point at coordiantes -> ({x + center_x}, {-y + center_y})";
                pixels[x + center_x][-y + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[x + center_x][-y + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(x + center_x, -y + center_y));

                status.Text += $"\r\nPlacing point at coordiantes -> ({-x + center_x}, {-y + center_y})";
                pixels[-x + center_x][-y + center_y].Fill = new SolidColorBrush(Colors.White);
                pixels[-x + center_x][-y + center_y].Tag = "<circle>";
                circle_points.Add(new Tuple<int, int>(-x + center_x, -y + center_y));

                // If the generated point is on the  
                // line x = y then the perimeter points 
                // have already been printed 
                if (x != y)
                {
                    status.Text += $"\r\nPlacing point at coordiantes -> ({y + center_x}, {x + center_y})";
                    pixels[y + center_x][x + center_y].Fill = new SolidColorBrush(Colors.White);
                    pixels[y + center_x][x + center_y].Tag = "<circle>";
                    circle_points.Add(new Tuple<int, int>(y + center_x, x + center_y));

                    status.Text += $"\r\nPlacing point at coordiantes -> ({-y + center_x}, {x + center_y})";
                    pixels[-y + center_x][x + center_y].Fill = new SolidColorBrush(Colors.White);
                    pixels[-y + center_x][x + center_y].Tag = "<circle>";
                    circle_points.Add(new Tuple<int, int>(-y + center_x, x + center_y));

                    status.Text += $"\r\nPlacing point at coordiantes -> ({y + center_x}, {-x + center_y})";
                    pixels[y + center_x][-x + center_y].Fill = new SolidColorBrush(Colors.White);
                    pixels[y + center_x][-x + center_y].Tag = "<circle>";
                    circle_points.Add(new Tuple<int, int>(y + center_x, -x + center_y));

                    status.Text += $"\r\nPlacing point at coordiantes -> ({-y + center_x}, {-x + center_y})";
                    pixels[-y + center_x][-x + center_y].Fill = new SolidColorBrush(Colors.White);
                    pixels[-y + center_x][-x + center_y].Tag = "<circle>";
                    circle_points.Add(new Tuple<int, int>(-y + center_x, -x + center_y));
                }
            }
            clear.IsEnabled = true;
        }
        ///перевыряэмо чи ыныцыалызований наш View по замовчуванні
        private bool IsViewEmpty()
        {
            int count = 0;
            foreach(var p in pixels)
            {
                foreach(var pixel in p)
                {
                    ///якщо тег не рівняється бланк
                    if ((pixel.Tag as string) != "<blank>")
                    {
                        count++;
                        if (count > 1)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void circle_center_x_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            ///перевіряємо чи вона існує
            if (status != null)
            {
                ///перевіряємо щоб текст існував
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {circle_center_x_input.Text}\r\nValidating...";
                    int x = -1;
                    int y = -1;
                    ///якщо получається витягти з тексту який ми ввели, якесь число значить йдем дальше
                    if (int.TryParse(circle_center_x_input.Text, out x))
                    {
                        if (int.TryParse(circle_center_y_input.Text, out y))
                        {
                            ///якщо координати в потрібній межі
                            if (CheckCircleCoords(x, y))
                            {
                                ///виводим статус
                                status.Text += "\r\nPASSED!";
                                ///запуск кнопки
                                demo_mode.IsEnabled = true;
                                circle_center_x = x;
                                circle_center_y = y;

                                pixels[x][y].Fill = new SolidColorBrush(Colors.White);
                                pixels[x][y].Tag = "<circle>";
                                GenerateCircle(circle_center_x, circle_center_y);
                                return;
                            }
                            else
                            {
                                status.Text += "\r\nPOINT IS OUT OF BONDS!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            demo_mode.IsEnabled = false;
        }

        private void circle_center_y_input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (status != null)
            {
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {circle_center_y_input.Text}\r\nValidating...";
                    int x = -1;
                    int y = -1;
                    if (int.TryParse(circle_center_x_input.Text, out x))
                    {
                        if (int.TryParse(circle_center_y_input.Text, out y))
                        {
                            if (CheckCircleCoords(x, y))
                            {
                                status.Text += "\r\nPASSED!";
                                demo_mode.IsEnabled = true;
                                circle_center_x = x;
                                circle_center_y = y;

                                pixels[x][y].Fill = new SolidColorBrush(Colors.White);
                                pixels[x][y].Tag = "<circle>";
                                GenerateCircle(circle_center_x, circle_center_y);
                                return;
                            }
                            else
                            {
                                status.Text += "\r\nPOINT IS OUT OF BONDS!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            demo_mode.IsEnabled = false;
        }

        private void DDA(int start_x, int start_y, int end_x, int end_y, bool isDemoMode)
        {
            Color color = default;

            if(isDemoMode)
            {
                color = Colors.Black;
            }
            else
            {
                color = Colors.Red;
            }
            // вибираємо дельту 
            int delta_x = end_x - start_x;
            int delta_y = end_y - start_y;

            // вичислямо кількість кроків,  яку потрібно досягти від нашої начальної точки до кінцевої 
            int steps = Math.Abs(delta_x) > Math.Abs(delta_y) ? Math.Abs(delta_x) : Math.Abs(delta_y);

            //  вибираємо інкремент x, y , будемо їх збільшувати
            float Xinc = delta_x / (float)steps;
            float Yinc = delta_y / (float)steps;

            // дані про те куди будемо класти нашу точку
            float X = start_x;
            float Y = start_y;
            for (int i = 0; i <= steps; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    lock (sync_root)
                    {
                        pixels[(int)X][(int)Y].Fill = new SolidColorBrush(color);
                    }
                });
                X += Xinc;           // increment in x at each step 
                Y += Yinc;           // increment in y at each step 
                Thread.Sleep(100);
            }
        }

        /// перевіряє чи точка знаходиться в середні кола і не виходить за його межі
        private bool IsInside(int circle_x, int circle_y,
                              int rad, int x, int y)
        {
            if ((x - circle_x) * (x - circle_x) +
                (y - circle_y) * (y - circle_y) <= rad * rad)
                return true;
            else
                return false;
        }

        /// чи існує створена точка в списку координат
        private bool IsExist(List<Tuple<int, int>> points, int x, int y)
        {
            foreach(var p in points)
            {
                if(p.Item1 == x && p.Item2 == y)
                {
                    return true;
                }
            }
            return false;
        }

        private void demo_mode_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random((int)DateTime.UtcNow.Ticks);
            List<Tuple<int, int>> ending_points = new List<Tuple<int, int>>();
            ///перевіряємо чи поставили галочку в чекбоксі 
            ///чи має значення
            if(line_mode.IsChecked.HasValue)
            {
                ///отримуєма значення
                if (line_mode.IsChecked.Value)
                {
                    ///отримуємо точки круга
                    ///список унікальних точок круга
                    var distinct = circle_points.Distinct().ToList();
                    for (int i = 0; i < 16; ++i)
                    {
                        ///вибираємо рандомні точки круга
                        ending_points.Add(distinct[random.Next(0, distinct.Count)]);
                    }
                }
            }
            
            ///якщо нічого не добавили, кількість точок менше нуля, то попередній іф не відробив
            if(ending_points.Count <= 0)
            {
                for (int i = 0; i < 16; ++i)
                {
                    int x = 32;
                    int y = 32;
                    do
                    {
                        x = random.Next(0, 32);
                        y = random.Next(0, 32);
                    }
                    while (!IsInside(circle_center_x, circle_center_y, circle_radius, x, y) || IsExist(ending_points, x, y));
                    ending_points.Add(new Tuple<int, int>(x, y));
                }
            }

            foreach(var p in ending_points)
            {
                new Thread(() =>
                {
                    DDA(circle_center_x, circle_center_y, p.Item1, p.Item2, true);
                }).Start();
            }
            clear.IsEnabled = true;
        }

        ///чи не виходить за межі гріда
        private bool CheckCoords(int starting_x, int starting_y, int ending_x, int ending_y)
        {
            if (starting_x >= 0 && starting_x < 32)
            {
                if (starting_y >= 0 && starting_y < 32)
                {
                    if (ending_x >= 0 && ending_x < 32)
                    {
                        if (ending_y >= 0 && ending_y < 32)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void start_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (status != null)
            {
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {start_x.Text}\r\nValidating...";
                    int starting_x = -1;
                    int starting_y = -1;
                    int ending_x = -1;
                    int ending_y = -1;
                    if (int.TryParse(start_x.Text, out starting_x))
                    {
                        if (int.TryParse(start_y.Text, out starting_y))
                        {
                            if (int.TryParse(end_x.Text, out ending_x))
                            {
                                if (int.TryParse(end_y.Text, out ending_y))
                                {
                                    if (CheckCoords(starting_x, starting_y, ending_x, ending_y))
                                    {
                                        status.Text += "\r\nPASSED!";
                                        main_mode.IsEnabled = true;

                                        pixels[starting_x][starting_y].Fill = new SolidColorBrush(Colors.Red);
                                        pixels[starting_x][starting_y].Tag = "<line>";

                                        line_start_x = starting_x;
                                        line_start_y = starting_y;
                                        line_end_x = ending_x;
                                        line_end_y = ending_y;
                                        return;
                                    }
                                    else
                                    {
                                        status.Text += "\r\nPOINT IS OUT OF BONDS!";
                                    }
                                }
                                else
                                {
                                    status.Text += "\r\nERROR!";
                                }
                            }
                            else
                            {
                                status.Text += "\r\nERROR!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            main_mode.IsEnabled = false;
        }

        private void start_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (status != null)
            {
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {start_y.Text}\r\nValidating...";
                    int starting_x = -1;
                    int starting_y = -1;
                    int ending_x = -1;
                    int ending_y = -1;
                    if (int.TryParse(start_x.Text, out starting_x))
                    {
                        if (int.TryParse(start_y.Text, out starting_y))
                        {
                            if (int.TryParse(end_x.Text, out ending_x))
                            {
                                if (int.TryParse(end_y.Text, out ending_y))
                                {
                                    if (CheckCoords(starting_x, starting_y, ending_x, ending_y))
                                    {
                                        status.Text += "\r\nPASSED!";
                                        main_mode.IsEnabled = true;

                                        pixels[starting_x][starting_y].Fill = new SolidColorBrush(Colors.Red);
                                        pixels[starting_x][starting_y].Tag = "<line>";

                                        line_start_x = starting_x;
                                        line_start_y = starting_y;
                                        line_end_x = ending_x;
                                        line_end_y = ending_y;
                                        return;
                                    }
                                    else
                                    {
                                        status.Text += "\r\nPOINT IS OUT OF BONDS!";
                                    }
                                }
                                else
                                {
                                    status.Text += "\r\nERROR!";
                                }
                            }
                            else
                            {
                                status.Text += "\r\nERROR!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            main_mode.IsEnabled = false;
        }

        private void end_x_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (status != null)
            {
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {end_x.Text}\r\nValidating...";
                    int starting_x = -1;
                    int starting_y = -1;
                    int ending_x = -1;
                    int ending_y = -1;
                    if (int.TryParse(start_x.Text, out starting_x))
                    {
                        if (int.TryParse(start_y.Text, out starting_y))
                        {
                            if (int.TryParse(end_x.Text, out ending_x))
                            {
                                if (int.TryParse(end_y.Text, out ending_y))
                                {
                                    if (CheckCoords(starting_x, starting_y, ending_x, ending_y))
                                    {
                                        status.Text += "\r\nPASSED!";
                                        main_mode.IsEnabled = true;

                                        pixels[starting_x][starting_y].Fill = new SolidColorBrush(Colors.Red);
                                        pixels[starting_x][starting_y].Tag = "<line>";

                                        line_start_x = starting_x;
                                        line_start_y = starting_y;
                                        line_end_x = ending_x;
                                        line_end_y = ending_y;
                                        return;
                                    }
                                    else
                                    {
                                        status.Text += "\r\nPOINT IS OUT OF BONDS!";
                                    }
                                }
                                else
                                {
                                    status.Text += "\r\nERROR!";
                                }
                            }
                            else
                            {
                                status.Text += "\r\nERROR!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            main_mode.IsEnabled = false;
        }

        private void end_y_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (status != null)
            {
                if (status.Text != null)
                {
                    if (pixels != null)
                    {
                        if (!IsViewEmpty())
                        {
                            InitializeView();
                        }
                    }
                    status.Text += $"\r\nReceived data -> {end_y.Text}\r\nValidating...";
                    int starting_x = -1;
                    int starting_y = -1;
                    int ending_x = -1;
                    int ending_y = -1;
                    if (int.TryParse(start_x.Text, out starting_x))
                    {
                        if (int.TryParse(start_y.Text, out starting_y))
                        {
                            if (int.TryParse(end_x.Text, out ending_x))
                            {
                                if (int.TryParse(end_y.Text, out ending_y))
                                {
                                    if (CheckCoords(starting_x, starting_y, ending_x, ending_y))
                                    {
                                        status.Text += "\r\nPASSED!";
                                        main_mode.IsEnabled = true;

                                        pixels[starting_x][starting_y].Fill = new SolidColorBrush(Colors.Red);
                                        pixels[starting_x][starting_y].Tag = "<line>";

                                        line_start_x = starting_x;
                                        line_start_y = starting_y;
                                        line_end_x = ending_x;
                                        line_end_y = ending_y;
                                        return;
                                    }
                                    else
                                    {
                                        status.Text += "\r\nPOINT IS OUT OF BONDS!";
                                    }
                                }
                                else
                                {
                                    status.Text += "\r\nERROR!";
                                }
                            }
                            else
                            {
                                status.Text += "\r\nERROR!";
                            }
                        }
                        else
                        {
                            status.Text += "\r\nERROR!";
                        }
                    }
                    else
                    {
                        status.Text += "\r\nERROR!";
                    }
                }
            }
            main_mode.IsEnabled = false;
        }

        private void main_mode_Click(object sender, RoutedEventArgs e)
        {
            status.Text += $"\r\nGenerating new line...";
            new Thread(() =>
            {
                DDA(line_start_x, line_start_y, line_end_x, line_end_y, false);
            }).Start();
            clear.IsEnabled = true;
        }
    }
}

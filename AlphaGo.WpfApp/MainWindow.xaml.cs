using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace STARK_INDUSTRIES_斯塔克工业_1.五子棋
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Alpha.Alpha _alpha = new Alpha.Alpha(20);

        public MainWindow()
        {
            InitializeComponent();
            for (var i = 0; i < 20; i++)
            {
                Grid.RowDefinitions.Add(new RowDefinition());
                Grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
            Grid.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(Finish), true);
            var t = new TextBlock
            {
                Text = _count.ToString()
            };
            _count++;
            t.Background = new SolidColorBrush(Colors.Yellow);
            t.Foreground = new SolidColorBrush(Colors.Black);
            t.SetValue(Grid.RowProperty, 9);
            t.SetValue(Grid.ColumnProperty, 9);
            Grid.Children.Add(t);
            _alpha.Finish(9, 9, Alpha.Alpha.Color.Black);
            _alpha.UpdateDp(Alpha.Alpha.Color.Black);
            //a.ConsoleB543();
            //a.ConsoleA543();
        }
        private int _count = 1;
        private void Finish(object sender, MouseButtonEventArgs args)
        {
            const double unitX = 40;
            const double unitY = 23;

            var row = (int)(args.MouseDevice.GetPosition(Grid).Y / unitY);
            var col = (int)(args.MouseDevice.GetPosition(Grid).X / unitX);

            var t = new TextBlock();
            var f = _alpha.Finish(row, col, Alpha.Alpha.Color.White);
            switch (f)
            {
                case true:
                    MessageBox.Show("你赢了");
                    break;
                case null:
                    return;
            }

            _alpha.UpdateDp(Alpha.Alpha.Color.White);
            t.Background = new SolidColorBrush(Colors.LightGreen);
            t.HorizontalAlignment = HorizontalAlignment.Stretch;
            t.Foreground = new SolidColorBrush(Colors.Black);
            t.Text = _count.ToString(); _count++;
            t.SetValue(Grid.RowProperty, row);
            t.SetValue(Grid.ColumnProperty, col);
            Grid.Children.Add(t);

            //a.ConsoleB543();
            //a.ConsoleA543();
            var b = _alpha.Choose();
            f = _alpha.Finish(b[0], b[1], Alpha.Alpha.Color.Black);
            switch (f)
            {
                case true:
                    MessageBox.Show("你输了");
                    break;
                case null:
                    return;
            }

            _alpha.UpdateDp(Alpha.Alpha.Color.Black);
            t = new TextBlock
            {
                Background = new SolidColorBrush(Colors.Yellow),
                Foreground = new SolidColorBrush(Colors.Black),
                Text = _count.ToString()
            };
            _count++;
            t.SetValue(Grid.RowProperty, b[0]);
            t.SetValue(Grid.ColumnProperty, b[1]);
            Grid.Children.Add(t);
        }
    }
}

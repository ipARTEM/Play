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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TagGameLib;

namespace TagGameWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ModelGame model;
        DateTime start;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            model = new ModelGame();
          
            model.RePaint += Model_RePaint;
            model.WinComplite += Model_WinComplite;
            model.Init();

            start = DateTime.Now;
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            tblTimer.Text = (DateTime.Now - start).ToString(@"mm\:ss\.f");
        }

        private void Model_WinComplite(object sender, EventArgs e)
        {
            //MessageBox.Show("You Win!!!");
            brd.Visibility = Visibility.Visible;
            timer.Stop();

            List<Record> rec;
            if (System.IO.File.Exists("records.txt"))
                rec = (System.IO.File.ReadAllLines("records.txt"))
                .Select(s => new Record(s))
                .ToList();
            else
                rec = new List<Record>();

            var r = new Record
            {
                Date = DateTime.Now.ToString(),
                Time = tblTimer.Text,
                Steps = model.Step.ToString()
            };
            rec.Add(r);

            var ordList = rec.OrderBy(x => x.Time)
                .Select((x, i) => { x.Pos = i + 1; return x; })
                .Take(12)
                .ToArray();
            System.IO.File.WriteAllLines("records.txt", ordList.Select(x=>x.ToString()));
            records.ItemsSource = ordList;
        }

        public void Model_RePaint(object sender, int[,] e)
        {
            int[][] map = new int[4][];
            for(int i=0; i < 4; i++)
            {
                map[i] = new int[4];
                for(int j = 0; j < 4; j++)
                {
                    map[i][j] = model[i, j];
                }
            }
            ic.ItemsSource = map;
            tblStep.Text = model.Step.ToString();
        }

        public void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var brd = sender as Border;
            var num = (int)brd.DataContext;
            model.Press(num);
        }

        private void btnAgain_Click(object sender, RoutedEventArgs e)
        {
            model.Init();
            brd.Visibility = Visibility.Collapsed;
            timer.Start();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    class Record
    {
        public Record()
        {

        }


        public Record(string s)     //Сиреализация
        {
            var field = s.Split('\t');
            Pos = int.Parse(field[0]);
            Date = field[1];
            Time = field[2];
            Steps = field[3];
        }

        public override string ToString()    //Десиреализация
        {
            return $"{Pos}\t{Date}\t{Time}\t{Steps}";
        }

        public int Pos { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Steps { get; set; }

    }
}

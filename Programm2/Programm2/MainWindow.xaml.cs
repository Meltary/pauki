using System;
using System.Collections.Generic;
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

namespace Programm2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Rectangle tank = new Rectangle();//танк
        List<Rectangle> pula = new List<Rectangle>();//пуля
        List<Rectangle> pauki = new List<Rectangle>();//пауки
        List<Point> tochka = new List<Point>();//положение пауков
        List<Point> steps = new List<Point>();//направление шагов пауков
        int opusk = 0;
        int puli = 0;
        String step = "right";
        int numpauk = 0;//кол-во пауков
        int ostpauk = 0;//кол-во оставшихся пауков
        int niz = 0; //кол-во ячеек до низа
        Random rand = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            ShowProg();
        }

        public void ShowProg()
        {

            //обнуление и очищение
            numpauk = 0;
            opusk = 0;
            puli = 0;
            ostpauk = 0;
            niz = 0;
            tochka.Clear();
            steps.Clear();
            pauki.Clear();
            canv.Children.Clear();

            //добавление танка
            tank.Fill = Brushes.Black;
            tank.Width = 20;
            tank.Height = 20;
            Canvas.SetLeft(tank, 200);
            Canvas.SetTop(tank, 340);
            canv.Children.Add(tank);

            //добавление пауков
            for (numpauk = 0; numpauk < 30; numpauk++)
            {
                if (numpauk < 10) tochka.Add(new Point((((numpauk + 1) * 2) - 1) * 20, 0));
                if (numpauk < 20 && numpauk >= 10) tochka.Add(new Point(((numpauk-10) * 2) * 20, 20));
                if (numpauk < 30 && numpauk >= 20) tochka.Add(new Point(((((numpauk-20) + 1) * 2) - 1) * 20, 40));
                pauki.Add(new Rectangle());
                pauki[numpauk].Fill = Brushes.Gray;
                pauki[numpauk].Width = 20;
                pauki[numpauk].Height = 20;
                canv.Children.Add(pauki[numpauk]);
                Canvas.SetLeft(pauki[numpauk], tochka[numpauk].X);
                Canvas.SetTop(pauki[numpauk], tochka[numpauk].Y);
            }

            ostpauk = numpauk;

            //открытие/закрытие
            if (MessageBox.Show("Начать?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Go();
            }
            else
                Close();
        }

        public async void Go()
        {
            while (ostpauk > 0 && niz < 340)
            {
                //движение паучков
                for (int i = 0; i < numpauk; i++)
                {
                    if (step == "right")
                    {
                        if (i < 10 || (i < 30 && i >= 20)) tochka[i] = new Point() { X = tochka[i].X - 20, Y = tochka[i].Y + opusk };
                        if (i < 20 && i >= 10) tochka[i] = new Point() { X = tochka[i].X + 20, Y = tochka[i].Y + opusk };
                    }
                    else
                    {
                        if (i < 10 || (i < 30 && i >= 20)) tochka[i] = new Point() { X = tochka[i].X + 20, Y = tochka[i].Y + opusk };
                        if (i < 20 && i >= 10) tochka[i] = new Point() { X = tochka[i].X - 20, Y = tochka[i].Y + opusk };
                    }
                    Canvas.SetLeft(pauki[i], tochka[i].X);
                    Canvas.SetTop(pauki[i], tochka[i].Y);
                }
                opusk = 20;
                niz = niz+20;
                if (step == "right") step = "left"; else step = "right";

                //задержка
                var result = await Task<string>.Factory.StartNew(() =>
                {
                    Thread.Sleep(1500);
                    return "1";
                });
            }
                    
            //конец игры
            MessageBox.Show("Конец");
            ShowProg();
        }

        public async void Run(int num)
        {
            while (Canvas.GetTop(pula[num]) > 0 || pula[num].Fill != Brushes.Transparent)
            {    
                for (int i = 0; i < numpauk; i++)
                {
                    if (Canvas.GetTop(pula[num]) == Canvas.GetTop(pauki[i]) && Canvas.GetLeft(pula[num]) == Canvas.GetLeft(pauki[i])+9 && pauki[i].Fill == Brushes.Gray)
                    {
                        pauki[i].Fill = Brushes.Transparent;
                        pula[num].Fill = Brushes.Transparent;
                        ostpauk--;
                        return;
                    }
                }      
   
                //движение
                Canvas.SetTop(pula[num], Canvas.GetTop(pula[num]) - 20); 

                //задержка
                var r = await Task<string>.Factory.StartNew(() =>
                {
                    Thread.Sleep(100);
                    return "1";
                });
            }
        }

        private async void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            //стрельба
            if (e.Key == Key.Space)
            {
                pula.Add(new Rectangle());
                pula[puli].Fill = Brushes.Brown;
                pula[puli].Width = 2;
                pula[puli].Height = 12;
                canv.Children.Add(pula[puli]);
                Canvas.SetLeft(pula[puli], Canvas.GetLeft(tank) + 9);
                Canvas.SetTop(pula[puli], Canvas.GetTop(tank) - 20);
                Run(puli);
                puli++;
            }

            //изменение направления
            if (e.Key == Key.Left)
            {
                while (e.IsDown)
                {
                    if (Canvas.GetLeft(tank) != 0) Canvas.SetLeft(tank, Canvas.GetLeft(tank) - 20);

                    //задержка
                    var res = await Task<string>.Factory.StartNew(() =>
                    {
                        Thread.Sleep(100);
                        return "1";
                    });
                }
            }

            if (e.Key == Key.Right)
            {
                while (e.IsDown)
                {
                    if (Canvas.GetLeft(tank) != 380) Canvas.SetLeft(tank, Canvas.GetLeft(tank) + 20);

                    //задержка
                    var res = await Task<string>.Factory.StartNew(() =>
                    {
                        Thread.Sleep(100);
                        return "1";
                    });
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow pMainwindow = null;
        public Window1 pSettingWindow = null;

        public Sandbox mySandbox = new Sandbox();
        public DispatcherTimer aTimer = new DispatcherTimer();
        public bool IsTimerRuning;
        private delegate void TimerDispatcherDelegate();


        public MainWindow()
        {
            InitializeComponent();
            pMainwindow = this;
            

            aTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            aTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);//update rate 1 ms
            aTimer.Start();
            IsTimerRuning = true;

            mySandbox.dt = 0.05;
            mySandbox.myConvertor.SpatialZoomRatio = 4;
            mySandbox.gravity.G = new Vector(0, 0);

            //mySandbox.Height = (this.Height - mySandbox.Bottomoffset) * mySandbox.SpatialZoomRatio;
            //mySandbox.Width = (this.Width - mySandbox.Rightoffest) * mySandbox.SpatialZoomRatio;

            mySandbox.myWall.Thickness = 20;
            
            mySandbox.gravity.gmod = Gravity.GravityMode.Linear;
            mySandbox.gravity.Centre = new Vector(mySandbox.Width / 2, mySandbox.Height / 2);

            mySandbox.myConvertor.SetSandBox(mySandbox, pMainwindow);

            Random RND = new Random(Application.Current.GetHashCode());
            for (int i = 0; i < 10; i++)
            {

                Ball b = new Ball();

                b.Name = i.ToString();

                b.Radius = (1 + RND.NextDouble()) * 50;
                b.Mass = 10.0 / (1000000.0) * b.Radius * b.Radius * b.Radius;
                //b.K = 5;
                //b.C = 0.5;

                Color randomColor = Color.FromArgb(255, (byte)RND.Next(255), (byte)RND.Next(255), (byte)RND.Next(255));
                b.bcolor = randomColor;

                b.pos.x = RND.NextDouble() * mySandbox.Width;
                b.pos.y = RND.NextDouble() * mySandbox.Height;

                b.p.x = 100 * RND.NextDouble() - 50;
                b.p.y = 100 * RND.NextDouble() - 50;


                //b.BallImage.Width = b.BallImage.Height = 2b.Radius / mySandbox.SpatialZoomRatio;
                b.BallImage.Fill = new SolidColorBrush(b.bcolor);
                //b.BallImage.Margin = new Thickness(b.pos.x, b.pos.y, 0, 0);

                this.Stage.Children.Add(b.BallImage);

                mySandbox.BallLsit.Add(b);
            }
            pSettingWindow = new Window1();
            this.Focus();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (mySandbox.IsErrorControlled)
                mySandbox.MotionInDt_New();
            else
                mySandbox.MotionInDt();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            mySandbox.myConvertor.SetSandBox(mySandbox, pMainwindow);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key.Equals(Key.Space)))
            {
                if (IsTimerRuning) { aTimer.Stop(); IsTimerRuning = false; return; }
                if (IsTimerRuning == false) { aTimer.Start(); IsTimerRuning = true; return; }
            }

            else if ((e.Key.Equals(Key.F2)))
            {
                pSettingWindow.Show();
                pSettingWindow.Focus();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                if (MessageBox.Show("Close Application?", "SandBox", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                {
                    return;
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            //Vector p = mySandbox.myConvertor.GetSandboxVectorforAPointOnScreen(Mouse.GetPosition(this));
            //pSettingWindow.TB_Ball_POS_X.Text = p.x.ToString();
            //pSettingWindow.TB_Ball_POS_Y.Text = p.y.ToString();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Ball selectedBall = null;
            if (mySandbox.myConvertor.FindBallContainAPoint(mySandbox.BallLsit, Mouse.GetPosition(this), out selectedBall))
            {
                int index = mySandbox.BallLsit.FindIndex(x => x == selectedBall);
                pSettingWindow.BallInfo.SelectedIndex = index;
                pSettingWindow.TB_BallName.Text = selectedBall.Name;
                pSettingWindow.TB_Ball_POS_X.Text = selectedBall.pos.x.ToString();
                pSettingWindow.TB_Ball_POS_Y.Text = selectedBall.pos.y.ToString();
                pSettingWindow.TB_Ball_P_X.Text = selectedBall.p.x.ToString();
                pSettingWindow.TB_Ball_P_Y.Text = selectedBall.p.y.ToString();
                pSettingWindow.TB_Mass.Text = selectedBall.Mass.ToString();
                pSettingWindow.TB_R.Text = (selectedBall.Radius).ToString();
                pSettingWindow.BallTemp.Width = selectedBall.BallImage.Width;
                pSettingWindow.BallTemp.Height = selectedBall.BallImage.Height;
                pSettingWindow.BallTemp.Fill = selectedBall.BallImage.Fill;
            }
            else
            {
                Vector p = mySandbox.myConvertor.GetSandboxVectorforAPointOnScreen(Mouse.GetPosition(this));
                pSettingWindow.TB_Ball_POS_X.Text = p.x.ToString();
                pSettingWindow.TB_Ball_POS_Y.Text = p.y.ToString();
            }
        }
    }
}

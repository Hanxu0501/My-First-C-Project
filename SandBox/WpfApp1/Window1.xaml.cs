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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            GravityAmp.Text = Vector.Abs(MainWindow.pMainwindow.mySandbox.gravity.G).ToString();
            GravityAng.Text = Vector.Angle(MainWindow.pMainwindow.mySandbox.gravity.G).ToString();
            GmodComboBox.Text = MainWindow.pMainwindow.mySandbox.gravity.gmod.ToString();
            SpatialZoomRatio.Text = MainWindow.pMainwindow.mySandbox.myConvertor.SpatialZoomRatio.ToString();
            TimeResolution.Text = MainWindow.pMainwindow.mySandbox.dt.ToString();
            CKBOXErrorControlled.IsChecked = MainWindow.pMainwindow.mySandbox.IsErrorControlled;

            foreach (Ball b in MainWindow.pMainwindow.mySandbox.BallLsit)
            {
                BallInfo.Items.Add(b.Name);
            }

        }

        private void Btn_setGravity_Click(object sender, RoutedEventArgs e)
        {
            double GAbs, GAng;
            if(!double.TryParse(GravityAmp.Text, out GAbs)) GAbs=0;
            if(!double.TryParse(GravityAng.Text, out GAng)) GAng=0;
            MainWindow.pMainwindow.mySandbox.gravity.G = new Vector(GAbs, GAng, "polar");
            switch (GmodComboBox.SelectedIndex)
            {
                case 0:
                    MainWindow.pMainwindow.mySandbox.gravity.gmod = Gravity.GravityMode.Linear;
                    break;

                case 1:
                    MainWindow.pMainwindow.mySandbox.gravity.gmod = Gravity.GravityMode.Concentric;
                    break;

                case 2:
                    MainWindow.pMainwindow.mySandbox.gravity.gmod = Gravity.GravityMode.None;
                    break;

                default:break;
            }

            double _spatio_z_r= MainWindow.pMainwindow.mySandbox.myConvertor.SpatialZoomRatio; //spatio zoom ratio
            
            if(!double.TryParse(SpatialZoomRatio.Text, out _spatio_z_r)) _spatio_z_r = MainWindow.pMainwindow.mySandbox.myConvertor.SpatialZoomRatio;
            MainWindow.pMainwindow.mySandbox.myConvertor.SpatialZoomRatio = _spatio_z_r;
            MainWindow.pMainwindow.mySandbox.myConvertor.SetSandBox(MainWindow.pMainwindow.mySandbox, MainWindow.pMainwindow);
           
            double _time_res=0.05;//time resolution
            if(!double.TryParse(TimeResolution.Text, out _time_res)) _time_res = MainWindow.pMainwindow.mySandbox.dt;
            MainWindow.pMainwindow.mySandbox.dt = _time_res;

            if (CKBOXErrorControlled.IsChecked == true) MainWindow.pMainwindow.mySandbox.IsErrorControlled = true;
            else MainWindow.pMainwindow.mySandbox.IsErrorControlled = false;
        }

        private void BallInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BallInfo.SelectedIndex != -1)
            {
                Ball b = MainWindow.pMainwindow.mySandbox.BallLsit[BallInfo.SelectedIndex];
                TB_BallName.Text = b.Name;
                TB_Ball_POS_X.Text = b.pos.x.ToString();
                TB_Ball_POS_Y.Text = b.pos.y.ToString();
                TB_Ball_P_X.Text = b.p.x.ToString();
                TB_Ball_P_Y.Text = b.p.y.ToString();
                TB_Mass.Text = b.Mass.ToString();
                TB_R.Text = (b.Radius).ToString();
                BallTemp.Width = b.BallImage.Width;
                BallTemp.Height = b.BallImage.Height;
                BallTemp.Fill = b.BallImage.Fill;
            }
        }

        private void BTN_UpdatBallPara_Click(object sender, RoutedEventArgs e)
        {
            if (BallInfo.SelectedIndex != -1)
            {
                Ball b = MainWindow.pMainwindow.mySandbox.BallLsit[BallInfo.SelectedIndex];
                b.Name = TB_BallName.Text;
                BallInfo.Items[BallInfo.SelectedIndex] = TB_BallName.Text;

                double px, py;
                double.TryParse(TB_Ball_P_X.Text, out px);
                double.TryParse(TB_Ball_P_Y.Text, out py);
                b.p = new Vector(px, py);

                double posx, posy;
                double.TryParse(TB_Ball_POS_X.Text, out posx);
                double.TryParse(TB_Ball_POS_Y.Text, out posy);
                b.pos = new Vector(posx, posy);

                double mass, r;
                double.TryParse(TB_Mass.Text, out mass);
                double.TryParse(TB_R.Text, out r);
                b.Mass = mass;
                b.Radius = r;

                BallInfo.SelectedItem = b.Name;
                MainWindow.pMainwindow.mySandbox.myConvertor.PutABallOnScreen(b);
            }
        }

        private void BtnAddBall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ball b = new Ball();
                b.Name = TB_BallName.Text;
               

                double px, py;
                if(!double.TryParse(TB_Ball_P_X.Text, out px)) px=0;
                if(!double.TryParse(TB_Ball_P_Y.Text, out py)) py=0;
                b.p = new Vector(px, py);

                double posx, posy;
                if(!double.TryParse(TB_Ball_POS_X.Text, out posx)) posx=0;
                if(!double.TryParse(TB_Ball_POS_Y.Text, out posy)) posy=0;
                b.pos = new Vector(posx, posy);

                double mass, r;
                if(!double.TryParse(TB_Mass.Text, out mass)) mass=1;
                if(!double.TryParse(TB_R.Text, out r)) r=80;
                b.Mass = mass;
                b.Radius = r;

                if (MainWindow.pMainwindow.mySandbox.BallLsit.Find(x=> x.pos == b.pos)==null)
                {
                    MainWindow.pMainwindow.mySandbox.BallLsit.Add(b);
                    MainWindow.pMainwindow.Stage.Children.Add(b.BallImage);
                    MainWindow.pMainwindow.mySandbox.myConvertor.PutABallOnScreen(b);
                    BallInfo.Items.Add(b.Name);
                }

                //BallInfo.Items.Clear();
                //foreach (Ball b1 in MainWindow.pMainwindow.mySandbox.BallLsit)
                //{
                //    BallInfo.Items.Add(b1.Name);
                //}
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }

        }

        private void RemoveThisBall_Click(object sender, RoutedEventArgs e)
        {
            if (BallInfo.SelectedIndex != -1)
            {
                Ball b = MainWindow.pMainwindow.mySandbox.BallLsit[BallInfo.SelectedIndex];
                
                MainWindow.pMainwindow.Stage.Children.Remove(b.BallImage);
                MainWindow.pMainwindow.mySandbox.BallLsit.Remove(b);
                BallInfo.Items.Clear();
                foreach (Ball b1 in MainWindow.pMainwindow.mySandbox.BallLsit)
                {
                    BallInfo.Items.Add(b1.Name);
                }
            }
        }


        private void BtnRest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.pMainwindow.Stage.Children.Clear();
                MainWindow.pMainwindow.mySandbox.BallLsit.Clear();
                BallInfo.Items.Clear();
                                                             
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
            }

        private void SettingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Escape))
            {
                this.Hide();
                MainWindow.pMainwindow.Focus();
            }
        }

        private void BTN_SaveToFile_Click(object sender, RoutedEventArgs e)
        {
            string path = "temp.txt";
            MainWindow.pMainwindow.mySandbox.SaveBallListToFile(MainWindow.pMainwindow.mySandbox.BallLsit, path);
        }

        private void BTN_LoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.pMainwindow.Stage.Children.Clear();
            MainWindow.pMainwindow.mySandbox.BallLsit.Clear();
            BallInfo.Items.Clear();
            string path = "temp.txt";
            MainWindow.pMainwindow.mySandbox.LoadBallListFromFile(MainWindow.pMainwindow.mySandbox.BallLsit, path);
            foreach (Ball b in MainWindow.pMainwindow.mySandbox.BallLsit)
            {
                BallInfo.Items.Add(b.Name);
                MainWindow.pMainwindow.Stage.Children.Add(b.BallImage);
                MainWindow.pMainwindow.mySandbox.myConvertor.PutAllofBallOnScreen(MainWindow.pMainwindow.mySandbox.BallLsit);
            }
        }
    }
}

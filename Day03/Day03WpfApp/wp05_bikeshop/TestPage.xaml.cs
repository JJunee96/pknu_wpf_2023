﻿using System;
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
using wp05_bikeShop.Logics;

namespace wp05_bikeShop
{
    /// <summary>
    /// SupportPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TestPage : Page
    {
        Car myCar = null;
        public TestPage()
        {
            InitializeComponent();
            InitCar();
        }

        private void InitCar()
        {
            // 일반적인 C#에서 클래스 객체 인스턴스 사용방법 동일
            myCar = new Car();
            myCar.Names = "아이오닉";
            myCar.Colorz = Colors.White;
            myCar.Speed = 220;

            // ListBox에 바인딩하기 위한 Car 리스트
            var rand = new Random();        //랜덤 색깔
            var cars = new List<Car>();
            for (int i = 0; i < 10; i++)
            {
                cars.Add(new Car()
                {
                    Speed = i * 10,
                    Colorz = Color.FromRgb((byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))
                });
            }
            // this.DataContext ==> 페이지 전체에 바인딩 하기 위한 데이터 연동
            CtlCars.DataContext = cars;    // 중요 ! 코드비하인드에서 만든 데이터(DB, excel ... )를 xaml에 있는 컨트롤에 바인딩하려면 => DataContext 써야함
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            TxtSample.Text = myCar.Speed.ToString(); // 전통적인 윈폼 방식
        }
    }
}

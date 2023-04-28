using MahApps.Metro.Controls;
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

namespace wpTest01
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void TxtHptName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TxtHptName_KeyDown(sender, e);
            }
        }

        private void BtnSearchHpt_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CboSelectArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void GrdResult_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void BtnAddFavorite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnViewFavorite_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelFavorite_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}

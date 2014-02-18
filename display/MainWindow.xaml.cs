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
using display.ViewModel;

namespace display
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            namesVM names = new namesVM();
            Common.list = names.Names;
            base.DataContext = names;
            InitializeComponent();
            atb_id.ItemsSource = Common.list;
            
            
        }

        public void ButtonClick(object sender, RoutedEventArgs e)
        {           
            string id = atb_id.Text;
            data details = new data(id);
            if (Common.error == true)
            {
                error error = new error();
                this.contentControl.Content = error;
            }
            else
            {
                this.contentControl.Content = details;
            }
            
       }

        private void search_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                ButtonClick(sender, e);
                //label_message.Visibility = Class1.visibility;
                atb_id.Text = String.Empty;
            }
        }

        private void update(object sender, RoutedEventArgs e)
        {
            update update = new update();
            this.contentControl.Content = update;
        }

        private void atb_id_GotFocus(object sender, RoutedEventArgs e)
        {
            atb_id.ItemsSource = Common.list;
            
        }

        private void atb_id_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!String.IsNullOrEmpty(atb_id.Text))
            {
                ButtonClick(sender, e);
            }
        }

        private void atb_id_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!String.IsNullOrEmpty(atb_id.Text))
            {
                ButtonClick(sender, e);
            }
        }

        //private void column0_MouseEnter(object sender, MouseEventArgs e)
        //{
        //    column0.Width = 200;
        //}

        //private void column0_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    column0.Width = 100;
        //}

        //private void searchbox(object sender, RoutedEventArgs e)
        //{
        //    atb_id.Visibility = Visibility.Visible;
        //}
    }
}

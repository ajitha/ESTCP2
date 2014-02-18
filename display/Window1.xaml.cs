using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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


namespace ESTCustomerProfiles
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        //SupportKeyVM svm;

        public ObservableCollection<SupportKey> supportkeys { get; set; }
        public ObservableCollection<CustomerInfo> customerinfos { get; set; }

        public Window1()
        {
            //svm = new SupportKeyVM ();

            //DataContext = svm.supportkeys;

            this.DataContext = this;
            supportkeys = new ObservableCollection<SupportKey>();
            customerinfos = new ObservableCollection<CustomerInfo>();

            InitializeComponent();
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var skey = button.DataContext as SupportKey;
                //svm.supportkeys.Remove(skey);
                supportkeys.Remove(skey);
            }
            else
            {
                return;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SupportKey sk = new SupportKey(textbox_key.Text, textbox_description.Text);

            if (textbox_key.Text != "" && textbox_description.Text != "")
            {
                //svm.supportkeys.Add(sk);
                supportkeys.Add(sk);
                textbox_key.Text = "";
                textbox_description.Text = "";

            }


        }

        private void btnCOntactDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddContact_Click(object sender, RoutedEventArgs e)
        {

            
            CustomerInfo custtemp = new CustomerInfo(textbox_name.Text, textbox_desingnation.Text,textbox_email.Text, textbox_workphone.Text , textbox_mobile.Text );

            if (textbox_name.Text != "" )
            {
                //svm.supportkeys.Add(sk);

                customerinfos.Add(custtemp);

                textbox_name.Text = "";
                textbox_desingnation.Text = "";
                textbox_email.Text = "";
                textbox_workphone.Text = "";
                textbox_mobile.Text = "";
                

            }
        }

        private void btnContactDelete_Click_1(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var customerinfotemp = button.DataContext as CustomerInfo;
                //svm.supportkeys.Remove(skey);
                customerinfos.Remove(customerinfotemp);
            }
            else
            {
                return;
            }
        }
    }
}

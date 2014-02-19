﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using display.ViewModel;
using display.Model;
using System.IO;

namespace display
{
    /// <summary>
    /// Interaction logic for update.xaml
    /// </summary>
    public partial class update : UserControl
    {
        //SupportKeyVM svm;
        string strAccessConn;

        public ObservableCollection<SupportKey> supportkeys { get; set; }
        public ObservableCollection<CustomerInfo> customerinfos { get; set; }

        public ObservableCollection<ComboBoxItem> regioncbItems { get; set; }
        public ComboBoxItem SelectedRegion { get; set; }

        OleDbConnection myAccessConn = null;


        //new mod
        Access access = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=profiles.MDB");
        public static List<string> existingCustIDs;
        //////////////////////

        public update()
        {
            //svm = new SupportKeyVM ();

            //new mod
            existingCustIDs = access.GetIDs();

            //DataContext = svm.supportkeys;

            this.DataContext = this;
            supportkeys = new ObservableCollection<SupportKey>();
            customerinfos = new ObservableCollection<CustomerInfo>();


#if USINGPROJECTSYSTEM
		strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\\..\\profiles.MDB";
#else
            strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=profiles.MDB";
#endif



            regioncbItems = new ObservableCollection<ComboBoxItem>();
            var cbItem = new ComboBoxItem { Content = "<--Select-->" };
            SelectedRegion = cbItem;
            regioncbItems.Add(cbItem);
            regioncbItems.Add(new ComboBoxItem { Content = "Option 1" });
            regioncbItems.Add(new ComboBoxItem { Content = "Option 2" });


            // combobox_region.Text = "Option 1";
            //combobox_region.SelectedItem = "Option 1";

            foreach (ComboBoxItem cbi in regioncbItems)
            {
                if (cbi.Content == "Option 2")
                {
                    SelectedRegion = cbi;
                }
            }

            InitializeComponent();
        }
        static Customer customer = new Customer();

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


            CustomerInfo custtemp = new CustomerInfo(textbox_name.Text, textbox_desingnation.Text, textbox_email.Text, textbox_workphone.Text, textbox_mobile.Text);

            if (textbox_name.Text != "")
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

        public OleDbCommand SaveBasicCustomerInfo()
        {

            string strAccessInsert = "INSERT INTO Customer(customerId,customerName,spptOrganization,offset) VALUES (@custid,@custname,@spptorg,@offset)";
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;


            cmd.CommandText = strAccessInsert;
            cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
            cmd.Parameters.AddWithValue("@custname", textbox_customername.Text);
            //cmd.Parameters.AddWithValue("@spptorg", SelectedRegion.Content);
            cmd.Parameters.AddWithValue("@spptorg", textbox_region.Text);
            string offset = textbox_offset.Text;
            if (String.IsNullOrEmpty(textbox_offset.Text))
            {
                offset = "0";
            }
            cmd.Parameters.AddWithValue("@offset", Convert.ToDecimal(offset));

            //openDatabaseConnection();
            cmd.Connection = myAccessConn;
            //cmd.ExecuteNonQuery();
            //myAccessConn.Close();

            return cmd;
        }



        public OleDbCommand SaveCaseHandling()
        {

            string strAccessInsert = "INSERT INTO Guidelines(customerId,guidelines) VALUES (@custid,@guidelines)";
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;


            cmd.CommandText = strAccessInsert;
            cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
            //cmd.Parameters.AddWithValue("@guidelines", new TextRange(richtextbox_guidelines.Document.ContentStart, richtextbox_guidelines.Document.ContentEnd).Text);


            TextRange tr = new TextRange(richtextbox_guidelines.Document.ContentStart,
                                richtextbox_guidelines.Document.ContentEnd);
            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Xaml);
            string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
            cmd.Parameters.AddWithValue("@guidelines", xamlText);
            //openDatabaseConnection();
            cmd.Connection = myAccessConn;
            //cmd.ExecuteNonQuery();
            //myAccessConn.Close();

            return cmd;
        }




        public List<OleDbCommand> SaveContacts()
        {
            List<OleDbCommand> cmdlist = new List<OleDbCommand>();


            foreach (CustomerInfo ci in customerinfos)
            {
                string strAccessInsert = "INSERT INTO Contacts(customerId,name,designation,email,workphone,mobile) VALUES (@custid,@name,@designation,@email,@workphone,@mobile)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@name", ci.name);
                cmd.Parameters.AddWithValue("@designation", ci.designation);
                cmd.Parameters.AddWithValue("@email", ci.email);
                cmd.Parameters.AddWithValue("@workphone", ci.workphone);
                cmd.Parameters.AddWithValue("@mobile", ci.mobile);

                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);

            }


            return cmdlist;



        }




        public List<OleDbCommand> SaveSupportKeys()
        {
            List<OleDbCommand> cmdlist = new List<OleDbCommand>();


            foreach (SupportKey sk in supportkeys)
            {
                string strAccessInsert = "INSERT INTO Support(customerId,supportKey,description,queue) VALUES (@custid,@spptkey,@desc,@queue)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@spptkey", sk.key);
                cmd.Parameters.AddWithValue("@desc", sk.description);
                cmd.Parameters.AddWithValue("@queue", textbox_queue.Text);
                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);

            }


            return cmdlist;



        }

        public OleDbCommand SaveSeverityActions()
        {

            string strAccessInsert = "INSERT INTO SeverityAction(customerId,action1,action2,action3,action4) VALUES (@custid,@action1,@action2,@action3,@action4)";
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = strAccessInsert;
            cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
            cmd.Parameters.AddWithValue("@action1", new TextRange(richtext_sevaction1.Document.ContentStart, richtext_sevaction1.Document.ContentEnd).Text);
            cmd.Parameters.AddWithValue("@action2", new TextRange(richtext_sevaction2.Document.ContentStart, richtext_sevaction2.Document.ContentEnd).Text);
            cmd.Parameters.AddWithValue("@action3", new TextRange(richtext_sevaction3.Document.ContentStart, richtext_sevaction3.Document.ContentEnd).Text);
            cmd.Parameters.AddWithValue("@action4", new TextRange(richtext_sevaction4.Document.ContentStart, richtext_sevaction4.Document.ContentEnd).Text);

            cmd.Connection = myAccessConn;


            return cmd;


        }


        public List<OleDbCommand> SaveSupportedSeverities()
        {
            List<OleDbCommand> cmdlist = new List<OleDbCommand>();

            if (checkbox_sev1.IsChecked == true)
            {
                string strAccessInsert = "INSERT INTO SupportedSeverities(customerId,severity) VALUES (@custid,@severity)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@severity", 1);
                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);
            }

            if (checkbox_sev2.IsChecked == true)
            {
                string strAccessInsert = "INSERT INTO SupportedSeverities(customerId,severity) VALUES (@custid,@severity)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@severity", 2);
                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);
            }
            if (checkbox_sev3.IsChecked == true)
            {
                string strAccessInsert = "INSERT INTO SupportedSeverities(customerId,severity) VALUES (@custid,@severity)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@severity", 3);
                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);
            }
            if (checkbox_sev4.IsChecked == true)
            {
                string strAccessInsert = "INSERT INTO SupportedSeverities(customerId,severity) VALUES (@custid,@severity)";
                OleDbCommand cmd = new OleDbCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = strAccessInsert;
                cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);
                cmd.Parameters.AddWithValue("@severity", 4);
                //openDatabaseConnection();
                cmd.Connection = myAccessConn;

                cmdlist.Add(cmd);
            }


            return cmdlist;
        }

        public void DeleteCustmerFromdb(String customerId)
        {



            openDatabaseConnection();

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = myAccessConn;


            cmd.CommandText = "DELETE FROM Customer WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Guidelines WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Contacts WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Holiday WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM SeverityAction WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM Support WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DELETE FROM SupportedSeverities WHERE (customerId='" + customerId + "')"; ;
            cmd.ExecuteNonQuery();

            myAccessConn.Close();


        }



        public void AddCustomer()
        {

            List<OleDbCommand> cmdstoexe = new List<OleDbCommand>();
            openDatabaseConnection();

            cmdstoexe.Add(SaveBasicCustomerInfo());
            cmdstoexe.Add(SaveSeverityActions());
            cmdstoexe.Add(SaveCaseHandling());

            foreach (OleDbCommand odbc in SaveContacts())
            {
                cmdstoexe.Add(odbc);
            };

            foreach (OleDbCommand odbc in SaveSupportedSeverities())
            {
                cmdstoexe.Add(odbc);
            };

            foreach (OleDbCommand odbc in SaveSupportKeys())
            {
                cmdstoexe.Add(odbc);
            };


            foreach (OleDbCommand odbc in cmdstoexe)
            {

                odbc.ExecuteNonQuery();
            }

            myAccessConn.Close();


        }


        public bool IsCustomerExisting()
        {

            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = CommandType.Text;

            string strAccessSelect = "SELECT customerId FROM Customer WHERE customerId=@custid";
            cmd.CommandText = strAccessSelect;
            cmd.Parameters.AddWithValue("@custid", textbox_customerid.Text);

            openDatabaseConnection();
            cmd.Connection = myAccessConn;

            OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(cmd);
            DataSet ds = new DataSet();
            myAccessConn.Close();


            try
            {
                myDataAdapter.Fill(ds, "Customer");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }


            //customer already exist in the database
            if (ds.Tables["Customer"].Rows.Count == 1)
            {
                //MessageBox.Show("Customer already exist in the database!! Handle!!");
                return true;
            }

            //add new customer to the database
            else
            {
                return false;
            }

            //string query = "INSERT INTO Customer(customerId) VALUES ('225')";
        }


        public void openDatabaseConnection()
        {
            try
            {
                myAccessConn = new OleDbConnection(strAccessConn);
                myAccessConn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: Failed to create a database connection. \n{0}", ex.Message);
                return;
            }


        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {


            if (IsCustomerExisting())
            {
                MessageBoxResult result = MessageBox.Show("User exists in the databse. Would you like to overwrite?", "Error", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        {
                            DeleteCustmerFromdb(textbox_customerid.Text);

                            AddCustomer();

                            break;
                        }
                    case MessageBoxResult.No:
                        {

                            break;
                        }
                }

            }
            else
            {
                AddCustomer();

            }
            //Access ac = new Access();
            namesVM n = new namesVM();
            Common.list = n.Names;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


        //new mod
        private void textbox_customerid_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (existingCustIDs.Contains(textbox_customerid.Text))
            {
                image_tick.Visibility = System.Windows.Visibility.Visible;

            }
            else
            {
                image_tick.Visibility = System.Windows.Visibility.Collapsed;
            }

        }

        private void textbox_customerid_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string text = textbox_customerid.Text;
            Access ac = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ESTProfiles.MDB");
            customer = ac.GetInfo(text);
            textbox_customername.Text = customer.customerName;
            textbox_offset.Text = customer.offset.ToString();
            textbox_region.Text = customer.spptOrganization;
            textbox_queue.Text = customer.Supports.FirstOrDefault().queue;
            List<string> names = new List<string>();
            foreach (var i in customer.Contacts.Distinct())
            {
                names.Add(i.name/* + " (" + i.designation + ")"*/);
            }
            textbox_name.ItemsSource = names;

        }

        private void textbox_key_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string text = textbox_key.Text;
            int key = int.Parse(text);

            //textbox_description.Text = customer.Supports.Where(m => m.supportKey == key).SingleOrDefault().description;
            var x = customer.Supports.Where(m => m.supportKey == key).FirstOrDefault();
            if (x != null)
            {
                textbox_description.Text = x.description;
            }

        }

        private void textbox_name_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string text = textbox_name.Text;
            //var split = text.Split(new char[] { '(' }, 2);
            //text = split[0];

            //textbox_description.Text = customer.Supports.Where(m => m.supportKey == key).SingleOrDefault().description;
            var x = customer.Contacts.Where(m => m.name == text).FirstOrDefault();
            if (x != null)
            {
                textbox_desingnation.Text = x.designation;
                List<string> desginations = new List<string>();
                foreach (var i in customer.Contacts.Where(m => m.name == x.name))
                {
                    desginations.Add(i.designation);
                }

                textbox_desingnation.ItemsSource = desginations;
                textbox_email.Text = x.email;
                textbox_mobile.Text = x.mobile;
                textbox_workphone.Text = x.workPhone;
            }
            //public ObservableCollection<SupportKey> supportkeys { get; set; }
            //public ObservableCollection<CustomerInfo> customerinfos { get; set; }

            //public update()
            //{
            //    this.DataContext = this;
            //    supportkeys = new ObservableCollection<SupportKey>();
            //    customerinfos = new ObservableCollection<CustomerInfo>();

            //    InitializeComponent();
            //}


            //private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
            //{

            //}

            //private void btnDelete_Click(object sender, RoutedEventArgs e)
            //{
            //    var button = sender as Button;
            //    if (button != null)
            //    {
            //        var skey = button.DataContext as SupportKey;
            //        supportkeys.Remove(skey);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            //private void Button_Click(object sender, RoutedEventArgs e)
            //{
            //}

            //private void btnAdd_Click(object sender, RoutedEventArgs e)
            //{
            //    SupportKey sk = new SupportKey(textbox_key.Text, textbox_description.Text);

            //    if (textbox_key.Text != "" && textbox_description.Text != "")
            //    {
            //        supportkeys.Add(sk);
            //        textbox_key.Text = "";
            //        textbox_description.Text = "";

            //    }


            //}

            //private void btnCOntactDelete_Click(object sender, RoutedEventArgs e)
            //{

            //}

            //private void btnAddContact_Click(object sender, RoutedEventArgs e)
            //{


            //    CustomerInfo custtemp = new CustomerInfo(textbox_name.Text, textbox_desingnation.Text,textbox_email.Text, textbox_workphone.Text , textbox_mobile.Text );

            //    if (textbox_name.Text != "" )
            //    {

            //        customerinfos.Add(custtemp);

            //        textbox_name.Text = "";
            //        textbox_desingnation.Text = "";
            //        textbox_email.Text = "";
            //        textbox_workphone.Text = "";
            //        textbox_mobile.Text = "";


            //    }
            //}

            //private void btnContactDelete_Click_1(object sender, RoutedEventArgs e)
            //{
            //    var button = sender as Button;
            //    if (button != null)
            //    {
            //        var customerinfotemp = button.DataContext as CustomerInfo;
            //        customerinfos.Remove(customerinfotemp);
            //    }
            //    else
            //    {
            //        return;
            //    }
            //}

            //private void btnSubmit_Click(object sender, RoutedEventArgs e)
            //{

            //}
        }
    }

}
using System;
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
using display.Model;
using System.Xml;
using System.Windows.Markup;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using CustomWPFControls;
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
        public ObservableCollection<string> holidays { get; set; }


        public ObservableCollection<ComboBoxItem> regioncbItems { get; set; }
        public ComboBoxItem SelectedRegion { get; set; }

        public int holidayDay { get; set; }
        public int holidayMonth { get; set; }
        public int holidayYear { get; set; }

        

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
            holidays = new ObservableCollection<string>();


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


            holidayDay = 11;

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


            //TextRange tr = new TextRange(richtextbox_guidelines.Document.ContentStart,
            //                    richtextbox_guidelines.Document.ContentEnd);
            //MemoryStream ms = new MemoryStream();
            //tr.Save(ms, DataFormats.Xaml);
            //string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
            //cmd.Parameters.AddWithValue("@guidelines", xamlText);

            string xamlText = ConvertToXaml(richtextbox_guidelines);
            cmd.Parameters.AddWithValue("@guidelines", xamlText);



            //openDatabaseConnection();
            cmd.Connection = myAccessConn;
            //cmd.ExecuteNonQuery();
            //myAccessConn.Close();

            return cmd;
        }

        public string ConvertToXaml(RichTextBox rtb)
        {
            TextRange tr = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);

            


            MemoryStream ms = new MemoryStream();
            tr.Save(ms, DataFormats.Rtf);
            string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray()); 
            return xamlText;
        }


        public List<OleDbCommand> SaveContacts()
        {
            List<OleDbCommand> cmdlist = new List<OleDbCommand>();

            if(!String.IsNullOrEmpty(textbox_name.Text)){
                CustomerInfo custtemp = new CustomerInfo(textbox_name.Text, textbox_desingnation.Text, textbox_email.Text, textbox_workphone.Text, textbox_mobile.Text);
                customerinfos.Add(custtemp);
            }
            


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

            //save non added support key info
            if (!String.IsNullOrEmpty(textbox_key.Text) )
            {
                SupportKey sk = new SupportKey(textbox_key.Text, textbox_description.Text);
                supportkeys.Add(sk);
            }

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

            
            
            //cmd.Parameters.AddWithValue("@action1", new TextRange(richtext_sevaction1.Document.ContentStart, richtext_sevaction1.Document.ContentEnd).Text);

            //TextRange tr = new TextRange(richtext_sevaction1.Document.ContentStart, richtext_sevaction1.Document.ContentEnd);
            //MemoryStream ms = new MemoryStream();
            //tr.Save(ms, DataFormats.Xaml);
            //string xamlText = ASCIIEncoding.Default.GetString(ms.ToArray());
            //cmd.Parameters.AddWithValue("@action1", xamlText);

            string xamltxt1 = ConvertToXaml(richtext_sevaction1);
            cmd.Parameters.AddWithValue("@action1", xamltxt1);
            
            //cmd.Parameters.AddWithValue("@action2", new TextRange(richtext_sevaction2.Document.ContentStart, richtext_sevaction2.Document.ContentEnd).Text);

            //TextRange tr2 = new TextRange(richtext_sevaction2.Document.ContentStart, richtext_sevaction2.Document.ContentEnd);
            //MemoryStream ms2 = new MemoryStream();
            //tr2.Save(ms2, DataFormats.Xaml);
            //string xamlText2 = ASCIIEncoding.Default.GetString(ms2.ToArray());
            //cmd.Parameters.AddWithValue("@action2", xamlText2);

            string xamlText2 = ConvertToXaml(richtext_sevaction2);
            cmd.Parameters.AddWithValue("@action2", xamlText2);

            //cmd.Parameters.AddWithValue("@action3", new TextRange(richtext_sevaction3.Document.ContentStart, richtext_sevaction3.Document.ContentEnd).Text);
            
            //TextRange tr3 = new TextRange(richtext_sevaction3.Document.ContentStart, richtext_sevaction3.Document.ContentEnd);
            //MemoryStream ms3 = new MemoryStream();
            //tr3.Save(ms3, DataFormats.Xaml);
            //string xamlText3 = ASCIIEncoding.Default.GetString(ms3.ToArray());
            //cmd.Parameters.AddWithValue("@action3", xamlText3);


            string xamlText3 = ConvertToXaml(richtext_sevaction3);
            cmd.Parameters.AddWithValue("@action3", xamlText3);



            //cmd.Parameters.AddWithValue("@action4", new TextRange(richtext_sevaction4.Document.ContentStart, richtext_sevaction4.Document.ContentEnd).Text);
            //TextRange tr4 = new TextRange(richtext_sevaction4.Document.ContentStart, richtext_sevaction4.Document.ContentEnd);
            //MemoryStream ms4 = new MemoryStream();
            //tr4.Save(ms4, DataFormats.Xaml);
            //string xamlText4 = ASCIIEncoding.Default.GetString(ms4.ToArray());
            //cmd.Parameters.AddWithValue("@action4", xamlText4);


            string xamlText4 = ConvertToXaml(richtext_sevaction4);
            cmd.Parameters.AddWithValue("@action4", xamlText4);
            //cmd.Parameters.AddWithValue("@action1", new TextRange(richtext_sevaction1.Document.ContentStart, richtext_sevaction1.Document.ContentEnd).Text);
            //cmd.Parameters.AddWithValue("@action2", new TextRange(richtext_sevaction2.Document.ContentStart, richtext_sevaction2.Document.ContentEnd).Text);
            //cmd.Parameters.AddWithValue("@action3", new TextRange(richtext_sevaction3.Document.ContentStart, richtext_sevaction3.Document.ContentEnd).Text);
            //cmd.Parameters.AddWithValue("@action4", new TextRange(richtext_sevaction4.Document.ContentStart, richtext_sevaction4.Document.ContentEnd).Text);


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

        public bool IsRichTextBoxEmpty( RichTextBox rtb) {
            var start = rtb.Document.ContentStart;
            var end = rtb.Document.ContentEnd;
            int difference = start.GetOffsetToPosition(end);

            if (difference <= 4)
            {
                return true;
            }
            else {
                return false;
            }
        
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {


            //WPFMessageBoxResult __Result = WPFMessageBox.Show("This options displays a message box with only message. The will be the message box with minimal options!!!");

            if (String.IsNullOrEmpty(textbox_customerid.Text) && String.IsNullOrEmpty(textbox_customername.Text)) {
                WPFMessageBox.Show("Error", "Error, Customer ID and Customer Name cannot be empty!", WPFMessageBoxButtons.OK, WPFMessageBoxImage.Error);
                return;
            }
            else if (String.IsNullOrEmpty(textbox_customerid.Text))
            {
                WPFMessageBox.Show("Error", "Error, Customer ID cannot be empty!", WPFMessageBoxButtons.OK, WPFMessageBoxImage.Error);
                return;
            }
            else if (String.IsNullOrEmpty(textbox_customername.Text))
            {
                WPFMessageBox.Show("Error", "Error, Customer Name cannot be empty!", WPFMessageBoxButtons.OK, WPFMessageBoxImage.Error);
                return;
            }
            else {
                List<string> emptyfields = new List<string> ();
                if(String.IsNullOrEmpty(textbox_region.Text)) {
                    emptyfields.Add("Region");
                    basicinfo_warning.Visibility = System.Windows.Visibility.Visible;
                }
                if(String.IsNullOrEmpty(textbox_offset.Text)){
                    emptyfields.Add("Offset");
                    basicinfo_warning.Visibility = System.Windows.Visibility.Visible;
                }
                if(String.IsNullOrEmpty(textbox_queue.Text)){
                    emptyfields.Add("Queue");
                    basicinfo_warning.Visibility = System.Windows.Visibility.Visible;
                }

                if (holidays.Count == 0) {
                    emptyfields.Add("Holidays");
                    holidays_warning.Visibility = System.Windows.Visibility.Visible;
                }

                if (supportkeys.Count == 0)
                {
                    emptyfields.Add("Support keys");
                    supportkey_warning.Visibility = System.Windows.Visibility.Visible;
                }
                if(IsRichTextBoxEmpty(richtext_sevaction1) || IsRichTextBoxEmpty(richtext_sevaction2) || IsRichTextBoxEmpty(richtext_sevaction3) || IsRichTextBoxEmpty(richtext_sevaction4) )
                {
                    emptyfields.Add("One or more Severity Actions");
                    sevactions_warning.Visibility = System.Windows.Visibility.Visible;
                }
                if (IsRichTextBoxEmpty(richtextbox_guidelines))
                {
                    emptyfields.Add("Case handling guidelines");
                    casehandling_warning.Visibility = System.Windows.Visibility.Visible;
                }

                if (customerinfos.Count == 0)
                {
                    emptyfields.Add("Contact Information");
                    contacts_warning.Visibility = System.Windows.Visibility.Visible;
                }

                string emptys = "";
                bool firstkeyword = true;
                foreach (string s in emptyfields) {

                    if (firstkeyword)
                    {
                        emptys += s;
                        firstkeyword = false;
                    }
                    else {
                        emptys = emptys + ", " +s;
                    }
                    
                }

                WPFMessageBoxResult __Result = WPFMessageBox.Show("Warning", emptys +" \nThese fields are missing. Do you wish to Continue?", WPFMessageBoxButtons.YesNoCancel, WPFMessageBoxImage.Alert);
                if (__Result == WPFMessageBoxResult.Yes)
                {

                }
                else {
                    return;
                }

            }


            if (IsCustomerExisting())
            {
                //MessageBoxResult result = MessageBox.Show("User exists in the databse. Would you like to overwrite?", "Error", MessageBoxButton.YesNo);

                WPFMessageBoxResult result = WPFMessageBox.Show("Overwrite?", "User exists in the databse. Would you like to overwrite?", WPFMessageBoxButtons.YesNo, WPFMessageBoxImage.Information);
                switch (result)
                {
                    case WPFMessageBoxResult.Yes:
                        {
                            DeleteCustmerFromdb(textbox_customerid.Text);

                            AddCustomer();

                            break;
                        }
                    case WPFMessageBoxResult.No:
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

            existingCustIDs = access.GetIDs();

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
                btnSubmit.Content = "Update";
                
                CustomerVM cvm = new CustomerVM(textbox_customerid.Text);


                textbox_customername.Text = cvm.Customer.customerName;
                textbox_offset.Text = cvm.Customer.offset.ToString();
                if (cvm.Support.FirstOrDefault() != null) {
                    textbox_queue.Text = cvm.Support.FirstOrDefault().queue;
                }
                
                textbox_region.Text = cvm.Customer.spptOrganization;


                //supported seveirities
                //ICollection<Supported_Severity> ssList = cvm.Customer.Supported_Severities;
                foreach(string ss in cvm.Severities){

                    switch (ss) { 
                        case "1" :
                            checkbox_sev1.IsChecked = true;
                            break;
                        case "2":
                            checkbox_sev2.IsChecked = true;
                            break;
                        case "3":
                            checkbox_sev3.IsChecked = true;
                            break;
                        case "4":
                            checkbox_sev4.IsChecked = true;
                            break;
                        default:
                            break;
                    
                    }
                }


                //support keys
                foreach (Support sk in cvm.Support) {
                    supportkeys.Add(new SupportKey(sk.supportKey.ToString(), sk.description));
                }

                //guidelines
                setRichText(richtextbox_guidelines,cvm.Guidelines);


                //sev actions
                setRichText(richtext_sevaction1, cvm.Actions.action1);
                setRichText(richtext_sevaction2, cvm.Actions.action2);
                setRichText(richtext_sevaction3, cvm.Actions.action3);
                setRichText(richtext_sevaction4, cvm.Actions.action4);
                
                //contacts
                foreach (Contact contact in cvm.Contacts) {
                    customerinfos.Add(new CustomerInfo(contact.name, contact.designation, contact.email, contact.workPhone, contact.mobile));
                    
                }


            }
            else
            {
                image_tick.Visibility = System.Windows.Visibility.Collapsed;
                ClearAllFields();
            }

        }

        private void textbox_customerid_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

            string text = textbox_customerid.Text;
            Access ac = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ESTProfiles.MDB");
            customer = ac.GetInfo(text);

            if (customer != null)
            {
                
                if (customer.Contacts.Count > 0)
                {
                    List<string> names = new List<string>();
                    foreach (var i in customer.Contacts.Distinct())
                    {
                        names.Add(i.name);
                    }
                    textbox_name.ItemsSource = names;



                }
            }


            if (!existingCustIDs.Contains(textbox_customerid.Text))
            {
                textbox_customername.Text = customer.customerName;
                textbox_offset.Text = customer.offset.ToString();
                textbox_region.Text = customer.spptOrganization;
                if (customer.Supports.Count > 0)
                {
                    textbox_queue.Text = customer.Supports.FirstOrDefault().queue;
                }
            }
        }


        private void textbox_customerid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                string text = textbox_customerid.Text;
                Access ac = new Access("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=ESTProfiles.MDB");
                customer = ac.GetInfo(text);

                if (customer != null)
                {

                    if (customer.Contacts.Count > 0)
                    {
                        List<string> names = new List<string>();
                        foreach (var i in customer.Contacts.Distinct())
                        {
                            names.Add(i.name);
                        }
                        textbox_name.ItemsSource = names;



                    }
                }


                if (!existingCustIDs.Contains(textbox_customerid.Text))
                {
                    textbox_customername.Text = customer.customerName;
                    textbox_offset.Text = customer.offset.ToString();
                    textbox_region.Text = customer.spptOrganization;
                    if (customer.Supports.Count > 0)
                    {
                        textbox_queue.Text = customer.Supports.FirstOrDefault().queue;
                    }
                }
            }
        }


        private void textbox_key_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string text = textbox_key.Text;
            if (!String.IsNullOrEmpty(text))
            {
                int key = int.Parse(text);
                var x = customer.Supports.Where(m => m.supportKey == key).FirstOrDefault();
                if (x != null)
                {
                    textbox_description.Text = x.description;
                }
            }
        }




        public void ClearAllFields() {
            textbox_customername.Text = "";
            textbox_offset.Text = "";

            textbox_queue.Text = "";
            

            textbox_region.Text = "";


            checkbox_sev1.IsChecked = false;

            checkbox_sev2.IsChecked = false;

            checkbox_sev3.IsChecked = false;

            checkbox_sev4.IsChecked = false;


            supportkeys.Clear();

            richtextbox_guidelines.Document.Blocks.Clear();

            richtext_sevaction1.Document.Blocks.Clear();
            richtext_sevaction2.Document.Blocks.Clear();
            richtext_sevaction3.Document.Blocks.Clear();
            richtext_sevaction4.Document.Blocks.Clear();


            customerinfos.Clear();


            btnSubmit.Content = "Add new customer";
        }



        public void setRichText(RichTextBox rtb, string txt) {

            //StringReader sr = new StringReader(txt);
            //XmlReader reader = XmlReader.Create(sr);
            //Section sec = (Section)XamlReader.Load(reader);
            //FlowDocument d = new FlowDocument();


            //while (sec.Blocks.Count > 0)
            //{
            //    var block = sec.Blocks.FirstBlock;
            //    sec.Blocks.Remove(block);
            //    d.Blocks.Add(block);
            //}
            //rtb.Document = d;


            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(txt));
            rtb.Selection.Load(stream, DataFormats.Rtf);
        
        }

        private void textbox_key_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
//            e.Handled = !IsTextAllowed(e.Text);
            e.Handled = !AreAllValidNumericChars(e.Text);
            base.OnPreviewTextInput(e);

        }

        private bool AreAllValidNumericChars(string str)
        {
            foreach (char c in str)
            {
                if (!Char.IsNumber(c)) return false;
            }

            return true;
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }





        private void PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text)) e.CancelCommand();
            }
            else e.CancelCommand();
        }

        //public ObservableCollection<SupportKey> supportkeys { get; set; }
        //public ObservableCollection<CustomerInfo> customerinfos { get; set; }


        private void textbox_name_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            string text = textbox_name.Text;
            if (!String.IsNullOrEmpty(text)) 
            {
                var x = customer.Contacts.Where(m => m.name == text).FirstOrDefault();
                if (x != null)
                {
                    //textbox_desingnation.Text = x.designation;
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
            }
        }

        private void textbox_name_DropDownClosed(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (!String.IsNullOrEmpty(textbox_name.Text)) {
                string text = textbox_name.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    var x = customer.Contacts.Where(m => m.name == text).FirstOrDefault();
                    if (x != null)
                    {
                        //textbox_desingnation.Text = x.designation;
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
                }
            }

        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {

            holidayDay = Convert.ToInt32((txtNum.Text));
            if (holidayDay >= 31)
            {
                holidayDay = 1;
            }
            else {
                holidayDay++;
            }
            

            txtNum.Text = holidayDay.ToString();
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            holidayDay = Convert.ToInt32((txtNum.Text));

            if (holidayDay <= 0)
            {
                holidayDay = 31;
            }
            else
            {
                holidayDay--;
            }
            
            txtNum.Text = holidayDay.ToString();
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AreAllValidNumericChars(txtNum.Text)) {
                txtNum.Text = "1";
            }

            if (txtNum.Text == "")
            {
                txtNum.Text = "1";
            }


            if (Convert.ToInt32(txtNum.Text) < 1 || Convert.ToInt32(txtNum.Text) > 31)
            {
                txtNum.Text = "1";
            }
        }

        private void monthUp_Click(object sender, RoutedEventArgs e)
        {

            holidayMonth = Convert.ToInt32((txtMonth.Text));
            if (holidayMonth == 12)
            {
                holidayMonth = 1;
            }
            else
            {
                holidayMonth++;
            }


            txtMonth.Text = holidayMonth.ToString();
        }

        private void monthDown_Click(object sender, RoutedEventArgs e)
        {

            holidayMonth = Convert.ToInt32((txtMonth.Text));
            if (holidayMonth == 1)
            {
                holidayMonth = 12;
            }
            else
            {
                holidayMonth--;
            }


            txtMonth.Text = holidayMonth.ToString();
        }

        private void yearUp_Click(object sender, RoutedEventArgs e)
        {
            holidayYear = Convert.ToInt32(txtYear.Text);
            holidayYear++;


            txtYear.Text = holidayYear.ToString();
        }

        private void yearDown_Click(object sender, RoutedEventArgs e)
        {
            holidayYear = Convert.ToInt32(txtYear.Text);
            holidayYear--;
            

            txtYear.Text = holidayYear.ToString();

        }

        private void txtMonth_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!AreAllValidNumericChars(txtMonth.Text))
            {
                txtMonth.Text = "1";
            }

            if (txtMonth.Text == "")
            {
                txtMonth.Text = "1";
            }

            if (Convert.ToInt32(txtMonth.Text) < 1 || Convert.ToInt32(txtMonth.Text) > 12)
            {
                txtMonth.Text = "1";
            }
        }

        private void txtYear_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsTextAllowed(txtYear.Text))
            {
                txtYear.Text = "2000";
            }
            if (txtYear.Text == "")
            {
                txtYear.Text = "2000";
            }
            if (Convert.ToInt32(txtYear.Text) < 0 || Convert.ToInt32(txtYear.Text) > 3000)
            {
                txtYear.Text = "2000";
            }
        }

        private void button_addholiday_click(object sender, RoutedEventArgs e)
        {
            DateTime thisDate1;

            string holiday = txtYear.Text + "-" + txtMonth.Text + "-" + txtNum.Text;

            if (DateTime.TryParse(holiday, out thisDate1))
            {
                if (!holidays.Contains(holiday)) {
                    holidays.Add(holiday);
                }

                
                Console.WriteLine("success");
            }
            else {
                MessageBox.Show("Invalid date");
            }


        }

        private void btnDeleteHoliday_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var skey = button.DataContext as string;
                //svm.supportkeys.Remove(skey);
                holidays.Remove(skey);
            }
            else
            {
                return;
            }
        }

        
            //var split = text.Split(new char[] { '(' }, 2);
            //text = split[0];

            //textbox_description.Text = customer.Supports.Where(m => m.supportKey == key).SingleOrDefault().description;
            
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using display.Properties;
using display.Model;
using System.Data.OleDb;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.IO;

namespace display.ViewModel
{
    class CustomerVM
    {
        public CustomerVM(string id)
        {
            _customer = Search(id);
            //if (String.IsNullOrEmpty(_customer.customerId))
            //{
            //    //MessageBox.Show("Customer does not exist in the database");
            //    Class1.visibility = Visibility.Visible;
            //}
            //else
            //{
            //    Class1.visibility = Visibility.Collapsed;
            //}

            _sev = _customer.Supported_Severities.ToList();
            _support = _customer.Supports.ToList();
            foreach (Supported_Severity i in _sev)
            {
                severities.Add(i.severity.ToString());
            }
            if (_customer.Guideline == null)
            {
                _guidelines = "Guidelines are not given";
            }
            else
            {
                _guidelines = _customer.Guideline.guidelines.ToString();
            }

            _holidays = _customer.Holidays.Select(m => m.holiday).ToList();
            _sevActions = _customer.Severity_Action;
            _contacts = _customer.Contacts.ToList();
        }

        Customer _customer = new Customer();
        List<Supported_Severity> _sev = new List<Supported_Severity>();
        List<string> severities = new List<string>();
        List<Support> _support = new List<Support>();
        string _guidelines;
        List<DateTime> _holidays = new List<DateTime>();
        Severity_Action _sevActions = new Severity_Action();
        List<Contact> _contacts = new List<Contact>();

        public Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public string Name
        {
            get { return Customer.customerName; }
            set { Customer.customerName = value; }
        }

        public string Info
        {
            get { return Customer.customerName + " (" + Customer.customerId + ")"; }
        }

        public decimal? Offset
        {
            get { return Customer.offset; }
            set { Customer.offset = value; }
        }

        public string SuppOrg
        {
            get { return Customer.spptOrganization; }
            set { Customer.spptOrganization = value; }
        }

        public string DocPath
        {
            get { return Customer.documentPath; }
            set { Customer.documentPath = value; }
        }

        public List<string> Severities
        {
            get { return severities; }
            set { severities = value; }
        }

        public List<Support> Support
        {
            get { return _support; }
            set { _support = value; }
        }

        public string Guidelines
        {
            get { return _guidelines; }
            set { _guidelines = value; }
        }

        public List<DateTime> Holidays
        {
            get { return _holidays; }
            set { _holidays = value; }
        }

        public Severity_Action Actions
        {
            get { return _sevActions; }
            set { _sevActions = value; }
        }

        public List<Contact> Contacts
        {
            get { return _contacts; }
            set { _contacts = value; }
        }

        public Customer Search(string searchString)
        {
            Access access = new Access();

            Customer customer = new Customer();
            if (!String.IsNullOrEmpty(searchString))
            {
                namesVM names = new namesVM();

                //string idAndName = names.names.ConvertAll(d => d.ToLower()).Where(m => m.Contains(searchString.ToLower())).FirstOrDefault().ToString();
                var split = searchString.Split(new char[] { ' ' }, 2);
                string number = split[0];
                customer = access.GetData(number);
            }

            return customer;
        }

        //    private string GetTextBlockXAMLContent(TextBlock textBlock)
        //    {
        //        // Get the content being displayed
        //        TextRange range = new TextRange(textBlock.ContentStart, textBlock.ContentEnd);

        //        // Save the content to stream and get it back as byte array
        //        MemoryStream stream = new MemoryStream();
        //        range.Save(stream, DataFormats.Xaml);
        //        byte[] bytes = stream.ToArray();

        //        // Convert to character and rebuild it into a string
        //        StringBuilder stringBuilder = new StringBuilder(bytes.Length);
        //        List<char> chars = new List<char>(bytes.Select(b => Convert.ToChar(b)));
        //        chars.ForEach(ch => stringBuilder.Append(ch));

        //        return stringBuilder.ToString();
        //    }
        //}
    }
}

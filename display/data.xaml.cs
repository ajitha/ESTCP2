using System;
using System.Globalization;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using display.Model;
using display.ViewModel;
using System.Diagnostics;

namespace display
{
    /// <summary>
    /// Interaction logic for details.xaml
    /// </summary>
    public partial class data : UserControl
    {
        public data(string id)
        {
            CustomerVM customer = new CustomerVM(id);
            if (String.IsNullOrEmpty(customer.Name))
            {
                Common.error = true;               
            }
            else
            {
                Common.error = false;
                base.DataContext = customer;
                InitializeComponent();


               

                

                //namesVM names = new namesVM();
                //atb_id.ItemsSource = names.Names;
                DispatcherTimer messageTimer = new DispatcherTimer();
                messageTimer.Tick += (sender, eventArgs) =>
                {
                    DateTime time = DateTime.UtcNow.AddHours(Convert.ToDouble(customer.Offset));
                    label_time.Content = time.ToShortTimeString();
                    label_date.Content = time.ToString("d", CultureInfo.CreateSpecificCulture("en-GB"));
                    string status = Status(time, customer.Holidays);
                    label_stat.Content = status;
                };
                messageTimer.Interval = new TimeSpan(0, 0, 1);
                messageTimer.Start();

                FlowDocument doc = new FlowDocument();
                doc = Severities(customer.Severities);
                this.richTextBox_sevSupp.Document = doc;
                doc = Support(customer.SuppOrg, customer.Support);
                this.richTextBox_keys.Document = doc;
                doc = SevActions(customer.Actions);
                this.richTextBox_actions.Document = doc;
                doc = Contacts(customer.Contacts);
                this.richTextBox_contacts.Document = doc;

                //StringReader sr = new StringReader(customer.Guidelines);
                //XmlReader reader = XmlReader.Create(sr);
                //Section sec = (Section)XamlReader.Load(reader);
                //FlowDocument d = new FlowDocument();





                //while (sec.Blocks.Count > 0)
                //{
                //    var block = sec.Blocks.FirstBlock;
                //    sec.Blocks.Remove(block);
                //    d.Blocks.Add(block);
                //}
                //this.richTextBox_guidelines.Document = d;
                

                //new mod
                richTextBox_guidelines.IsDocumentEnabled = true;
                richTextBox_guidelines.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate));

                //richTextBox_guidelines.Inlines.Add(XamlReader.Parse(customer.Guidelines) as Inline);
            }
            

        }
       

        //new mod
        private void HandleRequestNavigate(object sender, RoutedEventArgs args)
        {
            if (args is RequestNavigateEventArgs)
                Process.Start(((RequestNavigateEventArgs)args).Uri.ToString());
        }

        public void Richtext(FlowDocument d, string content, string title)
        {
            StringReader sr = new StringReader(content);
            XmlReader reader = XmlReader.Create(sr);
            Section sec = (Section)XamlReader.Load(reader);
            while (sec.Blocks.Count > 0)
            {
                var block = sec.Blocks.FirstBlock;
                sec.Blocks.Remove(block);
                d.Blocks.Add(new List(new ListItem(new Paragraph(new Run(title)))));
                d.Blocks.Add(block);
            }
        }

        public FlowDocument Severities(List<string> severities)
        {
            FlowDocument doc = new FlowDocument();
            List list = new List();
            if (severities.Count == 0)
            {
                list.ListItems.Add(new ListItem(new Paragraph(new Run("Severities are not assigned"))));
            }
            else
            {
                foreach (string i in severities)
                {
                    Paragraph para = new Paragraph(new Run(i));
                    list.ListItems.Add(new ListItem(para));
                }
            }
            doc.Blocks.Add(list);
            return doc;
        }
        
        public FlowDocument SevActions(Severity_Action actions)
        {
            FlowDocument doc = new FlowDocument();
            List list = new List();

            if (!String.IsNullOrEmpty(actions.action1))
            {
                //doc.Blocks.Add(new List(new ListItem(new Paragraph(new Run("Severity 1: ")))));
                //doc.Blocks.Add(list);
                //list.ListItems.Add(new ListItem(Richtext(doc, guideline)));
                Richtext(doc, actions.action1, "Severity 1: ");

            }
            if (!String.IsNullOrEmpty(actions.action2))
            {
                //doc.Blocks.Add(new List(new ListItem(new Paragraph(new Run("Severity 1: ")))));
                //doc.Blocks.Add(list);
                //list.ListItems.Add(new ListItem(Richtext(doc, guideline)));
                Richtext(doc, actions.action1, "Severity 2: ");

            }

            //if (!String.IsNullOrEmpty(actions.action1))
            //{
            //   Paragraph  para = new Paragraph(new Run("Severity 1: " + actions.action1));
            //   list.ListItems.Add(new ListItem(para)); 
            //   //doc.Blocks.Add(para);
            //}
            //if (!String.IsNullOrEmpty(actions.action2))
            //{
            //    Paragraph para = new Paragraph(new Run("Severity 2: " + actions.action2));
            //    list.ListItems.Add(new ListItem(para)); 
            //    //doc.Blocks.Add(para);
            //}
            if (!String.IsNullOrEmpty(actions.action3))
            {
                Paragraph para = new Paragraph(new Run("Severity 3: " + actions.action3));
                list.ListItems.Add(new ListItem(para));
                //doc.Blocks.Add(para);
            }
            if (!String.IsNullOrEmpty(actions.action4))
            {
                Paragraph para = new Paragraph(new Run("Severity 4: " + actions.action4));
                list.ListItems.Add(new ListItem(para)); 
                //doc.Blocks.Add(para);
            }
            if (!String.IsNullOrEmpty(actions.additionalInfo))
            {
                Paragraph para = new Paragraph(new Run("Additional Information: " + actions.additionalInfo));
                list.ListItems.Add(new ListItem(para)); 
                //doc.Blocks.Add(para);
            }
            doc.Blocks.Add(list);
            return doc;
            
        }

        public FlowDocument Support(string suppOrg, List<Support> support)
        {
            FlowDocument doc = new FlowDocument();
            doc.PagePadding = new Thickness(0, 10, 10, 10);
            if (suppOrg != null)
            {
                List list = new List();
                list.ListItems.Add(new ListItem(new Paragraph(new Run("Support Organization: " + suppOrg))));
                doc.Blocks.Add(list);
            }
            else
            {
                List list = new List();
                list.ListItems.Add(new ListItem(new Paragraph(new Run("Support Organization is not given"))));
                doc.Blocks.Add(list);
            }

            if (support.Count == 0)
            {
                List list = new List();
                list.ListItems.Add(new ListItem(new Paragraph(new Run("Support Keys are not given"))));
                doc.Blocks.Add(list);
            }
            else
            {
                Table table = new Table();
                doc.Blocks.Add(table);

                table.CellSpacing = 0;
                int noCol = 3;
                for (int x = 0; x < noCol; x++)
                {
                    table.Columns.Add(new TableColumn());
                }
                table.RowGroups.Add(new TableRowGroup());
                table.RowGroups[0].Rows.Add(new TableRow());
                TableRow header = table.RowGroups[0].Rows[0];
                header.FontWeight = FontWeights.Bold;
                header.Background = Brushes.Silver;
                header.Cells.Add(new TableCell(new Paragraph(new Run("Support Key"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Activity Description"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Queue"))));

                int j = 1;
                foreach (Support i in support)
                {
                    table.RowGroups[0].Rows.Add(new TableRow());
                    TableRow row = table.RowGroups[0].Rows[j];
                    row.FontWeight = FontWeights.Normal;
                    TableCell cell;
                    Thickness tn;

                    tn = new Thickness(1, 0, 0, 1);
                    cell = CreateCell(i.supportKey.ToString(), tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 0, 1);
                    cell = CreateCell(i.description, tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 1, 1);
                    cell = CreateCell(i.queue, tn);
                    row.Cells.Add(cell);

                    j++;
                }
            }
            return doc;
        }

        public FlowDocument Contacts(List<Contact> contacts)
        {
            FlowDocument doc = new FlowDocument();
            if (contacts.Count == 0)
            {
                List list = new List();
                list.ListItems.Add(new ListItem(new Paragraph(new Run("No contact details"))));
                doc.Blocks.Add(list);
            }
            else
            {
                Table table = new Table();
                doc.Blocks.Add(table);

                table.CellSpacing = 0;
                int noCol = 5;
                for (int x = 0; x < noCol; x++)
                {
                    table.Columns.Add(new TableColumn());
                }
                table.RowGroups.Add(new TableRowGroup());
                table.RowGroups[0].Rows.Add(new TableRow());
                TableRow header = table.RowGroups[0].Rows[0];
                header.FontWeight = FontWeights.Bold;
                header.Background = Brushes.Silver;
                header.Cells.Add(new TableCell(new Paragraph(new Run("Name"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Designation"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Email"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Work Phone"))));
                header.Cells.Add(new TableCell(new Paragraph(new Run("Mobile"))));

                int j = 1;
                foreach (Contact i in contacts)
                {
                    table.RowGroups[0].Rows.Add(new TableRow());
                    TableRow row = table.RowGroups[0].Rows[j];
                    row.FontWeight = FontWeights.Normal;
                    TableCell cell;
                    Thickness tn;

                    tn = new Thickness(1, 0, 0, 1);
                    cell = CreateCell(i.name, tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 0, 1);
                    cell = CreateCell(i.designation, tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 0, 1);
                    cell = CreateCell(i.email, tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 0, 1);
                    cell = CreateCell(i.workPhone, tn);
                    row.Cells.Add(cell);
                    tn = new Thickness(0, 0, 1, 1);
                    cell = CreateCell(i.mobile, tn);
                    row.Cells.Add(cell);
                    j++;
                }
            }
                        
            return doc;
        }

        public TableCell CreateCell(string content, Thickness thickness)
        {
            TableCell cell = new TableCell();
            if (content != null)
            {
                cell = new TableCell(new Paragraph(new Run(content)));
            }
            else
            {
                cell = new TableCell(new Paragraph(new Run("")));
            }
                cell.BorderThickness = thickness;
                cell.BorderBrush = new SolidColorBrush(Colors.Silver);
            
            return cell;
        }

        public string Status(DateTime now, List<DateTime> holidays)
        {
            string status;
            if (holidays.Contains(now))
            {
                status = "Holiday";
            }
            else
            {
                DateTime t2 = Convert.ToDateTime("8:00:00 AM");
                DateTime t3 = Convert.ToDateTime("5:00:00 PM");
                int before = DateTime.Compare(now, t2);
                int after = DateTime.Compare(now, t3);
                if (before > 0 && after < 0)
                {
                    status = "Online";
                }
                else
                {
                    status = "Offline";
                }
            }

            return status;
        }


    }

}


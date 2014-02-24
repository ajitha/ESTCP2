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
                if (customer.Offset == -50)
                {
                    //label_region.Visibility = Visibility.Collapsed;
                    label_time.Content = "N/A";
                }
                else
                {
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
                }
                

                FlowDocument doc = new FlowDocument();
                doc = Severities(customer.Severities);
                this.richTextBox_sevSupp.Document = doc;
                doc = Support(customer.SuppOrg, customer.Support);
                this.richTextBox_keys.Document = doc;
                doc = SevActions(customer.Actions);
                //this.richTextBox_actions.Document = doc;
                doc = Contacts(customer.Contacts);
                this.richTextBox_contacts.Document = doc;

                richTextBox_guidelines.IsDocumentEnabled = true;
                richTextBox_guidelines.AddHandler(Hyperlink.RequestNavigateEvent, new RoutedEventHandler(this.HandleRequestNavigate));
                FlowDocument d = new FlowDocument();
                if (!String.IsNullOrEmpty(customer.Guidelines))
                {
                    Richtext(richTextBox_guidelines, customer.Guidelines, null , null);
                }
                
                //richTextBox_guidelines.Document = d;
                
            }
            

        }
       

        //new mod
        private void HandleRequestNavigate(object sender, RoutedEventArgs args)
        {
            if (args is RequestNavigateEventArgs)
                Process.Start(((RequestNavigateEventArgs)args).Uri.ToString());
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

        public void Richtext(RichTextBox rtb, string content, string title, Label label)
        {
            if (!String.IsNullOrEmpty(title))
            {
                label.Visibility = Visibility.Visible;
            }
            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(content));
            //TextRange textRange = new TextRange(d.ContentEnd, d.ContentEnd);
            rtb.Selection.Load(stream, DataFormats.Rtf);
            rtb.Visibility = Visibility.Visible;

        }


        public FlowDocument SevActions(Severity_Action actions)
        {
            FlowDocument doc = new FlowDocument();
            if (!String.IsNullOrEmpty(actions.action1))
            {
               // Richtext(doc, actions.action1, "• Severity 1: ");
                Richtext(richTextBox_actions1, actions.action1, "Severity 1: ", S1);
            }
            if (!String.IsNullOrEmpty(actions.action2))
            {
                Richtext(richTextBox_actions2, actions.action2, "Severity 2: ", S2);
            }
            if (!String.IsNullOrEmpty(actions.action3))
            {
                Richtext(richTextBox_actions3, actions.action3, "Severity 3: ",S3);
            }
            if (!String.IsNullOrEmpty(actions.action4))
            {
                Richtext(richTextBox_actions4, actions.action4, "Severity 4: ",S4);
            }
            if (!String.IsNullOrEmpty(actions.additionalInfo))
            {
                Richtext(richTextBox_actions_a, actions.additionalInfo, "Additional Information: ", S_a);
            }
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


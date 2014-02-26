using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using display.Model;

namespace display
{
    class Access
    {
        public Access(string connectionString)
        {
            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=|DataDirectory|\\ESTProfiles.mdb";
            //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=profiles.mdb";

#if USINGPROJECTSYSTEM
		string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=..\\..\\profiles.MDB";
#else
            string connString = connectionString;
#endif

            this.conn = new OleDbConnection(connString);
            conn.Open();
            DataSet ds = new DataSet();
        }

        public OleDbConnection conn{ get; set; }
        public DataSet ds { get; set; }
        
        private OleDbDataReader ReadData(string query, string table)
        {
            OleDbCommand command = new OleDbCommand(query, conn);
            if (conn.State.Equals(ConnectionState.Closed))
            {
                conn.Open();
            }
            var reader = command.ExecuteReader();
            return reader;
        }

        public List<string> GetNames()
        {
            string query = "select customerId, customerName from Customer";
            List<string> namesList = new List<string>();
            string name;
            OleDbCommand command = new OleDbCommand(query, conn);
            if(conn.State.Equals(ConnectionState.Closed))
            {
                conn.Open();
            }
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                name = reader[0].ToString() + " " + reader[1].ToString();
                namesList.Add(name);
            }
            return namesList;
        }
        public List<List<string>> GetRegions()
        {
            string query = "select distinct suppOrg, offset from Customer";
            List<List<string>> items = new List<List<string>>();
            OleDbCommand command = new OleDbCommand(query, conn);
            if (conn.State.Equals(ConnectionState.Closed))
            {
                conn.Open();
            }
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                List<string> entry = new List<string>();
                entry.Add(reader[0].ToString());
                entry.Add(reader[1].ToString());

                items.Add(entry);
            }
            return items;
        }
    
        //new mod
        public List<string> GetIDs()
        {
            string query = "select customerId from Customer";
            List<string> IDList = new List<string>();
            OleDbCommand command = new OleDbCommand(query, conn);
            if (conn.State.Equals(ConnectionState.Closed))
            {
                conn.Open();
            }
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
               // name = reader[0].ToString() + " " + reader[1].ToString();
                IDList.Add(reader[0].ToString());
            }
            return IDList;
        }  


        public Customer GetData(string id)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            OleDbDataReader reader;

            reader = ReadData("select * from Customer where customerId = '" + id + "'", "Customer");
            Customer customer = new Customer();
            while(reader.Read())
            {
                customer.customerName = reader["customerName"].ToString();
                customer.spptOrganization = reader["spptOrganization"].ToString();
                customer.offset = decimal.Parse(reader["offset"].ToString());
                customer.documentPath = reader["documentPath"].ToString();
                customer.customerId = reader["customerId"].ToString();
            }

            reader = ReadData("select * from Support where customerId = '" + id + "'", "Support");
            while (reader.Read())
            {
                Support support = new Support();
                support.supportKey = int.Parse(reader["supportKey"].ToString());
                support.customerId = reader["customerId"].ToString();
                support.description = reader["description"].ToString();
                support.queue = reader["queue"].ToString();
                customer.Supports.Add(support);
            }
            reader = ReadData("select * from Contacts where customerId = '" + id + "'", "Contacts");
            while (reader.Read())
            {
                Contact contact = new Contact();
                contact.contactId = int.Parse(reader["contactId"].ToString());
                contact.customerId = reader["customerId"].ToString();
                contact.name = reader["name"].ToString();
                contact.designation = reader["designation"].ToString();
                contact.email = reader["email"].ToString();
                contact.workPhone = reader["workphone"].ToString();
                contact.mobile = reader["mobile"].ToString();
                contact.additionalInfo = reader["additionalInfo"].ToString();
                customer.Contacts.Add(contact);
            }

            reader = ReadData("select * from Guidelines where customerId = '" + id + "'", "Guidelines");
            while (reader.Read())
            {
                Guideline guidelines = new Guideline();
                var g = reader["guidelines"];
                guidelines.customerId = reader["customerId"].ToString();
                guidelines.guidelines = reader["guidelines"].ToString();
                customer.Guideline = guidelines;
                
            }

            reader = ReadData("select * from Holiday where customerId = '" + id + "'", "Holiday");
            while (reader.Read())
            {
                Holiday holiday = new Holiday();
                holiday.customerId = reader["customerId"].ToString();
                holiday.holiday = Convert.ToDateTime(reader["holiday"].ToString());
                customer.Holidays.Add(holiday);

            }

            reader = ReadData("select * from SeverityAction where customerId = '" + id + "'", "SeverityAction");
            while (reader.Read())
            {
                Severity_Action actions = new Severity_Action();
                actions.customerId = reader["customerId"].ToString();
                actions.action1 = reader["action1"].ToString();
                actions.action2 = reader["action2"].ToString();
                actions.action3 = reader["action3"].ToString();
                actions.action4 = reader["action4"].ToString();
                actions.additionalInfo = reader["additionalInfo"].ToString();
                customer.Severity_Action = actions;

            }

            reader = ReadData("select * from SupportedSeverities where customerId = '" + id + "'", "SupportedSeverities");
            while (reader.Read())
            {
                Supported_Severity supported_sev = new Supported_Severity();
                supported_sev.customerId = reader["customerId"].ToString();
                supported_sev.severity = int.Parse(reader["severity"].ToString());
                customer.Supported_Severities.Add(supported_sev);
            }
            
            return customer;
        }

        public Customer GetInfo(string id)
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
            OleDbDataReader reader;

            reader = ReadData("select * from Customer where customerId = '" + id + "'", "Customer");
            Customer customer = new Customer();
            while (reader.Read())
            {
                customer.customerName = reader["customerName"].ToString();
                customer.spptOrganization = reader["suppOrg"].ToString();
                customer.offset = decimal.Parse(reader["offset"].ToString());
                customer.customerId = reader["customerId"].ToString();
            }

            reader = ReadData("select * from Support where customerId = '" + id + "'", "Support");
            while (reader.Read())
            {
                Support support = new Support();
                support.supportKey = int.Parse(reader["supportKey"].ToString());
                support.customerId = reader["customerId"].ToString();
                support.description = reader["description"].ToString();
                support.queue = reader["queue"].ToString();
                customer.Supports.Add(support);
            }
            reader = ReadData("select * from Contacts where customerId = '" + id + "'", "Contacts");
            while (reader.Read())
            {
                Contact contact = new Contact();
                contact.contactId = int.Parse(reader["contactId"].ToString());
                contact.customerId = reader["customerId"].ToString();
                contact.name = reader["customerName"].ToString();
                contact.designation = reader["designation"].ToString();
                contact.email = reader["email"].ToString();
                contact.workPhone = reader["workPhone"].ToString();
                contact.mobile = reader["mobile"].ToString();
                customer.Contacts.Add(contact);
            }

            return customer;
        }






       
       
    }

}

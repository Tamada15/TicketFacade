using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace HomeWork
{
    public partial class Form1 : MaterialForm
    {
        Client client;
        Form2 form2;
        public Form1()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue900, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
            client = new Client();
            
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            monthCalendar1.Visible = true;
        }

        private void monthCalendar1_MouseLeave(object sender, EventArgs e)
        {
            monthCalendar1.Visible = false;
        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            monthCalendar2.Visible = true;
        }

        private void monthCalendar2_MouseLeave(object sender, EventArgs e)
        {
            monthCalendar2.Visible = false;
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            materialTextBox3.Text = monthCalendar1.SelectionRange.Start.ToString("dd/MM/yyyy");
            client.DateStart = monthCalendar1.SelectionRange.Start;
            monthCalendar2.MinDate = monthCalendar1.SelectionRange.Start;
            materialButton2.Enabled = true;

        }

        private void monthCalendar2_DateSelected(object sender, DateRangeEventArgs e)
        {
            materialTextBox4.Text = monthCalendar2.SelectionRange.Start.ToString("dd/MM/yyyy");
            client.DateEnd = monthCalendar2.SelectionRange.Start;
        }
        private void materialButton3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(materialTextBox1.Text)  || string.IsNullOrEmpty(materialTextBox3.Text) || string.IsNullOrEmpty(materialTextBox4.Text) || string.IsNullOrEmpty(materialComboBox1.SelectedItem.ToString()))
            {
                MessageBox.Show("Введите все значения");
            }

            else
            {
                form2 = new Form2(client,this);
                client.City = materialComboBox1.SelectedItem.ToString();
                form2.ShowDialog();
            }
        }

        private void materialTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();

            if (!Regex.Match(Symbol, @"[а-яА-Я]|[a-zA-Z]|[' ']").Success)
            {
                e.Handled = true;
            }
        }


        private void materialTextBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            string Symbol = e.KeyChar.ToString();

            if (!Regex.Match(Symbol, @"[а-яА-Я]|[a-zA-Z]|[' ']").Success)
            {
                e.Handled = true;
            }
        }

        private void materialTextBox1_TextChanged(object sender, EventArgs e)
        {
            client.name = materialTextBox1.Text;
        }

        public MaterialTextBox GetTextBox()
        {
           return materialTextBox1; 
        }









        


    }

    public class Client
    {
        public string name;
        public DateTime DateStart;
        public DateTime DateEnd;
        public string City;
        public Client()
        {
        }

        public void Sort(Facade facade)
        {
            facade.AirSort(this);
        }

    }


    public class Ticket
    {
        public string NameCity;
        public DateTime dateTime;
        public string index;
        public Ticket(string NameCity, string Departure,string index)
        {
            dateTime = new DateTime();
            this.NameCity = NameCity;
            dateTime = DateTime.Parse(Departure);
            this.index = index;
        }
    }

    public class Air
    {
        public List<Ticket> airs;
        public Form2 Forms;
        public Air(Form2 form2)
        {
            airs = new List<Ticket>();
            string Name;
            string Date;
            string index; 
            string connectionString = "initial Catalog=Air;Server=(localdb)\\MSSQLLocalDB";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "Output_Ticket_All";
            command.Connection = sqlConnection;
            SqlDataReader sqlDataReader = command.ExecuteReader();
            while (sqlDataReader.Read())
            {
                index = sqlDataReader.GetValue(0).ToString();
                Name = sqlDataReader.GetValue(1).ToString();
                Date = sqlDataReader.GetValue(2).ToString();
                
                Ticket ticket = new Ticket(Name,Date,index);
                airs.Add(ticket);
            }
            sqlConnection.Close();
            Forms = form2;
        }
        public void Sort(Client client)
        {
            string index;
            for (int i = 0; i != airs.Count; i++)
            {
                if (airs[i].dateTime.IsInRange(client.DateStart, client.DateEnd))
                {
                    index = airs[i].index;
                    MaterialListBoxItem materialListBoxItem = new MaterialListBoxItem(airs[i].NameCity + " " + airs[i].dateTime.ToString("dd/MM/yyyy"),index);
                    Forms.materialListBox1.AddItem(materialListBoxItem);
                }
            }
            if (Forms.materialListBox1.Items.Count == 0)
            {
                MessageBox.Show("Билеты не найдены!");
                Application.Exit();
            }
        }
    }

    public class Facade
    {
        Air air;
        Form2 form2;
        public Facade(Form2 form2)
        {
            this.form2 = form2;
            air = new Air(this.form2);
        }
        public void AirSort(Client client)
        {
            air.Sort(client);
        }
    }


    public static class DateTimeExtensions
    {
        public static bool IsInRange(this DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return dateToCheck >= startDate && dateToCheck < endDate;
        }
    }
}

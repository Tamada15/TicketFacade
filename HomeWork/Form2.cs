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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HomeWork
{
     public partial class Form2 : MaterialForm
    {
        Client client;
        Form1 form1;
        public Form2(Client client,Form1 form)
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Blue800, Primary.Blue900, Primary.Blue500, Accent.LightBlue200, TextShade.WHITE);
            form1 = form;
            this.client = client;
            client.Sort(new Facade(this));
            /*TaskProgressBar();*/

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if(materialListBox1.SelectedItem != null)
            {
                string Index = materialListBox1.SelectedItem.SecondaryText;
                MessageBox.Show("Билет номер "+ Index+" куплен");
                InsertBase();
                Application.Exit();
            }
            else
            {
                MessageBox.Show("Выберите билет");
            }
        }

        public void InsertBase()
        {
            MaterialTextBox materialTextBox = form1.GetTextBox();
            string connectionString = "initial Catalog=Air;Server=(localdb)\\MSSQLLocalDB";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "CreateClient";
            command.Connection = sqlConnection;
            command.Parameters.Add(new SqlParameter("Name", materialTextBox.Text));
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

            //Считывание нового клиента
            sqlConnection.Open();
            int idMax = 0;
            command = new SqlCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "Output_Max_Client";
            command.Connection = sqlConnection;
            SqlDataReader sqlDataReader = command.ExecuteReader();
            while (sqlDataReader.Read())
            {
                idMax = int.Parse(sqlDataReader.GetValue(0).ToString());
            }
            sqlConnection.Close();




            //Запись в таблицу Ticket_To_Client
            sqlConnection.Open();
            command = new SqlCommand();
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.CommandText = "Create_Ticket_Client";
            command.Connection = sqlConnection;
            command.Parameters.Add(new SqlParameter("idClient", idMax));
            command.Parameters.Add(new SqlParameter("idTicket", int.Parse(materialListBox1.SelectedItem.SecondaryText)));
            try
            {
                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

        }
    }
    
}

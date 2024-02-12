using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

namespace GoodsBDextra
{

    public partial class EnterWindows : Window
    {
        string type;
        public EnterWindows(string type)
        {
            InitializeComponent();

            this.type = type;
            text.Text = "Enter new " + type + " name:";
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text.Length == 0)
            {
                MessageBox.Show("Please enter " + type + " name!");
                return;
            }

            SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            try
            {
                connect.Open();
                command.Connection = connect;
                if (type == "Supplier")
                {
                    command.CommandText = "INSERT INTO Supplier (Name) VALUES ('" + txtName.Text + "');";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
                else
                {
                    command.CommandText = "INSERT INTO GoodType (Name) VALUES ('" + txtName.Text + "');";
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                command.Dispose();
                connect.Close();
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}

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
using System.Xml.Linq;

namespace GoodsBDextra
{
    /// <summary>
    /// Interaction logic for GoodsOperationsWindow.xaml
    /// </summary>
    public partial class GoodsOperationsWindow : Window
    {

        bool Edit;
        int Id;
        string Name, Date, Cost, Quantity, GoodType, Supplier;
        public GoodsOperationsWindow(bool Edit = false, int id = 0, string name = "Test", string date = "", string cost = "", string quantity = "", string goodtype = "", string supplier = "")
        {
            InitializeComponent();

            this.Edit = Edit;
            this.Id = id;
            Name = name;
            Date = date;
            Cost = cost;
            Quantity = quantity;
            GoodType = goodtype;
            Supplier = supplier;

            txtName.Text = name;
            dpDate.Text = date;
            txtCost.Text = cost;
            txtQuantity.Text = quantity;


            SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            try
            {
                connect.Open();
                command.Connection = connect;
                command.CommandText = "Select Name from GoodType;";
                SqlDataReader reader = command.ExecuteReader();

                cmbGoodType.Items.Clear();

                while (reader.Read())
                {
                    string typeName = reader["Name"].ToString();
                    cmbGoodType.Items.Add(typeName);
                }

                reader.Close();

                command.CommandText = "Select Name from Supplier;";
                reader = command.ExecuteReader();

                cmbSupplier.Items.Clear();

                while (reader.Read())
                {
                    string typeName = reader["Name"].ToString();
                    cmbSupplier.Items.Add(typeName);
                }
                reader.Close();
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
            cmbGoodType.SelectedIndex = 0;
            cmbSupplier.SelectedIndex = 0;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True");
            SqlCommand command = new SqlCommand();
            try
            {
                if (ValidateInput())
                {
                    connect.Open();
                    command.Connection = connect;

                    int supplierID = GetIDByName("Supplier", cmbSupplier.SelectedItem.ToString());
                    int goodTypeID = GetIDByName("GoodType", cmbGoodType.SelectedItem.ToString());

                    if (!Edit)
                    {
                        command.CommandText = $"INSERT INTO Goods (Name, DeliveryDate, Cost, Quantity, TypeID, SupplierID)" +
                                              $"VALUES ('{txtName.Text}', '{dpDate.Text}', {Convert.ToDouble(txtCost.Text)}, {Convert.ToInt32(txtQuantity.Text)}, '{goodTypeID}', '{supplierID}')";
                        int n = command.ExecuteNonQuery();
                    }
                    else
                    {
                        command.CommandText = $"UPDATE Goods SET " +
                      $"Name = '{txtName.Text}', " +
                      $"DeliveryDate = '{dpDate.Text}', " +
                      $"Cost = {Convert.ToDouble(txtCost.Text)}, " +
                      $"Quantity = {Convert.ToInt32(txtQuantity.Text)}, " +
                      $"TypeID = '{goodTypeID}', " +
                      $"SupplierID = '{supplierID}' " +
                      $"WHERE ID = {Id}";
                        int n = command.ExecuteNonQuery();
                    }

                    connect.Close();
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private int GetIDByName(string tableName, string name)
        {
            int id = -1; 

            using (SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True"))
            {
                connect.Open();

                string query = $"SELECT ID FROM {tableName} WHERE Name = @Name;";
                using (SqlCommand idCommand = new SqlCommand(query, connect))
                {
                    idCommand.Parameters.AddWithValue("@Name", name);

                    try
                    {
                        object result = idCommand.ExecuteScalar();
                        if (result != null)
                        {
                            id = Convert.ToInt32(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            return id;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrEmpty(txtName.Text) ||
                string.IsNullOrEmpty(dpDate.Text) ||
                string.IsNullOrEmpty(txtCost.Text) ||
                string.IsNullOrEmpty(txtQuantity.Text) ||
                cmbGoodType.SelectedItem == null ||
                cmbSupplier.SelectedItem == null)
            {
                MessageBox.Show("Please fill in all the fields.");
                return false;
            }

            if (!DateTime.TryParse(dpDate.Text, out _))
            {
                MessageBox.Show("Invalid date format.");
                return false;
            }

            if (!double.TryParse(txtCost.Text, out _) || !int.TryParse(txtQuantity.Text, out _))
            {
                MessageBox.Show("Invalid numeric values for Cost or Quantity.");
                return false;
            }

            return true;
        }
    }
}

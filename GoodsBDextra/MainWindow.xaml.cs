using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Collections;

namespace GoodsBDextra
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void CreateQuery(string query)
        {
            SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True");
            SqlCommand command = new SqlCommand();

            try
            {
                connect.Open();
                command.Connection = connect;
                command.CommandText = query;
                SqlDataReader reader = command.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                datagrid.ItemsSource = dt.DefaultView;

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
        }
        private void MenuItemShowAll_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID;");
        }

        private void MenuItemShowTypes_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT ID, Name FROM GoodType");
        }

        private void MenuItemShowSuppliers_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT ID, Name FROM Supplier");
        }

        private void MenuItemShowMaxQuantity_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT TOP 1 Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID ORDER BY Goods.Quantity DESC;");
        }

        private void MenuItemShowMinQuantity_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT TOP 1 Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID ORDER BY Goods.Quantity;");
        }

        private void MenuItemShowMaxPrice_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT TOP 1 Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID ORDER BY Goods.Cost DESC;");
        }

        private void MenuItemShowMinPrice_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT TOP 1 Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID ORDER BY Goods.Cost;");
        }

        private void MenuItemShowTheOldest_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT TOP 1 Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name AS SupplierName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID JOIN Supplier ON Goods.SupplierID = Supplier.ID ORDER BY Goods.DeliveryDate ASC");
        }

        private void MenuItemShowTypeStat_Click(object sender, RoutedEventArgs e)
        {
            CreateQuery("SELECT Supplier.ID AS ID_Supplier, Supplier.Name AS SupplierName, COUNT(Goods.ID) AS ProductCount FROM Supplier LEFT JOIN Goods ON Supplier.ID = Goods.SupplierID GROUP BY Supplier.ID, Supplier.Name");
        }

        private void MenuItemShowSelectedType_Click(object sender, RoutedEventArgs e)
        {
            SelectiveWindow selective = new SelectiveWindow();
            selective.ShowDialog();

            if (selective.DialogResult == true)
            {
                CreateQuery("SELECT Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName FROM Goods JOIN GoodType ON Goods.TypeID = GoodType.ID WHERE GoodType.Name = " + "'" + selective.Value + "'");
            }
            else if (selective.DialogResult == false)
            {
                return;
            }
        }

        private void MenuItemShowSelectedSupplier_Click(object sender, RoutedEventArgs e)
        {
            SelectiveWindow selective = new SelectiveWindow(false);
            selective.ShowDialog();

            if (selective.DialogResult == true)
            {
                CreateQuery("SELECT Goods.ID, Goods.Name AS GoodName, Goods.DeliveryDate, Goods.Cost, Goods.Quantity, GoodType.Name AS TypeName, Supplier.Name  FROM Goods JOIN Supplier ON Goods.SupplierID = Supplier.ID JOIN GoodType ON Goods.TypeID = GoodType.ID WHERE Supplier.Name = " + "'" + selective.Value + "'");
            }
            else if (selective.DialogResult == false)
            {
                return;
            }
        }

        private void MenuItemCreateNew_Click(object sender, RoutedEventArgs e)
        {
            GoodsOperationsWindow create_goods = new GoodsOperationsWindow();
            create_goods.ShowDialog();
            if (create_goods.DialogResult == true)
            {
                MenuItemShowAll_Click(sender, e);
            }
            else if (create_goods.DialogResult == false)
            {
                return;
            }
        }

        private void MenuItemCreateNewType_Click(object sender, RoutedEventArgs e)
        {
            EnterWindows create_supplier = new EnterWindows("Good Type");
            create_supplier.ShowDialog();

            if (create_supplier.DialogResult == true)
            {
                CreateQuery("SELECT ID, Name FROM GoodType");
            }
            else if (create_supplier.DialogResult == false)
            {
                return;
            }
        }

        private void MenuItemCreateNewSupplier_Click(object sender, RoutedEventArgs e)
        {
            EnterWindows create_supplier = new EnterWindows("Supplier");
            create_supplier.ShowDialog();

            if (create_supplier.DialogResult == true)
            {
                CreateQuery("SELECT ID, Name FROM Supplier");
            }
            else if (create_supplier.DialogResult == false)
            {
                return;
            }
        }

        private void MenuItemUpdateGoods_Click(object sender, RoutedEventArgs e)
        {
            DataGrid selectedDataGrid = datagrid; 
            var selectedItems = selectedDataGrid.SelectedItems;

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Choose a good first!");
                return;
            }

            var selectedRow = selectedItems[0] as DataRowView; 

            if (selectedRow != null)
            {
                int id = (int)selectedRow["ID"]; 
                string name = selectedRow["GoodName"].ToString();
                string date = selectedRow["DeliveryDate"].ToString();
                string cost = selectedRow["Cost"].ToString();
                string quantity = selectedRow["Quantity"].ToString();
                string type = selectedRow["TypeName"].ToString();
                string supplier = selectedRow["SupplierName"].ToString();

                GoodsOperationsWindow create_goods = new GoodsOperationsWindow(true,id,name,date,cost,quantity,type,supplier);
                create_goods.ShowDialog();
                if (create_goods.DialogResult == true)
                {
                    MenuItemShowAll_Click(sender, e);
                }
                else if (create_goods.DialogResult == false)
                {
                    return;
                }
            }
        }

        private void MenuItemUpdateType_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику для "Update Type"
        }

        private void MenuItemUpdateSupplier_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику для "Update Supplier"
        }

        private void MenuItemDeleteGoods_Click(object sender, RoutedEventArgs e)
        {
            DataGrid selectedDataGrid = datagrid;
            var selectedItems = selectedDataGrid.SelectedItems;

            if (selectedItems.Count == 0)
            {
                MessageBox.Show("Choose a good first!");
                return;
            }
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this?", "Attention!", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result != MessageBoxResult.OK)
            {
                return;
            }

            SqlConnection connect = new SqlConnection(@"Data Source=PECHKA\SQLEXPRESS;Initial Catalog=Storage;Integrated Security=True");
            SqlCommand command = new SqlCommand();
            try
            {
                connect.Open();
                command.Connection = connect;

                var selectedRow = selectedItems[0] as DataRowView;

                if (selectedRow != null)
                {
                    int id = (int)selectedRow["ID"];
                    CreateQuery($"DELETE FROM Goods WHERE id = {id}");
                    MenuItemShowAll_Click(sender, e);
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
        }

        private void MenuItemDeleteType_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику для "Delete Type"
        }

        private void MenuItemDeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Реализовать логику для "Delete Supplier"
        }
    }
}

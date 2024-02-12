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
            // TODO: Реализовать логику для "Update Goods"
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
            // TODO: Реализовать логику для "Delete Goods"
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

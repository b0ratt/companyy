using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace companyy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        NORTHWNDEntities context = new NORTHWNDEntities();
        CollectionViewSource custViewSource;
        CollectionViewSource ordViewSource;

        public MainWindow()
        {
            InitializeComponent();
            custViewSource = ((CollectionViewSource)(FindResource("customersViewSource")));
            ordViewSource = ((CollectionViewSource)(FindResource("customersOrdersViewSource")));
            DataContext = this;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        /// Load data by setting the CollectionViewSource.Source property:
        /// customersViewSource.Source = [generic data source]
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource customersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customersViewSource")));

            context.Customers.Load();

            custViewSource.Source = context.Customers.Local;
        }

        private void LastCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToLast();
        }

        private void PreviousCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToPrevious();
        }

        private void NextCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToNext();
        }

        private void FirstCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            custViewSource.View.MoveCurrentToFirst();
        }
        /// <summary>
        /// If existing window is visible, delete the customer and all their orders.
        /// </summary>
        /// <param name="sender">is a parameter called Sender that contains a reference to the control/object that raised the event.</param>
        /// <param name="e">is a parameter called e that contains the event data, see the EventArgs MSDN page for more information.</param>
        /// <returns>Deleting customer, refreshing window and displaying other customer with current orders</returns>
        private void DeleteCustomerCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            
            var cur = custViewSource.View.CurrentItem as Customers;

            var cust = (from c in context.Customers
                        where c.CustomerID == cur.CustomerID
                        select c).FirstOrDefault();

            if (cust != null)
            {
                foreach (var ord in cust.Orders.ToList())
                {
                    Delete_Order(ord);
                }
                context.Customers.Remove(cust);
            }
            context.SaveChanges();
            custViewSource.View.Refresh();
        }

        /// <summary>
        /// Commit changes from the new customer form, the new order form,  
        /// or edits made to the existing customer form.
        /// </summary>
        /// <param name="sender">is a parameter called Sender that contains a reference to the control/object that raised the event.</param>
        /// <param name="e">is a parameter called e that contains the event data, see the EventArgs MSDN page for more information.</param>
        /// <returns>Depending on active window (new customer or new order), data is being saved and window refreshed</returns>
        private void UpdateCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (newCustomerGrid.IsVisible)
            {
                // Create a new object because the old one  
                // is being tracked by EF now.  
                Customers newCustomer = new Customers
                {
                    Address = add_addressTextBox.Text,
                    City = add_cityTextBox.Text,
                    CompanyName = add_companyNameTextBox.Text,
                    ContactName = add_contactNameTextBox.Text,
                    ContactTitle = add_contactTitleTextBox.Text,
                    Country = add_countryTextBox.Text,
                    CustomerID = add_customerIDTextBox.Text,
                    Fax = add_faxTextBox.Text,
                    Phone = add_phoneTextBox.Text,
                    PostalCode = add_postalCodeTextBox.Text,
                    Region = add_regionTextBox.Text
                };

                // Perform very basic validation  
                if (newCustomer.CustomerID.Length == 5)
                {
                    // Insert the new customer at correct position:  
                    int len = context.Customers.Local.Count();
                    int pos = len;
                    for (int i = 0; i < len; ++i)
                    {
                        if (String.CompareOrdinal(newCustomer.CustomerID, context.Customers.Local[i].CustomerID) < 0)
                        {
                            pos = i;
                            break;
                        }
                    }
                    context.Customers.Local.Insert(pos, newCustomer);
                    custViewSource.View.Refresh();
                    custViewSource.View.MoveCurrentTo(newCustomer);
                }
                else
                {
                    throw new System.ArgumentOutOfRangeException("CustomerID must have 5 characters.");
                }

                newCustomerGrid.Visibility = Visibility.Collapsed;
                existingCustomerGrid.Visibility = Visibility.Visible;
            }
            else if (newOrderGrid.IsVisible)
            {
                // Order ID is auto-generated so we don't set it here.  
                // For CustomerID, address, etc we use the values from current customer.  
                // User can modify these in the datagrid after the order is entered.  

                Customers currentCustomer = (Customers)custViewSource.View.CurrentItem;

                Orders newOrder = new Orders()
                {
                    OrderDate = add_orderDatePicker.SelectedDate,
                    RequiredDate = add_requiredDatePicker.SelectedDate,
                    ShippedDate = add_shippedDatePicker.SelectedDate,
                    CustomerID = currentCustomer.CustomerID,
                    ShipAddress = currentCustomer.Address,
                    ShipCity = currentCustomer.City,
                    ShipCountry = currentCustomer.Country,
                    ShipName = currentCustomer.CompanyName,
                    ShipPostalCode = currentCustomer.PostalCode,
                    ShipRegion = currentCustomer.Region
                };

                try
                {
                    newOrder.EmployeeID = Int32.Parse(add_employeeIDTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("EmployeeID must be a valid integer value.");
                    return;
                }

                try
                {

                    // Acceptable ShipperID values are 1, 2, or 3.  
                    if (add_ShipViaTextBox.Text == "1" || add_ShipViaTextBox.Text == "2"
                        || add_ShipViaTextBox.Text == "3")
                    {
                        newOrder.ShipVia = Convert.ToInt32(add_ShipViaTextBox.Text);
                    }
                    else
                    {
                        MessageBox.Show("Shipper ID must be 1, 2, or 3 in Northwind.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("Ship Via must be convertible to int");
                    return;
                }

                try
                {
                    newOrder.Freight = Convert.ToDecimal(add_freightTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("Freight must be convertible to decimal.");
                    return;
                }
                // Add the order into the EF model  
                context.Orders.Add(newOrder);
                ordViewSource.View.Refresh();
            }

            // Save the changes, either for a new customer, a new order  
            // or an edit to an existing customer or order.
            context.SaveChanges();
        }
        /// <summary>
        /// Sets up the form so that user can enter data. Data is later  
        /// saved when user clicks Commit.  
        /// </summary>
        /// <param name="sender">is a parameter called Sender that contains a reference to the control/object that raised the event.</param>
        /// <param name="e">is a parameter called e that contains the event data, see the EventArgs MSDN page for more information.</param>
        /// <returns>Clear all the text boxes before adding a new customer. </returns>
        private void AddCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            existingCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.Visibility = Visibility.Collapsed;
            newCustomerGrid.Visibility = Visibility.Visible;

             
            foreach (var child in newCustomerGrid.Children)
            {
                var tb = child as TextBox;
                if (tb != null)
                {
                    tb.Text = "";
                }
            }
        }

        private void NewOrder_click(object sender, RoutedEventArgs e)
        {
            var cust = custViewSource.View.CurrentItem as Customers;
            if (cust == null)
            {
                MessageBox.Show("No customer selected.");
                return;
            }

            existingCustomerGrid.Visibility = Visibility.Collapsed;
            newCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.UpdateLayout();
            newOrderGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Cancels any input into the new customer form  
        /// </summary>
        /// <param name="sender">is a parameter called Sender that contains a reference to the control/object that raised the event.</param>
        /// <param name="e">is a parameter called e that contains the event data, see the EventArgs MSDN page for more information.</param>
        /// <returns>Clearing all inputs</returns>
        private void CancelCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            add_addressTextBox.Text = "";
            add_cityTextBox.Text = "";
            add_companyNameTextBox.Text = "";
            add_contactNameTextBox.Text = "";
            add_contactTitleTextBox.Text = "";
            add_countryTextBox.Text = "";
            add_customerIDTextBox.Text = "";
            add_faxTextBox.Text = "";
            add_phoneTextBox.Text = "";
            add_postalCodeTextBox.Text = "";
            add_regionTextBox.Text = "";

            existingCustomerGrid.Visibility = Visibility.Visible;
            newCustomerGrid.Visibility = Visibility.Collapsed;
            newOrderGrid.Visibility = Visibility.Collapsed;
        }

        private void Delete_Order(Orders order)
        {
            // Find the order in the EF model.  
            var ord = (from o in context.Orders.Local
                       where o.OrderID == order.OrderID
                       select o).FirstOrDefault();

            // Delete all the order_details that have  
            // this Order as a foreign key  
            foreach (var detail in ord.Order_Details.ToList())
            {
                context.Order_Details.Remove(detail);
            }

            // Now it's safe to delete the order.  
            context.Orders.Remove(ord);
            context.SaveChanges();

            // Update the data grid.  
            ordViewSource.View.Refresh();
        }
        /// Get the Order in the row in which the Delete button was clicked
        private void DeleteOrderCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {  
            Orders obj = e.Parameter as Orders;
            Delete_Order(obj);
        }
        ///</Snippet3>
    }
}
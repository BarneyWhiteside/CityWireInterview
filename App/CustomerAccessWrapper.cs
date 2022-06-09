using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class CustomerAccessWrapper : ICustomerDataAccess
    {
        public void AddCustomer(Customer customer) {
            CustomerDataAccess.AddCustomer(customer);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataConsole
{
    public class Customer
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }

        public Customer(int id, string firstName, string lastName, string address, string postalcode)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Address = address;
            this.PostalCode = postalcode;
        }

        public static List<Customer> Customers()
        {
            return new List<Customer>()
        {
            new Customer(1, "紫山", "太郎", "仙台市泉区紫山", "981-3205"),
            new Customer(2, "寺岡", "次郎", "仙台市泉区寺岡", "981-3204"),
            new Customer(3, "高森", "三郎", "仙台市泉区高森", "981-3203" ),
            new Customer(4, "桂", "四郎", "仙台市泉区桂", "981-3134" )
        };
        }
    }
}

using System;

namespace App
{
    public class Customer
    {
        //would change this whole property naming convention to avoid confusion between types and properties ie: m_firstName
        public int Id { get; set; }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string EmailAddress { get; set; }

        public bool HasCreditLimit { get; set; }

        public int CreditLimit { get; set; }

        public Company CustomerCompany { get; set; }

        public Customer(Company company, string firstName, string surname, string email, DateTime dateOfBirth)
        {
            CustomerCompany = company;
            Firstname = firstName;
            Surname = surname;
            EmailAddress = email;
            DateOfBirth = dateOfBirth;
        }
    }
}
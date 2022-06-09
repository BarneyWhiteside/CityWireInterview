using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App
{
    public class CustomerService
    {
        private static int CompanyCreditLimit;
        private static int MaxNameLength;
        private static int MaxEmailLength;
        private static int MinCustomerAge;

        public ICompanyRepository ServiceCompanyRepository;
        public ICustomerCreditService CreditService;
        public ICustomerDataAccess DataAccess;

        public CustomerService(int companyCreditLimit, int maxNameLength, int maxEmailLength, int minCustomerAge, ICompanyRepository companyRepository, ICustomerCreditService customerCreditService, ICustomerDataAccess customerDataAccess)
        {
            CompanyCreditLimit = companyCreditLimit;
            MaxNameLength = maxNameLength;
            MaxEmailLength = maxEmailLength;
            MinCustomerAge = minCustomerAge;

            ServiceCompanyRepository = companyRepository;
            CreditService = customerCreditService;
            DataAccess = customerDataAccess;
        }

        public bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            //With more time I would like to send a message with detail about which parameter failed and needs correcting, 
            //ideally returning a list with all of the issues
            if (!ValidateCustomerInput(firstName, surname, email, dateOfBirth)) return false;
            
            Company company = ServiceCompanyRepository.GetById(companyId);

            if (company is null) return false;

            Customer customer = new Customer(company, firstName, surname, email, dateOfBirth);

            int limitMultiplyer = 1;
            
            switch (company.Classification)
            {

                case Classification.Gold:
                    // Skip credit check
                    customer.HasCreditLimit = false;
                    break;
                case Classification.Silver:
                    // Do credit check and double credit limit
                    customer.HasCreditLimit = true;
                    limitMultiplyer = 2;
                    customer.CreditLimit = PerformCreditCheck(customer, limitMultiplyer);
                    break;
                default:
                    //perform standard credit check
                    customer.HasCreditLimit = true;
                    limitMultiplyer = 1;
                    customer.CreditLimit = PerformCreditCheck(customer, limitMultiplyer);
                    break;
            }

            if (customer.HasCreditLimit && customer.CreditLimit < CompanyCreditLimit) return false;

            DataAccess.AddCustomer(customer);

            return true;
        }

        public bool ValidateCustomerInput(string firstName, string surname, string email, DateTime dateOfBirth)
        {
            //If we implemented logging we could return which individual fields are failing.
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname) || firstName.Length > MaxNameLength || surname.Length > MaxNameLength) return false;
            
            if (!email.Contains("@") || !email.Contains(".") || email.Length > MaxEmailLength) return false;
            
            //MinValue means unassigned.
            if (dateOfBirth == DateTime.MinValue) return false;
            
            int age = ProcessAge(dateOfBirth);

            if (age < MinCustomerAge) return false;
            
            return true;
        }

        //this would be a suitable method for joining a utility class in a larger project
        public int ProcessAge(DateTime dateOfBirth)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            return age;
        }

        public int PerformCreditCheck(Customer customer, int limitMultiplyer)
        {
            int creditLimit = 0;
          
            creditLimit = CreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
            creditLimit = creditLimit*limitMultiplyer;
            
            return creditLimit;
        }
    }
}

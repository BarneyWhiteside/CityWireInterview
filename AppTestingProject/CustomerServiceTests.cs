using System;
using App;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AppTestingProject
{
    [TestClass]
    public class CustomerServiceTests
    {
        private Mock<ICompanyRepository> mockRepo;
        private Mock<ICustomerCreditService> mockCredit;
        private Mock<ICustomerDataAccess> mockAccess;

        //with more time, would do more comprehensive setup methods for the different test requirements
        public CustomerService TestSetup()
        {
            int CompanyCreditLimit = 500;
            int MaxNameLength = 50;
            int MaxEmailLength = 50;
            int MinCustomerAge = 21;
            mockRepo =  new Mock<ICompanyRepository>();
            mockCredit = new Mock<ICustomerCreditService>();
            mockAccess = new Mock<ICustomerDataAccess>();

            return new CustomerService(CompanyCreditLimit, MaxNameLength, MaxEmailLength, MinCustomerAge, mockRepo.Object, mockCredit.Object, mockAccess.Object);
        }

        [TestMethod]
        public void CustomerServiceTests_ValidateCustomerInput_True()
        {
            CustomerService customerService = TestSetup();

            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhiteside@hotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);

            bool result = customerService.ValidateCustomerInput(firstName, surname, email, dateOfBirth);

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void CustomerServiceTests_ValidateCustomerInput_False()
        {
            CustomerService customerService = TestSetup();

            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhitesidehotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);

            bool result = customerService.ValidateCustomerInput(firstName, surname, email, dateOfBirth);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void CustomerServiceTests_ProcessAge_True()
        {
            CustomerService customerService = TestSetup();

            DateTime dateOfBirth = new DateTime(1997, 12, 2);

            int age = customerService.ProcessAge(dateOfBirth);

            Assert.AreEqual(24, age);
        }

        [TestMethod]
        public void CustomerServiceTests_ProcessAge_False()
        {
            CustomerService customerService = TestSetup();

            DateTime dateOfBirth = new DateTime(1997, 12, 2);

            int age = customerService.ProcessAge(dateOfBirth);

            Assert.AreNotEqual(23, age);
        }

        [TestMethod]
        public void CustomerServiceTests_PerformCreditCheck_True()
        {
            CustomerService customerService = TestSetup();

            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhiteside@hotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);
            Company company = new Company();
            Customer customer = new Customer(company, firstName, surname, email, dateOfBirth);

            int mockCreditLimit = 500;
            int limitMultiplyer = 2;

            mockCredit.Setup(x => x.GetCreditLimit(firstName, surname, dateOfBirth)).Returns(mockCreditLimit);

            int creditLimit = customerService.PerformCreditCheck(customer, limitMultiplyer);

            Assert.AreEqual(mockCreditLimit*limitMultiplyer, creditLimit);
        }

        [TestMethod]
        public void CustomerServiceTests_PerformCreditCheck_False()
        {
            CustomerService customerService = TestSetup();

            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhiteside@hotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);
            Company company = new Company();
            Customer customer = new Customer(company, firstName, surname, email, dateOfBirth);

            int mockCreditLimit = 500;
            int limitMultiplyer = 2;

            mockCredit.Setup(x => x.GetCreditLimit(firstName, surname, dateOfBirth)).Returns(mockCreditLimit);

            int creditLimit = customerService.PerformCreditCheck(customer, limitMultiplyer);

            Assert.AreNotEqual(mockCreditLimit*limitMultiplyer - 1, creditLimit);
        }

        [TestMethod]
        public void CustomerServiceTests_CompanyRepositoryGetByID_True()
        {
            CustomerService customerService = TestSetup();

            int companyId = Guid.NewGuid().GetHashCode();
            Company companyDto = new Company
            {
                Id = companyId
            };

            mockRepo.Setup(x => x.GetById(companyId)).Returns(companyDto);

            Company company = customerService.ServiceCompanyRepository.GetById(companyId);

            Assert.AreEqual(companyId, company.Id);
        }

        [TestMethod]
        public void CustomerServiceTests_CompanyRepositoryGetByID_False()
        {
            CustomerService customerService = TestSetup();

            int companyId = Guid.NewGuid().GetHashCode();
            Company companyDto = new Company
            {
                Id = companyId
            };

            mockRepo.Setup(x => x.GetById(companyId)).Returns(companyDto);

            Company company = customerService.ServiceCompanyRepository.GetById(companyId);

            Assert.AreNotEqual(++companyId, company.Id);
        }

        [TestMethod]
        public void CustomerServiceTests_AddCustomer_True()
        {
            CustomerService customerService = TestSetup();

            int companyId = Guid.NewGuid().GetHashCode();
            Company companyDto = new Company
            {
                Id = companyId
            };

            mockRepo.Setup(x => x.GetById(companyId)).Returns(companyDto);

            int mockCreditLimit = 500;
            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhiteside@hotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);
            Company company = new Company();
            Customer customer = new Customer(company, firstName, surname, email, dateOfBirth);

            mockCredit.Setup(x => x.GetCreditLimit(firstName, surname, dateOfBirth)).Returns(mockCreditLimit);

            bool result = customerService.AddCustomer(firstName, surname, email, dateOfBirth, companyId);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CustomerServiceTests_AddCustomer_False()
        {
            CustomerService customerService = TestSetup();

            int companyId = Guid.NewGuid().GetHashCode();
            Company companyDto = new Company
            {
                Id = companyId
            };

            mockRepo.Setup(x => x.GetById(companyId)).Returns(companyDto);

            int mockCreditLimit = 500;
            string firstName = "Barney";
            string surname = "Whiteside";
            string email = "brwhitesidehotmail.co.uk";
            DateTime dateOfBirth = new DateTime(1997, 12, 2);
            Company company = new Company();
            Customer customer = new Customer(company, firstName, surname, email, dateOfBirth);

            mockCredit.Setup(x => x.GetCreditLimit(firstName, surname, dateOfBirth)).Returns(mockCreditLimit);

            bool result = customerService.AddCustomer(firstName, surname, email, dateOfBirth, companyId);

            Assert.IsFalse(result);
        }
    }
}

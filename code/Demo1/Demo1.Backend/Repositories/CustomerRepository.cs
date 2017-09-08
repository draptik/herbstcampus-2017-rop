using CSharpFunctionalExtensions;
using Demo1.Backend.Domain;

namespace Demo1.Backend.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        public Customer Create(Customer customer)
        {
            customer.Id = 42;
            return customer;
        }

        public Customer Upgrade(Customer customer)
        {
            customer.UpgradeToPremium();
            return customer;
        }

        public Customer GetById(long id)
        {
            return new Customer
            {
                Id = 42,
                Name = "Test",
                CreditCardNumber = "123"
            };
        }

        public Result<Customer> CreateRop(Customer customer)
        {
            customer.Id = 42;
            return Result.Ok(customer);
        }

        public Result<Customer> Update2(Customer customer)
        {
            return Result.Ok(customer);
        }

        public Result<Customer> UpgradeToPremium(Customer customer)
        {
            customer.UpgradeToPremium();
            return Result.Ok(customer);
        }
    }
}
using CSharpFunctionalExtensions;
using Demo1.Backend.Domain;

namespace Demo1.Backend.Repositories
{
    public interface ICustomerRepository
    {
        Customer Create(Customer customer);
        Customer Upgrade(Customer customer);
        Customer GetById(long id);
        Result<Customer> CreateRop(Customer customer);
        Result<Customer> Update2(Customer customer);

        Result<Customer> UpgradeToPremium(Customer customer);
    }
}
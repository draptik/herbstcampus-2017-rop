using System;
using CSharpFunctionalExtensions;
using Demo1.Backend.Domain;
using Demo1.Backend.Repositories;

namespace Demo1.Backend.ApplicationServices
{
    public interface IMailConfirmer
    {
        void SendWelcome(Customer customer);
        Result<Customer> SendWelcomeRop(Customer customer);
    }

    public class MailConfirmer : IMailConfirmer
    {
        private readonly ICustomerRepository _repository;

        public MailConfirmer(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public void SendWelcome(Customer customer)
        {
            customer.InvitationMailSentAtUtc = DateTimeOffset.UtcNow;
            _repository.Upgrade(customer);
        }

        public Result<Customer> SendWelcomeRop(Customer customer)
        {
            customer.InvitationMailSentAtUtc = DateTimeOffset.UtcNow;
            return _repository.Update2(customer);
        }
    }
}
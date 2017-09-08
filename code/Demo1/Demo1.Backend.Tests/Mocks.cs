using System;
using Demo1.Backend.ApplicationServices;
using Demo1.Backend.Domain;
using Demo1.Backend.ViewModels;
using Tynamix.ObjectFiller;

namespace Demo1.Backend.Tests
{
    class Mocks
    {
        public static CreateCustomerViewModel GetCreateCustomerViewModel()
        {
            var filler = new Filler<CreateCustomerViewModel>();
            filler.Setup()
                .OnProperty(x => x.WantsPremiumSupport).Use(false);
            var result = filler.Create();
            return result;
        }

        public static Customer GetCustomer()
        {
            var filler = new Filler<Customer>();
            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.Now);

            var result = filler.Create();
            return result;
        }

        public static CreditCardGatewayResponse GetCreditCardGatewayResponse()
        {
            var filler = new Filler<CreditCardGatewayResponse>();
            filler.Setup()
                .OnType<DateTimeOffset>().Use(DateTimeOffset.Now)
                .OnProperty(x => x.ChargeWasBooked).Use(true);

            var result = filler.Create();
            return result;

        }
    }
}
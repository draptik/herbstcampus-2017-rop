using CSharpFunctionalExtensions;
using Demo1.Backend.ApplicationServices;
using Demo1.Backend.Domain;
using Demo1.Backend.Infrastructure;
using Demo1.Backend.Repositories;
using Demo1.Backend.ViewModels;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Demo1.Backend.Tests
{
    public class CustomerRegistrationTests
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICreditCardGateway _creditCardGateway;
        private readonly IMailConfirmer _mailConfirmer;
        private readonly CreateCustomerViewModel _vm;
        private readonly CustomerRegistration _sut;
        private readonly CreditCardGatewayResponse _creditCardGatewayResponse;

        public CustomerRegistrationTests()
        {
            var mapper = AutomapperConfiguration.Init();

            _customerRepository = Substitute.For<ICustomerRepository>();
            _creditCardGateway = Substitute.For<ICreditCardGateway>();
            _mailConfirmer = Substitute.For<IMailConfirmer>();

            // Default objects ('stubs')
            var customer = Mocks.GetCustomer();
            _vm = Mocks.GetCreateCustomerViewModel();
            _creditCardGatewayResponse = Mocks.GetCreditCardGatewayResponse();

            // Default system under test ('sut')
            _sut = new CustomerRegistration(mapper, _customerRepository,
                _mailConfirmer, _creditCardGateway);

            // Default Behaviour
            _customerRepository.Create(Arg.Any<Customer>()).Returns(customer);
            _customerRepository.CreateRop(Arg.Any<Customer>()).Returns(Result.Ok(customer));
            _customerRepository.UpgradeToPremium(Arg.Any<Customer>()).Returns(Result.Ok(customer));
            _creditCardGateway.Charge(Arg.Any<string>()).Returns(_creditCardGatewayResponse);
            _creditCardGateway.ChargeRop(Arg.Any<string>()).Returns(Result.Ok(_creditCardGatewayResponse));
            _creditCardGateway.RollBackLastTransactionRop(Arg.Any<Customer>()).Returns(Result.Ok(customer));

            _mailConfirmer.SendWelcomeRop(Arg.Any<Customer>()).Returns(Result.Ok(customer));
        }
        
        [Fact]
        public void Registering_a_new_customer_with_valid_data_should_return_a_valid_view_model()
        {
            _vm.WantsPremiumSupport = false;

            var result = _sut.RegisterCustomer(_vm);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void Registering_a_new_non_premium_customer_should_not_charge_credit_card()
        {
            _vm.WantsPremiumSupport = false;

            _sut.RegisterCustomer(_vm);
            _creditCardGateway.DidNotReceive().ChargeRop(Arg.Any<string>());
        }

        [Fact]
        public void Registering_a_premium_cutomer_should_return_a_valid_view_model()
        {
            _vm.WantsPremiumSupport = true;

            var result = _sut.RegisterCustomer(_vm);
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void Registering_a_premium_customer_should_charge_the_credit_card()
        {
            _vm.WantsPremiumSupport = true;
            
            _sut.RegisterCustomer(_vm);

            _creditCardGateway.Received().ChargeRop(Arg.Any<string>());
        }

        [Fact]
        public void Registering_a_premium_customer_should_upgrade()
        {
            _vm.WantsPremiumSupport = true;

            _sut.RegisterCustomer(_vm);

            _customerRepository.Received().UpgradeToPremium(Arg.Any<Customer>());
        }

        [Fact]
        public void Registering_a_premium_customer_should_not_rollback_if_credit_card_was_booked()
        {
            _vm.WantsPremiumSupport = true;

            _sut.RegisterCustomer(_vm);

            _creditCardGateway.DidNotReceive().RollBackLastTransactionRop(Arg.Any<Customer>());
        }

        [Fact]
        public void Registering_a_premium_customer_should_rollback_if_credit_card_booking_failed()
        {
            _vm.WantsPremiumSupport = true;
            _creditCardGatewayResponse.ChargeWasBooked = false;

            _sut.RegisterCustomer(_vm);

            _creditCardGateway.Received().RollBackLastTransactionRop(Arg.Any<Customer>());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Registering_a_customer_should_send_welcome_mail(bool wantsPremium)
        {
            _vm.WantsPremiumSupport = wantsPremium;

            _sut.RegisterCustomer(_vm);

            _mailConfirmer.Received().SendWelcomeRop(Arg.Any<Customer>());
        }

        [Theory(Skip = "TODO: Extract validation method to separate class for mocking")]
        [InlineData(true)]
        [InlineData(false)]
        public void Registering_a_customer_should_not_send_welcome_mail_if_initial_save_failed(bool wantsPremium)
        {
            _vm.WantsPremiumSupport = wantsPremium;
            //_validation.Validate(_vm).Returns(Result.Fail...)

            _sut.RegisterCustomer(_vm);

            _mailConfirmer.DidNotReceive().SendWelcomeRop(Arg.Any<Customer>());
        }
    }
}

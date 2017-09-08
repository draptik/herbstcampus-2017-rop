using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CSharpFunctionalExtensions;
using Demo1.Backend.Domain;
using Demo1.Backend.Repositories;
using Demo1.Backend.ViewModels;

namespace Demo1.Backend.ApplicationServices
{
    public class CustomerRegistration : ICustomerRegistration
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMailConfirmer _mailConfirmer;
        private readonly ICreditCardGateway _creditCardGateway;
        private readonly IMapper _mapper;

        public CustomerRegistration(IMapper mapper,
            ICustomerRepository customerRepository, 
            IMailConfirmer mailConfirmer,
            ICreditCardGateway creditCardGateway)
        {
            _mapper = mapper;
            _customerRepository = customerRepository;
            _mailConfirmer = mailConfirmer;
            _creditCardGateway = creditCardGateway;
        }

        /// <summary>
        /// User story: Customer registration
        /// 
        /// - When a new customer registers on the web site
        ///     - the customer is persisted
        ///     - and the customer receives a welcome message by email.
        /// - If the customer chose to become a premium member,
        ///     - a registration fee is charged to the customer's credit card
        ///     - and the customer is upgraded to status "Premium".
        /// 
        /// 
        /// Error cases:
        /// 
        /// - If the customer provides invalid registration data
        ///   OR saving the customer registration fails
        ///     - the customer is not persisted
        ///     - the customer does not receive a welcome message by email
        ///     - and the error is logged
        /// - If sending the welcome message fails, 
        ///     - a log message is created.
        /// 
        /// - If the customer chose to become a premium member
        ///     - and if the credit card gateway is offline
        ///         - the customer is persisted as non-premium member
        ///         - the customer receives a welcome message by email
        ///         - the customer is not upgraded to "Premium"
        ///         - an error message is logged.
        ///     - if the credit card was charged, 
        ///       but the upgrade to premium failed
        ///         - the credit card charge is compensated by a rollback
        ///         - the error is logged
        ///         - the customer is upgraded to "Premium"
        /// </summary>
        public CustomerCreatedViewModel RegisterCustomer(
            CreateCustomerViewModel viewModel)
        {
            //return RegisterCustomerSimple0(viewModel);
            //return RegisterCustomerSimple1(viewModel);
            //return RegisterCustomerSimple2_ROP(viewModel);
            //return RegisterCustomer1(viewModel);
            //return RegisterCustomer2(viewModel);
            //return RegisterCustomer3(viewModel);
            //return RegisterCustomer4_withAllErrorCases(viewModel);
            return RegisterCustomer5_ROP(viewModel);
        }

        private Customer Validate(CreateCustomerViewModel vm)
        {
            return _mapper.Map<Customer>(vm);
        }

        private Result<Customer> ValidateRop(CreateCustomerViewModel vm)
        {
            try
            {
                return Result.Ok(_mapper.Map<Customer>(vm));
            }
            catch (Exception e)
            {
                return Result.Fail<Customer>(e.Message);
            }
        }

        // LOC: 4
        private CustomerCreatedViewModel RegisterCustomerSimple0(
            CreateCustomerViewModel createCustomerViewModel)
        {
            var customer = Validate(createCustomerViewModel);
            customer = _customerRepository.Create(customer);
            _mailConfirmer.SendWelcome(customer);

            return new CustomerCreatedViewModel(customer.Id) { Success = true };
        }

        // LOC: ~10
        private CustomerCreatedViewModel RegisterCustomerSimple1(
            CreateCustomerViewModel createCustomerViewModel)
        {
            Customer customer;
            try
            {
                customer = Validate(createCustomerViewModel);
            }
            catch (Exception e) { return CreateErrorResponse(e); }

            try
            {
                customer = _customerRepository.Create(customer);
            }
            catch (Exception e) { return CreateErrorResponse(e); }

            try
            {
                _mailConfirmer.SendWelcome(customer);

            }
            catch (Exception e)
            {
                // handle mailing exception (ie logging, retry-policy, etc)
            }

            return new CustomerCreatedViewModel(customer.Id);
        }

        // Railway Oriented Solution
        private CustomerCreatedViewModel RegisterCustomerSimple2_ROP(
            CreateCustomerViewModel createCustomerViewModel)
        {
            var customerResult = ValidateRop(createCustomerViewModel);
            var result = customerResult
                .OnSuccess(c => _customerRepository.CreateRop(c))
                .OnSuccess(c => _mailConfirmer.SendWelcomeRop(c))
                .OnBoth(cResultAtEnd => cResultAtEnd.IsSuccess
                    ? new CustomerCreatedViewModel(cResultAtEnd.Value.Id)
                    : CreateErrorResponseRop(cResultAtEnd.Error));

            return result;
        }



        // ~20 LOC
        private CustomerCreatedViewModel RegisterCustomer1(
            CreateCustomerViewModel createCustomerViewModel)
        {   
            var customer = Validate(createCustomerViewModel);

            customer = _customerRepository.Create(customer);

            if (createCustomerViewModel.WantsPremiumSupport)
            {
                var creditCardGatewayResponse = _creditCardGateway
                    .Charge(customer.CreditCardNumber);

                if (creditCardGatewayResponse.ChargeWasBooked)
                {
                    _customerRepository.Upgrade(customer);
                }
            }

            _mailConfirmer.SendWelcome(customer);

            return new CustomerCreatedViewModel(customer.Id) {Success = true};
        }

        // only hinting try/catch...
        private CustomerCreatedViewModel RegisterCustomer2(
            CreateCustomerViewModel createCustomerViewModel)
        {
            // can throw
            var customer = Validate(createCustomerViewModel);

            // can throw
            customer = _customerRepository.Create(customer);

            if (createCustomerViewModel.WantsPremiumSupport)
            {
                var creditCardGatewayResponse = _creditCardGateway
                    .Charge(customer.CreditCardNumber);

                if (creditCardGatewayResponse.ChargeWasBooked)
                {
                    // can throw
                    _customerRepository.Upgrade(customer);
                }
            }

            // can throw
            _mailConfirmer.SendWelcome(customer);

            return new CustomerCreatedViewModel(customer.Id);
        }

        // ~30 LOC (using 'compact' formatting)
        // Only by introducing simple try/catch blocks...
        private CustomerCreatedViewModel RegisterCustomer3(
            CreateCustomerViewModel createCustomerViewModel)
        {
            Customer customer;
            try { customer = Validate(createCustomerViewModel); }
            catch (Exception e) { return CreateErrorResponse(e); }

            try { customer = _customerRepository.Create(customer); }
            catch (Exception e) { return CreateErrorResponse(e); }

            if (createCustomerViewModel.WantsPremiumSupport)
            {
                var creditCardGatewayResponse = _creditCardGateway
                    .Charge(customer.CreditCardNumber);

                if (creditCardGatewayResponse.ChargeWasBooked)
                {
                    try { _customerRepository.Upgrade(customer); }
                    catch (Exception e) { return CreateErrorResponse(e); }
                }
            }

            try { _mailConfirmer.SendWelcome(customer); }
            catch (Exception e) { return CreateErrorResponse(e); }

            return new CustomerCreatedViewModel(customer.Id);
        }

        // ~60 LOC (3x HappyCase!)
        private CustomerCreatedViewModel RegisterCustomer4_withAllErrorCases(
            CreateCustomerViewModel createCustomerViewModel)
        {
            var errorMessages = new List<string>();

            Customer customer;
            try { customer = Validate(createCustomerViewModel); }
            catch (Exception e) { return CreateErrorResponse(e); }

            try { customer = _customerRepository.Create(customer); }
            catch (Exception e) { return CreateErrorResponse(e); }

            if (createCustomerViewModel.WantsPremiumSupport)
            {
                var creditCardGatewayResponse = _creditCardGateway
                    .Charge(customer.CreditCardNumber);

                if (creditCardGatewayResponse.Failure)
                {
                    AddToErrorMessage(
                        errorMessages, 
                        "CreditCard gateway is offline.", 
                        creditCardGatewayResponse.ErrorMessage);
                }
                else
                {
                    if (creditCardGatewayResponse.ChargeWasBooked)
                    {
                        try
                        {
                            _customerRepository.Upgrade(customer);
                        }
                        catch (Exception e)
                        {
                            _creditCardGateway.RollBackLastTransaction();
                            return CreateErrorResponse(e);
                        }
                    }
                }
            }

            try
            {
                _mailConfirmer.SendWelcome(customer);
            }
            catch (Exception e)
            {
                /* 
                 * compensation logic:
                 * Even if sending the mail failed: the user is registered. 
                 * We should continue. 
                 */
                AddToErrorMessage(
                    errorMessages, 
                    "But there was a problem sending the confirmation mail. ", 
                    e.Message);
            }

            if (errorMessages.Any())
            {
                var delimiter = Environment.NewLine;
                var formattedErrorMessage = errorMessages
                                                .Aggregate((x, y) => x + delimiter + y);

                return new CustomerCreatedViewModel
                {
                    Success = false, ErrorMessage = formattedErrorMessage
                };
            }

            return new CustomerCreatedViewModel(customer.Id);
        }

        // Using Railway Oriented Programming
        private CustomerCreatedViewModel RegisterCustomer5_ROP(
            CreateCustomerViewModel createCustomerViewModel)
        {
            var result = ValidateRop(createCustomerViewModel)
                .OnSuccess(c => _customerRepository.CreateRop(c))
                .OnSuccess(c => createCustomerViewModel.WantsPremiumSupport
                    ? _creditCardGateway.ChargeRop(c.CreditCardNumber)
                        .OnSuccess(gateway => gateway.ChargeWasBooked
                            ? _customerRepository.UpgradeToPremium(c)
                            : _creditCardGateway.RollBackLastTransactionRop(c))
                    : Result.Ok(c))
                .OnSuccess(c => _mailConfirmer.SendWelcomeRop(c))
                .OnBoth(cResultAtEnd => cResultAtEnd.IsSuccess
                    ? new CustomerCreatedViewModel(cResultAtEnd.Value.Id)
                    : CreateErrorResponseRop(cResultAtEnd.Error));
            return result;
        }

        private void AddToErrorMessage(ICollection<string> errorMessages, 
            string msg, string errorDetails)
        {
            errorMessages.Add("You have been registered successfully in our system." + 
                Environment.NewLine +
                msg + Environment.NewLine +
                $"Please contact our support at foo@bar.com and include the following Details: {errorDetails}");
        }

        private static CustomerCreatedViewModel CreateErrorResponse(Exception e)
        {
            // skipped logging...
            return new CustomerCreatedViewModel
            {
                Success = false, ErrorMessage = e.Message
            };
        }

        private static CustomerCreatedViewModel CreateErrorResponseRop(string errorMessages)
        {
            // skipped logging...
            return new CustomerCreatedViewModel
            {
                Success = false, ErrorMessage = errorMessages
            };
        }
    }
}
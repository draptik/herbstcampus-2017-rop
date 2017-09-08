using CSharpFunctionalExtensions;
using Demo1.Backend.Domain;

namespace Demo1.Backend.ApplicationServices
{
    public interface ICreditCardGateway
    {
        CreditCardGatewayResponse Charge(string creditcardNumber);
        void RollBackLastTransaction();
        Result<CreditCardGatewayResponse> ChargeRop(string creditCardNumber);
        Result<Customer> RollBackLastTransactionRop(Customer customer);
    }

    public class CreditCardGateway : ICreditCardGateway
    {
        public CreditCardGatewayResponse Charge(string creditcardNumber)
        {
            return new CreditCardGatewayResponse
            {
                ChargeWasBooked = true
            };
        }

        public void RollBackLastTransaction()
        {
            // nop
        }

        public Result<CreditCardGatewayResponse> ChargeRop(string creditCardNumber)
        {
            return Result.Ok(new CreditCardGatewayResponse());
        }

        public Result<Customer> RollBackLastTransactionRop(Customer customer)
        {
            return Result.Ok(customer);
        }
    }

    public class CreditCardGatewayResponse
    {
        public bool ChargeWasBooked { get; set; }

        public bool Success { get; set; } = true;

        public bool Failure => !Success;

        public string ErrorMessage { get; set; } = string.Empty;
    }
}
using Demo1.Backend.Domain;

namespace Demo1.Backend.ViewModels
{
    public class CreateCustomerViewModel : ResponseViewModel
    {
        public CreateCustomerViewModel()
        {
            WantsPremiumSupport = false;
        }

        public CreateCustomerViewModel(Customer customer)
        {
            Name = customer.Name;
            CreditCardNumber = customer.CreditCardNumber;
        }

        public string Name { get; set; }

        public string CreditCardNumber { get; set; }

        public bool WantsPremiumSupport { get; set; }
    }
}
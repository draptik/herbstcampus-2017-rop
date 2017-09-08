using Demo1.Backend.Domain;

namespace Demo1.Backend.ViewModels
{
    public class ShowCustomerViewModel : ResponseViewModel
    {
        public ShowCustomerViewModel(Customer customer)
        {
            Name = customer.Name;
            Id = customer.Id;
            IsPremiumCustomer = customer.IsPremiumMember;
        }

        public string Name { get; set; }

        public long Id { get; set; }

        public bool IsPremiumCustomer { get; set; }
    }
}
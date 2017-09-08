namespace Demo1.Backend.ViewModels
{
    public class CustomerCreatedViewModel : ResponseViewModel
    {
        public CustomerCreatedViewModel(long customerId)
        {
            Id = customerId;
        }

        public CustomerCreatedViewModel()
        {
        }

        public long Id { get; set; }
    }
}
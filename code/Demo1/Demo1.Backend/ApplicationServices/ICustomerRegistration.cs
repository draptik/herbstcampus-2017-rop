using Demo1.Backend.ViewModels;

namespace Demo1.Backend.ApplicationServices
{
    /// <summary>
    /// In a real project, this interface would be located in a separate project.
    /// </summary>
    public interface ICustomerRegistration
    {
        CustomerCreatedViewModel RegisterCustomer(CreateCustomerViewModel viewModel);
    }
}
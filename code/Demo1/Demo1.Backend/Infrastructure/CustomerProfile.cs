using AutoMapper;
using Demo1.Backend.Domain;
using Demo1.Backend.ViewModels;

namespace Demo1.Backend.Infrastructure
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CreateCustomerViewModel>();
            CreateMap<CreateCustomerViewModel, Customer>();
        }
    }
}
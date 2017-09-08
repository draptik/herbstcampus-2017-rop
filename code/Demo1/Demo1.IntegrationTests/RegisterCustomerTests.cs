using System.IO;
using System.Web;
using System.Web.Mvc;
using Demo1.Backend.ApplicationServices;
using Demo1.Backend.Repositories;
using Demo1.Backend.ViewModels;
using Demo1.Web.Controllers;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Demo1.IntegrationTests
{
    public class RegisterCustomerTests
    {
        private CreateCustomerViewModel _stubbedViewModel;
        private ICustomerRepository _stubbedCustomerRepository;
        private ICustomerRegistration _mockedCustomerRegistrationService;

        [Fact]
        public void Registering_a_valid_customer_should_redirect_to_show_customer_with_correct_id()
        {
            // Arrange
            InitHttpContext();
            InitMocks();

            const long customerId = 42;
            _mockedCustomerRegistrationService.RegisterCustomer(_stubbedViewModel)
                .Returns(new CustomerCreatedViewModel(customerId));

            var sut = new HomeController(_mockedCustomerRegistrationService, _stubbedCustomerRepository);

            // Act
            var response = sut.Create(_stubbedViewModel);

            // Assert
            response.Should().BeOfType<RedirectToRouteResult>();
            var redirectToRouteResult = (RedirectToRouteResult) response;
            redirectToRouteResult.RouteValues["action"].Should().Be("ShowCustomer");
            redirectToRouteResult.RouteValues["id"].Should().Be(customerId);
        }

        [Fact]
        public void Registering_a_customer_with_some_error_during_the_process_should_redirect_to_create()
        {
            // Arrange
            InitHttpContext();
            InitMocks();

            _mockedCustomerRegistrationService.RegisterCustomer(Arg.Any<CreateCustomerViewModel>())
                .Returns(new CustomerCreatedViewModel{ Success = false });

            var sut = new HomeController(_mockedCustomerRegistrationService, _stubbedCustomerRepository);

            // Act
            var response = sut.Create(_stubbedViewModel);

            // Assert
            response.Should().BeOfType<RedirectToRouteResult>();
            var redirectToRouteResult = (RedirectToRouteResult) response;
            redirectToRouteResult.RouteValues["action"].Should().Be("Create");
        }

        private void InitMocks()
        {
            _stubbedViewModel = Substitute.For<CreateCustomerViewModel>();
            _stubbedCustomerRepository = Substitute.For<ICustomerRepository>();
            _mockedCustomerRegistrationService = Substitute.For<ICustomerRegistration>();
        }

        private static void InitHttpContext()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );
        }
    }
}
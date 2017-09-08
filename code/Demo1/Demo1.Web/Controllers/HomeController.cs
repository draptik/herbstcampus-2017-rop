using System.Web.Mvc;
using Demo1.Backend.ApplicationServices;
using Demo1.Backend.Repositories;
using Demo1.Backend.ViewModels;
using Vereyon.Web;

namespace Demo1.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICustomerRegistration _customerRegistration;
        private readonly ICustomerRepository _customerRepository;

        public HomeController(ICustomerRegistration customerRegistration, 
            ICustomerRepository customerRepository)
        {
            _customerRegistration = customerRegistration;
            _customerRepository = customerRepository;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateCustomerViewModel vm)
        {
            var response = _customerRegistration.RegisterCustomer(vm);
            if (response.Failure)
            {
                FlashMessage.Warning(response.ErrorMessage);
                return RedirectToAction("Create");
            }

            FlashMessage.Confirmation("Customer created successfully.");
            return RedirectToAction("ShowCustomer", new {id = response.Id});
        }


        public ActionResult ShowCustomer(long id)
        {
            var vm = _customerRepository.GetById(id);
            return View(vm);
        }
    }
}
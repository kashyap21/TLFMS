using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.Customers
{
    /// <summary>
    /// Controller for ticket actions (View,Insert,Update,Delete)
    /// </summary>
    public class CustomersController : Controller
    {
        #region Private members

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext;

        /// <summary>
        /// Instance of customer
        /// </summary>
        private GenericRepository<Customer> _objCustomer;

        #endregion Private members

        #region Public constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomersController()
        {
            this._dbContext = BaseContext.GetDbContext();
            this._objCustomer = new GenericRepository<Customer>();
        }

        #endregion Public constructor

        #region Action Method

        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }
            return View("~/Views/Customers/Index.cshtml");
        }

        public ActionResult Create()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            return View("~/Views/Customers/Create.cshtml");
        }

        public ActionResult Edit(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            Customer customer = _objCustomer.FindById(id);
            CustomerViewModel customerViewModel = new CustomerViewModel();
            customerViewModel.Id = id;
            customerViewModel.Name = customer.Name;
            customerViewModel.ShortName = customer.ShortName;
            customerViewModel.Address1 = customer.Address1;
            customerViewModel.Address2 = customer.Address2;
            customerViewModel.City = customer.City;
            customerViewModel.State = customer.State;
            customerViewModel.CountryId = customer.CountryId ?? 0;
            //customerViewModel.CountryName = customer.Country.Name;
            customerViewModel.PinCode = customer.Pincode;
            customerViewModel.PhoneNo = customer.PhoneNo;
            customerViewModel.FaxNo = customer.FaxNo;
            customerViewModel.Email = customer.Email;
            customerViewModel.WebSite = customer.Website;
            customerViewModel.CS_DedicatedNumber = customer.CS_DedicatedNumber;
            customerViewModel.CS_DomainName = customer.CS_DomainName;
            customerViewModel.OwnerId = customer.OwnerId;
            //customerViewModel.OwnerName = customer.Employee.FiistName + " " + customer.Employee.LastName;
            customerViewModel.ContactPersonName = customer.ContactPersonName;
            customerViewModel.IsActive = customer.IsActive;

            return View(customerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                Customer newCustomer = new Customer();

                newCustomer.Name = customerViewModel.Name;
                newCustomer.ShortName = customerViewModel.ShortName;
                newCustomer.Address1 = customerViewModel.Address1;
                newCustomer.Address2 = customerViewModel.Address2;
                newCustomer.City = customerViewModel.City;
                newCustomer.State = customerViewModel.State;
                newCustomer.CountryId = customerViewModel.CountryId;
                newCustomer.Pincode = customerViewModel.PinCode;
                newCustomer.PhoneNo = customerViewModel.PhoneNo;
                newCustomer.FaxNo = customerViewModel.FaxNo;
                newCustomer.Email = customerViewModel.Email;
                newCustomer.Website = customerViewModel.WebSite;
                newCustomer.OwnerId = customerViewModel.OwnerId;
                newCustomer.ContactPersonName = customerViewModel.ContactPersonName;
                newCustomer.CS_DomainName = customerViewModel.CS_DomainName;
                newCustomer.CS_DedicatedNumber = customerViewModel.CS_DedicatedNumber;
                newCustomer.IsActive = true;
                _objCustomer.Insert(newCustomer);
                if (Request.UrlReferrer.ToString() == null)
                {
                    return View("~/Views/Project/Create.cshtml");
                }
                return RedirectToAction("Index", ControllerName.Customer);
            }
            return View(customerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CustomerViewModel customerViewModel)
        {
            if (ModelState.IsValid)
            {
                Customer updateCustomer = new Customer();
                updateCustomer.Id = customerViewModel.Id;
                updateCustomer.Name = customerViewModel.Name;
                updateCustomer.ShortName = customerViewModel.ShortName;
                updateCustomer.Address1 = customerViewModel.Address1;
                updateCustomer.Address2 = customerViewModel.Address2;
                updateCustomer.City = customerViewModel.City;
                updateCustomer.State = customerViewModel.State;
                updateCustomer.CountryId = customerViewModel.CountryId;
                //updateCustomer.CountryName = customerViewModel.Country.Name;
                updateCustomer.Pincode = customerViewModel.PinCode;
                updateCustomer.PhoneNo = customerViewModel.PhoneNo;
                updateCustomer.FaxNo = customerViewModel.FaxNo;
                updateCustomer.Email = customerViewModel.Email;
                updateCustomer.Website = customerViewModel.WebSite;
                updateCustomer.CS_DedicatedNumber = customerViewModel.CS_DedicatedNumber;
                updateCustomer.CS_DomainName = customerViewModel.CS_DomainName;
                updateCustomer.OwnerId = customerViewModel.OwnerId;
                //updateCustomer.OwnerName = customerViewModel.Employee.FiistName + " " + customerViewModel.Employee.LastName;
                updateCustomer.ContactPersonName = customerViewModel.ContactPersonName;
                updateCustomer.IsActive = customerViewModel.IsActive;

                _objCustomer.Update(updateCustomer);
                return RedirectToAction(ActionName.CustomerMgtIndex);
            }
            return View(customerViewModel);
        }

        public ActionResult ViewCustomers([DataSourceRequest]DataSourceRequest request, int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            return Json(GetCustomers(id).ToDataSourceResult(request));
        }

        #endregion Action Method

        #region Public methods

        /// <summary>
        /// Display employee data
        /// </summary>
        /// <returns>Json result of employee data</returns>
        public JsonResult ViewOwnerName(string filter)
        {
            return this.Json(GetOwnerName(filter), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Display country data
        /// </summary>
        /// <returns>Json result of country data</returns>
        public JsonResult ViewCountry()
        {
            return this.Json(GetCountry(), JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<CustomerViewModel> GetOwnerName(string filter)
        {
            var ownerData =  (from emp in _dbContext.Employee
                    select new CustomerViewModel
                    {
                        OwnerId = emp.Id,
                        OwnerName = emp.FiistName + " " + emp.LastName
                    }).OrderBy(order => order.OwnerName).AsEnumerable();

            if (!string.IsNullOrEmpty(filter))
            {
                ownerData = ownerData.Where(x => x.OwnerName.Contains(filter));
            }
            return ownerData;
        }

        public IEnumerable<CustomerViewModel> GetCountry()
        {
            return (from emp in _dbContext.Country
                    select new CustomerViewModel
                    {
                        CountryId = emp.Id,
                        CountryName = emp.Name
                    }).ToList().OrderBy(order => order.CountryName);
        }

        public IEnumerable<CustomerViewModel> GetCustomers(int id)
        {
            var CustomerDetails = (from customer in _dbContext.Customer.Include("Employee").Include("Country")
                                   select new CustomerViewModel
                                   {
                                       Id = customer.Id,
                                       Name = customer.Name,
                                       ShortName = customer.ShortName,
                                       Address1 = customer.Address1,
                                       Address2 = customer.Address2,
                                       City = customer.City,
                                       State = customer.State,
                                       CountryId = customer.CountryId ?? 0,
                                       CountryName = customer.Country.Name,
                                       PinCode = customer.Pincode,
                                       PhoneNo = customer.PhoneNo,
                                       FaxNo = customer.FaxNo,
                                       Email = customer.Email,
                                       WebSite = customer.Website,
                                       OwnerId = customer.OwnerId,
                                       OwnerName = customer.Employee.FiistName + " " + customer.Employee.LastName,
                                       ContactPersonName = customer.ContactPersonName,
                                       IsActive = customer.IsActive,
                                       CS_DedicatedNumber = customer.CS_DedicatedNumber,
                                       CS_DomainName = customer.CS_DomainName
                                   }).ToList();
            if (id == 1)
            {
                return CustomerDetails.Where(cust => cust.IsActive == true).OrderBy(order => order.Name);
            }
            return CustomerDetails.OrderBy(order => order.Name);
        }

        public JsonResult ViewCustomersById(int id)
        {
            return this.Json(GetCustomers(id), JsonRequestBehavior.AllowGet);
        }

        #endregion Public methods
    }
}
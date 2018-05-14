using System.Linq;
using System.Web.Mvc;
using TLFSupport.Common;
using TLFSupport.Model;
using TLFSupport.Model.CustomeModel;
using TLFSupport.Model.DatabaseModel;
using TLFSupport.Model.DatabaseModel.Repositories;

namespace TLFSupport.Control.Controllers.Configuration
{
    /// <summary>
    /// Controller for configuration of PBX & email
    /// </summary>
    public class ConfigurationController : BaseController
    {
        #region Private Member
        /// <summary>
        /// Instance of the ticket
        /// </summary>
        private GenericRepository<CS_Configuration> _objConfiguration;

        /// <summary>
        /// Database context
        /// </summary>
        private TLFCSEntities _dbContext = BaseContext.GetDbContext();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigurationController()
        {
            this._objConfiguration = new GenericRepository<CS_Configuration>();
        }
        #endregion

        #region Action Method
        /// <summary>
        /// Ticket Details
        /// </summary>
        /// <returns>Details of tickets</returns>
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction(ActionName.AuthenticationLogin, ControllerName.Authentication);
            }

            ConfigurationViewModel model = new ConfigurationViewModel();
            var data = _dbContext.CS_Configuration.Select(a => a).First();
            model.Id = data.Id;
            model.PBX_IP = data.PBX_IP;
            model.PBX_Port = data.PBX_Port;
            model.SupportEmailAddress = data.SupportEmailAddress;
            model.Password = data.Password;
            model.SMTPHost = data.SMTPHost;
            model.SMTPPort = data.SMTPPort;
            model.MIMEHost = data.MIMEHost;
            model.MIMEPort = data.MIMEPort;
            model.SLABreachEmailAddress = data.SLABreachEmailAddress;
            return View(model);

        }

        /// <summary>
        /// Updates configuration
        /// </summary>
        /// <param name="model">Model of configuration</param>
        /// <returns>Action result</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfigurationViewModel model)
        {
            CS_Configuration ConfigObj = new CS_Configuration();
            ConfigObj.Id = model.Id;
            ConfigObj.PBX_IP = model.PBX_IP;
            ConfigObj.PBX_Port = model.PBX_Port;
            ConfigObj.SupportEmailAddress = model.SupportEmailAddress;
            ConfigObj.Password = model.Password;
            ConfigObj.SMTPHost = model.SMTPHost;
            ConfigObj.SMTPPort = model.SMTPPort;
            ConfigObj.MIMEHost = model.MIMEHost;
            ConfigObj.MIMEPort = model.MIMEPort;
            ConfigObj.SLABreachEmailAddress = model.SLABreachEmailAddress;

            _objConfiguration.Update(ConfigObj);
            return RedirectToAction("Index");
        } 
        #endregion
    }
}
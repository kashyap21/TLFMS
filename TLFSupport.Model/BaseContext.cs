using TLFSupport.Model.DatabaseModel;

namespace TLFSupport.Model
{
    /// <summary>
    /// Handle all request of customer for Dbcontext
    /// </summary>
    public static class BaseContext
    {
        #region Public methods
        /// <summary>
        ///  Create an instance of DBCustomerSupportEntities
        /// </summary>
        /// <returns>An instance of DBCustomerSupportEntities</returns>
        public static TLFCSEntities GetDbContext()
        {
            var context = new TLFCSEntities();
            return context;
        }

        #endregion
    }
}

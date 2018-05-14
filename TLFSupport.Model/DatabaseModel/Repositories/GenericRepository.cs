using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace TLFSupport.Model.DatabaseModel.Repositories
{
    /// <summary>
    ///  A GenericRepository represents the collection of Generic method for 
    ///  insert,delete,update,get and find data from database of given type.
    /// </summary>
    /// <typeparam name="T">Type of the entity</typeparam>
    public class GenericRepository<T>
        where T : class        
    {
        #region Public methods
        /// <summary>
        /// Get all data of respective entity
        /// </summary>
        /// <param name="contextEntity">Instance of the context class</param>
        /// <returns>Get data of respective entity into list </returns>
        public IEnumerable<T> GetAll(DbContext contextEntity)
        {
            using (contextEntity)
            {
                DbSet<T> table = contextEntity.Set<T>();
                return table.ToList();
            }
        }

        /// <summary>
        /// Find entity by id
        /// </summary>
        /// <param name="id">Id of the respective entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        /// <returns>Entity data</returns>
        public T FindById(object id, DbContext contextEntity)
        {
            using (contextEntity)
            {
                DbSet<T> dataTable = contextEntity.Set<T>();
                return dataTable.Find(id);
            }
        }

        /// <summary>
        /// Inserting entity
        /// </summary>
        /// <param name="insertEntity">Instance of the entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        public void Insert(T insertEntity, DbContext contextEntity)
        {
            using (contextEntity)
            {
                DbSet<T> table = contextEntity.Set<T>();
                table.Add(insertEntity);
                contextEntity.SaveChanges();
            }
        }

        /// <summary>
        /// Updating entity
        /// </summary>
        /// <param name="updateEntity">Instance of the entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        public void Update(T updateEntity, DbContext contextEntity)
        {
            using (contextEntity)
            {
                DbSet<T> table = contextEntity.Set<T>();
                table.Attach(updateEntity);
                contextEntity.Entry(updateEntity).State = EntityState.Modified;
                contextEntity.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting entity
        /// </summary>
        /// <param name="deleteEntity">Instance of the entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        public void Delete(T deleteEntity, DbContext contextEntity)
        {
            using (contextEntity)
            {
                DbSet<T> table = contextEntity.Set<T>();
                table.Remove(deleteEntity);
                contextEntity.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting entity by its id
        /// </summary>
        /// <param name="id">Id of particular entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        public void DeleteById(object id, DbContext contextEntity)
        {
            using (contextEntity)
            {
                //DbSet<T> dataTable = findContext.Set<T>();
                //T deleteEntity = dataTable.Find(id);
                //Delete(deleteEntity);
                //FindById(id, contextEntity);
                Delete(FindById(id, contextEntity), contextEntity);
            }
        }

        /// <summary>
        /// Get all data of respective entity
        /// </summary>
        /// <returns>Get data of respective entity into list </returns>
        public IEnumerable<T> GetAll()
        {
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {
                DbSet<T> table = context.Set<T>();                
                return table.ToList();
            }
        }

        /// <summary>
        /// Find entity by id
        /// </summary>
        /// <param name="id">Id of the respective entity</param>
        /// <returns>Entity data</returns>
        public T FindById(object id)
        {
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {
                DbSet<T> table = context.Set<T>();
                return table.Find(id);
            }
        }

        /// <summary>
        /// Inserting entity
        /// </summary>
        /// <param name="insertEntity">Instance of the entity</param>
        public void Insert(T insertEntity)
        {
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {
                DbSet<T> table = context.Set<T>();
                table.Add(insertEntity);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Updating entity
        /// </summary>
        /// <param name="updateEntity">Instance of the entity</param>
        public void Update(T updateEntity)
        {
           // _dbSet.Attach(updateEntity);
            //_objContext.Entry(updateEntity).State = EntityState.Modified;
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {
                DbSet<T> table = context.Set<T>();
                table.Attach(updateEntity);
                context.Entry(updateEntity).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting entity
        /// </summary>
        /// <param name="deleteEntity">Instance of the entity</param>
        public void Delete(T deleteEntity)
        {
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {
                DbSet<T> table = context.Set<T>();
                table.Attach(deleteEntity);
                table.Remove(deleteEntity);
                context.SaveChanges();
            }
        }

        /// <summary>
        /// Deleting entity by its id
        /// </summary>
        /// <param name="id">Id of particular entity</param>
        /// <param name="contextEntity">Instance of the context class</param>
        public void DeleteById(object id)
        {
            using (TLFCSEntities context = BaseContext.GetDbContext())
            {                
                Delete(FindById(id));
            }
        }
        #endregion
    }
}



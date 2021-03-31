using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FAXER.PORTAL.Repos
{
    public class Repository<T>  where T : class
    {
        private FAXER.PORTAL.DB.FAXEREntities _entities = new DB.FAXEREntities();
        private System.Data.Entity.DbSet<T> set = null;

        public Repository()
        {
            set = _entities.Set<T>();
        }

        public T FindByID(int id)
        {
            return (T)set.Find(new object[] { id });
        }

        public T FindByID(string id)
        {
            return (T)set.Find(new object[] { id });
        }

        public virtual List<T> GetAll()
        {
            return set.ToList();
        }

        public List<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return set.Where(predicate).ToList();
        }

        public virtual T Add(T entity)
        {
            _entities.Set<T>().Add(entity);
            _entities.Entry(entity).State = System.Data.Entity.EntityState.Added;
            Save();
            return entity;
        }

        public virtual void Delete(T entity)
        {
            _entities.Set<T>().Remove(entity);
        }


        public virtual void Edit(T entity)
        {
            _entities.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            Save();
        }

        public virtual void Save()
        {
            _entities.SaveChanges();
        }
    }
}
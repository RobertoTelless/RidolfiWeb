using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Model;

namespace DataServices.Repositories
{
    public class RepositoryBase<TEntity> : IDisposable, IRepositoryBase<TEntity> where TEntity : class
    {
        protected RidolfiDB_WebEntities Db = new RidolfiDB_WebEntities();

        public void Add(TEntity obj)
        {
            Db.Set<TEntity>().Add(obj);
            Db.SaveChanges();
        }

        public TEntity GetById(int? id)
        {
            return Db.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Db.Set<TEntity>().ToList();
        }

        public void Update(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Modified;
            Db.SaveChanges();
        }
        public void Detach(TEntity obj)
        {
            Db.Entry(obj).State = EntityState.Detached;
        }
        public void Remove(TEntity obj)
        {
            Db.Set<TEntity>().Remove(obj);
            Db.SaveChanges();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        //protected DbBackupEntities bkp = new DbBackupEntities();

        //public void AddBkp(TEntity obj)
        //{
        //    bkp.Set<TEntity>().Add(obj);
        //    bkp.SaveChanges();
        //}

        //public TEntity GetByIdBkp(int? id)
        //{
        //    return bkp.Set<TEntity>().Find(id);
        //}

        //public IEnumerable<TEntity> GetAllBkp()
        //{
        //    return bkp.Set<TEntity>().ToList();
        //}

        //public void UpdateBkp(TEntity obj)
        //{
        //    bkp.Entry(obj).State = EntityState.Modified;
        //    bkp.SaveChanges();
        //}
        //public void DetachBkp(TEntity obj)
        //{
        //    bkp.Entry(obj).State = EntityState.Detached;
        //}
        //public void RemoveBkp(TEntity obj)
        //{
        //    bkp.Set<TEntity>().Remove(obj);
        //    bkp.SaveChanges();
        //}
    }
}

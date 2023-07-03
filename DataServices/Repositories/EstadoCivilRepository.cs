using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class EstadoCivilRepository : RepositoryBase<ESTADO_CIVIL>, IEstadoCivilRepository
    {
        public ESTADO_CIVIL GetItemById(Int32 id)
        {
            IQueryable<ESTADO_CIVIL> query = Db.ESTADO_CIVIL;
            query = query.Where(p => p.ESCI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ESTADO_CIVIL> GetAllItens()
        {
            IQueryable<ESTADO_CIVIL> query = Db.ESTADO_CIVIL;
            return query.ToList();
        }
    }
}

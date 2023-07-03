using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TitularidadeRepository : RepositoryBase<TITULARIDADE>, ITitularidadeRepository
    {
        public TITULARIDADE GetItemById(Int32 id)
        {
            IQueryable<TITULARIDADE> query = Db.TITULARIDADE;
            query = query.Where(p => p.TITU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TITULARIDADE> GetAllItens()
        {
            IQueryable<TITULARIDADE> query = Db.TITULARIDADE;
            return query.ToList();
        }

        public TITULARIDADE CheckExist(TITULARIDADE conta)
        {
            IQueryable<TITULARIDADE> query = Db.TITULARIDADE;
            query = query.Where(p => p.TITU_NM_NOME == conta.TITU_NM_NOME);
            return query.FirstOrDefault();
        }

        public List<TITULARIDADE> GetAllItensAdm()
        {
            IQueryable<TITULARIDADE> query = Db.TITULARIDADE;
            return query.ToList();
        }

    }
}

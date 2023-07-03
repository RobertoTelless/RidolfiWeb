using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class NaturezaRepository : RepositoryBase<NATUREZA>, INaturezaRepository
    {
        public NATUREZA GetItemById(Int32 id)
        {
            IQueryable<NATUREZA> query = Db.NATUREZA;
            query = query.Where(p => p.NATU_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NATUREZA> GetAllItens()
        {
            IQueryable<NATUREZA> query = Db.NATUREZA;
            return query.ToList();
        }

        public NATUREZA CheckExist(NATUREZA natureza)
        {
            IQueryable<NATUREZA> query = Db.NATUREZA;
            query = query.Where(p => p.NATU_NM_NOME == natureza.NATU_NM_NOME);
            return query.FirstOrDefault();
        }

        public NATUREZA CheckExist(String natureza)
        {
            IQueryable<NATUREZA> query = Db.NATUREZA;
            query = query.Where(p => p.NATU_NM_NOME.ToUpper() == natureza.ToUpper());
            return query.FirstOrDefault();
        }

        public List<NATUREZA> GetAllItensAdm()
        {
            IQueryable<NATUREZA> query = Db.NATUREZA;
            return query.ToList();
        }

    }
}

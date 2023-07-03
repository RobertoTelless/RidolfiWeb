using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TabelaIRRFRepository : RepositoryBase<TABELA_IRRF>, ITabelaIRRFRepository
    {
        public TABELA_IRRF CheckExist(TABELA_IRRF conta)
        {
            IQueryable<TABELA_IRRF> query = Db.TABELA_IRRF;
            query = query.Where(p => p.IRRF_IN_FAIXA == conta.IRRF_IN_FAIXA);
            return query.FirstOrDefault();
        }

        public TABELA_IRRF GetItemById(Int32 id)
        {
            IQueryable<TABELA_IRRF> query = Db.TABELA_IRRF;
            query = query.Where(p => p.IRRF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TABELA_IRRF> GetAllItens()
        {
            IQueryable<TABELA_IRRF> query = Db.TABELA_IRRF.Where(p => p.IRRF_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<TABELA_IRRF> GetAllItensAdm()
        {
            IQueryable<TABELA_IRRF> query = Db.TABELA_IRRF;
            return query.ToList();
        }
    }
}

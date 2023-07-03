using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class EscolaridadeRepository : RepositoryBase<ESCOLARIDADE>, IEscolaridadeRepository
    {
        public ESCOLARIDADE CheckExist(ESCOLARIDADE conta)
        {
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE;
            query = query.Where(p => p.ESCO_NM_NOME == conta.ESCO_NM_NOME);
            return query.FirstOrDefault();
        }

        public ESCOLARIDADE GetItemById(Int32 id)
        {
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE;
            query = query.Where(p => p.ESCO_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ESCOLARIDADE> GetAllItens()
        {
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE.Where(p => p.ESCO_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<ESCOLARIDADE> GetAllItensAdm()
        {
            IQueryable<ESCOLARIDADE> query = Db.ESCOLARIDADE;
            return query.ToList();
        }
    }
}
 
using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoPesquisaRepository : RepositoryBase<TIPO_PESQUISA>, ITipoPesquisaRepository
    {
        public TIPO_PESQUISA CheckExist(TIPO_PESQUISA conta, Int32 idAss)
        {
            IQueryable<TIPO_PESQUISA> query = Db.TIPO_PESQUISA;
            query = query.Where(p => p.TIPS_NM_NOME == conta.TIPS_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_PESQUISA GetItemById(Int32 id)
        {
            IQueryable<TIPO_PESQUISA> query = Db.TIPO_PESQUISA;
            query = query.Where(p => p.TIPS_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_PESQUISA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_PESQUISA> query = Db.TIPO_PESQUISA.Where(p => p.TIPS_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_PESQUISA> query = Db.TIPO_PESQUISA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}

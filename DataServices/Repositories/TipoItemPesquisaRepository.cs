using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoItemPesquisaRepository : RepositoryBase<TIPO_ITEM_PESQUISA>, ITipoItemPesquisaRepository
    {
        public TIPO_ITEM_PESQUISA CheckExist(TIPO_ITEM_PESQUISA conta, Int32 idAss)
        {
            IQueryable<TIPO_ITEM_PESQUISA> query = Db.TIPO_ITEM_PESQUISA;
            query = query.Where(p => p.TIIT_NM_NOME == conta.TIIT_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public TIPO_ITEM_PESQUISA GetItemById(Int32 id)
        {
            IQueryable<TIPO_ITEM_PESQUISA> query = Db.TIPO_ITEM_PESQUISA;
            query = query.Where(p => p.TIIT_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_ITEM_PESQUISA> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_ITEM_PESQUISA> query = Db.TIPO_ITEM_PESQUISA.Where(p => p.TIIT_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<TIPO_ITEM_PESQUISA> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<TIPO_ITEM_PESQUISA> query = Db.TIPO_ITEM_PESQUISA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}

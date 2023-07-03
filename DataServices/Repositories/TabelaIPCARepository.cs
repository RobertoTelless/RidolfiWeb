using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TabelaIPCARepository : RepositoryBase<TABELA_IPCA>, ITabelaIPCARepository
    {
        public TABELA_IPCA CheckExist(TABELA_IPCA conta)
        {
            IQueryable<TABELA_IPCA> query = Db.TABELA_IPCA;
            query = query.Where(p => p.IPCA_DT_REFERENCIA == conta.IPCA_DT_REFERENCIA);
            return query.FirstOrDefault();
        }

        public TABELA_IPCA GetItemById(Int32 id)
        {
            IQueryable<TABELA_IPCA> query = Db.TABELA_IPCA;
            query = query.Where(p => p.IPCA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TABELA_IPCA> GetAllItens()
        {
            IQueryable<TABELA_IPCA> query = Db.TABELA_IPCA.Where(p => p.IPCA_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<TABELA_IPCA> GetAllItensAdm()
        {
            IQueryable<TABELA_IPCA> query = Db.TABELA_IPCA;
            return query.ToList();
        }

        public List<TABELA_IPCA> ExecuteFilter(DateTime? data)
        {
            List<TABELA_IPCA> lista = new List<TABELA_IPCA>();
            IQueryable<TABELA_IPCA> query = Db.TABELA_IPCA;
            if (data != null)
            {
                query = query.Where(p => p.IPCA_DT_REFERENCIA == data);
            }
            if (query != null)
            {
                lista = query.ToList<TABELA_IPCA>();
            }
            return lista;
        }

    }
}

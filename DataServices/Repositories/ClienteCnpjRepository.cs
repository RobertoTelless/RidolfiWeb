using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class ClienteCnpjRepository : RepositoryBase<CLIENTE_QUADRO_SOCIETARIO>, IClienteCnpjRepository
    {
        public CLIENTE_QUADRO_SOCIETARIO CheckExist(CLIENTE_QUADRO_SOCIETARIO cqs)
        {
            IQueryable<CLIENTE_QUADRO_SOCIETARIO> query = Db.CLIENTE_QUADRO_SOCIETARIO;
            query = query.Where(p => p.CLIE_CD_ID == cqs.CLIE_CD_ID && p.CLQS_NM_NOME == cqs.CLQS_NM_NOME);
            return query.FirstOrDefault();
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            IQueryable<CLIENTE_QUADRO_SOCIETARIO> query = Db.CLIENTE_QUADRO_SOCIETARIO;
            return query.ToList();
        }

        public List<CLIENTE_QUADRO_SOCIETARIO> GetByCliente(CLIENTE cliente)
        {
            IQueryable<CLIENTE_QUADRO_SOCIETARIO> query = Db.CLIENTE_QUADRO_SOCIETARIO;
            query = query.Where(p => p.CLIE_CD_ID == cliente.CLIE_CD_ID);
             return query.ToList();
        }

    }
}
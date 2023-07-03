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

namespace DataServices.Repositories
{
    public class AssinanteCnpjRepository : RepositoryBase<ASSINANTE_QUADRO_SOCIETARIO>, IAssinanteCnpjRepository
    {
        public ASSINANTE_QUADRO_SOCIETARIO CheckExist(ASSINANTE_QUADRO_SOCIETARIO cqs)
        {
            IQueryable<ASSINANTE_QUADRO_SOCIETARIO> query = Db.ASSINANTE_QUADRO_SOCIETARIO;
            query = query.Where(p => p.ASSI_CD_ID == cqs.ASSI_CD_ID && p.ASQS_NM_NOME == cqs.ASQS_NM_NOME);
            return query.FirstOrDefault();
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens()
        {
            IQueryable<ASSINANTE_QUADRO_SOCIETARIO> query = Db.ASSINANTE_QUADRO_SOCIETARIO;
            return query.ToList();
        }

        public List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente)
        {
            IQueryable<ASSINANTE_QUADRO_SOCIETARIO> query = Db.ASSINANTE_QUADRO_SOCIETARIO;
            query = query.Where(p => p.ASSI_CD_ID == cliente.ASSI_CD_ID);
            return query.ToList();
        }
    }
}
 
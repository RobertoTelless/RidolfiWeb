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
    public class TipoCRMRepository : RepositoryBase<TIPO_CRM>, ITipoCRMRepository
    {
        public TIPO_CRM GetItemById(Int32 id)
        {
            IQueryable<TIPO_CRM> query = Db.TIPO_CRM;
            query = query.Where(p => p.TICR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_CRM> GetAllItensAdm()
        {
            IQueryable<TIPO_CRM> query = Db.TIPO_CRM;
            return query.ToList();
        }

        public List<TIPO_CRM> GetAllItens()
        {
            IQueryable<TIPO_CRM> query = Db.TIPO_CRM.Where(p => p.TICR_IN_ATIVO == 1);
            return query.ToList();
        }

    }
}
 
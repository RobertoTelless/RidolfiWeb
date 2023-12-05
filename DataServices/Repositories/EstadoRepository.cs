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
    public class EstadoRepository : RepositoryBase<ESTADO>, IEstadoRepository
    {
        protected CRMSysDBEntities Db = new CRMSysDBEntities();

        public ESTADO GetItemById(Int32 id)
        {
            IQueryable<ESTADO> query = Db.ESTADO;
            query = query.Where(p => p.ESTA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<ESTADO> GetAllItens(Int32 idAss)
        {
            IQueryable<ESTADO> query = Db.ESTADO.Where(p => p.ESTA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.OrderBy(a => a.ESTA_ID_SESSION);
            query = query.Include(p => p.ESTADO_BASE);
            return query.ToList();
        }
    }
}
 
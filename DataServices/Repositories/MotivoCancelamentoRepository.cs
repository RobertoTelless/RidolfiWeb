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
    public class MotivoCancelamentoRepository : RepositoryBase<MOTIVO_CANCELAMENTO>, IMotivoCancelamentoRepository
    {
        public MOTIVO_CANCELAMENTO CheckExist(MOTIVO_CANCELAMENTO conta, Int32 idAss)
        {
            IQueryable<MOTIVO_CANCELAMENTO> query = Db.MOTIVO_CANCELAMENTO;
            query = query.Where(p => p.MOCA_NM_NOME == conta.MOCA_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOCA_IN_TIPO == 1);
            return query.FirstOrDefault();
        }

        public MOTIVO_CANCELAMENTO GetItemById(Int32 id)
        {
            IQueryable<MOTIVO_CANCELAMENTO> query = Db.MOTIVO_CANCELAMENTO;
            query = query.Where(p => p.MOCA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MOTIVO_CANCELAMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MOTIVO_CANCELAMENTO> query = Db.MOTIVO_CANCELAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOCA_IN_TIPO == 1);
            return query.ToList();
        }

        public List<MOTIVO_CANCELAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<MOTIVO_CANCELAMENTO> query = Db.MOTIVO_CANCELAMENTO.Where(p => p.MOCA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOCA_IN_TIPO == 1);
            return query.ToList();
        }

    }
}
 
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
    public class MotivoEncerramentoRepository : RepositoryBase<MOTIVO_ENCERRAMENTO>, IMotivoEncerramentoRepository
    {
        public MOTIVO_ENCERRAMENTO CheckExist(MOTIVO_ENCERRAMENTO conta, Int32 idAss)
        {
            IQueryable<MOTIVO_ENCERRAMENTO> query = Db.MOTIVO_ENCERRAMENTO;
            query = query.Where(p => p.MOEN_NM_NOME == conta.MOEN_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEN_IN_TIPO == 1);
            return query.FirstOrDefault();
        }

        public MOTIVO_ENCERRAMENTO GetItemById(Int32 id)
        {
            IQueryable<MOTIVO_ENCERRAMENTO> query = Db.MOTIVO_ENCERRAMENTO;
            query = query.Where(p => p.MOEN_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<MOTIVO_ENCERRAMENTO> query = Db.MOTIVO_ENCERRAMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEN_IN_TIPO == 1);
            return query.ToList();
        }

        public List<MOTIVO_ENCERRAMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<MOTIVO_ENCERRAMENTO> query = Db.MOTIVO_ENCERRAMENTO.Where(p => p.MOEN_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            query = query.Where(p => p.MOEN_IN_TIPO == 1);
            return query.ToList();
        }

    }
}
 
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
    public class EmpresaMaquinaRepository : RepositoryBase<EMPRESA_MAQUINA>, IEmpresaMaquinaRepository
    {
        public EMPRESA_MAQUINA CheckExist(EMPRESA_MAQUINA tarefa, Int32 idUsu)
        {
            IQueryable<EMPRESA_MAQUINA> query = Db.EMPRESA_MAQUINA;
            query = query.Where(p => p.EMPR_CD_ID == tarefa.EMPR_CD_ID);
            query = query.Where(p => p.MAQN_CD_ID == tarefa.MAQN_CD_ID);
            query = query.Where(p => p.EMMA_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<EMPRESA_MAQUINA> GetAllItens()
        {
            return Db.EMPRESA_MAQUINA.ToList();
        }

        public EMPRESA_MAQUINA GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_MAQUINA> query = Db.EMPRESA_MAQUINA.Where(p => p.EMMA_CD_ID == id);
            return query.FirstOrDefault();
        }

        public EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina)
        {
            IQueryable<EMPRESA_MAQUINA> query = Db.EMPRESA_MAQUINA;
            query = query.Where(x => x.EMPR_CD_ID == empresa);
            query = query.Where(x => x.MAQN_CD_ID == maquina);
            return query.FirstOrDefault();
        }
    }
}
 
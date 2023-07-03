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
    public class EmpresaPlataformaRepository : RepositoryBase<EMPRESA_PLATAFORMA>, IEmpresaPlataformaRepository
    {
        public EMPRESA_PLATAFORMA CheckExist(EMPRESA_PLATAFORMA tarefa, Int32 idUsu)
        {
            IQueryable<EMPRESA_PLATAFORMA> query = Db.EMPRESA_PLATAFORMA;
            query = query.Where(p => p.EMPR_CD_ID == tarefa.EMPR_CD_ID);
            query = query.Where(p => p.PLEN_CD_ID == tarefa.PLEN_CD_ID);
            query = query.Where(p => p.EMPL_IN_ATIVO == 1);
            return query.FirstOrDefault();
        }

        public List<EMPRESA_PLATAFORMA> GetAllItens()
        {
            return Db.EMPRESA_PLATAFORMA.ToList();
        }

        public EMPRESA_PLATAFORMA GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_PLATAFORMA> query = Db.EMPRESA_PLATAFORMA.Where(p => p.EMPL_CD_ID == id);
            return query.FirstOrDefault();
        }

        public EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma)
        {
            IQueryable<EMPRESA_PLATAFORMA> query = Db.EMPRESA_PLATAFORMA;
            query = query.Where(x => x.EMPR_CD_ID == empresa);
            query = query.Where(x => x.PLEN_CD_ID == plataforma);
            return query.FirstOrDefault();
        }
    }
}
 
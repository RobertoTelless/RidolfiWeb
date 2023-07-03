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
    public class EmpresaAnexoRepository : RepositoryBase<EMPRESA_ANEXO>, IEmpresaAnexoRepository
    {
        public List<EMPRESA_ANEXO> GetAllItens()
        {
            return Db.EMPRESA_ANEXO.ToList();
        }

        public EMPRESA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<EMPRESA_ANEXO> query = Db.EMPRESA_ANEXO.Where(p => p.EMAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 
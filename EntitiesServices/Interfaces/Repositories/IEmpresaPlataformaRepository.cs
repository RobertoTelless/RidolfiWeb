using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaPlataformaRepository : IRepositoryBase<EMPRESA_PLATAFORMA>
    {
        EMPRESA_PLATAFORMA CheckExist(EMPRESA_PLATAFORMA item, Int32 idAss);
        List<EMPRESA_PLATAFORMA> GetAllItens();
        EMPRESA_PLATAFORMA GetItemById(Int32 id);
        EMPRESA_PLATAFORMA GetByEmpresaPlataforma(Int32 empresa, Int32 plataforma);
    }
}

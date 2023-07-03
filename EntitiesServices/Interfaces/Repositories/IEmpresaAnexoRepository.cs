using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaAnexoRepository : IRepositoryBase<EMPRESA_ANEXO>
    {
        List<EMPRESA_ANEXO> GetAllItens();
        EMPRESA_ANEXO GetItemById(Int32 id);
    }
}

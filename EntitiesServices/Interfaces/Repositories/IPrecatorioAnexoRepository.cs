using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPrecatorioAnexoRepository : IRepositoryBase<PRECATORIO_ANEXO>
    {
        List<PRECATORIO_ANEXO> GetAllItens();
        PRECATORIO_ANEXO GetItemById(Int32 id);
    }
}

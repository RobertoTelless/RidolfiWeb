using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IHonorarioAnexoRepository : IRepositoryBase<HONORARIO_ANEXO>
    {
        List<HONORARIO_ANEXO> GetAllItens();
        HONORARIO_ANEXO GetItemById(Int32 id);
    }
}

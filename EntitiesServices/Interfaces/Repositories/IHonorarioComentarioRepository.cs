using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IHonorarioComentarioRepository : IRepositoryBase<HONORARIO_ANOTACOES>
    {
        List<HONORARIO_ANOTACOES> GetAllItens();
        HONORARIO_ANOTACOES GetItemById(Int32 id);
    }
}

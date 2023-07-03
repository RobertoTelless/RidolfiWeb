using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaCustoFixoRepository : IRepositoryBase<CATEGORIA_CUSTO_FIXO>
    {
        CATEGORIA_CUSTO_FIXO CheckExist(CATEGORIA_CUSTO_FIXO item, Int32 idAss);
        List<CATEGORIA_CUSTO_FIXO> GetAllItens(Int32 idAss);
        CATEGORIA_CUSTO_FIXO GetItemById(Int32 id);
        List<CATEGORIA_CUSTO_FIXO> GetAllItensAdm(Int32 idAss);
    }
}

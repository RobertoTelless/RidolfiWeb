using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoEmbalagemRepository : IRepositoryBase<TIPO_EMBALAGEM>
    {
        TIPO_EMBALAGEM CheckExist(TIPO_EMBALAGEM item, Int32 idAss);
        List<TIPO_EMBALAGEM> GetAllItens(Int32 idAss);
        TIPO_EMBALAGEM GetItemById(Int32 id);
        List<TIPO_EMBALAGEM> GetAllItensAdm(Int32 idAss);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoFollowRepository : IRepositoryBase<TIPO_FOLLOW>
    {
        TIPO_FOLLOW CheckExist(TIPO_FOLLOW item, Int32 idAss);
        List<TIPO_FOLLOW> GetAllItens(Int32 idAss);
        TIPO_FOLLOW GetItemById(Int32 id);
        List<TIPO_FOLLOW> GetAllItensAdm(Int32 idAss);
    }
}

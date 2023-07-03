using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICategoriaNotificacaoRepository : IRepositoryBase<CATEGORIA_NOTIFICACAO>
    {
        CATEGORIA_NOTIFICACAO CheckExist(CATEGORIA_NOTIFICACAO item, Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllItens(Int32 idAss);
        CATEGORIA_NOTIFICACAO GetItemById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllItensAdm(Int32 idAss);
    }
}

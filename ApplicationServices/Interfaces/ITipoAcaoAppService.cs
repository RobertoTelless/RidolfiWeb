using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoAcaoAppService : IAppServiceBase<TIPO_ACAO>
    {
        Int32 ValidateCreate(TIPO_ACAO item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_ACAO item, TIPO_ACAO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_ACAO item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_ACAO item, USUARIO usuario);

        TIPO_ACAO CheckExist(TIPO_ACAO item, Int32 idAss);
        List<TIPO_ACAO> GetAllItens(Int32 idAss);
        TIPO_ACAO GetItemById(Int32 id);
        List<TIPO_ACAO> GetAllItensAdm(Int32 idAss);

    }
}

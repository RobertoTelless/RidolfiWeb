using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaNotificacaoAppService : IAppServiceBase<CATEGORIA_NOTIFICACAO>
    {
        Int32 ValidateCreate(CATEGORIA_NOTIFICACAO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_NOTIFICACAO item, CATEGORIA_NOTIFICACAO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_NOTIFICACAO item, CATEGORIA_NOTIFICACAO itemAntes);
        Int32 ValidateDelete(CATEGORIA_NOTIFICACAO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_NOTIFICACAO item, USUARIO usuario);

        CATEGORIA_NOTIFICACAO CheckExist(CATEGORIA_NOTIFICACAO item, Int32 idAss);
        List<CATEGORIA_NOTIFICACAO> GetAllItens(Int32 idAss);
        CATEGORIA_NOTIFICACAO GetItemById(Int32 id);
        List<CATEGORIA_NOTIFICACAO> GetAllItensAdm(Int32 idAss);
    }
}

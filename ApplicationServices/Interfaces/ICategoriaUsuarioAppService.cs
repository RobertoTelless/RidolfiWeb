using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaUsuarioAppService : IAppServiceBase<CATEGORIA_USUARIO>
    {
        Int32 ValidateCreate(CATEGORIA_USUARIO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_USUARIO item, CATEGORIA_USUARIO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_USUARIO item, CATEGORIA_USUARIO itemAntes);
        Int32 ValidateDelete(CATEGORIA_USUARIO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_USUARIO item, USUARIO usuario);

        CATEGORIA_USUARIO CheckExist(CATEGORIA_USUARIO item, Int32 idAss);
        List<CATEGORIA_USUARIO> GetAllItens(Int32 idAss);
        CATEGORIA_USUARIO GetItemById(Int32 id);
        List<CATEGORIA_USUARIO> GetAllItensAdm(Int32 idAss);
    }
}

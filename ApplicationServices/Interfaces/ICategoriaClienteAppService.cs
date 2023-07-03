using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaClienteAppService : IAppServiceBase<CATEGORIA_CLIENTE>
    {
        Int32 ValidateCreate(CATEGORIA_CLIENTE item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_CLIENTE item, CATEGORIA_CLIENTE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_CLIENTE item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_CLIENTE item, USUARIO usuario);

        CATEGORIA_CLIENTE CheckExist(CATEGORIA_CLIENTE item, Int32 idAss);
        List<CATEGORIA_CLIENTE> GetAllItens(Int32 idAss);
        CATEGORIA_CLIENTE GetItemById(Int32 id);
        List<CATEGORIA_CLIENTE> GetAllItensAdm(Int32 idAss);
    }
}

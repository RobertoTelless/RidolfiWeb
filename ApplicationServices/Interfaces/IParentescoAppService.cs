using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IParentescoAppService : IAppServiceBase<PARENTESCO>
    {
        Int32 ValidateCreate(PARENTESCO item, USUARIO usuario);
        Int32 ValidateEdit(PARENTESCO item, PARENTESCO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(PARENTESCO item, USUARIO usuario);
        Int32 ValidateReativar(PARENTESCO item, USUARIO usuario);

        PARENTESCO CheckExist(PARENTESCO item);
        List<PARENTESCO> GetAllItens();
        PARENTESCO GetItemById(Int32 id);
        List<PARENTESCO> GetAllItensAdm();

    }
}

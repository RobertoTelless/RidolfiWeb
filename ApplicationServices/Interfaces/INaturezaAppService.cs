using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface INaturezaAppService : IAppServiceBase<NATUREZA>
    {
        Int32 ValidateCreate(NATUREZA item, USUARIO usuario);
        Int32 ValidateEdit(NATUREZA item, NATUREZA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(NATUREZA item, USUARIO usuario);
        Int32 ValidateReativar(NATUREZA item, USUARIO usuario);

        NATUREZA CheckExist(NATUREZA item);
        List<NATUREZA> GetAllItens();
        NATUREZA GetItemById(Int32 id);
        List<NATUREZA> GetAllItensAdm();

    }
}

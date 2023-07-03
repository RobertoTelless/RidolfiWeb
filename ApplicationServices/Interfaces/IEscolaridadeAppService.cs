using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEscolaridadeAppService : IAppServiceBase<ESCOLARIDADE>
    {
        Int32 ValidateCreate(ESCOLARIDADE item, USUARIO usuario);
        Int32 ValidateEdit(ESCOLARIDADE item, ESCOLARIDADE itemAntes, USUARIO usuario);
        Int32 ValidateDelete(ESCOLARIDADE item, USUARIO usuario);
        Int32 ValidateReativar(ESCOLARIDADE item, USUARIO usuario);

        ESCOLARIDADE CheckExist(ESCOLARIDADE item);
        List<ESCOLARIDADE> GetAllItens();
        ESCOLARIDADE GetItemById(Int32 id);
        List<ESCOLARIDADE> GetAllItensAdm();

    }
}

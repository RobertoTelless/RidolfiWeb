using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICargoAppService : IAppServiceBase<CARGO>
    {
        Int32 ValidateCreate(CARGO item, USUARIO usuario);
        Int32 ValidateEdit(CARGO item, CARGO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CARGO item, CARGO itemAntes);
        Int32 ValidateDelete(CARGO item, USUARIO usuario);
        Int32 ValidateReativar(CARGO item, USUARIO usuario);

        CARGO CheckExist(CARGO item, Int32 idAss);
        List<CARGO> GetAllItens(Int32 idAss);
        CARGO GetItemById(Int32 id);
        List<CARGO> GetAllItensAdm(Int32 idAss);
    }
}

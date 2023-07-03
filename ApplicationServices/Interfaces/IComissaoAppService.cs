using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IComissaoAppService : IAppServiceBase<COMISSAO>
    {
        Int32 ValidateCreate(COMISSAO item);
        Int32 ValidateEdit(COMISSAO item, COMISSAO itemAntes);
        Int32 ValidateDelete(COMISSAO item, USUARIO usuario);
        Int32 ValidateReativar(COMISSAO item, USUARIO usuario);

        COMISSAO GetItemById(Int32 id);
        List<COMISSAO> GetAllItens(Int32 idAss);
        List<COMISSAO> GetAllItensAdm(Int32 idAss);
        Tuple<Int32, List<COMISSAO>, Boolean> ExecuteFilterComissao(DateTime? data, Int32? idAss);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITRFAppService : IAppServiceBase<TRF>
    {
        Int32 ValidateCreate(TRF perfil, USUARIO usuario);
        Int32 ValidateEdit(TRF perfil, TRF perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(TRF perfil, USUARIO usuario);
        Int32 ValidateReativar(TRF perfil, USUARIO usuario);

        TRF CheckExist(TRF conta);
        List<TRF> GetAllItens();
        List<TRF> GetAllItensAdm();
        TRF GetItemById(Int32 id);
        Int32 ExecuteFilter(String nome, Int32? uf, out List<TRF> objeto);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}

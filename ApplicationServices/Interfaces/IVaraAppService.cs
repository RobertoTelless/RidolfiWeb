using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IVaraAppService : IAppServiceBase<VARA>
    {
        Int32 ValidateCreate(VARA perfil, USUARIO usuario);
        Int32 ValidateEdit(VARA perfil, VARA perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(VARA perfil, USUARIO usuario);
        Int32 ValidateReativar(VARA perfil, USUARIO usuario);

        VARA CheckExist(VARA conta);
        List<VARA> GetAllItens();
        List<VARA> GetAllItensAdm();
        VARA GetItemById(Int32 id);
        Int32 ExecuteFilter(String nome, Int32? trf, out List<VARA> objeto);
        List<TRF> GetAllTRF();
    }
}

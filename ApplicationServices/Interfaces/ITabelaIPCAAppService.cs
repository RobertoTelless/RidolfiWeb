using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITabelaIPCAAppService : IAppServiceBase<TABELA_IPCA>
    {
        Int32 ValidateCreate(TABELA_IPCA perfil, USUARIO usuario);
        Int32 ValidateEdit(TABELA_IPCA perfil, TABELA_IPCA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(TABELA_IPCA item, TABELA_IPCA itemAntes);
        Int32 ValidateDelete(TABELA_IPCA perfil, USUARIO usuario);
        Int32 ValidateReativar(TABELA_IPCA perfil, USUARIO usuario);

        TABELA_IPCA CheckExist(TABELA_IPCA conta);
        TABELA_IPCA GetItemById(Int32 id);
        List<TABELA_IPCA> GetAllItens();
        List<TABELA_IPCA> GetAllItensAdm();
        Int32 ExecuteFilter(DateTime? data, out List<TABELA_IPCA> objeto);

    }
}

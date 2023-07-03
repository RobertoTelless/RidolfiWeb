using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITabelaIRRFAppService : IAppServiceBase<TABELA_IRRF>
    {
        Int32 ValidateCreate(TABELA_IRRF perfil, USUARIO usuario);
        Int32 ValidateEdit(TABELA_IRRF perfil, TABELA_IRRF perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(TABELA_IRRF item, TABELA_IRRF itemAntes);
        Int32 ValidateDelete(TABELA_IRRF perfil, USUARIO usuario);
        Int32 ValidateReativar(TABELA_IRRF perfil, USUARIO usuario);

        TABELA_IRRF CheckExist(TABELA_IRRF conta);
        TABELA_IRRF GetItemById(Int32 id);
        List<TABELA_IRRF> GetAllItens();
        List<TABELA_IRRF> GetAllItensAdm();

    }
}

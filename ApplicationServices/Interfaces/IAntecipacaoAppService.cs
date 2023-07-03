using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAntecipacaoAppService : IAppServiceBase<ANTECIPACAO>
    {
        Int32 ValidateCreate(ANTECIPACAO item, USUARIO usuario);
        Int32 ValidateEdit(ANTECIPACAO item, ANTECIPACAO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(ANTECIPACAO item, ANTECIPACAO itemAntes);
        Int32 ValidateDelete(ANTECIPACAO item, USUARIO usuario);
        Int32 ValidateReativar(ANTECIPACAO item, USUARIO usuario);

        ANTECIPACAO CheckExist(ANTECIPACAO item, Int32 idEmpr);
        ANTECIPACAO GetItemById(Int32 id);
        List<ANTECIPACAO> GetAllItens(Int32 idAss);
    }
}

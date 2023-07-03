using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAntecipacaoService : IServiceBase<ANTECIPACAO>
    {
        Int32 Create(ANTECIPACAO perfil, LOG log);
        Int32 Create(ANTECIPACAO perfil);
        Int32 Edit(ANTECIPACAO perfil, LOG log);
        Int32 Edit(ANTECIPACAO perfil);
        Int32 Delete(ANTECIPACAO perfil, LOG log);

        ANTECIPACAO CheckExist(ANTECIPACAO item, Int32 idEmpr);
        ANTECIPACAO GetItemById(Int32 id);
        List<ANTECIPACAO> GetAllItens(Int32 idAss);
    }
}

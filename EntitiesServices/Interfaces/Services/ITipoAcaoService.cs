using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoAcaoService : IServiceBase<TIPO_ACAO>
    {
        Int32 Create(TIPO_ACAO perfil, LOG log);
        Int32 Create(TIPO_ACAO perfil);
        Int32 Edit(TIPO_ACAO perfil, LOG log);
        Int32 Edit(TIPO_ACAO perfil);
        Int32 Delete(TIPO_ACAO perfil, LOG log);

        TIPO_ACAO CheckExist(TIPO_ACAO item, Int32 idAss);
        TIPO_ACAO GetItemById(Int32 id);
        List<TIPO_ACAO> GetAllItens(Int32 idAss);
        List<TIPO_ACAO> GetAllItensAdm(Int32 idAss);
    }
}

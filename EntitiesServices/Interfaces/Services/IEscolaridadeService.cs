using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IEscolaridadeService : IServiceBase<ESCOLARIDADE>
    {
        Int32 Create(ESCOLARIDADE perfil, LOG log);
        Int32 Create(ESCOLARIDADE perfil);
        Int32 Edit(ESCOLARIDADE perfil, LOG log);
        Int32 Edit(ESCOLARIDADE perfil);
        Int32 Delete(ESCOLARIDADE perfil, LOG log);

        ESCOLARIDADE CheckExist(ESCOLARIDADE item);
        ESCOLARIDADE GetItemById(Int32 id);
        List<ESCOLARIDADE> GetAllItens();
        List<ESCOLARIDADE> GetAllItensAdm();
    }
}

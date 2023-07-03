using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface INaturezaService : IServiceBase<NATUREZA>
    {
        Int32 Create(NATUREZA perfil, LOG log);
        Int32 Create(NATUREZA perfil);
        Int32 Edit(NATUREZA perfil, LOG log);
        Int32 Edit(NATUREZA perfil);
        Int32 Delete(NATUREZA perfil, LOG log);

        NATUREZA CheckExist(NATUREZA item);
        NATUREZA GetItemById(Int32 id);
        List<NATUREZA> GetAllItens();
        List<NATUREZA> GetAllItensAdm();
    }
}

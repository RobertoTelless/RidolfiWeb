using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPlanoService : IServiceBase<PLANO>
    {
        Int32 Create(PLANO item, LOG log);
        Int32 Create(PLANO item);
        Int32 Edit(PLANO item, LOG log);
        Int32 Edit(PLANO item);
        Int32 Delete(PLANO item, LOG log);

        PLANO CheckExist(PLANO item);
        PLANO GetItemById(Int32 id);
        List<PLANO> GetAllItens();
        List<PLANO> GetAllItensAdm();
        List<PLANO> GetAllValidos();
        List<PLANO> ExecuteFilter(String nome, String descricao);

        List<PLANO_PERIODICIDADE> GetAllPeriodicidades();
        PLANO_PERIODICIDADE GetPeriodicidadeById(Int32 id);
    }
}

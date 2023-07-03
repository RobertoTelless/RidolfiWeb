using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPlanoAppService : IAppServiceBase<PLANO>
    {
        Int32 ValidateCreate(PLANO item, USUARIO usuario);
        Int32 ValidateEdit(PLANO item, PLANO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PLANO item, PLANO itemAntes);
        Int32 ValidateDelete(PLANO item, USUARIO usuario);
        Int32 ValidateReativar(PLANO item, USUARIO usuario);

        List<PLANO> GetAllItens();
        List<PLANO> GetAllItensAdm();
        PLANO GetItemById(Int32 id);
        PLANO CheckExist(PLANO conta);
        List<PLANO> GetAllValidos();
        Tuple<Int32, List<PLANO>, Boolean> ExecuteFilter(String nome, String descricao);

        List<PLANO_PERIODICIDADE> GetAllPeriodicidades();
        PLANO_PERIODICIDADE GetPeriodicidadeById(Int32 id);
    }
}

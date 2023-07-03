using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaCustoVariavelRepository : IRepositoryBase<EMPRESA_CUSTO_VARIAVEL>
    {
        EMPRESA_CUSTO_VARIAVEL CheckExistCustoVariavel(EMPRESA_CUSTO_VARIAVEL item, Int32 idAss);
        List<EMPRESA_CUSTO_VARIAVEL> GetAllItens(Int32 id);
        EMPRESA_CUSTO_VARIAVEL GetItemById(Int32 id);

    }
}

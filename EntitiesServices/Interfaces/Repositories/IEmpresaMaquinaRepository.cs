using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEmpresaMaquinaRepository : IRepositoryBase<EMPRESA_MAQUINA>
    {
        EMPRESA_MAQUINA CheckExist(EMPRESA_MAQUINA item, Int32 idAss);
        List<EMPRESA_MAQUINA> GetAllItens();
        EMPRESA_MAQUINA GetItemById(Int32 id);
        EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina);
    }
}

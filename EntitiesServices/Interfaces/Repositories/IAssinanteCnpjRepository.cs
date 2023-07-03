using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAssinanteCnpjRepository : IRepositoryBase<ASSINANTE_QUADRO_SOCIETARIO>
    {
        ASSINANTE_QUADRO_SOCIETARIO CheckExist(ASSINANTE_QUADRO_SOCIETARIO cqs);
        List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens();
        List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente);
    }
}

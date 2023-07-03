using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAssinanteCnpjService : IServiceBase<ASSINANTE_QUADRO_SOCIETARIO>
    {
        ASSINANTE_QUADRO_SOCIETARIO CheckExist(ASSINANTE_QUADRO_SOCIETARIO cqs);
        List<ASSINANTE_QUADRO_SOCIETARIO> GetAllItens();
        List<ASSINANTE_QUADRO_SOCIETARIO> GetByCliente(ASSINANTE cliente);
        Int32 Create(ASSINANTE_QUADRO_SOCIETARIO cqs, LOG log);
        Int32 Create(ASSINANTE_QUADRO_SOCIETARIO cqs);
    }
}

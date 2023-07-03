using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IClienteCnpjAppService : IAppServiceBase<CLIENTE_QUADRO_SOCIETARIO>
    {
        List<CLIENTE_QUADRO_SOCIETARIO> GetAllItens();
        List<CLIENTE_QUADRO_SOCIETARIO> GetByCliente(CLIENTE cliente);
        Int32 ValidateCreate(CLIENTE_QUADRO_SOCIETARIO item, USUARIO usuario);
    }
}
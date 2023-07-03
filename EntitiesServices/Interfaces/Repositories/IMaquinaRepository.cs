using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMaquinaRepository : IRepositoryBase<MAQUINA>
    {
        MAQUINA CheckExist(MAQUINA item, Int32 idAss);
        MAQUINA GetItemById(Int32 id);
        List<MAQUINA> GetAllItens(Int32 idAss);
        List<MAQUINA> GetAllItensAdm(Int32 idAss);
        List<MAQUINA> ExecuteFilter(String provedor, String nome, Int32? idAss);
    }
}


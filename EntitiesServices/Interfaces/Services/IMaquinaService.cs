using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IMaquinaService : IServiceBase<MAQUINA>
    {
        Int32 Create(MAQUINA perfil, LOG log);
        Int32 Create(MAQUINA perfil);
        Int32 Edit(MAQUINA perfil, LOG log);
        Int32 Edit(MAQUINA perfil);
        Int32 Delete(MAQUINA perfil, LOG log);

        MAQUINA CheckExist(MAQUINA item, Int32 idAss);
        MAQUINA GetItemById(Int32 id);
        List<MAQUINA> GetAllItens(Int32 idAss);
        List<MAQUINA> GetAllItensAdm(Int32 idAss);
        List<MAQUINA> ExecuteFilter(String provedor, String nome, Int32? idAss);
    }
}

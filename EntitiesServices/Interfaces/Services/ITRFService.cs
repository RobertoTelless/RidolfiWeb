using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITRFService : IServiceBase<TRF>
    {
        Int32 Create(TRF perfil, LOG log);
        Int32 Create(TRF perfil);
        Int32 Edit(TRF perfil, LOG log);
        Int32 Edit(TRF perfil);
        Int32 Delete(TRF perfil, LOG log);

        TRF CheckExist(TRF conta);
        TRF GetItemById(Int32 id);
        List<TRF> GetAllItens();
        List<TRF> GetAllItensAdm();
        List<TRF> ExecuteFilter(String nome, Int32? uf);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}

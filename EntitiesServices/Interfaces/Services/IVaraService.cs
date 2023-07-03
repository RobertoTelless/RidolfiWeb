using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IVaraService : IServiceBase<VARA>
    {
        Int32 Create(VARA perfil, LOG log);
        Int32 Create(VARA perfil);
        Int32 Edit(VARA perfil, LOG log);
        Int32 Edit(VARA perfil);
        Int32 Delete(VARA perfil, LOG log);

        VARA CheckExist(VARA conta);
        VARA GetItemById(Int32 id);
        List<VARA> GetAllItens();
        List<VARA> GetAllItensAdm();
        List<VARA> ExecuteFilter(String nome, Int32? trf);

        List<TRF> GetAllTRF();
    }
}

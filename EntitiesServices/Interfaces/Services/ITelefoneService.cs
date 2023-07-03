using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITelefoneService : IServiceBase<TELEFONES>
    {
        Int32 Create(TELEFONES perfil, LOG log);
        Int32 Create(TELEFONES perfil);
        Int32 Edit(TELEFONES perfil, LOG log);
        Int32 Edit(TELEFONES perfil);
        Int32 Delete(TELEFONES perfil, LOG log);

        TELEFONES CheckExist(TELEFONES conta, Int32 idAss);
        TELEFONES GetItemById(Int32 id);
        List<TELEFONES> GetAllItens(Int32 idAss);
        List<TELEFONES> GetAllItensAdm(Int32 idAss);

        List<CATEGORIA_TELEFONE> GetAllTipos(Int32 idAss);
        List<TELEFONES> ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITelefoneAppService : IAppServiceBase<TELEFONES>
    {
        Int32 ValidateCreate(TELEFONES perfil, USUARIO usuario);
        Int32 ValidateEdit(TELEFONES perfil, TELEFONES perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(TELEFONES item, TELEFONES itemAntes);
        Int32 ValidateDelete(TELEFONES perfil, USUARIO usuario);
        Int32 ValidateReativar(TELEFONES perfil, USUARIO usuario);

        List<TELEFONES> GetAllItens(Int32 idAss);
        List<TELEFONES> GetAllItensAdm(Int32 idAss);
        TELEFONES GetItemById(Int32 id);
        TELEFONES CheckExist(TELEFONES conta, Int32 idAss);

        List<CATEGORIA_TELEFONE> GetAllTipos(Int32 idAss);
        Int32 ExecuteFilter(Int32? catId, String nome, String telefone, String cidade, Int32? uf, String celular, String email, Int32 idAss, out List<TELEFONES> objeto);
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

    }
}

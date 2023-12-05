using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteRepository : IRepositoryBase<CLIENTE>
    {
        CLIENTE CheckExist(CLIENTE item, Int32 idAss);
        CLIENTE CheckExistDoctos(String cpf, String cnpj, Int32 idAss);
        CLIENTE GetByEmail(String email);
        CLIENTE GetItemById(Int32 id);
        List<CLIENTE> GetAllItens(Int32 idAss);
        List<CLIENTE> GetAllItensAdm(Int32 idAss);
        List<CLIENTE> GetAllItensUltimos(Int32 num, Int32 idAss);
        List<CLIENTE> ExecuteFilter(Int32? id, Int32? catId, String razao, String nome, String cpf, String cnpj, String email, String cidade, Int32? uf, Int32? ativo, Int32? filial, Int32? usu, Int32 idAss);
        List<CLIENTE> FiltrarContatos(GRUPO grupo, Int32 idAss);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.Repositories
{
    public interface IBeneficiarioRepository : IRepositoryBase<BENEFICIARIO>
    {
        BENEFICIARIO CheckExist(BENEFICIARIO item);
        BENEFICIARIO CheckExist(String nome, String cpf);
        BENEFICIARIO GetItemById(Int32 id);
        List<BENEFICIARIO> GetAllItens();
        List<BENEFICIARIO> GetAllItensAdm();
        List<BENEFICIARIO> ExecuteFilter(Int32? tipo, Int32? sexo, Int32? estado, Int32? escolaridade, Int32? parentesco, String razao, String nome, DateTime? dataNasc, String cpf, String cnpj, String parente);
    }
}

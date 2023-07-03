using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITemplateEMailAppService : IAppServiceBase<TEMPLATE_EMAIL>
    {
        Int32 ValidateCreate(TEMPLATE_EMAIL item, USUARIO usuario);
        Int32 ValidateEdit(TEMPLATE_EMAIL item, TEMPLATE_EMAIL itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TEMPLATE_EMAIL item, USUARIO usuario);
        Int32 ValidateReativar(TEMPLATE_EMAIL item, USUARIO usuario);

        TEMPLATE_EMAIL GetByCode(String code, Int32 idAss);
        TEMPLATE_EMAIL CheckExist(TEMPLATE_EMAIL item, Int32 idAss);
        List<TEMPLATE_EMAIL> GetAllItens(Int32 idAss);
        TEMPLATE_EMAIL GetItemById(Int32 id);
        List<TEMPLATE_EMAIL> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss, out List<TEMPLATE_EMAIL> objeto);
    }
}

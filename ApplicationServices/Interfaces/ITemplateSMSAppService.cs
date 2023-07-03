using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITemplateSMSAppService : IAppServiceBase<TEMPLATE_SMS>
    {
        Int32 ValidateCreate(TEMPLATE_SMS item, USUARIO usuario);
        Int32 ValidateEdit(TEMPLATE_SMS item, TEMPLATE_SMS itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TEMPLATE_SMS item, USUARIO usuario);
        Int32 ValidateReativar(TEMPLATE_SMS item, USUARIO usuario);

        TEMPLATE_SMS GetByCode(String code, Int32 idAss);
        TEMPLATE_SMS CheckExist(TEMPLATE_SMS item, Int32 idAss);
        List<TEMPLATE_SMS> GetAllItens(Int32 idAss);
        TEMPLATE_SMS GetItemById(Int32 id);
        List<TEMPLATE_SMS> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(String sigla, String nome, String conteudo, Int32 idAss, out List<TEMPLATE_SMS> objeto);
    }
}

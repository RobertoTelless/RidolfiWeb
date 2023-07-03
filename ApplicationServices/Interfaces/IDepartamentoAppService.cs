using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IDepartamentoAppService : IAppServiceBase<DEPARTAMENTO>
    {
        Int32 ValidateCreate(DEPARTAMENTO item, USUARIO usuario);
        Int32 ValidateEdit(DEPARTAMENTO item, DEPARTAMENTO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(DEPARTAMENTO item, DEPARTAMENTO itemAntes);
        Int32 ValidateDelete(DEPARTAMENTO item, USUARIO usuario);
        Int32 ValidateReativar(DEPARTAMENTO item, USUARIO usuario);

        DEPARTAMENTO CheckExist(DEPARTAMENTO conta, Int32 idAss);
        List<DEPARTAMENTO> GetAllItens(Int32 idAss);
        List<DEPARTAMENTO> GetAllItensAdm(Int32 idAss);
        DEPARTAMENTO GetItemById(Int32 id);
    }
}

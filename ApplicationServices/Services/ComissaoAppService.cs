using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;
using ApplicationServices.Interfaces;
using ModelServices.Interfaces.EntitiesServices;
using CrossCutting;
using System.Text.RegularExpressions;

namespace ApplicationServices.Services
{
    public class ComissaoAppService : AppServiceBase<COMISSAO>, IComissaoAppService
    {
        private readonly IComissaoService _baseService;

        public ComissaoAppService(IComissaoService baseService): base(baseService)
        {
            _baseService = baseService;
        }

        public List<COMISSAO> GetAllItens(Int32 idAss)
        {
            List<COMISSAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public List<COMISSAO> GetAllItensAdm(Int32 idAss)
        {
            List<COMISSAO> lista = _baseService.GetAllItens(idAss);
            return lista;
        }

        public COMISSAO GetItemById(Int32 id)
        {
            COMISSAO item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(COMISSAO item)
        {
            try
            {
                // Persiste
                Int32 volta = _baseService.Create(item);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(COMISSAO item, COMISSAO itemAntes)
        {
            try
            {
                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(COMISSAO item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.COMI_IN_ATIVO = 0;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(COMISSAO item, USUARIO usuario)
        {
            try
            {
                // Acerta campos
                item.COMI_IN_ATIVO = 1;

                // Persiste
                return _baseService.Edit(item);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Tuple<Int32, List<COMISSAO>, Boolean> ExecuteFilterComissao(DateTime? data, Int32? idAss)
        {
            try
            {
                List<COMISSAO> objeto = new List<COMISSAO>();
                Int32 volta = 0;

                // Processa filtro
                objeto = _baseService.ExecuteFilterComissao(data, idAss);
                if (objeto.Count == 0)
                {
                    volta = 1;
                }

                // Monta tupla
                var tupla = Tuple.Create(volta, objeto, true);
                return tupla;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

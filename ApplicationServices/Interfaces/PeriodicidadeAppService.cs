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
using DataServices.Repositories;

namespace ApplicationServices.Services
{
    public class PeriodicidadeAppService : AppServiceBase<PERIODICIDADE_TAREFA>, IPeriodicidadeAppService
    {
        private readonly IPeriodicidadeService _baseService;

        public PeriodicidadeAppService(IPeriodicidadeService baseService) : base(baseService)
        {
            _baseService = baseService;
        }


        public PERIODICIDADE_TAREFA CheckExist(PERIODICIDADE_TAREFA conta)
        {
            PERIODICIDADE_TAREFA item = _baseService.CheckExist(conta);
            return item;
        }

        public List<PERIODICIDADE_TAREFA> GetAllItens()
        {
            List<PERIODICIDADE_TAREFA> lista = _baseService.GetAllItens();
            return lista;
        }

        public List<PERIODICIDADE_TAREFA> GetAllItensAdm()
        {
            List<PERIODICIDADE_TAREFA> lista = _baseService.GetAllItensAdm();
            return lista;
        }

        public PERIODICIDADE_TAREFA GetItemById(Int32 id)
        {
            PERIODICIDADE_TAREFA item = _baseService.GetItemById(id);
            return item;
        }

        public Int32 ValidateCreate(PERIODICIDADE_TAREFA item, USUARIO usuario)
        {
            try
            {
                // Completa objeto
                item.PETA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "AddPERI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE_TAREFA>(item)
                };

                // Persiste
                Int32 volta = _baseService.Create(item, log);
                return volta;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateEdit(PERIODICIDADE_TAREFA item, PERIODICIDADE_TAREFA itemAntes, USUARIO usuario)
        {
            try
            {
                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_NM_OPERACAO = "EditPERI",
                    LOG_IN_ATIVO = 1,
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE_TAREFA>(item),
                    LOG_TX_REGISTRO_ANTES = Serialization.SerializeJSON<PERIODICIDADE_TAREFA>(itemAntes)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateDelete(PERIODICIDADE_TAREFA item, USUARIO usuario)
        {
            try
            {
                // Checa integridade
                if (item.TAREFA.Count > 0)
                {
                    return 1;
                }
                //if (item.CUSTO_FIXO.Count > 0)
                //{
                //    return 1;
                //}

                // Acerta campos
                item.PETA_IN_ATIVO = 0;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "DelPERI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE_TAREFA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Int32 ValidateReativar(PERIODICIDADE_TAREFA item, USUARIO usuario)
        {
            try
            {
                // Verifica integridade referencial

                // Acerta campos
                item.PETA_IN_ATIVO = 1;

                // Monta Log
                LOG log = new LOG
                {
                    LOG_DT_DATA = DateTime.Now,
                    ASSI_CD_ID = usuario.ASSI_CD_ID,
                    USUA_CD_ID = usuario.USUA_CD_ID,
                    LOG_IN_ATIVO = 1,
                    LOG_NM_OPERACAO = "ReatPERI",
                    LOG_TX_REGISTRO = Serialization.SerializeJSON<PERIODICIDADE_TAREFA>(item)
                };

                // Persiste
                return _baseService.Edit(item, log);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PERIODICIDADE_TAREFA> GetByAssinante(USUARIO usuario)
        {
            return _baseService.GetAllItens().Where(x => x.ASSI_CD_ID == usuario.ASSI_CD_ID).ToList();
        }
    }
}

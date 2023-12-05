using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Interfaces;
using EntitiesServices.Model;
using System.Globalization;
using ERP_Condominios_Solution;
using CRMPresentation.App_Start;
using EntitiesServices.WorkClasses;
using AutoMapper;
using ERP_Condominios_Solution.ViewModels;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections;
using System.Web.UI.WebControls;
using System.Runtime.Caching;
using Image = iTextSharp.text.Image;
using System.Text;
using System.Net;
using CrossCutting;


namespace ERP_Condominios_Solution.Controllers
{
    public class TabelaAuxiliarController : Controller
    {
        private readonly ICargoAppService carApp;
        private readonly ILogAppService logApp;
        private readonly ICategoriaClienteAppService ccApp;
        private readonly ITipoAcaoAppService taApp;
        private readonly IMotivoCancelamentoAppService mcApp;
        private readonly IMotivoEncerramentoAppService meApp;
        private readonly ITipoTarefaAppService ttApp;
        private readonly ICategoriaAgendaAppService caApp;
        private readonly ITipoFollowAppService foApp;

        CARGO objetoCargo = new CARGO();
        CARGO objetoAntesCargo = new CARGO();
        List<CARGO> listaMasterCargo = new List<CARGO>();
        CATEGORIA_CLIENTE objetoCatCliente = new CATEGORIA_CLIENTE();
        CATEGORIA_CLIENTE objetoAntesCatCliente = new CATEGORIA_CLIENTE();
        List<CATEGORIA_CLIENTE> listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
        TIPO_ACAO objetoTipoAcao = new TIPO_ACAO();
        TIPO_ACAO objetoAntesTipoAcao = new TIPO_ACAO();
        List<TIPO_ACAO> listaMasterTipoAcao= new List<TIPO_ACAO>();
        MOTIVO_CANCELAMENTO objetoMotCancelamento = new MOTIVO_CANCELAMENTO();
        MOTIVO_CANCELAMENTO objetoAntesMotCancelamento = new MOTIVO_CANCELAMENTO();
        List<MOTIVO_CANCELAMENTO> listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
        MOTIVO_ENCERRAMENTO objetoMotEncerramento = new MOTIVO_ENCERRAMENTO();
        MOTIVO_ENCERRAMENTO objetoAntesMotEncerramento = new MOTIVO_ENCERRAMENTO();
        List<MOTIVO_ENCERRAMENTO> listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
        TIPO_TAREFA objetoTarefa = new TIPO_TAREFA();
        TIPO_TAREFA objetoAntesTarefa = new TIPO_TAREFA();
        List<TIPO_TAREFA> listaMasterTarefa = new List<TIPO_TAREFA>();
        CATEGORIA_AGENDA objetoCatAgenda = new CATEGORIA_AGENDA();
        CATEGORIA_AGENDA objetoAntesCatAgenda = new CATEGORIA_AGENDA();
        List<CATEGORIA_AGENDA> listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
        TIPO_FOLLOW objetoFollow = new TIPO_FOLLOW();
        TIPO_FOLLOW objetoAntesFollow = new TIPO_FOLLOW();
        List<TIPO_FOLLOW> listaMasterFollow = new List<TIPO_FOLLOW>();
        String extensao;

        public TabelaAuxiliarController(ICargoAppService carApps, ILogAppService logApps, ICategoriaClienteAppService ccApps, ITipoAcaoAppService taApps, IMotivoCancelamentoAppService mcApps, IMotivoEncerramentoAppService meApps, ITipoTarefaAppService ttApps,ICategoriaAgendaAppService caApps, ITipoFollowAppService foApps)
        {
            carApp = carApps;
            logApp = logApps;
            ccApp = ccApps;
            taApp = taApps;
            mcApp = mcApps;
            meApp = meApps; 
            ttApp = ttApps;
            caApp = caApps;
            foApp = foApps;
        }

        [HttpGet]
        public ActionResult Index()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return View();
        }

        public ActionResult Voltar()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("CarregarBase", "BaseAdmin");
        }

        public ActionResult VoltarGeral()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaTabelasAuxiliares", "BaseAdmin");
        }

        public ActionResult VoltarDash()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            return RedirectToAction("MontarTelaDashboardAdministracao", "BaseAdmin");
        }

        [HttpGet]
        public ActionResult MontarTelaCargo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCargo"] == null)
            {
                listaMasterCargo = carApp.GetAllItens(idAss);
                Session["ListaCargo"] = listaMasterCargo;
            }
            ViewBag.Listas = (List<CARGO>)Session["ListaCargo"];
            ViewBag.Title = "Cargo";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CARGO>)Session["ListaCargo"]).Count;

            if (Session["MensCargo"] != null)
            {
                if ((Int32)Session["MensCargo"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0154", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCargo"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0155", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCargo"] = 1;
            Session["MensCargo"] = 0;
            objetoCargo = new CARGO();
            return View(objetoCargo);
        }

        public ActionResult RetirarFiltroCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCargo"] = null;
            Session["FiltroCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult MostrarTudoCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCargo = carApp.GetAllItensAdm(idAss);
            Session["FiltroCargo"] = null;
            Session["ListaCargo"] = listaMasterCargo;
            return RedirectToAction("MontarTelaCargo");
        }

        public ActionResult VoltarBaseCargo()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCargo"] == 2)
            {
                return RedirectToAction("IncluirUsuario", "Usuario");
            }
            if ((Int32)Session["VoltaCargo"] == 3)
            {
                return RedirectToAction("VoltarAnexoUsuario", "Usuario");
            }
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult IncluirCargo()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CARGO item = new CARGO();
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CARG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCargo(CargoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = carApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCargo"] = 3;
                        return RedirectToAction("MontarTelaCargo");
                    }
                    Session["IdVolta"] = item.CARG_CD_ID;

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    Session["ListaCargo"] = null;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerCargo(Int32 id)
        {
            
            // Prepara view
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCargo(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            Session["Cargo"] = item;
            Session["IdCargo"] = id;
            CargoViewModel vm = Mapper.Map<CARGO, CargoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCargo(CargoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CARGO item = Mapper.Map<CargoViewModel, CARGO>(vm);
                    Int32 volta = carApp.ValidateEdit(item, objetoAntesCargo, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCargo = new List<CARGO>();
                    Session["ListaCargo"] = null;
                    return RedirectToAction("MontarTelaCargo");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCargo(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CARGO item = carApp.GetItemById(id);
            objetoAntesCargo = item;
            item.CARG_IN_ATIVO = 0;
            Int32 volta = carApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCargo"] = 4;
                return RedirectToAction("MontarTelaCargo");
            }
            listaMasterCargo = new List<CARGO>();
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }

        [HttpGet]
        public ActionResult ReativarCargo(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CARGO item = carApp.GetItemById(id);
            item.CARG_IN_ATIVO = 1;
            objetoAntesCargo = item;
            Int32 volta = carApp.ValidateReativar(item, usuario);
            listaMasterCargo = new List<CARGO>();
            Session["ListaCargo"] = null;
            return RedirectToAction("MontarTelaCargo");
        }
    
        [HttpGet]
        public ActionResult MontarTelaCatCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCatCliente"] == null)
            {
                listaMasterCatCliente= ccApp.GetAllItens(idAss);
                Session["ListaCatCliente"] = listaMasterCatCliente;
            }
            ViewBag.Listas = (List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"];
            ViewBag.Title = "CatCliente";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_CLIENTE>)Session["ListaCatCliente"]).Count;

            if (Session["MensCatCliente"] != null)
            {
                if ((Int32)Session["MensCatCliente"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0176", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatCliente"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatCliente"] = 1;
            Session["MensCatCliente"] = 0;
            objetoCatCliente = new CATEGORIA_CLIENTE();
            return View(objetoCatCliente);
        }

        public ActionResult RetirarFiltroCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatCliente"] = null;
            Session["FiltroCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult MostrarTudoCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatCliente= ccApp.GetAllItensAdm(idAss);
            Session["FiltroCatCliente"] = null;
            Session["ListaCatCliente"] = listaMasterCatCliente;
            return RedirectToAction("MontarTelaCatCliente");
        }

        public ActionResult VoltarBaseCatCliente()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaCatCliente"] == 2)
            {
                return RedirectToAction("IncluirCliente", "Cliente");
            }
            if ((Int32)Session["VoltaCatCliente"] == 3)
            {
                return RedirectToAction("VoltarAnexoCliente", "Cliente");
            }
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult IncluirCatCliente()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_CLIENTE item = new CATEGORIA_CLIENTE();
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CACL_IN_ATIVO = 1;
            vm.CACL_IN_LIMITE_MAXIMO_EMITIDAS = 0;
            vm.CACL_IN_LIMITE_MINIMO_APROVADAS = 0;
            vm.CACL_IN_LIMITE_MINIMO_EMITIDAS = 0;
            vm.CACL_IN_LIMITE_MINIMO_REPROVADAS = 0;
            vm.CACL_IN_LIMITE__MAXIMO_APROVADAS = 0;
            vm.CACL_IN_LIMITE__MAXIMO_REPROVADAS = 0;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatCliente(CategoriaClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica ordem
                    CATEGORIA_CLIENTE cl = ccApp.GetAllItens(idAss).Where(p => p.CACL_IN_ORDEM == vm.CACL_IN_ORDEM).FirstOrDefault();
                    if (cl != null)
                    {
                        ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0236", CultureInfo.CurrentCulture));
                        return View(vm);
                    }

                    // Executa a operação
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ccApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatCliente"] = 3;
                        return RedirectToAction("MontarTelaCatCliente");
                    }
                    Session["IdVolta"] = item.CACL_CD_ID;

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    return RedirectToAction("VoltarBaseCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerCatCliente(Int32 id)
        {
            
            // Prepara view
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarCatCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            objetoAntesCatCliente= item;
            Session["CatCliente"] = item;
            Session["IdCatCliente"] = id;
            CategoriaClienteViewModel vm = Mapper.Map<CATEGORIA_CLIENTE, CategoriaClienteViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatCliente(CategoriaClienteViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_CLIENTE item = Mapper.Map<CategoriaClienteViewModel, CATEGORIA_CLIENTE>(vm);
                    Int32 volta = ccApp.ValidateEdit(item, objetoAntesCatCliente, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatCliente= new List<CATEGORIA_CLIENTE>();
                    Session["ListaCatCliente"] = null;
                    return RedirectToAction("MontarTelaCatCliente");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 0;
            Int32 volta = ccApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatCliente"] = 4;
                return RedirectToAction("MontarTelaCatCliente");
            }
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult ReativarCatCliente(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatCliente");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_CLIENTE item = ccApp.GetItemById(id);
            item.CACL_IN_ATIVO = 1;
            Int32 volta = ccApp.ValidateReativar(item, usuario);
            listaMasterCatCliente = new List<CATEGORIA_CLIENTE>();
            Session["ListaCatCliente"] = null;
            return RedirectToAction("MontarTelaCatCliente");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoAcao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaTipoAcao"] == null)
            {
                listaMasterTipoAcao = taApp.GetAllItens(idAss);
                Session["ListaTipoAcao"] = listaMasterTipoAcao;
            }
            ViewBag.Listas = (List<TIPO_ACAO>)Session["ListaTipoAcao"];
            ViewBag.Title = "TipoAcao";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoAcao = ((List<TIPO_ACAO>)Session["ListaTipoAcao"]).Count;

            if (Session["MensTipoAcao"] != null)
            {
                if ((Int32)Session["MensTipoAcao"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0183", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoAcao"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0184", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoAcao"] = 1;
            Session["MensTipoAcao"] = 0;
            objetoTipoAcao = new TIPO_ACAO();
            return View(objetoTipoAcao);
        }

        public ActionResult RetirarFiltroTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoAcao"] = null;
            Session["FiltroTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult MostrarTudoTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTipoAcao= taApp.GetAllItensAdm(idAss);
            Session["FiltroTipoAcao"] = null;
            Session["ListaTipoAcao"] = listaMasterTipoAcao;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        public ActionResult VoltarBaseTipoAcao()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaTipoAcao"] == 2)
            {
                return RedirectToAction("IncluirAcao", "CRM");
            }
            if ((Int32)Session["VoltaTipoAcao"] == 3)
            {
                return RedirectToAction("AcompanhamentoProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult IncluirTipoAcao()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");

            // Prepara view
            TIPO_ACAO item = new TIPO_ACAO();
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIAC_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoAcao(TipoAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = taApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoAcao"] = 3;
                        return RedirectToAction("MontarTelaTipoAcao");
                    }
                    Session["IdVolta"] = item.TIAC_CD_ID;

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoAcao(Int32 id)
        {
            
            // Prepara view
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            Session["TipoAcao"] = item;
            Session["IdTipoAcao"] = id;
            TipoAcaoViewModel vm = Mapper.Map<TIPO_ACAO, TipoAcaoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoAcao(TipoAcaoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_ACAO item = Mapper.Map<TipoAcaoViewModel, TIPO_ACAO>(vm);
                    Int32 volta = taApp.ValidateEdit(item, objetoAntesTipoAcao, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTipoAcao = new List<TIPO_ACAO>();
                    Session["ListaTipoAcao"] = null;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_ACAO item = taApp.GetItemById(id);
            objetoAntesTipoAcao = item;
            item.TIAC_IN_ATIVO = 0;
            Int32 volta = taApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoAcao"] = 4;
                return RedirectToAction("MontarTelaTipoAcao");
            }
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult ReativarTipoAcao(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoAcao");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_ACAO item = taApp.GetItemById(id);
            item.TIAC_IN_ATIVO = 1;
            objetoAntesTipoAcao = item;
            Int32 volta = taApp.ValidateReativar(item, usuario);
            listaMasterTipoAcao = new List<TIPO_ACAO>();
            Session["ListaTipoAcao"] = null;
            return RedirectToAction("MontarTelaTipoAcao");
        }

        [HttpGet]
        public ActionResult MontarTelaMotCancelamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaMotCancelamento"] == null)
            {
                listaMasterMotCancelamento = mcApp.GetAllItens(idAss);
                Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            }
            ViewBag.Listas = (List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"];
            ViewBag.Title = "MotCancelamento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_CANCELAMENTO>)Session["ListaMotCancelamento"]).Count;

            if (Session["MensMotCancelamento"] != null)
            {
                if ((Int32)Session["MensMotCancelamento"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0185", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotCancelamento"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0186", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotCancelamento"] = 1;
            Session["MensMotCancelamento"] = 0;
            objetoMotCancelamento = new MOTIVO_CANCELAMENTO();
            return View(objetoMotCancelamento);
        }

        public ActionResult RetirarFiltroMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult MostrarTudoMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotCancelamento = mcApp.GetAllItensAdm(idAss);
            Session["ListaMotCancelamento"] = listaMasterMotCancelamento;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        public ActionResult VoltarBaseMotCancelamento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotCancelamento"] == 2)
            {
                return RedirectToAction("VoltarCancelarPedido", "CRM");
            }
            if ((Int32)Session["VoltaMotCancelamento"] == 3)
            {
                return RedirectToAction("VoltarCancelarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult IncluirMotCancelamento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_CANCELAMENTO item = new MOTIVO_CANCELAMENTO();
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOCA_IN_ATIVO = 1;
            vm.MOCA_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotCancelamento(MotivoCancelamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = mcApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotCancelamento"] = 3;
                        return RedirectToAction("MontarTelaMotCancelamento");
                    }
                    Session["IdVolta"] = item.MOCA_CD_ID;

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    return RedirectToAction("VoltarBaseMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerMotCancelamento(Int32 id)
        {
            
            // Prepara view
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarMotCancelamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            Session["MotCancelamento"] = item;
            Session["IdMotCancelamento"] = id;
            MotivoCancelamentoViewModel vm = Mapper.Map<MOTIVO_CANCELAMENTO, MotivoCancelamentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotCancelamento(MotivoCancelamentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_CANCELAMENTO item = Mapper.Map<MotivoCancelamentoViewModel, MOTIVO_CANCELAMENTO>(vm);
                    Int32 volta = mcApp.ValidateEdit(item, objetoAntesMotCancelamento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
                    Session["ListaMotCancelamento"] = null;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirMotCancelamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            objetoAntesMotCancelamento = item;
            item.MOCA_IN_ATIVO = 0;
            Int32 volta = mcApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotCancelamento"] = 4;
                return RedirectToAction("MontarTelaMotCancelamento");
            }
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult ReativarMotCancelamento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotCancelamento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_CANCELAMENTO item = mcApp.GetItemById(id);
            item.MOCA_IN_ATIVO = 1;
            objetoAntesMotCancelamento = item;
            Int32 volta = mcApp.ValidateReativar(item, usuario);
            listaMasterMotCancelamento = new List<MOTIVO_CANCELAMENTO>();
            Session["ListaMotCancelamento"] = null;
            return RedirectToAction("MontarTelaMotCancelamento");
        }

        [HttpGet]
        public ActionResult MontarTelaMotEncerramento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaMotEncerramento"] == null)
            {
                listaMasterMotEncerramento = meApp.GetAllItens(idAss);
                Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            }
            ViewBag.Listas = (List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"];
            ViewBag.Title = "MotEncerramento";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<MOTIVO_ENCERRAMENTO>)Session["ListaMotEncerramento"]).Count;

            if (Session["MensMotEncerramento"] != null)
            {
                if ((Int32)Session["MensMotEncerramento"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0185", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensMotEncerramento"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0186", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaMotEncerramento"] = 1;
            Session["MensMotEncerramento"] = 0;
            objetoMotEncerramento = new MOTIVO_ENCERRAMENTO();
            return View(objetoMotEncerramento);
        }

        public ActionResult RetirarFiltroMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult MostrarTudoMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterMotEncerramento = meApp.GetAllItensAdm(idAss);
            Session["ListaMotEncerramento"] = listaMasterMotEncerramento;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        public ActionResult VoltarBaseMotEncerramento()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if ((Int32)Session["VoltaMotEncerramento"] == 2)
            {
                return RedirectToAction("VoltarEncerrarProcessoCRM", "CRM");
            }
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult IncluirMotEncerramento()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_ENCERRAMENTO item = new MOTIVO_ENCERRAMENTO();
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.MOEN_IN_ATIVO = 1;
            vm.MOEN_IN_TIPO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirMotEncerramento(MotivoEncerramentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            List<SelectListItem> tipo = new List<SelectListItem>();
            tipo.Add(new SelectListItem() { Text = "Processos CRM", Value = "1" });
            tipo.Add(new SelectListItem() { Text = "Atendimentos", Value = "2" });
            ViewBag.Tipo = new SelectList(tipo, "Value", "Text");
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = meApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensMotEncerramento"] = 3;
                        return RedirectToAction("MontarTelaMotEncerramento");
                    }
                    Session["IdVolta"] = item.MOEN_CD_ID;

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    return RedirectToAction("VoltarBaseMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarMotEncerramento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            Session["MotEncerramento"] = item;
            Session["IdMotEncerramento"] = id;
            MotivoEncerramentoViewModel vm = Mapper.Map<MOTIVO_ENCERRAMENTO, MotivoEncerramentoViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarMotEncerramento(MotivoEncerramentoViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    MOTIVO_ENCERRAMENTO item = Mapper.Map<MotivoEncerramentoViewModel, MOTIVO_ENCERRAMENTO>(vm);
                    Int32 volta = meApp.ValidateEdit(item, objetoAntesMotEncerramento, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
                    Session["ListaMotEncerramento"] = null;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirMotEncerramento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCargo");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            objetoAntesMotEncerramento = item;
            item.MOEN_IN_ATIVO = 0;
            Int32 volta = meApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensMotEncerramento"] = 4;
                return RedirectToAction("MontarTelaMotEncerramento");
            }
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult ReativarMotEncerramento(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaMotEncerramento");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            MOTIVO_ENCERRAMENTO item = meApp.GetItemById(id);
            item.MOEN_CD_ID = 1;
            objetoAntesMotEncerramento = item;
            Int32 volta = meApp.ValidateReativar(item, usuario);
            listaMasterMotEncerramento = new List<MOTIVO_ENCERRAMENTO>();
            Session["ListaMotEncerramento"] = null;
            return RedirectToAction("MontarTelaMotEncerramento");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoTarefa()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaTipoTarefa"] == null)
            {
                listaMasterTarefa = ttApp.GetAllItens(idAss);
                Session["ListaTipoTarefa"] = listaMasterTarefa;
            }
            ViewBag.Listas = (List<TIPO_TAREFA>)Session["ListaTipoTarefa"];
            ViewBag.Title = "TipoTarefa";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoTarefa = ((List<TIPO_TAREFA>)Session["ListaTipoTarefa"]).Count;

            if (Session["MensTipoTarefa"] != null)
            {
                if ((Int32)Session["MensTipoTarefa"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0212", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTarefa"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoTarefa"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0213", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoTarefa"] = 1;
            Session["MensTipoTarefa"] = 0;
            objetoTarefa= new TIPO_TAREFA();
            return View(objetoTarefa);
        }

        public ActionResult RetirarFiltroTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoTarefa"] = null;
            Session["FiltroTipoTarefa"] = null;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        public ActionResult MostrarTudoTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterTarefa = ttApp.GetAllItensAdm(idAss);
            Session["FiltroTipoTarefa"] = null;
            Session["ListaTipoTarefa"] = listaMasterTarefa;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        public ActionResult VoltarBaseTipoTarefa()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult IncluirTipoTarefa()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_TAREFA item = new TIPO_TAREFA();
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TITR_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoTarefa(TipoTarefaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_TAREFA item = Mapper.Map<TipoTarefaViewModel, TIPO_TAREFA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = ttApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoTarefa"] = 3;
                        return RedirectToAction("MontarTelaTipoTarefa");
                    }
                    Session["IdVolta"] = item.TITR_CD_ID;

                    // Sucesso
                    listaMasterTarefa = new List<TIPO_TAREFA>();
                    Session["ListaTipoTarefa"] = null;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoTarefa(Int32 id)
        {
            
            // Prepara view
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            Session["TipoTarefa"] = item;
            Session["IdTipoTarefa"] = id;
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoTarefa(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            Session["TipoTarefa"] = item;
            Session["IdTipoTarefa"] = id;
            TipoTarefaViewModel vm = Mapper.Map<TIPO_TAREFA, TipoTarefaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoTarefa(TipoTarefaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_TAREFA item = Mapper.Map<TipoTarefaViewModel, TIPO_TAREFA>(vm);
                    Int32 volta = ttApp.ValidateEdit(item, objetoAntesTarefa, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterTarefa = new List<TIPO_TAREFA>();
                    Session["ListaTipoTarefa"] = null;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoTarefa(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            item.TITR_IN_ATIVO = 0;
            Int32 volta = ttApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoTarefa"] = 4;
                return RedirectToAction("MontarTelaTipoTarefa");
            }
            listaMasterTarefa = new List<TIPO_TAREFA>();
            Session["ListaTipoTarefa"] = null;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult ReativarTipoTarefa(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_TAREFA item = ttApp.GetItemById(id);
            objetoAntesTarefa = item;
            item.TITR_IN_ATIVO = 1;
            Int32 volta = ttApp.ValidateReativar(item, usuario);
            listaMasterTarefa = new List<TIPO_TAREFA>();
            Session["ListaTipoTarefa"] = null;
            return RedirectToAction("MontarTelaTipoTarefa");
        }

        [HttpGet]
        public ActionResult MontarTelaCatAgenda()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaCatAgenda"] == null)
            {
                listaMasterCatAgenda= caApp.GetAllItens(idAss);
                Session["ListaCatAgenda"] = listaMasterCatAgenda;
            }
            ViewBag.Listas = (List<CATEGORIA_AGENDA>)Session["ListaCatAgenda"];
            ViewBag.Title = "CatAgenda";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.Cargo = ((List<CATEGORIA_AGENDA>)Session["ListaCatAgenda"]).Count;

            if (Session["MensCatAgenda"] != null)
            {
                if ((Int32)Session["MensCatAgenda"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0255", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAgenda"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensCatAgenda"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0177", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaCatAgenda"] = 1;
            Session["MensCatAgenda"] = 0;
            objetoCatAgenda= new CATEGORIA_AGENDA();
            return View(objetoCatAgenda);
        }

        public ActionResult RetirarFiltroCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaCatAgenda"] = null;
            Session["FiltroCatAgenda"] = null;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        public ActionResult MostrarTudoCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterCatAgenda = caApp.GetAllItensAdm(idAss);
            Session["FiltroCatAgenda"] = null;
            Session["ListaCatAgenda"] = listaMasterCatAgenda;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        public ActionResult VoltarBaseCatAgenda()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult IncluirCatAgenda()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_AGENDA item = new CATEGORIA_AGENDA();
            CategoriaAgendaViewModel vm = Mapper.Map<CATEGORIA_AGENDA, CategoriaAgendaViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.CAAG_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirCatAgenda(CategoriaAgendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    CATEGORIA_AGENDA item = Mapper.Map<CategoriaAgendaViewModel, CATEGORIA_AGENDA>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = caApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensCatAgenda"] = 3;
                        return RedirectToAction("MontarTelaCatAgenda");
                    }
                    Session["IdVolta"] = item.CAAG_CD_ID;

                    // Sucesso
                    listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
                    Session["ListaCatAgenda"] = null;
                    return RedirectToAction("VoltarBaseCatAgenda");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult EditarCatAgenda(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            objetoAntesCatAgenda = item;
            Session["CatAgenda"] = item;
            Session["IdCatAgenda"] = id;
            CategoriaAgendaViewModel vm = Mapper.Map<CATEGORIA_AGENDA, CategoriaAgendaViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarCatAgenda(CategoriaAgendaViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    CATEGORIA_AGENDA item = Mapper.Map<CategoriaAgendaViewModel, CATEGORIA_AGENDA>(vm);
                    Int32 volta = caApp.ValidateEdit(item, objetoAntesCatAgenda, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
                    Session["ListaCatAgenda"] = null;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirCatAgenda(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            item.CAAG_IN_ATIVO = 0;
            Int32 volta = caApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensCatAgenda"] = 4;
                return RedirectToAction("MontarTelaCatAgenda");
            }
            listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
            Session["ListaCatAgenda"] = null;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult ReativarCatAgenda(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaCatAgenda");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            CATEGORIA_AGENDA item = caApp.GetItemById(id);
            item.CAAG_IN_ATIVO = 1;
            Int32 volta = caApp.ValidateReativar(item, usuario);
            listaMasterCatAgenda = new List<CATEGORIA_AGENDA>();
            Session["ListaCatAgenda"] = null;
            return RedirectToAction("MontarTelaCatAgenda");
        }

        [HttpGet]
        public ActionResult MontarTelaTipoFollow()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA == "VIS")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("CarregarBase", "BaseAdmin");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Carrega listas
            if (Session["ListaTipoFollow"] == null)
            {
                listaMasterFollow= foApp.GetAllItens(idAss);
                Session["ListaTipoFollow"] = listaMasterFollow;
            }
            ViewBag.Listas = (List<TIPO_FOLLOW>)Session["ListaTipoFollow"];
            ViewBag.Title = "TipoTarefa";
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Indicadores
            ViewBag.TipoFollow = ((List<TIPO_FOLLOW>)Session["ListaTipoFollow"]).Count;

            if (Session["MensTipoFollow"] != null)
            {
                if ((Int32)Session["MensTipoFollow"] == 3)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0212", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoFollow"] == 2)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0011", CultureInfo.CurrentCulture));
                }
                if ((Int32)Session["MensTipoFollow"] == 4)
                {
                    ModelState.AddModelError("", CRMSys_Base.ResourceManager.GetString("M0213", CultureInfo.CurrentCulture));
                }
            }

            // Abre view
            Session["VoltaTipoFollow"] = 1;
            Session["MensTipoFollow"] = 0;
            objetoFollow = new TIPO_FOLLOW();
            return View(objetoFollow);
        }

        public ActionResult RetirarFiltroTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Session["ListaTipoFollow"] = null;
            Session["FiltroTipoFollow"] = null;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        public ActionResult MostrarTudoTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            listaMasterFollow= foApp.GetAllItensAdm(idAss);
            Session["FiltroTipoFollow"] = null;
            Session["ListaTipoFollow"] = listaMasterFollow;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        public ActionResult VoltarBaseTipoFollow()
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            return RedirectToAction("MontarTelaTipoFollow");
        }

        [HttpGet]
        public ActionResult IncluirTipoFollow()
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];

                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER" )
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Prepara listas
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_FOLLOW item = new TIPO_FOLLOW();
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            vm.ASSI_CD_ID = usuario.ASSI_CD_ID;
            vm.TIFL_IN_ATIVO = 1;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IncluirTipoFollow(TipoFollowViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
                {
                    // Executa a operação
                    TIPO_FOLLOW item = Mapper.Map<TipoFollowViewModel, TIPO_FOLLOW>(vm);
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    Int32 volta = foApp.ValidateCreate(item, usuarioLogado);

                    // Verifica retorno
                    if (volta == 1)
                    {
                        Session["MensTipoFollow"] = 3;
                        return RedirectToAction("MontarTelaTipoFollow");
                    }
                    Session["IdVolta"] = item.TIFL_CD_ID;

                    // Sucesso
                    listaMasterFollow = new List<TIPO_FOLLOW>();
                    Session["ListaTipoFollow"] = null;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult VerTipoFollow(Int32 id)
        {
            
            // Prepara view
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            Session["TipoFollow"] = item;
            Session["IdTipoFollow"] = id;
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            return View(vm);
        }

        [HttpGet]
        public ActionResult EditarTipoFollow(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            ViewBag.Perfil = usuario.PERFIL.PERF_SG_SIGLA;

            // Prepara view
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            Session["TipoFollow"] = item;
            Session["IdTipoFollow"] = id;
            TipoFollowViewModel vm = Mapper.Map<TIPO_FOLLOW, TipoFollowViewModel>(item);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditarTipoFollow(TipoFollowViewModel vm)
        {
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];
            if (ModelState.IsValid)
            {
                try
            {
                    // Executa a operação
                    USUARIO usuarioLogado = (USUARIO)Session["UserCredentials"];
                    TIPO_FOLLOW item = Mapper.Map<TipoFollowViewModel, TIPO_FOLLOW>(vm);
                    Int32 volta = foApp.ValidateEdit(item, objetoAntesFollow, usuarioLogado);

                    // Verifica retorno

                    // Sucesso
                    listaMasterFollow = new List<TIPO_FOLLOW>();
                    Session["ListaTipoFollow"] = null;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.Message;
                    return View(vm);
                }
            }
            else
            {
                return View(vm);
            }
        }

        [HttpGet]
        public ActionResult ExcluirTipoFollow(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoTarefa");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow= item;
            item.TIFL_IN_ATIVO = 0;
            Int32 volta = foApp.ValidateDelete(item, usuario);
            if (volta == 1)
            {
                Session["MensTipoFollow"] = 4;
                return RedirectToAction("MontarTelaTipoFollow");
            }
            listaMasterFollow = new List<TIPO_FOLLOW>();
            Session["ListaTipoFollow"] = null;
            return RedirectToAction("MontarTelaTipoFollow");
        }

        [HttpGet]
        public ActionResult ReativarTipoFollow(Int32 id)
        {
            // Verifica se tem usuario logado
            USUARIO usuario = new USUARIO();
            if ((String)Session["Ativa"] == null)
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            if ((USUARIO)Session["UserCredentials"] != null)
            {
                usuario = (USUARIO)Session["UserCredentials"];
                // Verfifica permissão
                if (usuario.PERFIL.PERF_SG_SIGLA != "ADM" & usuario.PERFIL.PERF_SG_SIGLA != "GER")
                {
                    Session["MensPermissao"] = 2;
                    return RedirectToAction("MontarTelaTipoFollow");
                }
            }
            else
            {
                return RedirectToAction("Logout", "ControleAcesso");
            }
            Int32 idAss = (Int32)Session["IdAssinante"];

            // Executar
            TIPO_FOLLOW item = foApp.GetItemById(id);
            objetoAntesFollow = item;
            item.TIFL_IN_ATIVO = 1;
            Int32 volta = foApp.ValidateReativar(item, usuario);
            listaMasterFollow = new List<TIPO_FOLLOW>();
            Session["ListaTipoFollow"] = null;
            return RedirectToAction("MontarTelaTipoFollow");
        }

    }
}
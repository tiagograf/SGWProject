using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGW.Portal.Controllers
{
	[Authorize]
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.Message = "Lista de Atividades";

			var list = new List<Tuple<string, string, string, string>>();
			list.Add(new Tuple<string, string, string, string>("Pedido", "00001", "Gerente de Vendas", "Confirmar Envio"));
			list.Add(new Tuple<string, string, string, string>("Pedido", "00009", "Gerente de Vendas", "Confirmar Envio"));
			list.Add(new Tuple<string, string, string, string>("Pedido", "00010", "Gerente de Vendas", "Solicitar Produção"));
			ViewBag.ActionRequests = list;

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Sistema Gerenciador de Workflows";

			return View();
		}

		public ActionResult Configuration()
		{
			ViewBag.Message = "Workflows, preferências e outros";

			return View();
		}
	}
}

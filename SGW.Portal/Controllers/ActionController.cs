using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGW.Portal.Models;

namespace SGW.Portal.Controllers
{
    public class ActionController : Controller
    {
        //
        // GET: /Action/

        public ActionResult Index(Guid processInstanceId)
        {
			//load data from process instance
			ViewBag.Message = "Etapa Manual (Confirmar Envio)";
			ActionModel act = new ActionModel();
			act.UserDefinedId = "PED-2013-00001";
			act.Step = "Confirmar Envio";
			act.Assignee = "Tiago";
			act.Entity = "Pedido";
			act.Description =
				"Efetuar o Envio dos itens do pedido antes de aprovar esta etapa.\r\nCaso o pedido não possa ser entregue seleciona a ação REJEITAR.\r\nSe o pedido já foi totalmente enviado selecionar a ação APROVAR.";


			var items = new List<SelectListItem>();
			foreach (var e in typeof(StepActionResult).GetEnumValues())
				items.Add(new SelectListItem() { Text = e.ToString(), Selected = false, Value = e.ToString()});

			//ViewBag.Action = act;
			ViewBag.Results =  items;

            return View(act);
        }


    }
}

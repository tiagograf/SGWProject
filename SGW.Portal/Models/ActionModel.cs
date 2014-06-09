using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SGW.Portal.Models
{
	public enum StepActionResult
	{
		Aprovar,
		Rejeitar
	}
	public class ActionModel
	{
		[Required]
		[Display(Name = "Entidade")]
		public string Entity { get; set; }

		[Required]
		[Display(Name = "Identificador")]
		public string UserDefinedId { get; set; }

		[Display(Name = "Etapa")]
		public string Step { get; set; }

		[Display(Name = "Usuário/Grupo")]
		public string Assignee { get; set; }

		[Display(Name = "Descrição/Instruções da Etapa")]
		public string Description { get; set; }

		[Display(Name = "Ação")]
		public StepActionResult Action { get; set; }
	}
}

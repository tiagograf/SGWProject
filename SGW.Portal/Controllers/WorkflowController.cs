using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using SGW.BusinessLogic.BusinessObject;
using SGW.Common.DataContract;
using SGW.Portal.Models;

namespace SGW.Portal.Controllers
{
	[Authorize]
	public class WorkflowController : Controller
    {
        //
        // GET: /Workflow/

        public ActionResult Index()
        {
            return View();
        }

		public ActionResult WorkflowConfiguration(Guid workflowId, bool displayOnly = true)
		{
			var entityBO = BusinessLogic.Core.GetFactory().GetInstance<IEntityBO>();
			var wfBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkflowBO>();
			var conditionBO = BusinessLogic.Core.GetFactory().GetInstance<IConditionBO>();

			WorkflowStateModel model = new WorkflowStateModel();
			model.EditMode = !displayOnly;
			model.ConditionList = conditionBO.GetAll().Select(o => new ConditionModel() { 
				Id = o.Id,
				Name = o.Name,
				EntityId = o.EntityId,
				ConditionType = o.ConditionType,
				ConditionDisplayText = conditionBO.GetDisplayText(o)
			}).ToList();

			model.EntityList = entityBO.GetAll().Select(o => new SelectListItem(){ Value = o.Id.ToString(), Selected = false, Text = o.Description});

			List<WorkflowStateDataContract> workflowStateList = new List<WorkflowStateDataContract>();
			Session["WorkflowStateList"] = workflowStateList;

			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = new List<WorkflowStateTransitionDataContract>();
			Session["WorkflowStateTransitionList"] = workflowStateTransitionList;

			if (workflowId == Guid.Empty)
			{
				ViewBag.Message = "Novo Workflow";
				model.WorkflowId = Guid.NewGuid();
				model.EditMode = true;
				return View(model);
			}

			ViewBag.Message = "Editar Workflow";

			WorkflowDataContract dt = wfBO.GetById(workflowId);

			model.Name = dt.Description;
			model.EntityId = dt.EntityId;
			model.Active = dt.Active;
			model.ConditionId = dt.StartConditionId;
			model.TriggerDecisionText = model.ConditionList.Where(o => o.Id.Equals(dt.StartConditionId)).First().ConditionDisplayText;
			model.WorkflowId = dt.Id;
			model.WorkflowStateList = dt.WorkflowStateList;

			Session["WorkflowStateList"] = dt.WorkflowStateList;
			Session["WorkflowStateTransitionList"] = dt.WorkflowStateTransitionList;
	



			return View(model);
		}

		[HttpPost]
		public ActionResult SaveWorkflow(WorkflowStateModel model)
		{
			var workflowBO = BusinessLogic.Core.GetFactory().GetInstance<BusinessLogic.BusinessObject.IWorkflowBO>();
			WorkflowDataContract dt = new WorkflowDataContract();

			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];

			bool update = !model.WorkflowId.Equals(Guid.Empty);
			
			dt.Active = model.Active;
			dt.EntityId = model.EntityId;
			dt.Description = model.Name;
			dt.StartConditionId = model.ConditionId;
			dt.WorkflowStateList = workflowStateList;
			dt.WorkflowStateTransitionList = workflowStateTransitionList;
			if (!update)
			{
				dt.Id = Guid.NewGuid();
				var result = workflowBO.Add(dt);
			}
			else
			{
				dt.Id = model.WorkflowId;
				workflowBO.Update(dt);
			}

			if (model.Active)
				return RedirectToAction("WorkflowList","Workflow");
			else
				return RedirectToAction("WorkflowTransitionDetail", "Workflow", new { workflowId = dt.Id, displayOnly = false });
		}

		public ActionResult WorkflowList()
		{
			ViewBag.Message = "Lista de Workflows.";

			var workflowBO = BusinessLogic.Core.GetFactory().GetInstance<BusinessLogic.BusinessObject.IWorkflowBO>();			
			ViewBag.WorkflowList = workflowBO.GetAll();

			return View();
		}

		[HttpPost]
		public ActionResult RemoveState(Guid stateId)
		{

			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];
			workflowStateList.RemoveAll(o => o.Id.Equals(stateId));
			workflowStateTransitionList.RemoveAll(o => o.ToStateId.Equals(stateId) || o.FromStateId.Equals(stateId));
			Session["WorkflowStateList"] = workflowStateList;
			Session["WorkflowStateTransitionList"] = workflowStateTransitionList;
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult RemoveStep(Guid stepId)
		{
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];
			workflowStepList.RemoveAll(o => o.Id.Equals(stepId));
			workflowStepTransitionList.RemoveAll(o => o.ToStepId.Equals(stepId) || o.FromStepId.Equals(stepId));
			Session["WorkflowStepList"] = workflowStepList;
			Session["WorkflowStepTransitionList"] = workflowStepTransitionList;
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult AddState(int leftPos, int topPos, string description, Guid workflowId, Guid entityStatusId, Guid stateId, bool initialState)
		{
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			if (Guid.Empty.Equals(stateId) && workflowStateList.Where(o => o.EntityStatusId.Equals(entityStatusId)).Any())
				return Json(new { Id = "", Message = "Status da entidade já adicionado ao workflow!" });

			WorkflowStateDataContract data;
			if (Guid.Empty.Equals(stateId))
			{
				data = new WorkflowStateDataContract();
				data.Id = Guid.NewGuid();
				workflowStateList.Add(data);
			}
			else
				data = workflowStateList.Where(o => o.Id.Equals(stateId)).First();

			data.Description = description;
			data.UILeftPosition = leftPos;
			data.UITopPosition = topPos;
			data.WorkflowId = workflowId;
			data.EntityStatusId = entityStatusId;
			data.InitialState = initialState;

			Session["WorkflowStateList"] = workflowStateList;
			return Json(new { Id = data.Id.ToString() }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult AddStep(int leftPos, int topPos, string description, Guid workflowId, Guid fromStateId, Guid stepId, string comments, string joinDecision, Guid? participantId, Guid stepTypeId, bool initialState)
		{
			var wfBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkflowBO>();
			

			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			WorkflowStepDataContract data;
			if (Guid.Empty.Equals(stepId))
			{
				data = new WorkflowStepDataContract();
				data.Id = Guid.NewGuid();
				workflowStepList.Add(data);
			}
			else
				data = workflowStepList.Where(o => o.Id.Equals(stepId)).FirstOrDefault();

			data.Description = description;
			data.UILeftPosition = leftPos;
			data.UITopPosition = topPos;
			data.WorkflowId = workflowId;
			data.JoinDecision = joinDecision;
			data.ParticipantId = participantId;
			data.FromStateId = fromStateId;
			data.StepTypeId = stepTypeId;
			data.StepType = wfBO.GetStepTypeById(stepTypeId); 
			data.Comments = comments;
			data.Initial = initialState;

			var manualActionList = (List<ManualActionDataContract>)Session["ManualActionList"];
			foreach (var item in manualActionList.Where(o => o.StepId.Equals(Guid.Empty)))
				item.StepId = data.Id;

			var decisionConditionList = (List<DecisionConditionDataContract>)Session["DecisionConditionList"];
			foreach (var item in decisionConditionList.Where(o => o.StepId.Equals(Guid.Empty)))
				item.StepId = data.Id;
			
			var tranDecisionList = (List<TransitionDecisionDataContract>)Session["TransitionDecisionList"];
			foreach (var item in tranDecisionList.Where(o => o.StepId.Equals(Guid.Empty)))
				item.StepId = data.Id;

			Session["WorkflowStepList"] = workflowStepList;

			data.ManualActionList = manualActionList;
			data.DecisionConditionList = decisionConditionList;
			data.TransitionDecisionList = tranDecisionList;

			string emailBody = "", emailSubject = "", emailTo = "", actionSQLProcedure = "", actionSQLCommand = "", uploadFileType = "";
			Guid emailParticipantId = Guid.Empty;

			data.UploadFileType = uploadFileType;
			data.ActionSQLProcedure = actionSQLProcedure;
			data.ActionSQLCommand = actionSQLCommand;
			data.EmailParticipantId = emailParticipantId;
			data.EmailBody = emailBody;
			data.EmailSubject = emailSubject;
			data.EmailTo = emailTo;

			data.ActionSQLProcedure = actionSQLProcedure;
			data.ActionSQLCommand = actionSQLCommand;

			return Json(new { Id = data.Id.ToString() }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult SaveStatePosition(int leftPos, int topPos, string objName)
		{
			var id = Guid.Parse(objName.Replace("state_", ""));
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			var item = workflowStateList.Where(o => o.Id.Equals(id)).First();
			item.UILeftPosition = leftPos;
			item.UITopPosition = topPos;

			Session["WorkflowStateList"] = workflowStateList;
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public ActionResult SaveStepPosition(int leftPos, int topPos, string objName)
		{
			var id = Guid.Parse(objName.Replace("step_", ""));
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			var item = workflowStepList.Where(o => o.Id.Equals(id)).First();
			item.UILeftPosition = leftPos;
			item.UITopPosition = topPos;

			Session["WorkflowStepList"] = workflowStepList;
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult AddStateDefinition(int leftPos, int topPos, Guid entityId, string stateName, Guid workflowId)
		{
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			var entityBO = BusinessLogic.Core.GetFactory().GetInstance<IEntityBO>();
			WorkflowStateDefinitionModel model = new WorkflowStateDefinitionModel();
			model.Left = leftPos;
			model.Top = topPos;
			model.StateName = stateName;
			model.WorkflowId = workflowId;
			model.EntityStatusList = entityBO.GetAllStatus(entityId).Select(o => new SelectListItem(){ Text = o.Name, Value = o.Id.ToString(), Selected = false });
			model.InitialState = false;

			if (stateName.Contains("state_"))
			{
				model.StateId = Guid.Parse(stateName.Replace("state_", ""));
				var item = workflowStateList.Where(o => o.Id.Equals(model.StateId)).First();
				model.Description = item.Description;
				model.EntityStatusId = item.EntityStatusId;
				model.InitialState = item.InitialState;
			}

			return Json(SGW.Portal.HtmlHelperExtender.RenderPartialViewToString(this, "Partial/WorkflowStateDefinition", model));
		}

		[HttpPost]
		public JsonResult GetStepDefinition(int leftPos, int topPos, Guid fromStateId, string stepName, Guid workflowId, string stepType)
		{
			var conditionBO = BusinessLogic.Core.GetFactory().GetInstance<IConditionBO>();
			var entityBO = BusinessLogic.Core.GetFactory().GetInstance<IEntityBO>();
			var wfBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkflowBO>();

			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];

			WorkflowStepDefinitionModel model = new WorkflowStepDefinitionModel();
			model.Left = leftPos;
			model.Top = topPos;
			model.StepName = stepName;
			model.WorkflowId = workflowId;
			
			model.ConditionList = conditionBO.GetAll().Select(o => new ConditionModel()
			{
				Id = o.Id,
				Name = o.Name,
				EntityId = o.EntityId,
				ConditionType = o.ConditionType,
				ConditionDisplayText = conditionBO.GetDisplayText(o)
			}).ToList();

			model.EntityId = wfBO.GetById(workflowId).EntityId;
			model.EditMode = true;
			model.FromStateId = fromStateId;
			model.StepTypeList = wfBO.GetAllStepType();
			model.StepTypeId = wfBO.GetStepTypeByCommand(stepType).Id;
			model.StepTypeCommand = stepType;
			model.ParticipantList = wfBO.GetAllParticipant();
			model.JoinDecisionList = new List<SelectListItem>();
			model.JoinDecisionList.Add(new SelectListItem() { Text = "Todas etapas anteriores concluídas", Value = "All" });
			model.JoinDecisionList.Add(new SelectListItem() { Text = "Uma etapa anterior concluída", Value = "One"});
			model.TransitionDecisionList = new List<TransitionDecisionDataContract>();
			model.DecisionConditionList = new List<DecisionConditionDataContract>();
			model.ActionSQLProcedures = conditionBO.GetProcedures().Select(o => new SelectListItem() { Text = o, Value = o}).ToList();
			model.TransitionList = workflowStepTransitionList.Select(o => new SelectListItem() { Text = string.Format("{0}({1})", o.Description, o.ToStep.Description), Value = o.Id.ToString() });
			
			model.ManualActionList = new List<ManualActionDataContract>();
			model.DecisionConditionList = new List<DecisionConditionDataContract>();
			model.TransitionDecisionList = new List<TransitionDecisionDataContract>();

			if (stepName.Contains("step_"))
			{
				model.StepId = Guid.Parse(stepName.Replace("step_", ""));
				var item = workflowStepList.Where(o => o.Id.Equals(model.StepId)).First();
				model.Description = item.Description;
				model.Comments = item.Comments;
				model.InitialState = item.Initial;
				model.ParticipantId = item.ParticipantId;

				model.JoinDecisionId = item.JoinDecision;

				model.ManualActionList = item.ManualActionList;

				model.DecisionConditionList = item.DecisionConditionList;

				model.TransitionDecisionList = item.TransitionDecisionList;

				model.UploadFileType = item.UploadFileType;

				model.EmailParticipantId = item.EmailParticipantId;
				model.EmailBody = item.EmailBody;
				model.EmailSubject = item.EmailSubject;
				model.EmailTo = item.EmailTo;

				model.ActionSQLProcedure = item.ActionSQLProcedure;
				model.ActionSQLCommand = item.ActionSQLCommand;
			}


			Session["ManualActionList"] = model.ManualActionList;
			Session["DecisionConditionList"] = model.DecisionConditionList;
			Session["TransitionDecisionList"] = model.TransitionDecisionList;

			return Json(SGW.Portal.HtmlHelperExtender.RenderPartialViewToString(this, "Partial/WorkflowStepDefinition", model));
		}

		[HttpPost]
		public JsonResult StateTransitionDefinition(Guid workflowId)
		{
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];

			var entityBO = BusinessLogic.Core.GetFactory().GetInstance<IEntityBO>();
			WorkflowStateTransitionDefinitionModel model = new WorkflowStateTransitionDefinitionModel();
			model.WorkflowId = workflowId;
			model.StateList = workflowStateList.Select(o => new SelectListItem() { Text = o.Description, Selected = false, Value = o.Id.ToString() });
			model.TransitionList = workflowStateTransitionList.Select(o => new WorkflowStateTransitionDefinitionModel()
			{
				Id = o.Id,
				Description = o.Description,
				FromState = o.FromStateId,
				FromStateDescription = GetStateName(o.FromStateId),
				ToState = o.ToStateId,
				ToStateDescription = GetStateName(o.ToStateId),
				WorkflowId = o.WorkflowId
			});

			return Json(SGW.Portal.HtmlHelperExtender.RenderPartialViewToString(this, "Partial/WorkflowStateTransitionDefinition", model));

		}

		[HttpPost]
		public JsonResult StepTransitionDefinition(Guid workflowId)
		{
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];

			var entityBO = BusinessLogic.Core.GetFactory().GetInstance<IEntityBO>();
			WorkflowStepTransitionDefinitionModel model = new WorkflowStepTransitionDefinitionModel();
			model.WorkflowId = workflowId;
			model.StepList = workflowStepList.Select(o => new SelectListItem() { Text = o.Description, Selected = false, Value = o.Id.ToString() });
			model.TransitionList = workflowStepTransitionList.Select(o => new WorkflowStepTransitionDefinitionModel()
			{
				Id = o.Id,
				Description = o.Description,
				FromStepId = o.FromStepId,
				FromStepDescription = GetStepName(o.FromStepId),
				ToStepId = o.ToStepId,
				ToStepDescription = GetStepName(o.ToStepId),
				WorkflowId = o.WorkflowId
			});

			return Json(SGW.Portal.HtmlHelperExtender.RenderPartialViewToString(this, "Partial/WorkflowStepTransitionDefinition", model));

		}

		[HttpPost]
		public JsonResult AddStateTransition(string description, Guid fromStateId, Guid toStateId, Guid workflowId, bool autoTran)
		{
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];
			WorkflowStateTransitionDataContract tran = new WorkflowStateTransitionDataContract();
			tran.Description = description;
			tran.FromStateId = fromStateId;
			tran.ToStateId = toStateId;
			tran.WorkflowId = workflowId;
			tran.AutoTransition = autoTran;
			tran.Id = Guid.NewGuid();

			workflowStateTransitionList.Add(tran);
			Session["WorkflowStateTransitionList"] = workflowStateTransitionList;

			return Json(new { Id = tran.Id.ToString() }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult RemoveStateTransition(Guid transitionId)
		{
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];
			foreach (var item in workflowStateTransitionList)
			{
				if (item.Id.Equals(transitionId))
				{
					workflowStateTransitionList.Remove(item);
					break;
				}
			}
			Session["WorkflowStateTransitionList"] = workflowStateTransitionList;

			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult AddStepTransition(string description, Guid fromStepId, Guid toStepId, Guid workflowId)
		{
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];
			var tran = new WorkflowStepTransitionDataContract();
			tran.Description = description;
			tran.FromStepId = fromStepId;
			tran.ToStepId = toStepId;
			tran.WorkflowId = workflowId;
			tran.Id = Guid.NewGuid();
			tran.ToStep = workflowStepList.Where(o => o.Id.Equals(tran.ToStepId)).First();
			tran.FromStep = workflowStepList.Where(o => o.Id.Equals(tran.FromStepId)).First();
			workflowStepTransitionList.Add(tran);
			Session["WorkflowStepTransitionList"] = workflowStepTransitionList;

			return Json(new { Id = tran.Id.ToString() }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult RemoveStepTransition(Guid transitionId)
		{
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];
			foreach (var item in workflowStepTransitionList)
			{
				if (item.Id.Equals(transitionId))
				{
					workflowStepTransitionList.Remove(item);
					break;
				}
			}
			Session["WorkflowStepTransitionList"] = workflowStepTransitionList;

			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult StateTransitionList()
		{
			List<WorkflowStateTransitionDataContract> workflowStateTransitionList = (List<WorkflowStateTransitionDataContract>)Session["WorkflowStateTransitionList"];
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];

			var result = (from t in workflowStateTransitionList
						  join sFrom in workflowStateList on t.FromStateId equals sFrom.Id
						  join sTo in workflowStateList on t.ToStateId equals sTo.Id
						  select new
						  {
							  Description = t.Description,
							  From = "state_" + t.FromStateId.ToString(),
							  Target = "state_" + t.ToStateId.ToString()
						  }).ToList();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpGet]
		public JsonResult StepTransitionList()
		{
			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];

			var result = (from t in workflowStepTransitionList
						  join sFrom in workflowStepList on t.FromStepId equals sFrom.Id
						  join sTo in workflowStepList on t.ToStepId equals sTo.Id
						  select new
						  {
							  Description = t.Description,
							  From = "step_" + t.FromStepId.ToString(),
							  Target = "step_" + t.ToStepId.ToString()
						  }).ToList();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult GetSteps(Guid workflowId, Guid fromStateId, int containerLeftOffset, int containerTopOffset)
		{
			var wfBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkflowBO>();

			List<WorkflowStepTransitionDataContract> workflowStepTransitionList = (List<WorkflowStepTransitionDataContract>)Session["WorkflowStepTransitionList"];
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];

			var stateTranList = wfBO.GetAllWorkflowStateTransition(workflowId);
			stateTranList = stateTranList.Where(o => o.FromStateId.Equals(fromStateId)).ToList();

			int pos = 50;

			if (!workflowStepList.Any())
			{
				workflowStepList.Add(new WorkflowStepDataContract()
				{
					Id = Guid.NewGuid(),
					Description = stateTranList.First().FromState.Description,
					Initial = true,
					UILeftPosition = pos + containerLeftOffset,
					UITopPosition = pos + containerTopOffset,
					StepType = new StepTypeDataContract() { StepCommand = "state" }
				});

				foreach (var item in stateTranList)
				{
					pos += 100;

					workflowStepList.Add(new WorkflowStepDataContract()
					{
						Id = Guid.NewGuid(),
						Description = item.ToState.Description,
						Initial = false,
						UILeftPosition = pos + containerLeftOffset,
						UITopPosition = pos + containerTopOffset,
						StepType = new StepTypeDataContract() { StepCommand = "state" }
					});

				}
				Session["WorkflowStepList"] = workflowStepList; 
			}

			var result = (from t in workflowStepList
						  select new
						  {
							  Left = t.UILeftPosition,
							  Top = t.UITopPosition,
							  Command = t.StepType.StepCommand,
							  Initial = t.Initial,
							  Id = t.Id.ToString(),
							  Description = t.Description
						  }).ToList();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		public JsonResult AddManualAction(string code, string description, Guid stepId)
		{
			var list = (List<ManualActionDataContract>)Session["ManualActionList"];
			list.Add(new ManualActionDataContract() { Code = code, Description = description, StepId = stepId });
			Session["ManualActionList"] = list;

			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult RemoveManualAction(string code, Guid stepId)
		{
			var list = (List<ManualActionDataContract>)Session["ManualActionList"];
			list.RemoveAll(o => o.Code.Equals(code));
			Session["ManualActionList"] = list;

			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult AddDecisionCondition(string code, string description, Guid conditionId, Guid stepId)
		{
			var list = (List<DecisionConditionDataContract>)Session["DecisionConditionList"];
			list.Add(new DecisionConditionDataContract() { Code = code, ConditionId = conditionId, Description = description, StepId = stepId});
			Session["DecisionConditionList"] = list;

			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult RemoveDecisionCondition(string code, Guid stepId)
		{
			var list = (List<DecisionConditionDataContract>)Session["DecisionConditionList"];
			list.RemoveAll(o => o.Code.Equals(code));
			Session["DecisionConditionList"] = list;

			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult AddTranDecision(string code, string description, Guid transitionId, Guid stepId)
		{
			var list = (List<TransitionDecisionDataContract>)Session["TransitionDecisionList"];
			list.Add(new TransitionDecisionDataContract() { Code = code, TransitionId = transitionId, TransitionDescription = description, StepId = stepId });
			Session["TransitionDecisionList"] = list;

			return Json(new { success = true });
		}

		[HttpPost]
		public JsonResult RemoveTranDecision(string code, Guid stepId)
		{
			var list = (List<TransitionDecisionDataContract>)Session["TransitionDecisionList"];
			list.RemoveAll(o => o.Code.Equals(code));
			Session["TransitionDecisionList"] = list;

			return Json(new { success = true });
		}

		public string GetStateName(Guid stateId)
		{
			List<WorkflowStateDataContract> workflowStateList = (List<WorkflowStateDataContract>)Session["WorkflowStateList"];
			var result = workflowStateList.Where(o => o.Id.Equals(stateId)).Select(o => o.Description).FirstOrDefault();
			return result;
		}

		public string GetStepName(Guid stepId)
		{
			List<WorkflowStepDataContract> workflowStepList = (List<WorkflowStepDataContract>)Session["WorkflowStepList"];
			var result = workflowStepList.Where(o => o.Id.Equals(stepId)).Select(o => o.Description).FirstOrDefault();
			return result;
		}


		public ActionResult WorkflowTransitionDetail(Guid workflowId, bool displayOnly = true)
		{
			var wfBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkflowBO>();
			var conditionBO = BusinessLogic.Core.GetFactory().GetInstance<IConditionBO>();
			var model = new WorkflowTransitionDefinitionModel();
			var stateTransitionList = wfBO.GetAllWorkflowStateTransition(workflowId);

			model.EditMode = !displayOnly;

			var groupList = stateTransitionList.GroupBy(o => o.FromStateId).ToList();
			model.StateTransitionList = new List<SelectListItem>();

			foreach (var item in groupList)
			{
				var list = stateTransitionList.Where(o => o.FromStateId.Equals(item.Key)).Select(o => o).ToList();
				StringBuilder sb = new StringBuilder();

				foreach (var item2 in list)
				{
					sb.Append(item.First().FromState.Description);
					sb.Append(string.Format("({0}) -> ", item2.Description));
					sb.Append(item2.ToState.Description);
					sb.Append(" x ");
				}

				model.StateTransitionList.Add(new SelectListItem() { Text = sb.Remove(sb.Length -3,3).ToString(), Value = item.Key.ToString() });
			}

			model.WorkflowId = workflowId;
			model.WorkflowName = wfBO.GetById(workflowId).Description;

			Session["WorkflowStepTransitionList"] = new List<WorkflowStepTransitionDataContract>();
			Session["WorkflowStepList"] = new List<WorkflowStepDataContract>();

			return View(model);
		}
    }
}

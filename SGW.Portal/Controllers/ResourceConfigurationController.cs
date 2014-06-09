using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SGW.BusinessLogic.BusinessObject;
using SGW.Common;
using SGW.Common.DataContract;
using SGW.Portal.Models;

namespace SGW.Portal.Controllers
{

	[Authorize]
	public class ResourceConfigurationController : Controller
    {
        //
        // GET: /ResourceConfiguration/
		[HttpGet]
        public ActionResult EditResource(Guid resourceId, bool displayonly = true)
        {			
			var resourceBO = BusinessLogic.Core.GetFactory().GetInstance<IResourceBO>();
			var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
			ResourceDataContract res = resourceBO.GetById(resourceId, true);
			ResourceConfigurationModel model = new ResourceConfigurationModel();
			model.Active = res.Active;
			model.Email = res.Email;
			model.Name = res.Name;
			model.Id = res.Id;
			model.EditMode = !displayonly;
			model.ResourceRoles = res.ResourceRoles;
			model.Roles = roleBO.GetAll().Select(r => new SelectListItem() { Selected = res.ResourceRoles.Any(rr => rr.Id.Equals(r.Id)), Text = r.Description, Value = r.Id.ToString() }).ToList();
			return View(model);
        }

		[HttpGet]
		public ActionResult WorkgroupRoleList(WorkgroupRoleListModel model)
		{
			var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
			var workgroupBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkgroupBO>();
			if (model == null)
				model = new WorkgroupRoleListModel();
			else if (!string.IsNullOrEmpty(model.Message))
				ModelState.AddModelError("", model.Message);
			model.Roles = roleBO.GetAll();
			model.Workgroups = workgroupBO.GetAll();
			return View(model);
		}

		[HttpGet]
		public ActionResult WorkgroupRoleEdit(WorkgroupRoleOperation op, Guid entityId)
		{
			WorkgroupRoleListModel model = new WorkgroupRoleListModel();
			if (op == WorkgroupRoleOperation.Role)
			{
				var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
				RoleDataContract role = roleBO.GetById(entityId);
				model.RoleId = role.Id;
				model.RoleDescription = role.Name;
			}
			else if (op == WorkgroupRoleOperation.Workgroup)
			{
				var wgBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkgroupBO>();
				WorkgroupDataContract wg = wgBO.GetById(entityId);
				model.WorkgroupId = wg.Id;
				model.WorkgroupDescription = wg.Name;
				model.ParentWorkgroupId = wg.ParentWorkgroupId;
			}

			return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
		}

		[HttpGet]
		public ActionResult WorkgroupRoleDelete(WorkgroupRoleOperation op, Guid entityId)
		{
			WorkgroupRoleListModel model = new WorkgroupRoleListModel();
			OperationResult result;
			if (op == WorkgroupRoleOperation.Role)
			{
				var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
				RoleDataContract role = roleBO.GetById(entityId);
				result = roleBO.Delete(role);
			}
			else 
			{
				var wgBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkgroupBO>();
				WorkgroupDataContract wg = wgBO.GetById(entityId);
				result = wgBO.Delete(wg);
			}

			if (result.Status == OperationResultStatus.Succesfull)
				return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
			else 
				model.Message = result.Message;
			
			return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
		}

		[HttpPost]
		public ActionResult AddRole(WorkgroupRoleListModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
			RoleDataContract role;

			bool update = model.RoleId != null && model.RoleId != Guid.Empty;
			if (!update)
			{
				role = new RoleDataContract();
				role.Id = Guid.NewGuid();
			}
			else
				role = roleBO.GetById(model.RoleId);


			role.Name = model.RoleDescription;
			if (string.IsNullOrEmpty(role.Name))
			{
				model.Message = "Campo Descrição do Cargo é obrigatório.";
				return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
			}

			OperationResult result;
			if (update)
				result = roleBO.Update(role);
			else
				result = roleBO.Add(role);
			

			if (result.Status == OperationResultStatus.ValidationFailure)
				model.Message = result.Message;
			else if (result.Status == OperationResultStatus.UnexpectedError)
				model.Message = result.Exception.ToString();
			else
				return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration");

			return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
		}

		[HttpPost]
		public ActionResult AddWorkgroup(WorkgroupRoleListModel model)
		{
			if (model == null)
				throw new ArgumentNullException("model");

			var wgBO = BusinessLogic.Core.GetFactory().GetInstance<IWorkgroupBO>();
			WorkgroupDataContract wg;

			bool update = model.WorkgroupId != null && model.WorkgroupId != Guid.Empty;
			if (!update) 
			{
				wg = new WorkgroupDataContract();
				wg.Id = Guid.NewGuid(); 
			}
			else
				wg = wgBO.GetById(model.WorkgroupId); 

			wg.Name = model.WorkgroupDescription;
			wg.ParentWorkgroupId = model.ParentWorkgroupId;
			if (string.IsNullOrEmpty(wg.Name))
			{
				model.Message = "Campo Descrição do Grupo é obrigatório.";
				return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
			}

			OperationResult result;
			if (update)
				result = wgBO.Update(wg);
			else
				result = wgBO.Add(wg);

			if (result.Status == OperationResultStatus.ValidationFailure)
				model.Message = result.Message;
			else if (result.Status == OperationResultStatus.UnexpectedError)
				model.Message = result.Exception.ToString();
			else
				return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration");

			return RedirectToAction("WorkgroupRoleList", "ResourceConfiguration", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditResource(ResourceConfigurationModel model)
		{
			if (string.IsNullOrEmpty(model.Password) ||
				string.IsNullOrEmpty(model.ConfirmPassword) ||
				!model.Password.Equals(model.ConfirmPassword))
			{
				ModelState.AddModelError("", "Senha e/ou Confirmação Inválido(s).");
				return View(model);
			}

			var resourceBO = BusinessLogic.Core.GetFactory().GetInstance<IResourceBO>();
			var roleBO = BusinessLogic.Core.GetFactory().GetInstance<IRoleBO>();
			
			ResourceDataContract dataContract;
			dataContract = resourceBO.GetById(model.Id);
			if (dataContract == null)
				dataContract = new ResourceDataContract();

			dataContract.Password = model.Password;
			dataContract.Name = model.Name;
			dataContract.Email = model.Email;
			dataContract.Active = model.Active;
			dataContract.Id = model.Id;
			//dataContract.ResourceRoles = roleBO.GetAll().Where(r => .Contains(r.Description)).ToList();
			var result = resourceBO.Update(dataContract);
			if (result.Status == Common.OperationResultStatus.ValidationFailure)
			{
				ModelState.AddModelError("", result.Message);
				return View(model);
			}
			else
				return RedirectToAction("EditResource", "ResourceConfiguration", new { resourceId = model.Id });
		}

		public enum WorkgroupRoleOperation
		{
			Workgroup,
			Role
		}
	}
}

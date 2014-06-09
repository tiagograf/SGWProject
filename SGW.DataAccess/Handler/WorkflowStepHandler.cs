﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGW.Common.DataContract;

namespace SGW.DataAccess.Handler
{
	public class WorkflowStepHandler : BaseHandler<WorkflowStepDataContract, SGW_WorkflowStep>, IWorkflowStepHandler
    {
		public override Common.OperationResult Add(Common.DataContract.WorkflowStepDataContract dataContract)
		{
			if (dataContract == null)
				throw new ArgumentException("Cannot be Null", "dataContract");

			try
			{
				Core.MainDataContextInstance().SGW_WorkflowSteps.InsertOnSubmit(GetLinqObj(dataContract));
				Core.MainDataContextInstance().SubmitChanges();
				return new Common.OperationResult();
			}
			catch (Exception ex)
			{
				return new Common.OperationResult(ex);
			}

		}


		public override Common.OperationResult Update(WorkflowStepDataContract dataContract)
		{
			if (dataContract == null)
				throw new ArgumentException("Cannot be Null", "dataContract");

			try
			{
				SGW_WorkflowStep obj = Core.MainDataContextInstance().SGW_WorkflowSteps.Where(w => w.StepId.Equals(dataContract.Id)).FirstOrDefault();
				this.GetLinqObj(dataContract, obj);
				Core.MainDataContextInstance().SubmitChanges();
				return new Common.OperationResult();
			}
			catch (Exception ex)
			{
				return new Common.OperationResult(ex);
			}

		}

		public override Common.DataContract.WorkflowStepDataContract GetDataContract(SGW_WorkflowStep linqObj)
		{
			if (linqObj == null)
				return null;

			Common.DataContract.WorkflowStepDataContract dataContract = new Common.DataContract.WorkflowStepDataContract();
			dataContract.Id = linqObj.StepId;
			dataContract.Description = linqObj.Name;
			dataContract.Comments = linqObj.Comments;
			dataContract.JoinDecision = linqObj.JoinDecisionId;
			dataContract.FromStateId = linqObj.FromStateId;
			dataContract.StepTypeId = linqObj.StepTypeId;
			dataContract.UILeftPosition = linqObj.UILeftPosition;
			dataContract.UITopPosition = linqObj.UITopPosition;
			dataContract.WorkflowId = linqObj.WorkflowId;

			dataContract.CreatedBy = linqObj.CreatedBy;
			dataContract.CreatedOn = linqObj.CreatedOn;
			dataContract.UpdatedBy = linqObj.UpdatedBy;
			dataContract.UpdatedOn = linqObj.UpdatedOn;
			return dataContract;
		}


		public override SGW_WorkflowStep GetLinqObj(Common.DataContract.WorkflowStepDataContract dataContract)
		{
			return GetLinqObj(dataContract, new SGW_WorkflowStep());
		}

		public override SGW_WorkflowStep GetLinqObj(Common.DataContract.WorkflowStepDataContract dataContract, SGW_WorkflowStep linq)
		{
			if (dataContract == null)
				return null;

			linq.StepId = dataContract.Id;
			linq.Name = dataContract.Name;
			linq.Comments = dataContract.Comments;
			linq.JoinDecisionId = dataContract.JoinDecision;
			linq.FromStateId = dataContract.FromStateId;
			linq.StepTypeId = dataContract.StepTypeId;
			linq.UILeftPosition = dataContract.UILeftPosition;
			linq.UITopPosition = dataContract.UITopPosition;
			linq.WorkflowId = dataContract.WorkflowId;

			linq.CreatedBy = dataContract.CreatedBy;
			linq.CreatedOn = dataContract.CreatedOn;
			linq.UpdatedBy = dataContract.UpdatedBy;
			linq.UpdatedOn = dataContract.UpdatedOn;
			return linq;
		}


		public override Common.OperationResult Delete(Common.DataContract.WorkflowStepDataContract dataContract)
		{
			if (dataContract == null)
				throw new ArgumentException("Cannot be Null", "dataContract");
			
			try
			{
				SGW_WorkflowStep obj = Core.MainDataContextInstance().SGW_WorkflowSteps.Where(o => o.StepId.Equals(dataContract.Id)).FirstOrDefault();
				if (obj == null)
					return new Common.OperationResult(Common.OperationResultStatus.ValidationFailure, "Status da Entidade não encontrado.");

				Core.MainDataContextInstance().SGW_WorkflowSteps.DeleteOnSubmit(obj);
				Core.MainDataContextInstance().SubmitChanges();
				return new Common.OperationResult();
			}
			catch (Exception ex)
			{
				return new Common.OperationResult(ex);
			}

		}

		public override Common.DataContract.WorkflowStepDataContract GetById(Guid id)
		{
			if (id == null || id == Guid.Empty)
				throw new ArgumentException("Cannot be Null", "id");

			SGW_WorkflowStep obj = Core.MainDataContextInstance().SGW_WorkflowSteps.Where(o => o.StepId.Equals(id)).FirstOrDefault();
			return GetDataContract(obj);
		}

		public override IEnumerable<Common.DataContract.WorkflowStepDataContract> GetAll()
		{
			return Core.MainDataContextInstance().SGW_WorkflowSteps.Select(o => GetDataContract(o)).ToList();
		}

		public override WorkflowStepDataContract GetByDescription(string description)
		{
			if (string.IsNullOrEmpty(description))
				throw new ArgumentException("Cannot be Null", "description");

			SGW_WorkflowStep obj = Core.MainDataContextInstance().SGW_WorkflowSteps.Where(o => o.Name.Equals(description)).FirstOrDefault();
			return GetDataContract(obj);			
		}

	}
}

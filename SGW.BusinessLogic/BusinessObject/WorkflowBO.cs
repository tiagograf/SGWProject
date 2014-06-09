using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGW.Common.DataContract;
using SGW.DataAccess.Handler;

namespace SGW.BusinessLogic.BusinessObject
{
    public class WorkflowBO : IWorkflowBO
    {

		public Common.OperationResult Add(Common.DataContract.WorkflowDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowHandler>();
			dataContract.CreatedOn = DateTime.Now;
			dataContract.CreatedBy = Common.SessionData.ResourceId;
			var result = handler.Add(dataContract);

			if (result.Status != Common.OperationResultStatus.Succesfull)
				return result;

			foreach (var item in dataContract.WorkflowStateList)
			{
				item.WorkflowId = dataContract.Id;
				result = this.AddState(item);
				if (result.Status != Common.OperationResultStatus.Succesfull)
					return result;
			}

			foreach (var item in dataContract.WorkflowStateTransitionList)
			{
				item.WorkflowId = dataContract.Id;
				result = this.AddStateTransition(item);
				if (result.Status != Common.OperationResultStatus.Succesfull)
					return result;
			}

			return result;
		}

		public Common.OperationResult Update(WorkflowDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowHandler>();
			var handlerTransition = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateTransitionHandler>();
			var handlerState = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateHandler>();

			var result = handler.Update(dataContract);


			if (result.Status != Common.OperationResultStatus.Succesfull)
				return result;

			var stateList = handlerState.GetAll(dataContract.Id);
			foreach (var item in dataContract.WorkflowStateList)
			{
				if (!stateList.Any(o => o.Id.Equals(item.Id)))
				{
					item.WorkflowId = dataContract.Id;
					result = this.AddState(item);
					if (result.Status != Common.OperationResultStatus.Succesfull)
						return result;
				}
				else
				{
					result = handlerState.Update(item);
					if (result.Status != Common.OperationResultStatus.Succesfull)
						return result;

				}
			}

			var tranList = handlerTransition.GetAll(dataContract.Id);
			foreach (var item in dataContract.WorkflowStateTransitionList)
			{
				if (!tranList.Any(o => o.Id.Equals(item.Id)))
				{
					item.WorkflowId = dataContract.Id;
					result = this.AddStateTransition(item);
					if (result.Status != Common.OperationResultStatus.Succesfull)
						return result;
				}
			}


			tranList = handlerTransition.GetAll(dataContract.Id);
			foreach (var item in tranList)
			{
				if (!dataContract.WorkflowStateTransitionList.Any(o => o.Id.Equals(item.Id)))
				{
					result = handlerTransition.Delete(item);
					if (result.Status != Common.OperationResultStatus.Succesfull)
						return result;
				}
			}

			stateList = handlerState.GetAll(dataContract.Id);
			foreach (var item in stateList)
			{
				if (!dataContract.WorkflowStateList.Any(o => o.Id.Equals(item.Id)))
				{
					result = handlerState.Delete(item);
					if (result.Status != Common.OperationResultStatus.Succesfull)
						return result;
				}
			}


			return result;
		}

		public Common.OperationResult Delete(Common.DataContract.WorkflowDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowHandler>();
			return handler.Delete(dataContract);
		}

		public Common.DataContract.WorkflowDataContract GetById(Guid id)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowHandler>();
			var handlerTransition = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateTransitionHandler>();
			var handlerState = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateHandler>();
			var result = handler.GetById(id);

			if (result != null)
			{
				result.WorkflowStateTransitionList = handlerTransition.GetAll(id);
				result.WorkflowStateList = handlerState.GetAll(id);
			}

			return result;
		}

		public IEnumerable<Common.DataContract.WorkflowDataContract> GetAll()
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowHandler>();
			return handler.GetAll();
		}

		public Common.OperationResult AddState(Common.DataContract.WorkflowStateDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateHandler>();
			dataContract.CreatedOn = DateTime.Now;
			dataContract.CreatedBy = Common.SessionData.ResourceId;
			return handler.Add(dataContract);
		}

		public Common.OperationResult AddStateTransition(Common.DataContract.WorkflowStateTransitionDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateTransitionHandler>();
			dataContract.CreatedOn = DateTime.Now;
			dataContract.CreatedBy = Common.SessionData.ResourceId;
			return handler.Add(dataContract);
		}

		public WorkflowStateDataContract GetStateById(Guid stateId)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateHandler>();
			var entitySatusHandler = DataAccess.Core.GetFactory().GetInstance<IEntityStatusHandler>();
			var state = handler.GetById(stateId);
			state.EntityStatus = entitySatusHandler.GetById(state.EntityStatusId);

			return state;
		}

		public IEnumerable<WorkflowStateTransitionDataContract> GetAllWorkflowStateTransition(Guid workflowId)
		{
			var stateHandler = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateHandler>();
			var tranHandler = DataAccess.Core.GetFactory().GetInstance<IWorkflowStateTransitionHandler>();

			var transitions = tranHandler.GetAll(workflowId);
			foreach(var item in transitions)
			{
				item.FromState = stateHandler.GetById(item.FromStateId);
				item.ToState = stateHandler.GetById(item.ToStateId);
			}
			return transitions;
		}

		public IEnumerable<StepTypeDataContract> GetAllStepType()
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IStepTypeHandler>();
			return handler.GetAll();
		}
		public IEnumerable<ParticipantDataContract> GetAllParticipant()
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IParticipantHandler>();
			return handler.GetAll();
		}
		public StepTypeDataContract GetStepTypeByCommand(string command)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IStepTypeHandler>();
			return handler.GetByDescription(command);
		}

		public StepTypeDataContract GetStepTypeById(Guid id)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IStepTypeHandler>();
			return handler.GetById(id);
		}
	}
}

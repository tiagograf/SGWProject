using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGW.Common.DataContract;

namespace SGW.BusinessLogic.BusinessObject
{
	public interface IWorkflowBO
	{
		Common.OperationResult Add(WorkflowDataContract dataContract);
		Common.OperationResult Update(WorkflowDataContract dataContract);
		Common.OperationResult Delete(WorkflowDataContract dataContract);
		WorkflowDataContract GetById(Guid id);
		IEnumerable<Common.DataContract.WorkflowDataContract> GetAll();

		Common.OperationResult AddState(WorkflowStateDataContract dataContract);
		WorkflowStateDataContract GetStateById(Guid stateId);

		IEnumerable<WorkflowStateTransitionDataContract> GetAllWorkflowStateTransition(Guid workflowId);

		IEnumerable<StepTypeDataContract> GetAllStepType();
		IEnumerable<ParticipantDataContract> GetAllParticipant();
		StepTypeDataContract GetStepTypeByCommand(string command);
		StepTypeDataContract GetStepTypeById(Guid id);
	}
}

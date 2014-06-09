﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace SGW.Common.DataContract
{
	[DataContract]
	public class WorkflowStepDataContract : BaseDataContract
	{
		[Required]
		public Guid WorkflowId { get; set; }

		[Required]
		public int UILeftPosition  { get; set; }
		
		[Required]
		public int UITopPosition  { get; set; }

		[Required]
		public Guid StepTypeId {get; set;}
	
		[Required]
		public string Name {get; set;}
	
		[Required]
		public Guid FromStateId {get;set;}
		
		[Required]
		public string JoinDecision {get;set;}

		[Required]
		public bool Initial { get; set; }

		[Required]
		public Guid? ParticipantId { get; set; }

		public string Comments { get; set; }
		public DateTime? CreatedOn  { get; set; }
		public Guid? CreatedBy  { get; set; }
		public DateTime? UpdatedOn  { get; set; }
		public Guid? UpdatedBy  { get; set; }

		public StepTypeDataContract StepType { get; set; }


		public List<ManualActionDataContract> ManualActionList { get; set; }
		public List<DecisionConditionDataContract> DecisionConditionList { get; set; }
		public List<TransitionDecisionDataContract> TransitionDecisionList { get; set; }
		public string UploadFileType { get; set; }
		public Guid? EmailParticipantId  { get; set; }

		public string EmailBody  { get; set; }
		public string EmailSubject { get; set; }
		public string EmailTo { get; set; }

		public string ActionSQLProcedure { get; set; }
		public string ActionSQLCommand { get; set; }
	}

	[DataContract]
	public class DecisionConditionDataContract
	{
		public Guid StepId { get; set; }
		public Guid ConditionId { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}

	[DataContract]
	public class ManualActionDataContract
	{
		public Guid StepId { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}

	[DataContract]
	public class TransitionDecisionDataContract
	{
		public Guid StepId { get; set; }
		public string Code { get; set; }
		public Guid TransitionId { get; set; }
		public string TransitionDescription { get; set; }
	}
}

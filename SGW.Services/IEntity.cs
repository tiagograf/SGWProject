using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SGWServices
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
	[ServiceContract]
	public interface IEntity
	{

		[OperationContract]
		string GetCurrentEntityStatus(Guid entityId);

		[OperationContract]
		EntityDataContract GetDataUsingDataContract(EntityDataContract composite);

		// TODO: Add your service operations here
	}


	// Use a data contract as illustrated in the sample below to add composite types to service operations.
	[DataContract]
	public class EntityDataContract
	{
		[DataMember]
		public bool EntityId { get; set; }

		[DataMember]
		public string Entity { get; set; }

		[DataMember]
		public string UserDefinedId { get; set; }

		[DataMember]
		public string Data { get; set; }
	}
}

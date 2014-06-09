﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGW.Common.DataContract;
using SGW.DataAccess.Handler;

namespace SGW.BusinessLogic.BusinessObject
{
    public class ResourceBO : IResourceBO
    {

		public Common.OperationResult Add(Common.DataContract.ResourceDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			dataContract.CreatedBy = Common.SessionData.ResourceId;
			dataContract.CreatedOn = DateTime.Now;
			return handler.Add(dataContract);
		}

		public Common.OperationResult Update(ResourceDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			dataContract.UpdatedBy = Common.SessionData.ResourceId;
			dataContract.UpdatedOn = DateTime.Now;

			if (dataContract.ResourceRoles != null)
				handler.UpdateResourceRoles(dataContract);

			return handler.Update(dataContract);
		}

		public Common.OperationResult Delete(Common.DataContract.ResourceDataContract dataContract)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			return handler.Delete(dataContract);
		}

		public Common.DataContract.ResourceDataContract GetById(Guid id)
		{
			return GetById(id, false);
		}
		public Common.DataContract.ResourceDataContract GetById(Guid id, bool loadRelatedEntities)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			ResourceDataContract res = handler.GetById(id);
			if (loadRelatedEntities)
			{
				var roleHandler = DataAccess.Core.GetFactory().GetInstance<IRoleHandler>();
				res.ResourceRoles = roleHandler.GetByResourceId(id);
			}
			return res;
		}


		public bool Login(string username, string password)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			var resource = handler.GetByDescription(username);
			
			if (resource == null)
				return false;
			else
				return resource.Password.Equals(password);
		}


		public Common.DataContract.ResourceDataContract GetByEmail(string email)
		{
			var handler = DataAccess.Core.GetFactory().GetInstance<IResourceHandler>();
			return handler.GetByEmail(email);			
		}
	}
}

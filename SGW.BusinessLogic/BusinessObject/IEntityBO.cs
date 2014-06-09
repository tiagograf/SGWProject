﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SGW.Common.DataContract;

namespace SGW.BusinessLogic.BusinessObject
{
	public interface IEntityBO
	{
		Common.OperationResult Add(EntityDataContract dataContract);
		Common.OperationResult Update(EntityDataContract dataContract);
		Common.OperationResult Delete(EntityDataContract dataContract);
		IEnumerable<EntityDataContract> GetAll();
		EntityDataContract GetById(Guid id);
		IEnumerable<EntityStatusDataContract> GetAllStatus(Guid entityId);
		IEnumerable<string> GetTables();
		IEnumerable<KeyValuePair<string,string>> GetColumns(string tableName);
		string GetColumnType(string databaseType);
	}
}

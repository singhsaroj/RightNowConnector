// ===========================================================================================
//  Oracle RightNow Connect
//  CTI Sample Code
// ===========================================================================================
//  Copyright © Oracle Corporation.  All rights reserved.
// 
//  Sample code for training only. This sample code is provided "as is" with no warranties 
//  of any kind express or implied. Use of this sample code is pursuant to the applicable
//  non-disclosure agreement and or end user agreement and or partner agreement between
//  you and Oracle Corporation. You acknowledge Oracle Corporation is the exclusive
//  owner of the object code, source code, results, findings, ideas and any works developed
//  in using this sample code.
// ===========================================================================================
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Oracle.RightNow.Cti.Common.ConnectService;
using Oracle.RightNow.Cti.Model;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;

namespace Oracle.RightNow.Cti {
    public class RightNowObjectProvider
    {
        private IGlobalContext _globalContext;

        private GenericObject _icGenericObject = new GenericObject();
        private RNObjectType _rnObjType = new RNObjectType();

        public RightNowObjectProvider(IGlobalContext globalContext)
        {
            _globalContext = globalContext;

        }

        public T GetObject<T>(string predicate = null) where T : class, new()
        {
            var objectType = typeof(T);

            var customObjectAttribute = objectType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
            if (customObjectAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowCustomObjectAttribute to associate the proper metadata with the type");

            var query = string.Format("SELECT * from {0}.{1} {2}", customObjectAttribute.PackageName, customObjectAttribute.ObjectName, predicate ?? string.Empty);

            var request = new QueryCSVRequest(getClientInfoHeader(), query, 1, ",", false, true);

            var rightNowChannel = getChannel();
            var result = rightNowChannel.QueryCSV(request);

            return materializeObjects<T>(result, objectType, 1).FirstOrDefault();
        }

        public IList<long> GetObjectIds(string entityName, string predicate)
        {
            var request = new QueryCSVRequest(getClientInfoHeader(),
                string.Format("SELECT id FROM {0} WHERE {1}", entityName, predicate), 1000, ",", false, true);

            var rightNowChannel = getChannel();

            var result = rightNowChannel.QueryCSV(request);

            var ids = new List<long>();
            if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length > 0)
            {
                foreach (var row in result.CSVTableSet.CSVTables[0].Rows)
                {
                    var resultRow = row.Split(',');
                    ids.Add(long.Parse(resultRow[0]));
                }
            }
            return ids;
        }

        public IEnumerable<T> GetObjects<T>(string predicate = null) where T : class, new()
        {
            var objectType = typeof(T);
            var result = new QueryCSVResponse();
            try
            {
               

                var customObjectAttribute = objectType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
                if (customObjectAttribute == null)
                    throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowCustomObjectAttribute to associate the proper metadata with the type");

                var query = string.Format("SELECT * from {0}.{1} {2}", customObjectAttribute.PackageName, customObjectAttribute.ObjectName, predicate ?? string.Empty);

                var request = new QueryCSVRequest(getClientInfoHeader(), query, 100, ",", false, true);

                var rightNowChannel = getChannel();
                 result = rightNowChannel.QueryCSV(request);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error(string.Format("Exception at calling GetObject  >> {0}", ex.Message));
                
            }
            return materializeObjects<T>(result, objectType);
        }

        public IncidentInfo GetFirstIncidentDetails(int accountid) {
            var firstincident = new IncidentInfo();
            try {
                
                var request = new QueryCSVRequest(getClientInfoHeader(),
                  string.Format(@"SELECT 
	                                                    ID,
	                                                    Channel,
	                                                    Queue.Name,
	                                                    ReferenceNumber,
	                                                    Source.Name SourceName,
                                                        PrimaryContact.Contact ContactId,
                                                        PrimaryContact.Contact.Name ContactName,
                                                        PrimaryContact.ParentContact.Emails.Address Email,
	                                                    Subject,
                                                        UpdatedTime
                                                    FROM Incident
                                                    WHERE AssignedTo.Account.Id = {0}
                                                    AND StatusWithType.StatusType.Id = 1
                                                    ORDER BY UpdatedTime DESC 
                                                    LIMIT 1"
                                                   , accountid), 1000, ",", false, true);
                var rightNowChannel = getChannel();

                var result = rightNowChannel.QueryCSV(request);

                if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length == 1)
                {
                    var resultRow = result.CSVTableSet.CSVTables[0].Rows[0].Split(',');
                            firstincident.Id = long.Parse(resultRow[0]);
                            firstincident.Channel = getRightNowChannel(resultRow[1]);
                            firstincident.QueueName = resultRow[2];
                            firstincident.ReferenceNumber = resultRow[3];
                            firstincident.SourceName = resultRow[4];
                            firstincident.ContactId = long.Parse(resultRow[5]);
                            firstincident.ContactName = resultRow[6];
                            firstincident.ContactEmail = resultRow[7];
                            firstincident.Subject = resultRow[8];
                            firstincident.LastUpdate = DateTime.Parse(resultRow[9]);
                }
            }
            catch (Exception ex) {
                _globalContext.LogMessage(string.Format("Error polling incidents: {0}", ex));
                Logger.Logger.Log.Error("RightNowObjectProvider: ", ex);
            }
            return firstincident;
        }


        public IList<IncidentInfo> GetPendingIncidents(DateTime cutOffTime)
        {
            var incidents = new List<IncidentInfo>();

            try
            {
                //  cutOffTime = cutOffTime.AddSeconds(_globalContext.TimeOffset);
//                var request = new QueryCSVRequest(getClientInfoHeader(),
//                    string.Format(@"SELECT 
//	                                                    ID,
//	                                                    Channel,
//	                                                    Queue.Name,
//	                                                    ReferenceNumber,
//	                                                    Source.Name SourceName,
//                                                        PrimaryContact.Contact ContactId,
//                                                        PrimaryContact.Contact.Name ContactName,
//                                                        PrimaryContact.ParentContact.Emails.Address Email,
//	                                                    Subject,
//                                                        UpdatedTime
//                                                    FROM Incident
//                                                    WHERE UpdatedTime > '{0}'
//                                                    AND StatusWithType.StatusType.Id = 1
//                                                    AND AssignedTo.Account.Id IS NULL
//                                                    AND PrimaryContact.ParentContact.Emails.Address IS NOT NULL", cutOffTime.ToString("yyyy-MM-dd HH:mm:ss")),
//                    1000, ",", false, true);
                var request = new QueryCSVRequest(getClientInfoHeader(),
                   string.Format(@"SELECT 
	                                                    ID,
	                                                    Channel,
	                                                    Queue.Name,
	                                                    ReferenceNumber,
	                                                    Source.Name SourceName,
                                                        PrimaryContact.Contact ContactId,
                                                        PrimaryContact.Contact.Name ContactName,
                                                        PrimaryContact.ParentContact.Emails.Address Email,
	                                                    Subject,
                                                        UpdatedTime
                                                    FROM Incident
                                                    WHERE UpdatedTime > '{0}'", cutOffTime.ToString("yyyy-MM-dd HH:mm:ss")),
                   1000, ",", false, true);

                var rightNowChannel = getChannel();

                var result = rightNowChannel.QueryCSV(request);

                if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length > 0)
                {
                    foreach (var row in result.CSVTableSet.CSVTables[0].Rows)
                    {
                        var resultRow = row.Split(',');
                        var incident = new IncidentInfo
                        {
                            Id = long.Parse(resultRow[0]),
                            Channel = getRightNowChannel(resultRow[1]),
                            QueueName = resultRow[2],
                            ReferenceNumber = resultRow[3],
                            SourceName = resultRow[4],
                            ContactId = long.Parse(resultRow[5]),
                            ContactName = resultRow[6],
                            ContactEmail = resultRow[7],
                            Subject = resultRow[8],
                            LastUpdate = DateTime.Parse(resultRow[9])
                        };
                        incidents.Add(incident);
                    }
                }
            }
            catch (Exception exc)
            {
                _globalContext.LogMessage(string.Format("Error polling incidents: {0}", exc));
                Logger.Logger.Log.Error("RightNowObjectProvider: ", exc);
            }

            return incidents;
        }

        private RightNowChannel getRightNowChannel(string channelValue)
        {
            RightNowChannel channel;
            Enum.TryParse(channelValue, out channel);
            return channel;
        }

        internal StaffAccountInfo GetStaffAccountInformation(int id)
        {
            var request = new QueryCSVRequest(getClientInfoHeader(),
            string.Format(@"SELECT DisplayName, CustomFields.Oracle_Cti.acd_id, CustomFields.Oracle_Cti.acd_password, CustomFields.c.AgentID, CustomFields.c.Password, CustomFields.c.Extension,CustomFields.c.Queue FROM Account WHERE id= {0}", id), 1, ",", false, true);

            var rightNowChannel = getChannel();

            var result = rightNowChannel.QueryCSV(request);

            var staffAccount = new StaffAccountInfo();

            if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length > 0 && result.CSVTableSet.CSVTables[0].Rows.Length > 0)
            {
                var resultRow = result.CSVTableSet.CSVTables[0].Rows[0].Split(',');
                staffAccount.Name = resultRow[0];
                staffAccount.AcdId = resultRow[1];
                staffAccount.AcdPassword = resultRow[1];
                int val;
                int.TryParse(resultRow[3], out val);
                staffAccount.AgentID = val;                
                staffAccount.Password = resultRow[4];
                int.TryParse(resultRow[5], out val);
                staffAccount.Extension = val;
                int.TryParse(resultRow[6], out val);
                staffAccount.Queue= val;
            }

            return staffAccount;
        }

        internal void UpdateStaffAccountInformation(StaffAccountInfo staffActInfo)
        {
            try
            {
                Logger.Logger.Log.Debug("RightNowObjectProvider: Get User Account Details");
                Account acobj = new Account();
                acobj.ID = new ID() { id = Global.Context.AccountId, idSpecified = true };
                GenericObject genericObject = new GenericObject();
                GenericObject genericObjectFinal = new GenericObject();

                RNObjectType rnobj = new RNObjectType();

                List<GenericField> genericFields = new List<GenericField>();
                List<GenericField> genericField = new List<GenericField>();
                //Convert.ToInt64(AgentID))
                staffActInfo.Password = string.IsNullOrEmpty(staffActInfo.Password) ? " " : staffActInfo.Password;
                genericFields.Add(createGenericField("AgentID", ItemsChoiceType.StringValue, staffActInfo.AgentID.ToString()));
                genericFields.Add(createGenericField("password", ItemsChoiceType.StringValue, staffActInfo.Password));
                genericFields.Add(createGenericField("Extension", ItemsChoiceType.StringValue, staffActInfo.Extension.ToString()));
                genericFields.Add(createGenericField("Queue", ItemsChoiceType.IntegerValue, staffActInfo.Queue));

                genericObject.GenericFields = genericFields.ToArray();
                rnobj.TypeName = "AccountCustomFields";
                genericObject.ObjectType = rnobj;

                genericField.Add(createGenericField("c", ItemsChoiceType.ObjectValue, genericObject));
                genericObjectFinal.GenericFields = genericField.ToArray();
                genericObjectFinal.ObjectType = new RNObjectType() { TypeName = "AccountCustomFieldsc" };
                acobj.CustomFields = genericObjectFinal;

                UpdateProcessingOptions cpo = new UpdateProcessingOptions();
                cpo.SuppressExternalEvents = false;
                cpo.SuppressRules = false;
                UpdateRequest cre = new UpdateRequest(getClientInfoHeader(), new RNObject[] { acobj }, cpo);
                UpdateResponse res = getChannel().Update(cre);
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("RightNowObjectProvider:", ex);                
            }

        }

        private RightNowSyncPortChannel getChannel()
        {
            Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
            ((BasicHttpBinding)binding).Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
            ((BasicHttpBinding)binding).MaxBufferSize = 1024 * 1024 * 1024;
            ((BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024 * 1024;
            BindingElementCollection elements = binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().IncludeTimestamp = false;

            binding = new CustomBinding(elements);

            var channelFactory = new ChannelFactory<RightNowSyncPortChannel>(binding, new EndpointAddress(_globalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap)));

            _globalContext.PrepareConnectSession(channelFactory);
            var rightNowChannel = channelFactory.CreateChannel();
            return rightNowChannel;
        }

        private ClientInfoHeader getClientInfoHeader()
        {
            return new ClientInfoHeader { AppID = "Oracle RightNow CTI" };
        }

        private IEnumerable<T> materializeObjects<T>(QueryCSVResponse result, Type objectType, int objectCount = 100) where T : class, new()
        {
            var entities = new List<T>();
            if (result.CSVTableSet != null && result.CSVTableSet.CSVTables != null && result.CSVTableSet.CSVTables.Length > 0)
            {
                var table = result.CSVTableSet.CSVTables[0];
                var columns = new List<string>(table.Columns.Split(','));
                var mapping = new Dictionary<int, PropertyInfo>();

                var properties = objectType.GetProperties();
                foreach (var property in properties)
                {
                    var attribute = property.GetCustomAttributes(typeof(RightNowCustomObjectFieldAttribute), true).FirstOrDefault() as RightNowCustomObjectFieldAttribute;
                    if (attribute != null)
                    {
                        int position = columns.IndexOf(attribute.Name);
                        if (position > -1)
                        {
                            mapping.Add(position, property);
                        }
                    }
                }

                foreach (var row in table.Rows)
                {
                    var values = row.Split(',');
                    var entity = new T();
                    foreach (var entry in mapping.Keys)
                    {
                        var propertyInfo = mapping[entry];
                        propertyInfo.SetValue(entity, Parse(values[entry], propertyInfo.PropertyType), null);
                    }

                    entities.Add(entity);
                }
            }

            return entities;
        }

        private object Parse(string value, Type propertyType)
        {
            object result = null;

            if (propertyType == typeof(String))
            {
                result = value;
            }
            else if (propertyType == typeof(Int32))
            {
                result = int.Parse(value);
            }
            else if (propertyType == typeof(Int64))
            {
                result = Int64.Parse(value);
            }
            else if (propertyType == typeof(Boolean))
            {
                result = string.Compare(value, "1") == 0;
            }
            else if (propertyType.IsEnum)
            {
                if (Enum.IsDefined(propertyType, int.Parse(value)))
                {
                    result = Enum.Parse(propertyType, value);
                }
            }
            else if (propertyType == typeof(DateTime))
            {
                result = DateTime.Parse(value);
            }

            return result;
        }

        /********************/


        public UpdateResponse UpdateObject<T>(T obj, long uniqueId) where T : class, new()
        {
            UpdateResponse updateResponse = null;
            try
            {
                Type objType = typeof(T);
                var objAttribute = objType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
                if (objAttribute == null)
                    throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

                GenericObject genericObject = new GenericObject();
                RNObjectType rnObjType = new RNObjectType() { Namespace = objAttribute.PackageName, TypeName = objAttribute.ObjectName };
                genericObject.ObjectType = rnObjType;

                List<GenericField> genericFields = new List<GenericField>();
                PropertyInfo[] properties = objType.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    RightNowCustomObjectFieldAttribute attribute = property.GetCustomAttributes(typeof(RightNowCustomObjectFieldAttribute), true).FirstOrDefault() as RightNowCustomObjectFieldAttribute;
                    if (attribute != null && attribute.CanUpdate)
                    {
                        var propValue = property.GetValue(obj, null);
                        //if (!string.IsNullOrWhiteSpace(propValue.ToString())) && !string.IsNullOrWhiteSpace(propValue.ToString())
                        if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            genericFields.Add(createGenericField(attribute, propValue));
                        }
                    }
                }

                genericObject.GenericFields = genericFields.ToArray();
                ID autoID = new ID();
                autoID.id = uniqueId;
                autoID.idSpecified = true;
                genericObject.ID = autoID;

                UpdateProcessingOptions options = new UpdateProcessingOptions();
                options.SuppressExternalEvents = false;
                options.SuppressRules = false;
                UpdateRequest updateRequest = new UpdateRequest(getClientInfoHeader(), new RNObject[] { genericObject }, options);
                updateResponse = this.getChannel().Update(updateRequest);
               
            }
            catch (Exception ex)
            {
                Logger.Logger.Log.Error("RightNowObjectProvider: Update failed", ex);
            }
            return updateResponse;
        }

        public CreateResponse CreateObject<T>(T obj) where T : class , new()
        {
            Type objType = typeof(T);
            var objAttribute = objType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
            if (objAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

            GenericObject genericObject = new GenericObject();
            RNObjectType rnObjType = new RNObjectType() { Namespace = objAttribute.PackageName, TypeName = objAttribute.ObjectName };
            genericObject.ObjectType = rnObjType;

            List<GenericField> genericFields = new List<GenericField>();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object attribute = property.GetCustomAttributes(typeof(RightNowCustomObjectFieldAttribute), true).FirstOrDefault();
                if (attribute != null)
                {
                    RightNowCustomObjectFieldAttribute rightNowAttribute = attribute as RightNowCustomObjectFieldAttribute;
                    if (rightNowAttribute.CanUpdate)
                    {

                        var propValue = property.GetValue(obj, null);
                        if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            genericFields.Add(createGenericField(rightNowAttribute, propValue));
                        }
                    }
                }
            }

            genericObject.GenericFields = genericFields.ToArray();
            CreateProcessingOptions options = new CreateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            CreateRequest createRequest = new CreateRequest(getClientInfoHeader(), new RNObject[] { genericObject }, options);
            CreateResponse createResponcse = this.getChannel().Create(createRequest);
            return createResponcse;
        }

        public void DeleteObject<T>(long uniqueId) where T : class, new()
        {
            Type objType = typeof(T);
            var objAttribute = objType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
            if (objAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

            GenericObject genericObject = new GenericObject();
            RNObjectType rnObjType = new RNObjectType() { Namespace = objAttribute.PackageName, TypeName = objAttribute.ObjectName };
            genericObject.ObjectType = rnObjType;

            List<GenericField> genericFields = new List<GenericField>();

            genericObject.GenericFields = genericFields.ToArray();
            ID autoID = new ID();
            autoID.id = uniqueId;
            autoID.idSpecified = true;
            genericObject.ID = autoID;

            DestroyProcessingOptions options = new DestroyProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            DestroyRequest destroyRequest = new DestroyRequest(getClientInfoHeader(), new RNObject[] { genericObject }, options);
            DestroyResponse destroyResponse = getChannel().Destroy(destroyRequest);
        }

        public GenericField createGenericField(RightNowCustomObjectFieldAttribute attribute, object value)
        {
            GenericField genericField = new GenericField();
            genericField.name = attribute.Name;
            genericField.DataValue = new DataValue();

            ItemsChoiceType fieldType = ItemsChoiceType.StringValue;
            if (attribute.FieldType != null)
            {
                fieldType = attribute.FieldType;
            }

            if (fieldType == ItemsChoiceType.NamedIDValue)
            {
                NamedID accountID = new NamedID();
                accountID.ID = new ID();
                long id = 0;
                Int64.TryParse(value.ToString(), out id);
                accountID.ID.id = id;
                accountID.ID.idSpecified = true;
                genericField.DataValue.Items = new object[] { accountID };
            }
            else
            {
                genericField.DataValue.Items = new object[] { value };
            }
            //genericField.dataType = (ConnectService.DataTypeEnum)fieldType;
            genericField.DataValue.ItemsElementName = new ItemsChoiceType[] { fieldType };
            return genericField;
        }

        public GenericField createGenericField(string Name, ItemsChoiceType itemsChoiceType, object Value)
        {
            GenericField gf = new GenericField();
            gf.name = Name;
            gf.DataValue = new DataValue();
            gf.DataValue.ItemsElementName = new ItemsChoiceType[] { itemsChoiceType };
            gf.DataValue.Items = new object[] { Value };
            return gf;
        }

        public Dictionary<long, string> GetNameValues(string fieldName, string package = null)
        {
            GetValuesForNamedIDRequest namedIdRequest = new GetValuesForNamedIDRequest(getClientInfoHeader(), null, fieldName);
            Dictionary<long, string> namedIds = new Dictionary<long, string>();

            GetValuesForNamedIDResponse resp = getChannel().GetValuesForNamedID(namedIdRequest);

            foreach (NamedID namedId in resp.Entry)
            {
                if (namedId.ID.idSpecified)
                {
                    namedIds[namedId.ID.id] = namedId.Name;
                }
            }
            return namedIds;
        }

        public List<string> GetStandardWorkSpace()
        {
            List<string> workSpace = new List<string>();
            foreach (WorkspaceRecordType work in (WorkspaceRecordType[])Enum.GetValues(typeof(WorkspaceRecordType)))
            {
                if (work.ToString() != "None")
                    workSpace.Add(work.ToString());
            }
            return workSpace;
        }

        public ScreenPopConfig GetScreenConfigByProfile(int profileid)
        {
            ScreenPopConfig list = GetObject<ScreenPopConfig>(string.Format("where Profileid={0}", profileid));
            return list;
        }


        public ScreenPopConfig GetScreenConfigByProfileId(int profileid)
        {
            ScreenPopConfig list = GetScreenConfigByProfile(profileid);
            if (list != null)
            {
                list.ScreenPopOptionsList = new List<ScreenPopOptions>(GetObjects<ScreenPopOptions>(string.Format("where ScreenPopConfigID={0}", list.ID))).ToList();
            }
            else
            {
                list = new ScreenPopConfig();
                list.CanScreenPop = false;
                list.DefaultWorkSpace = "";
                list.ChatDefaultWorkspace = "";
                list.IncidentDefaultWorkspace = "";
                list.ProfileId = profileid;
                list.IsDefault = true;
                list.ScreenPopOptionsList = null;
                list.AutoRecieve = false;
                list.VoiceScreenPop = false;
                list.ChatScreenPop = false;
                list.IncidentScreenPop = false;
                
            }
            return list;
        }

        public List<ScreenPopOptions> GetScreenPopOptionsByConfigID(int configid)
        {
            return new List<ScreenPopOptions>(GetObjects<ScreenPopOptions>(string.Format("where ScreenPopConfigID={0}", configid))).ToList();
        }

        public List<ScreenPopOptions> GetReasonByCode(int profileID,ReasonCodes reasoncode)
        {
            ScreenPopConfig wrapupreason = GetScreenConfigByProfileId(profileID);
            List<ScreenPopOptions> opts = new List<ScreenPopOptions>();
            if (wrapupreason.ScreenPopOptionsList != null)
            {
                switch (reasoncode)
                {
                    case ReasonCodes.AUXReason:
                        if (wrapupreason.AUXReasonEnabled)
                            opts =wrapupreason.ScreenPopOptionsList.Where(reason => reason.Type == (int)reasoncode).ToList();
                        break;
                    case ReasonCodes.LogoutReason:
                        if (wrapupreason.LogoutReasonEnabled)
                            opts = wrapupreason.ScreenPopOptionsList.Where(reason => reason.Type == (int)reasoncode).ToList();
                        break;
                    case ReasonCodes.WrapupReason:
                        if (wrapupreason.WrapupReasonEnabled)
                            opts = wrapupreason.ScreenPopOptionsList.Where(reason => reason.Type == (int)reasoncode).ToList();
                        break;                    
                }
            }
            return opts;
            
        }      

        public IEnumerable<MetaDataClass> GetAllMetatData()
        {
            GetMetaDataRequest metaDataRequest = new GetMetaDataRequest(getClientInfoHeader());
            GetMetaDataResponse metaDataResponse = getChannel().GetMetaData(metaDataRequest);
            IEnumerable<MetaDataClass> filterMetaDataClasses = metaDataResponse.MetaDataClass;

            return filterMetaDataClasses;
        }
    }
    public enum ReasonCodes
    {
        SearchOption = 0,
        WrapupReason = 1,
        LogoutReason = 2,
        AUXReason = 3
    }
}
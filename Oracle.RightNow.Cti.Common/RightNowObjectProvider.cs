
using Oracle.RightNow.Cti.Common.ConnectService;
using RightNow.AddIns.AddInViews;
using RightNow.AddIns.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Oracle.RightNow.Cti.Common
{
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

            var customObjectAttribute = objectType.GetCustomAttributes(typeof(RightNowCustomObjectAttribute), true).FirstOrDefault() as RightNowCustomObjectAttribute;
            if (customObjectAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowCustomObjectAttribute to associate the proper metadata with the type");

            var query = string.Format("SELECT * from {0}.{1} {2}", customObjectAttribute.PackageName, customObjectAttribute.ObjectName, predicate ?? string.Empty);

            var request = new QueryCSVRequest(getClientInfoHeader(), query, 100, ",", false, true);

            var rightNowChannel = getChannel();
            var result = rightNowChannel.QueryCSV(request);

            return materializeObjects<T>(result, objectType);
        }

        //        public IList<IncidentInfo> GetPendingIncidents(DateTime cutOffTime)
        //        {
        //            var incidents = new List<IncidentInfo>();

        //            try
        //            {
        //                //  cutOffTime = cutOffTime.AddSeconds(_globalContext.TimeOffset);
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

        //                var rightNowChannel = getChannel();

        //                var result = rightNowChannel.QueryCSV(request);

        //                if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length > 0)
        //                {
        //                    foreach (var row in result.CSVTableSet.CSVTables[0].Rows)
        //                    {
        //                        var resultRow = row.Split(',');
        //                        var incident = new IncidentInfo
        //                        {
        //                            Id = long.Parse(resultRow[0]),
        //                            Channel = getRightNowChannel(resultRow[1]),
        //                            QueueName = resultRow[2],
        //                            ReferenceNumber = resultRow[3],
        //                            SourceName = resultRow[4],
        //                            ContactId = long.Parse(resultRow[5]),
        //                            ContactName = resultRow[6],
        //                            ContactEmail = resultRow[7],
        //                            Subject = resultRow[8],
        //                            LastUpdate = DateTime.Parse(resultRow[9])
        //                        };
        //                        incidents.Add(incident);
        //                    }
        //                }
        //            }
        //            catch (Exception exc)
        //            {
        //                _globalContext.LogMessage(string.Format("Error polling incidents: {0}", exc));
        //            }

        //            return incidents;
        //        }

        //        private RightNowChannel getRightNowChannel(string channelValue)
        //        {
        //            RightNowChannel channel;
        //            Enum.TryParse(channelValue, out channel);
        //            return channel;
        //        }

        //        internal StaffAccountInfo GetStaffAccountInformation(int id)
        //        {
        //            var request = new QueryCSVRequest(getClientInfoHeader(),
        //                string.Format("SELECT DisplayName,CustomFields.Oracle_Cti.acd_id,CustomFields.Oracle_Cti.acd_password FROM Account WHERE id= {0}", id), 1, ",", false, true);

        //            var rightNowChannel = getChannel();

        //            var result = rightNowChannel.QueryCSV(request);

        //            var staffAccount = new StaffAccountInfo();

        //            if (result.CSVTableSet != null && result.CSVTableSet.CSVTables.Length > 0 && result.CSVTableSet.CSVTables[0].Rows.Length > 0)
        //            {
        //                var resultRow = result.CSVTableSet.CSVTables[0].Rows[0].Split(',');
        //                staffAccount.Name = resultRow[0];
        //                staffAccount.AcdId = resultRow[1];
        //                staffAccount.AcdPassword = resultRow[1];
        //            }

        //            return staffAccount;
        //        }

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

            Logger.Logger.Log.Debug(string.Format("Update : {0}", uniqueId));

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
                Logger.Logger.Log.Error("RightNowObject:", ex);
                
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

        public CreateResponse CreateObjects<T>(List<T> lstobj)
        {
            List<GenericObject> lstgenericObject = new List<GenericObject>();

            foreach (T sam in lstobj)
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
                            var propValue = property.GetValue(sam, null);
                            if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                            {
                                genericFields.Add(createGenericField(rightNowAttribute, propValue));
                            }
                        }
                    }
                }

                genericObject.GenericFields = genericFields.ToArray();
                lstgenericObject.Add(genericObject);
            }

            CreateProcessingOptions options = new CreateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            RNObject[] rnobj = lstgenericObject.ToArray<RNObject>();

            CreateRequest createRequest = new CreateRequest(getClientInfoHeader(), rnobj, options);
            CreateResponse createResponcse = this.getChannel().Create(createRequest);
            return createResponcse;
        }

        public UpdateResponse UpdateObjects<T>(List<T> lstobj)
        {
            List<GenericObject> lstgenericObject = new List<GenericObject>();

            foreach (T sam in lstobj)
            {
                ID autoID = new ID();

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
                        var propValue = property.GetValue(sam, null);
                        //if (!string.IsNullOrWhiteSpace(propValue.ToString())) && !string.IsNullOrWhiteSpace(propValue.ToString())
                        if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            genericFields.Add(createGenericField(attribute, propValue));
                        }
                    }
                    else
                    {
                        if (attribute.Name == "ID")
                        {
                            var propValue = property.GetValue(sam, null);
                            autoID.id = Convert.ToInt64(propValue);
                        }
                    }

                }

                genericObject.GenericFields = genericFields.ToArray();

                autoID.idSpecified = true;
                genericObject.ID = autoID;
                lstgenericObject.Add(genericObject);
            }
            UpdateProcessingOptions options = new UpdateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            RNObject[] rnobj = lstgenericObject.ToArray<RNObject>();
            UpdateRequest updateRequest = new UpdateRequest(getClientInfoHeader(), rnobj, options);
            UpdateResponse updateResponse = this.getChannel().Update(updateRequest);
            return updateResponse;
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

        public void DeleteObjects<T>(List<T> lstObj) where T : class, new()
        {
            List<GenericObject> lstgenericObject = new List<GenericObject>();

            foreach (T sam in lstObj)
            {

                ID autoID = new ID();

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
                        var propValue = property.GetValue(sam, null);
                        //if (!string.IsNullOrWhiteSpace(propValue.ToString())) && !string.IsNullOrWhiteSpace(propValue.ToString())
                        if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            genericFields.Add(createGenericField(attribute, propValue));
                        }
                    }
                    else
                    {
                        if (attribute.Name == "ID")
                        {
                            var propValue = property.GetValue(sam, null);
                            autoID.id = Convert.ToInt64(propValue);
                        }
                    }

                }

                genericObject.GenericFields = genericFields.ToArray();

                autoID.idSpecified = true;
                genericObject.ID = autoID;
                lstgenericObject.Add(genericObject);
            }
            DestroyProcessingOptions options = new DestroyProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            RNObject[] rnobj = lstgenericObject.ToArray<RNObject>();
            DestroyRequest destroyRequest = new DestroyRequest(getClientInfoHeader(), rnobj, options);
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
              
                if ((work.ToString() != "None")&&(work.ToString() != "Generic")&&(work.ToString() != "Chat"))
                {
                        workSpace.Add(work.ToString()); 
                }
            }
            return workSpace;
        }

        public ScreenPopConfig GetScreenConfigByProfileId(int profileid)
        {
            ScreenPopConfig list = GetObject<ScreenPopConfig>(string.Format("where Profileid={0}", profileid));
            if (list != null)
            {
                list.ScreenPopOptionsList = new List<ScreenPopOptions>(GetObjects<ScreenPopOptions>(string.Format("where ScreenPopConfigID={0}", list.ID))).ToList();
            }
            else
            {
                list = new ScreenPopConfig();
                list.CanScreenPop = false;
                list.AutoRecieve = false;
                list.VoiceScreenPop = false;
                list.ChatScreenPop = false;
                list.IncidentScreenPop = false;
                list.DefaultWorkSpace = "";
                //list.ChatDefaultWorkSpace = "";
                //list.IncidentDefaultWorkSpace = "";
                list.ProfileId = profileid;
                list.IsDefault = true;
                list.ScreenPopOptionsList = null;
            }
            return list;
        }

        public List<ScreenPopOptions> GetReasonByCode(int profileID, ReasonCodes reasoncode)
        {
            ScreenPopConfig wrapupreason = GetScreenConfigByProfileId(profileID);
            if (wrapupreason.ScreenPopOptionsList != null)
                return wrapupreason.ScreenPopOptionsList.Where(reason => reason.Type == (int)reasoncode).ToList();
            else
                return null;
        }

        public List<AgentState> GetAgentState()
        {
            List<AgentState> agentState = new List<AgentState>(GetObjects<AgentState>());
            return agentState;
        }

        public enum ReasonCodes
        {
            SearchOption = 0,
            WrapupReason = 1,
            LogoutReason = 2,
            AUXReason = 3
        }

        public IEnumerable<MetaDataClass> GetAllMetatData()
        {
            GetMetaDataRequest metaDataRequest = new GetMetaDataRequest(getClientInfoHeader());
            GetMetaDataResponse metaDataResponse = getChannel().GetMetaData(metaDataRequest);
            IEnumerable<MetaDataClass> filterMetaDataClasses = metaDataResponse.MetaDataClass;

            return filterMetaDataClasses;
        }

        public void DestroyReasoncodes(long config)
        {
            List<RNObject> destroyfields = new List<RNObject>();

            List<ScreenPopOptions> profiledata = new List<ScreenPopOptions>(GetObjects<ScreenPopOptions>(string.Format(" where ScreenPopConfigID={0}", config)));

            foreach (ScreenPopOptions rn in profiledata)
            {
                _icGenericObject = new GenericObject();
                _rnObjType.Namespace = OracleCtiObjectStrings.ScreenPopPackageName;
                _rnObjType.TypeName = OracleCtiObjectStrings.ScreenPopOptions;
                _icGenericObject.ObjectType = _rnObjType;
                NamedID relationid = new NamedID();
                ID incID = new ID();
                incID.id = config;
                incID.idSpecified = true;
                relationid.ID = incID;

                ID rowid = new ID();
                rowid.id = rn.ID;
                rowid.idSpecified = true;
                _icGenericObject.ID = rowid;

                List<GenericField> gfs = new List<GenericField>();
                //gfs.Add(createGenericField("AgentEntityID", ItemsChoiceType.NamedIDValue, relationid));
                _icGenericObject.GenericFields = gfs.ToArray();
                destroyfields.Add(_icGenericObject);
            }

            if (destroyfields.Count() > 0)
            {
                DestroyProcessingOptions drop = new DestroyProcessingOptions();
                drop.SuppressExternalEvents = false;
                drop.SuppressRules = false;
                DestroyRequest destroyReq = new DestroyRequest(getClientInfoHeader(), destroyfields.ToArray(), drop);
                DestroyResponse destroyRes = getChannel().Destroy(destroyReq);
            }
        }
    }

}

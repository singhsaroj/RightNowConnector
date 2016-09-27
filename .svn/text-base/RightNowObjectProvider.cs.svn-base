using System;
using System.Collections.Generic;
using System.Linq;
using inContact.Integration.RightNow.ConnectService;
using System.ServiceModel.Channels;
using System.ServiceModel;
using RightNow.AddIns.AddInViews;
using System.Reflection;

namespace inContact.Integration.RightNow
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

        #region Commented Codes

        /*List<RightNowObjectType> objectType = new List<RightNowObjectType>() { 
            new RightNowObjectType() { ValueObject = "--SELECT--" },
            new RightNowObjectType() { ValueObject = "CallInfo" },
            new RightNowObjectType() { ValueObject = "ScriptVariable" },
            new RightNowObjectType() { ValueObject = "WorkItemInfo" },
            new RightNowObjectType() { ValueObject = "ChatInfo" } 
        };

        List<RightNowObjectFields> objectTypeField = new List<RightNowObjectFields>() {             
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="AgentName"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="ANI"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="CallDurationInSeconds"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="CallDirection"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="ContactID"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="DispositionValue"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="Comment"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="DNIS"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="SkillName"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="CustomValue1"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="CustomValue2"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="Commitment Amount"},
            new RightNowObjectFields(){ ValueObject="CallInfo", ValueField="Callback Time"},            

            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="AgentName"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="DurationInSeconds"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="ContactID"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="DispositionValue"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="Comment"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="SkillName"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="ID"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="Type"},
            new RightNowObjectFields(){ ValueObject="WorkItemInfo", ValueField="Payload"},

            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="AgentName"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="DurationInSeconds"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="ContactID"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="DispositionValue"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="Comment"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="SkillName"},
            new RightNowObjectFields(){ ValueObject="ChatInfo", ValueField="ChatTranscript"}
        }; */

        /* public IEnumerable<MetaDataAttribute> GetEntitesFields(string[] metaDataLink)
         {
             MetaDataAttribute[] attr = GetMetaDataForObjects(metaDataLink);
             List<MetaDataAttribute> filterattr = attr.Where(ar => ar.UsageOnCreate == MetaDataUsageEnum.ALLOWED || ar.UsageOnCreate == MetaDataUsageEnum.REQUIRED).ToList();
             List<MetaDataAttribute> customAttribute = new List<MetaDataAttribute>();
             foreach (MetaDataAttribute ar in filterattr)
             {
                 if (ar.DataTypeName == "GenericObject")
                 {
                     MetaDataAttribute[] custattr = GetMetaDataForObjects(new string[] { ar.MetaDataLink });
                     foreach (MetaDataAttribute customattr in custattr)
                     {
                         if (customattr.MetaDataLink != null)
                         {
                             MetaDataAttribute[] custAttrPack = GetMetaDataForObjects(new string[] { customattr.MetaDataLink });
                             foreach (MetaDataAttribute cuar in custAttrPack)
                             {
                                 cuar.MetaDataLink = customattr.MetaDataLink;
                                 customAttribute.Add(cuar);
                             }
                         }
                     }
                 }
                 else
                 {
                     customAttribute.Add(ar);
                 }
             }
             return customAttribute;
         }

         public IEnumerable<RightNowObjectType> LoadObjectData()
         {
             return objectType;
         }

         public IEnumerable<RightNowObjectFields> LoadObjectField(string objecttype)
         {
             return objectTypeField.Where(p => p.ValueObject == objecttype);
         } 

         public AgentConfiguration GetAgent(int profileID)
         {
             AgentConfiguration agentConfig;
             agentConfig = GetObject<AgentConfiguration>(string.Format("where Profileid={0}", profileID));
             if (agentConfig != null)
             {
                 List<AgentConfigEntity> listentity = new List<AgentConfigEntity>(GetObjects<AgentConfigEntity>(string.Format("where AgentConfigID={0}", agentConfig.RowId))).ToList();
                 foreach (AgentConfigEntity en in listentity)
                 {
                     en.Fields = new List<AgentConfigField>(GetObjects<AgentConfigField>(string.Format("where AgentEntityID={0}", en.RowId))).ToList();

                 }
                 agentConfig.Entities = listentity;
             }
             else
             {
                 agentConfig = new AgentConfiguration();
                 agentConfig.CanCreateTask = false;
                 agentConfig.CanPopTask = false;
                 agentConfig.CanStoreScriptVariables = false;
                 agentConfig.ProfileId = profileID;
                 agentConfig.UseDefault = true;
             }
             return agentConfig;
         }

         public void SaveConfigData(AgentConfiguration agentConfigData)
         {
             try
             {
                 if (agentConfigData.ProfileId == 0 || (agentConfigData.ProfileId > 0 && !agentConfigData.UseDefault))
                 {
                     AgentConfiguration agentConfiguration = GetObject<AgentConfiguration>(string.Format("where ProfileID={0}", agentConfigData.ProfileId));
                     CreateResponse createResponse = null;
                     UpdateResponse updateResponse = null;
                     long agentConfigId = 0;
                     if (agentConfiguration == null)
                     {
                         createResponse = CreateObject<AgentConfiguration>(agentConfigData);
                         RNObject[] results = createResponse.RNObjectsResult;
                         if (results.Count() > 0)
                             agentConfigId = results[0].ID.id;
                     }
                     else
                     {
                         updateResponse = UpdateObject<AgentConfiguration>(agentConfigData, agentConfiguration.RowId);
                         agentConfigId = agentConfiguration.RowId;
                     }

                     if (agentConfigId > 0)
                     {
                         this.Destory(agentConfigId);
                         foreach (AgentConfigEntity config in agentConfigData.Entities)
                         {
                             config.AgentConfigID = agentConfigId;
                             if (config.Fields.Count(p => p.EntityName == config.Entity && p.ValueObject != "--SELECT--") > 0)
                             {
                                 CreateResponse entityCreateResponse = CreateObject<AgentConfigEntity>(config);
                                 RNObject[] rnObject = entityCreateResponse.RNObjectsResult;
                                
                                 long agentConfigEntityId = rnObject == null ? 0 : rnObject[0].ID.id ;
                                 if (agentConfigEntityId > 0)
                                 {
                                      foreach (AgentConfigField field in config.Fields)
                                      {
                                          if (field.AgentEntityID == 0) 
                                          {
                                              field.AgentEntityID = agentConfigEntityId; 
                                          }
                                          CreateObject<AgentConfigField>(field);
                                      }
                                 }
                             }
                         }
                     }
                 }
                 else if (agentConfigData.ProfileId > 0 && agentConfigData.UseDefault)
                 {
                     AgentConfiguration profile = GetObject<AgentConfiguration>(string.Format("where ProfileID={0}", agentConfigData.ProfileId));
                     if (profile != null)
                     {
                         this.Destory(profile.RowId);
                     }
                 }
             }
             catch (Exception)
             {
                 throw;
             }
         }

         private void Destory(long configID)
         {
             List<AgentConfigEntity> entities = new List<AgentConfigEntity>(GetObjects<AgentConfigEntity>(string.Format(" where AgentConfigID={0}", configID)));
             if (entities.Count() > 0)
             {
                 foreach (AgentConfigEntity entity in entities)
                 {
                     List<AgentConfigField> profiledata = new List<AgentConfigField>(GetObjects<AgentConfigField>(string.Format(" where AgentEntityID={0}", entity.RowId)));
                     foreach (AgentConfigField agentField in profiledata)
                     {
                         this.DeleteObject<AgentConfigField>(agentField.RowId);
                     }

                     this.DeleteObject<AgentConfigEntity>(entity.RowId);
                 }
                 this.DeleteObject<AgentConfiguration>(configID);
             }
         }

         private long CreateAgentConfig(AgentConfiguration agentdata)
         {
             try
             {
                 _icGenericObject = new GenericObject();
                 _rnObjType.Namespace = CtiObjects.PackageName;
                 _rnObjType.TypeName = CtiObjects.AgentConfiguration;
                 _icGenericObject.ObjectType = _rnObjType;

                 long Id = -1;

                 AgentConfiguration agentconfig = GetObject<AgentConfiguration>(string.Format("where ProfileID={0}", agentdata.ProfileId));

                 List<GenericField> gfsConfigprofileSet = new List<GenericField>();
                 gfsConfigprofileSet.Add(createGenericField("ProfileID", ItemsChoiceType.IntegerValue, agentdata.ProfileId));
                 gfsConfigprofileSet.Add(createGenericField("CanCreateTask", ItemsChoiceType.BooleanValue, agentdata.CanCreateTask));
                 gfsConfigprofileSet.Add(createGenericField("CanPopTask", ItemsChoiceType.BooleanValue, agentdata.CanPopTask));
                 gfsConfigprofileSet.Add(createGenericField("CanStoreScriptVariables", ItemsChoiceType.BooleanValue, agentdata.CanStoreScriptVariables));

                 if (agentconfig != null)
                 {
                     _icGenericObject.GenericFields = gfsConfigprofileSet.ToArray();
                     ID autoID = new ID();
                     autoID.id = agentconfig.RowId;
                     autoID.idSpecified = true;
                     _icGenericObject.ID = autoID;

                     Id = autoID.id;
                     UpdateProcessingOptions updateProc = new UpdateProcessingOptions();
                     updateProc.SuppressExternalEvents = false;
                     updateProc.SuppressRules = false;

                     UpdateRequest updateReq = new UpdateRequest(ClientHeader, new RNObject[] { _icGenericObject }, updateProc);
                     UpdateResponse updateRes = GetChannel().Update(updateReq);

                 }
                 else
                 {
                     _icGenericObject.GenericFields = gfsConfigprofileSet.ToArray();
                     CreateProcessingOptions createprocess = new CreateProcessingOptions();
                     createprocess.SuppressExternalEvents = false;
                     createprocess.SuppressRules = false;

                     CreateRequest cre = new CreateRequest(ClientHeader, new RNObject[] { _icGenericObject }, createprocess);
                     CreateResponse CreRes = GetChannel().Create(cre);
                     RNObject[] results = CreRes.RNObjectsResult;

                     if (results.Count() > 0)
                         Id = results[0].ID.id;
                 }

                 return Id;
             }
             catch (Exception ex)
             {
                 throw;
             }
         } 

         private long CreateAgentConfigEntity(AgentConfigEntity entity)
         {
             try
             {
                 _icGenericObject = new GenericObject();
                 _rnObjType.Namespace = CtiObjects.PackageName;
                 _rnObjType.TypeName = CtiObjects.AgentConfigEntity;
                 _icGenericObject.ObjectType = _rnObjType;


                 NamedID confid = new NamedID();
                 ID idva = new ID();
                 idva.id = entity.AgentConfigID;
                 idva.idSpecified = true;
                 confid.ID = idva;

                 List<GenericField> gfsConfigprofileSet = new List<GenericField>();
                 gfsConfigprofileSet.Add(createGenericField("AgentConfigID", ItemsChoiceType.NamedIDValue, confid));
                 gfsConfigprofileSet.Add(createGenericField("Entity", ItemsChoiceType.StringValue, entity.Entity));
                 gfsConfigprofileSet.Add(createGenericField("MetaDataLink", ItemsChoiceType.StringValue, entity.MetaDataLink));
                 _icGenericObject.GenericFields = gfsConfigprofileSet.ToArray();

                 CreateProcessingOptions createprocess = new CreateProcessingOptions();
                 createprocess.SuppressExternalEvents = false;
                 createprocess.SuppressRules = false;

                 CreateRequest cre = new CreateRequest(ClientHeader, new RNObject[] { _icGenericObject }, createprocess);
                 CreateResponse CreRes = GetChannel().Create(cre);
                 RNObject[] results = CreRes.RNObjectsResult;

                 if (results == null)
                     return -1;

                 return results[0].ID.id;
             }
             catch (Exception ex)
             {
                 throw;
             }
         } 

         private GenericField createGenericField(string Name, ItemsChoiceType itemsChoiceType, object Value)
         {
             GenericField gf = new GenericField();
             gf.name = Name;
             gf.DataValue = new DataValue();
             gf.DataValue.ItemsElementName = new ItemsChoiceType[] { itemsChoiceType };
             gf.DataValue.Items = new object[] { Value };
             return gf;
         } 

         private void CreateConfigEntityField(List<AgentConfigField> fields, long agentEntityID)
         {
             try
             {
                 List<RNObject> rnfields = new List<RNObject>();

                 foreach (AgentConfigField field in fields)
                 {
                     if (field.ValueObject != "--SELECT--")
                     {
                         _icGenericObject = new GenericObject();
                         _rnObjType.Namespace = CtiObjects.PackageName;
                         _rnObjType.TypeName = CtiObjects.AgentConfigField;
                         _icGenericObject.ObjectType = _rnObjType;

                         List<GenericField> gfsConfigprofileSet = new List<GenericField>();
                         gfsConfigprofileSet.Add(createGenericField("AgentEntityID", ItemsChoiceType.NamedIDValue, icentityid));
                         gfsConfigprofileSet.Add(createGenericField("ValueObject", ItemsChoiceType.StringValue, field.ValueObject));
                         gfsConfigprofileSet.Add(createGenericField("ValueField", ItemsChoiceType.StringValue, field.ValueField));
                         gfsConfigprofileSet.Add(createGenericField("FieldName", ItemsChoiceType.StringValue, field.FieldName));

                         if(field.FieldMetaDataLink!=null)
                         gfsConfigprofileSet.Add(createGenericField("FieldMetaDataLink", ItemsChoiceType.StringValue, field.FieldMetaDataLink));

                         if (field.FieldType!= null)
                         gfsConfigprofileSet.Add(createGenericField("FieldType", ItemsChoiceType.StringValue, field.FieldType));

                         _icGenericObject.GenericFields = gfsConfigprofileSet.ToArray();
                         rnfields.Add(_icGenericObject);
                     }
                 }
                 CreateProcessingOptions createprocess = new CreateProcessingOptions();
                 createprocess.SuppressExternalEvents = false;
                 createprocess.SuppressRules = false;

                 if (rnfields.Count() > 0)
                 {
                     CreateRequest cre = new CreateRequest(ClientHeader, rnfields.ToArray(), createprocess);
                     CreateResponse CreRes = GetChannel().Create(cre);
                     RNObject[] results = CreRes.RNObjectsResult;
                 }
             }
             catch (Exception ex)
             {
                 throw;
             }
         } 

         private void DestroyProfile(long profileid)
         {
             _icGenericObject = new GenericObject();
             _rnObjType.Namespace = CtiObjects.PackageName;
             _rnObjType.TypeName = CtiObjects.AgentConfiguration;
             _icGenericObject.ObjectType = _rnObjType;
             List<GenericField> gfs = new List<GenericField>();
             ID proid = new ID();
             proid.id = profileid;
             proid.idSpecified = true;
             _icGenericObject.ID = proid;
             DestroyProcessingOptions drop = new DestroyProcessingOptions();
             drop.SuppressExternalEvents = false;
             drop.SuppressRules = false;
             DestroyRequest destroyReq = new DestroyRequest(ClientHeader, new RNObject[] { _icGenericObject }, drop);
             DestroyResponse destroyRes = GetChannel().Destroy(destroyReq);
         }

         private void DestroyAgentEntity(long profileidentityid)
         {
             List<RNObject> destroyentity = new List<RNObject>();
             List<AgentConfigEntity> entitydata = new List<AgentConfigEntity>(GetObjects<AgentConfigEntity>(string.Format(" where AgentConfigID={0}", profileidentityid)));
             foreach (AgentConfigEntity rn in entitydata)
             {
                 _icGenericObject = new GenericObject();
                 _rnObjType.Namespace = CtiObjects.PackageName;
                 _rnObjType.TypeName = CtiObjects.AgentConfigEntity;
                 _icGenericObject.ObjectType = _rnObjType;

                 NamedID prorelationid = new NamedID();
                 ID incID = new ID();
                 incID.id = profileidentityid;
                 incID.idSpecified = true;
                 prorelationid.ID = incID;

                 ID rowid = new ID();
                 rowid.id = rn.RowId;
                 rowid.idSpecified = true;
                 _icGenericObject.ID = rowid;

                 List<GenericField> gfs = new List<GenericField>();
                 gfs.Add(createGenericField("AgentConfigID", ItemsChoiceType.NamedIDValue, prorelationid));

                 _icGenericObject.GenericFields = gfs.ToArray();
                 destroyentity.Add(_icGenericObject);
             }

             if (destroyentity.Count() > 0)
             {
                 DestroyProcessingOptions drop = new DestroyProcessingOptions();
                 drop.SuppressExternalEvents = false;
                 drop.SuppressRules = false;
                 DestroyRequest destroyReq = new DestroyRequest(ClientHeader, destroyentity.ToArray(), drop);
                 DestroyResponse destroyRes = GetChannel().Destroy(destroyReq);
             }
         }

         private void DestroyAgentEntityField(long entityidentityid)
         {
             List<RNObject> destroyfields = new List<RNObject>();

             List<AgentConfigField> profiledata = new List<AgentConfigField>(GetObjects<AgentConfigField>(string.Format(" where AgentEntityID={0}", entityidentityid)));
             foreach (AgentConfigField rn in profiledata)
             {
                 _icGenericObject = new GenericObject();
                 _rnObjType.Namespace = CtiObjects.PackageName;
                 _rnObjType.TypeName = CtiObjects.AgentConfigField;
                 _icGenericObject.ObjectType = _rnObjType;
                 NamedID relationid = new NamedID();
                 ID incID = new ID();
                 incID.id = entityidentityid;
                 incID.idSpecified = true;
                 relationid.ID = incID;

                 ID rowid = new ID();
                 rowid.id = rn.RowId;
                 rowid.idSpecified = true;
                 _icGenericObject.ID = rowid;

                 List<GenericField> gfs = new List<GenericField>();
                 gfs.Add(createGenericField("AgentEntityID", ItemsChoiceType.NamedIDValue, relationid));
                 _icGenericObject.GenericFields = gfs.ToArray();
                 destroyfields.Add(_icGenericObject);
             }

             if (destroyfields.Count() > 0)
             {
                 DestroyProcessingOptions drop = new DestroyProcessingOptions();
                 drop.SuppressExternalEvents = false;
                 drop.SuppressRules = false;
                 DestroyRequest destroyReq = new DestroyRequest(ClientHeader, destroyfields.ToArray(), drop);
                 DestroyResponse destroyRes = GetChannel().Destroy(destroyReq);
             }
         } */

        #endregion

        public GenericField createGenericField(string Name, ItemsChoiceType itemsChoiceType, object Value)
        {
            GenericField gf = new GenericField();
            gf.name = Name;
            gf.DataValue = new DataValue();
            gf.DataValue.ItemsElementName = new ItemsChoiceType[] { itemsChoiceType };
            gf.DataValue.Items = new object[] { Value };
            return gf;
        } 

        public IEnumerable<MetaDataClass> GetAllMetatData()
        {
            GetMetaDataRequest metaDataRequest = new GetMetaDataRequest(ClientHeader);
            GetMetaDataResponse metaDataResponse = GetChannel().GetMetaData(metaDataRequest);
            IEnumerable<MetaDataClass> filterMetaDataClasses = metaDataResponse.MetaDataClass;

            return filterMetaDataClasses;
        }

        public MetaDataAttribute[] GetMetaDataForObjects(string[] classes, RNObjectType[] rnObjectType = null, string[] metaDataLink = null)
        {
            //Invoke the GetMetaDataForClass operation
            try
            {
                GetMetaDataForClassRequest metaDataForClassReq = new GetMetaDataForClassRequest(ClientHeader, classes, rnObjectType, metaDataLink);
                GetMetaDataForClassResponse metadataForClassRes = GetChannel().GetMetaDataForClass(metaDataForClassReq);

                if (metadataForClassRes.MetaDataClass.Count() == 1)
                    return metadataForClassRes.MetaDataClass[0].Attributes;
            }
            catch (Exception) { }
            return null;
        }

        public T GetObject<T>(string predicate = null) where T : class, new()
        {
            var objectType = typeof(T);

            var objectAttribute = objectType.GetCustomAttributes(typeof(RightNowObjectAttribute), true).FirstOrDefault() as RightNowObjectAttribute;
            if (objectAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowCustomObjectAttribute to associate the proper metadata with the type");

            var query = string.Format("SELECT * from {0}.{1} {2}", objectAttribute.PackageName, objectAttribute.ObjectName, predicate ?? string.Empty);

            var request = new QueryCSVRequest(ClientHeader, query, 1, ",", false, true);

            var rightNowChannel = GetChannel();
            var result = rightNowChannel.QueryCSV(request);

            return MaterializeObjects<T>(result, objectType, 1).FirstOrDefault();
        }

        public Dictionary<long, string> GetNameValues(string fieldName, string package = null)
        {
            GetValuesForNamedIDRequest namedIdRequest = new GetValuesForNamedIDRequest(ClientHeader, null, fieldName);
            Dictionary<long, string> namedIds = new Dictionary<long, string>();

            GetValuesForNamedIDResponse resp = GetChannel().GetValuesForNamedID(namedIdRequest);

            foreach (NamedID namedId in resp.Entry)
            {
                if (namedId.ID.idSpecified)
                {
                    namedIds[namedId.ID.id] = namedId.Name;
                }
            }
            return namedIds;
        }

        public IEnumerable<T> GetObjects<T>(string predicate = null) where T : class, new()
        {
            var objectType = typeof(T);

            var customObjectAttribute = objectType.GetCustomAttributes(typeof(RightNowObjectAttribute), true).FirstOrDefault() as RightNowObjectAttribute;
            if (customObjectAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

            var query = string.Format("SELECT * from {0}.{1} {2}", customObjectAttribute.PackageName, customObjectAttribute.ObjectName, predicate ?? string.Empty);

            var request = new QueryCSVRequest(ClientHeader, query, 100, ",", false, true);

            var rightNowChannel = GetChannel();
            var result = rightNowChannel.QueryCSV(request);

            return MaterializeObjects<T>(result, objectType);
        }

        public UpdateResponse UpdateObject<T>(T obj, long uniqueId) where T : class, new()
        {
            Type objType = typeof(T);
            var objAttribute = objType.GetCustomAttributes(typeof(RightNowObjectAttribute), true).FirstOrDefault() as RightNowObjectAttribute;
            if (objAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

            GenericObject genericObject = new GenericObject();
            RNObjectType rnObjType = new RNObjectType() { Namespace = objAttribute.PackageName, TypeName = objAttribute.ObjectName };
            genericObject.ObjectType = rnObjType;

            List<GenericField> genericFields = new List<GenericField>();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                RightNowFieldAttribute attribute = property.GetCustomAttributes(typeof(RightNowFieldAttribute), true).FirstOrDefault() as RightNowFieldAttribute;
                if (attribute != null && attribute.CanUpdate)
                {
                    var propValue = property.GetValue(obj, null);
                    if (!string.IsNullOrWhiteSpace(propValue.ToString()))
                    {
                        genericFields.Add(CreateGenericField(attribute, propValue));
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
            UpdateRequest updateRequest = new UpdateRequest(this.ClientHeader, new RNObject[] { genericObject }, options);
            UpdateResponse updateResponse = this.GetChannel().Update(updateRequest);
            return updateResponse;
        }

        public CreateResponse CreateObject<T>(T obj) where T : class , new()
        {
            Type objType = typeof(T);
            var objAttribute = objType.GetCustomAttributes(typeof(RightNowObjectAttribute), true).FirstOrDefault() as RightNowObjectAttribute;
            if (objAttribute == null)
                throw new InvalidOperationException("The type provided is not a RightNow custom object type. Please use the RightNowObjectAttribute to associate the proper metadata with the type");

            GenericObject genericObject = new GenericObject();
            RNObjectType rnObjType = new RNObjectType() { Namespace = objAttribute.PackageName, TypeName = objAttribute.ObjectName };
            genericObject.ObjectType = rnObjType;

            List<GenericField> genericFields = new List<GenericField>();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object attribute = property.GetCustomAttributes(typeof(RightNowFieldAttribute), true).FirstOrDefault();
                if (attribute != null)
                {
                    RightNowFieldAttribute rightNowAttribute = attribute as RightNowFieldAttribute;
                    if (rightNowAttribute.CanUpdate)
                    {
                        var propValue = property.GetValue(obj, null);
                        if (propValue != null && !string.IsNullOrWhiteSpace(propValue.ToString()))
                        {
                            genericFields.Add(CreateGenericField(rightNowAttribute, propValue));
                        }
                    }
                }
            }

            genericObject.GenericFields = genericFields.ToArray();
            CreateProcessingOptions options = new CreateProcessingOptions();
            options.SuppressExternalEvents = false;
            options.SuppressRules = false;
            CreateRequest createRequest = new CreateRequest(this.ClientHeader, new RNObject[] { genericObject }, options);
            CreateResponse createResponcse = this.GetChannel().Create(createRequest);
            return createResponcse;
        }

        public void DeleteObject<T>(long uniqueId) where T : class, new()
        {
            Type objType = typeof(T);
            var objAttribute = objType.GetCustomAttributes(typeof(RightNowObjectAttribute), true).FirstOrDefault() as RightNowObjectAttribute;
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
            DestroyRequest destroyRequest = new DestroyRequest(ClientHeader, new RNObject[] { genericObject }, options);
            DestroyResponse destroyResponse = GetChannel().Destroy(destroyRequest);
        }

        public ClientInfoHeader ClientHeader
        {
            get
            {
                return new ClientInfoHeader()
                {
                    AppID = "inContact CTI"
                };
            }
        }

        public RightNowSyncPortChannel GetChannel()
        {
            Binding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);

            ((BasicHttpBinding)binding).Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            ((BasicHttpBinding)binding).MaxReceivedMessageSize = 1024 * 1024 * 1024;
            ((BasicHttpBinding)binding).MaxBufferSize = 1024 * 1024 * 1024;

            BindingElementCollection elements = binding.CreateBindingElements();
            elements.Find<SecurityBindingElement>().IncludeTimestamp = false;

            binding = new CustomBinding(elements);

            var channelFactory = new ChannelFactory<RightNowSyncPortChannel>(binding, new EndpointAddress(_globalContext.GetInterfaceServiceUrl(ConnectServiceType.Soap)));

            _globalContext.PrepareConnectSession(channelFactory);

            var rightNowChannel = channelFactory.CreateChannel();
            return rightNowChannel;
        }

        private IEnumerable<T> MaterializeObjects<T>(QueryCSVResponse result, Type objectType, int objectCount = 100) where T : class, new()
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
                    var attribute = property.GetCustomAttributes(typeof(RightNowFieldAttribute), true).FirstOrDefault() as RightNowFieldAttribute;
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

        private GenericField CreateGenericField(RightNowFieldAttribute attribute, object value)
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
            genericField.DataValue.ItemsElementName = new ItemsChoiceType[] { fieldType };
            return genericField;
        }

        public NamedID GetNameIDUsingFieldName(string fieldName, string status)
        {
            GetValuesForNamedIDRequest request = new GetValuesForNamedIDRequest();
            request.FieldName = fieldName;
            request.ClientInfoHeader = ClientHeader;
            GetValuesForNamedIDResponse valuesForNamedID = GetChannel().GetValuesForNamedID(request);
            foreach (NamedID profile in valuesForNamedID.Entry)
            {
                if (profile.Name == status)
                    return profile;
            }
            return null;
        }

        
    }
}

﻿using System;
using System.Linq;
using System.Collections.Generic;
using BExIS.Dlm.Entities.DataStructure;
using BExIS.Dlm.Services.DataStructure;
using BExIS.Web.Shell.Areas.RPM.Classes;

namespace BExIS.Web.Shell.Areas.RPM.Models
{
    public struct ItemStruct
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
    public class FilterValueStruct
    {
        public string Name { get; set; }
        public List<long> Appearance { get; set; }

        public FilterValueStruct()
        {
            this.Name = "";
            this.Appearance = new List<long>();
        }
    } 
    public class AttributeFilterStruct
    {
        public Dictionary<string, FilterValueStruct> Values { get; set; }

        public AttributeFilterStruct()
        {
            this.Values = new Dictionary<string, FilterValueStruct>();
        }
    }

    public class AttributeFilterModel
    {
        public Dictionary<string,AttributeFilterStruct> AttributeFilterDictionary { get; set; }

        public AttributeFilterModel()
        {
            this.AttributeFilterDictionary = new Dictionary<string, AttributeFilterStruct>();
        }

        public AttributeFilterModel fill()
        {
            this.AttributeFilterDictionary = new Dictionary<string, AttributeFilterStruct>();
            AttributePreviewModel attributePreviewModel = new AttributePreviewModel().fill(false);

            this.AttributeFilterDictionary.Add("Unit", new AttributeFilterStruct());
            this.AttributeFilterDictionary.Add("Data Type", new AttributeFilterStruct());

            string key = "";
            FilterValueStruct value = new FilterValueStruct();

            foreach (AttributePreviewStruct aps in attributePreviewModel.AttributePreviews)
            {
                key = aps.Unit.Name.ToLower().Replace(" ", "");
                value = new FilterValueStruct();

                if (this.AttributeFilterDictionary["Unit"].Values.ContainsKey(key))
                {
                    this.AttributeFilterDictionary["Unit"].Values[key].Appearance.Add(aps.Id);
                }
                else
                {
                    value.Name = aps.Unit.Name;
                    value.Appearance.Add(aps.Id);
                    this.AttributeFilterDictionary["Unit"].Values.Add(key, value);
                }

                key = aps.DataType.ToLower().Replace(" ", "");
                value = new FilterValueStruct();

                if (this.AttributeFilterDictionary["Data Type"].Values.ContainsKey(key))
                {
                    this.AttributeFilterDictionary["Data Type"].Values[key].Appearance.Add(aps.Id);
                }
                else
                {
                    value.Name = aps.DataType;
                    value.Appearance.Add(aps.Id);
                    this.AttributeFilterDictionary["Data Type"].Values.Add(key, value);
                }
            }
            foreach (KeyValuePair<string, AttributeFilterStruct> kv in this.AttributeFilterDictionary)
            {
                kv.Value.Values = kv.Value.Values.OrderBy(v => v.Value.Name).ToDictionary(v => v.Key, v => v.Value);
            }
            return this;
        }
    }

    public class AttributePreviewStruct
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemStruct Unit;
        public string DataType { get; set; }
        public Dictionary<long, string> Constraints { get; set; }
        public bool inUse { get; set; }


        public AttributePreviewStruct()
        {
            this.Id = 0;
            this.Name = "";
            this.Description = "";
            this.Unit = new ItemStruct();
            this.DataType = "";
            this.Constraints = new Dictionary<long, string>();
            this.inUse = false;
        }

        public AttributePreviewStruct fill(long attributeId)
        {
            return this.fill(attributeId, true);
        }

        public AttributePreviewStruct fill(long attributeId, bool getConstraints)
        {
            DataContainerManager dataAttributeManager = new DataContainerManager();
            DataAttribute DataAttribute = dataAttributeManager.DataAttributeRepo.Get(attributeId);

            return this.fill(DataAttribute, getConstraints);
        }

        public AttributePreviewStruct fill(DataAttribute dataAttribute)
        {
            return this.fill(dataAttribute, true);
        }

        public AttributePreviewStruct fill(DataAttribute dataAttribute, bool getConstraints)
        {
            this.Id = dataAttribute.Id;
            this.Name = dataAttribute.Name;
            this.Description = dataAttribute.Description;
            this.Unit.Id = dataAttribute.Unit.Id;
            this.Unit.Name = dataAttribute.Unit.Name;
            this.DataType = dataAttribute.DataType.Name;

            if (getConstraints)
            {
                if (dataAttribute.Constraints != null)
                {
                    foreach (Constraint c in dataAttribute.Constraints)
                    {
                        c.Materialize();
                        this.Constraints.Add(c.Id, c.FormalDescription);
                    }
                }
            }

            if (dataAttribute.UsagesAsVariable.Count > 0)
                this.inUse = true;
            else
                this.inUse = false;

            return this;
        }
    }
    public class VariablePreviewStruct : AttributePreviewStruct
    {
        public bool isOptional { get; set; }
        public List<ItemStruct> convertibleUnits { get; set; }
        public AttributePreviewStruct Attribute { get; set; }


        public VariablePreviewStruct()
        {
            this.Id = 0;
            this.Name = "";
            this.Description = "";
            this.isOptional = true;
            this.Unit = new ItemStruct();
            this.convertibleUnits = new List<ItemStruct>();
            this.DataType = "";
            this.Constraints = new Dictionary<long, string>();
            this.Attribute = new AttributePreviewStruct();
            this.inUse = false;
        }

        public VariablePreviewStruct fill(long attributeId)
        {
            return this.fill(attributeId, true);
        }

        public VariablePreviewStruct fill(long attributeId, bool getConstraints)
        {
            DataContainerManager dataAttributeManager = new DataContainerManager();
            DataAttribute dataAttribute = dataAttributeManager.DataAttributeRepo.Get(attributeId);
            Variable variable = new Variable()
            {
                Label = dataAttribute.Name,
                Description = dataAttribute.Description,
                Unit = dataAttribute.Unit,
                DataAttribute = dataAttribute
            };

            return this.fill(variable, getConstraints);
        }

        public VariablePreviewStruct fill(Variable variable)
        {
            return this.fill(variable, true);
        }

        public VariablePreviewStruct fill(Variable variable, bool getConstraints)
        {
            variable.Unit = variable.Unit ?? new Unit();
            variable.Unit.Dimension = variable.Unit.Dimension ?? new Dimension();
            variable.DataAttribute = variable.DataAttribute ?? new DataAttribute();
            variable.DataAttribute.DataType = variable.DataAttribute.DataType ?? new DataType();

            this.Id = variable.Id;
            this.Name = variable.Label;
            this.Description = variable.Description;
            this.isOptional = variable.IsValueOptional;
            this.Unit.Id = variable.Unit.Id;
            this.Unit.Name = variable.Unit.Name;
            this.convertibleUnits = getUnitListByDimenstionAndDataType(variable.Unit.Dimension.Id, variable.DataAttribute.DataType.Id);
            this.DataType = variable.DataAttribute.DataType.Name;

            if (getConstraints)
            {
                if (variable.DataAttribute.Constraints != null)
                {
                    foreach (Constraint c in variable.DataAttribute.Constraints)
                    {
                        c.Materialize();
                        this.Constraints.Add(c.Id, c.FormalDescription);
                    }
                }
            }

            this.Attribute = Attribute.fill(variable.DataAttribute, false);

            return this;
        }

        public List<ItemStruct> getUnitListByDimenstionAndDataType(long dimensionId, long dataTypeId)
        {
            List<ItemStruct> UnitStructs = new List<ItemStruct>();
            UnitManager unitmanager = new UnitManager();
            List<Unit> units = new List<Unit>();
            if(unitmanager.DimensionRepo.Get(dimensionId) != null)
                units = unitmanager.DimensionRepo.Get(dimensionId).Units.ToList();

            ItemStruct tempUnitStruct = new ItemStruct();
            foreach (Unit u in units)
            {
                if (u.Name.ToLower() != "none")
                {
                    foreach (DataType dt in u.AssociatedDataTypes)
                    {
                        if (dt.Id == dataTypeId)
                        {
                            tempUnitStruct.Id = u.Id;
                            tempUnitStruct.Name = u.Name;
                            UnitStructs.Add(tempUnitStruct);
                            break;
                        }
                    }
                }
                else
                {
                    tempUnitStruct.Id = u.Id;
                    tempUnitStruct.Name = u.Name;
                    UnitStructs.Add(tempUnitStruct);
                }
            }

            return UnitStructs;
        }
    }

    public class AttributeEditStruct:AttributePreviewStruct
    {
        public ItemStruct DataType { get; set; }
        public List<ItemStruct> Units { get; set; }
        public List<ItemStruct> DataTypes { get; set; }

        public AttributeEditStruct()
        {
            this.Id = 0;
            this.Name = "";
            this.Description = "";
            this.Unit = new ItemStruct();
            this.DataType = new ItemStruct();
            this.Constraints = new Dictionary<long, string>();
            this.inUse = false;
            this.Units = new List<ItemStruct>();
            this.DataTypes = new List<ItemStruct>();

            foreach (Unit u in new UnitManager().Repo.Get())
            {
                this.Units.Add(new ItemStruct()
                {
                    Name = u.Name,
                    Id = u.Id
                });
            }

            this.Units = this.Units.OrderBy(u=>u.Name).ToList();
        }
    }


    public class AttributePreviewModel
    {
        public List<AttributePreviewStruct> AttributePreviews { get; set; }

        public AttributePreviewModel()
        {
            this.AttributePreviews = new List<AttributePreviewStruct>();
        }

        public AttributePreviewModel fill()
        {
            return this.fill(true);
        }

        public AttributePreviewModel fill(bool getConstraints)
        {
            this.AttributePreviews = new List<AttributePreviewStruct>();
            DataContainerManager dataAttributeManager = new DataContainerManager();

            foreach (DataAttribute da in dataAttributeManager.DataAttributeRepo.Get().ToList())
            {                
                this.AttributePreviews.Add(new AttributePreviewStruct().fill(da, getConstraints));
            }
            return this;
        }
    }

    public class DataStructurePreviewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool inUse { get; set; }
        public List<VariablePreviewStruct> VariablePreviews { get; set; }

        public DataStructurePreviewModel()
        {
            this.Id = 0;
            this.Name = "";
            this.Description = "";
            this.inUse = false;
            this.VariablePreviews = new List<VariablePreviewStruct>();
        }

        public DataStructurePreviewModel fill()
        {
            return this.fill(0);
        }

        public DataStructurePreviewModel fill(long dataStructureId)
        {
            if (dataStructureId > 0)
            {
                DataStructureManager dataStructureManager = new DataStructureManager();
                if (dataStructureManager.StructuredDataStructureRepo.Get(dataStructureId) != null)
                {
                    StructuredDataStructure dataStructure = dataStructureManager.StructuredDataStructureRepo.Get(dataStructureId);
                    VariablePreviewStruct variablePreview;

                    this.Id = dataStructure.Id;
                    this.Name = dataStructure.Name;
                    this.Description = dataStructure.Description;

                    if(dataStructure.Datasets.Count > 0)
                    {
                        this.inUse = true;
                    }

                    foreach (Variable v in DataStructureIO.getOrderedVariables(dataStructure))
                    {
                        variablePreview = new VariablePreviewStruct().fill(v);
                        this.VariablePreviews.Add(variablePreview);
                    }
                }
                else if (dataStructureManager.UnStructuredDataStructureRepo.Get(dataStructureId) != null)
                {
                    UnStructuredDataStructure dataStructure = dataStructureManager.UnStructuredDataStructureRepo.Get(dataStructureId);

                    this.Id = dataStructure.Id;
                    this.Name = dataStructure.Name;
                    this.Description = dataStructure.Description;
                    this.VariablePreviews = null;

                    if(dataStructure.Datasets.Count > 0)
                    {
                        this.inUse = true;
                    }
                }
                return this;
            }
            else
            {
                return new DataStructurePreviewModel();
            }
        }
    }

    public class storeVariableStruct
    {
        public long Id { get; set; }
        public long AttributeId { get; set; }
        public string Lable { get; set; }
        public string Description { get; set; }
        public long UnitId { get; set; }
        public bool isOptional { get; set; }

        public storeVariableStruct()
        {
            this.Id = 0;
            this.AttributeId = 0;
            this.Lable = "";
            this.Description = "";
            this.UnitId = 0;
            this.isOptional = true;
        }
    }
}
    
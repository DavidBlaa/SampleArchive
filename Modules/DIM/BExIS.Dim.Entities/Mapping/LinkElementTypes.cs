﻿namespace BExIS.Dim.Entities.Mapping
{
    public enum Key
    {
        Author,
        Description,
        License,
        ProjectTitle,
        Title,
        Plot = 10000, // RDB Module addition,
        Site = 10001, // RDB Module addition,
        Barcode = 10002,// RDB Module addition
        SampleType = 10003// RDB Module addition
    }

    public enum LinkElementComplexity
    {
        None,
        Simple,
        Complex
    }

    public enum LinkElementType
    {
        MetadataStructure = 0,
        XSD = 1,
        System = 2,
        SimpleMetadataAttribute = 3,
        ComplexMetadataAttribute = 4,
        MetadataAttributeUsage = 5,
        MetadataNestedAttributeUsage = 6,
        MetadataPackage = 7,
        MetadataPackageUsage = 8,
        PartyType = 9,
        PartyCustomType = 10,
        Key = 11,
        Role = 12,
        PartyRelationshipType = 13
    }



    public class LinkElementMetadataStructure : LinkElement { }
    public class LinkElementXsd : LinkElement { }
    public class LinkElementSystem : LinkElement { }
    public class LinkElementSimpleMetadataAttribute : LinkElement { }
    public class LinkElementComplexMetadatAttribute : LinkElement { }
    public class LinkElementMetadatAttributeUsage : LinkElement { }
    public class LinkElementMetadataNestedAttributeUsage : LinkElement { }
    public class LinkElementMetadataPackage : LinkElement { }
    public class LinkElementMetadataPackageUsage : LinkElement { }
    public class LinkElementPartyType : LinkElement { }
    public class LinkElementParty : LinkElement { }
    public class LinkElementKey : LinkElement { }
    public class LinkElementRole : LinkElement { }



}
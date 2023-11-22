﻿/*
	Copyright © Bryan Apellanes 2015  
*/
namespace Bam.Net.Data.Schema
{
    public interface IDaoGenerator
    {
        FileInfo DaoAssemblyFile { get; set; }
        IDaoCodeWriter DaoCodeWriter { get; set; }
        bool DisposeOnComplete { get; set; }
        bool GenerateQiClasses { get; set; }
        string Namespace { get; set; }
        IDaoTargetStreamResolver TargetStreamResolver { get; set; }

        event GeneratorEventDelegate GenerateComplete;
        event GeneratorEventDelegate GenerateStarted;

        void Generate(ISchemaDefinition schema);
        void Generate(ISchemaDefinition schema, Func<string, Stream> targetResolver = null, string root = "./", string partialsDir = null);
        void Generate(ISchemaDefinition schema, string root);
        void Generate(ISchemaDefinition schema, string root, string partialsDir);
    }
}
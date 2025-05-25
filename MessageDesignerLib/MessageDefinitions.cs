using System;
using System.Collections.Generic;

namespace MessageDesignerLib
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Root
    {
        public Messages Messages { get; set; }
    }
    public class Messages
    {
        public Message Message { get; set; }
    }
    public class Message
    {
        public string Name { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
    public class Request
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string IsCanFdFrame { get; set; }
        public RequestHexData RequestHexData { get; set; }
    }
    public class RequestHexData
    {
        public string IsPatternBased { get; set; }
        public string DataString { get; set; }
        public DataPatterns DataPatterns { get; set; }
    }
    public class Response
    {
        public string Id { get; set; }
        public string Payload { get; set; }
        public string IsCanFdFrame { get; set; }
        public ResponseHexData ResponseHexData { get; set; }
    }
    public class ResponseHexData
    {
        public string IsPatternBased { get; set; }
        public string DataString { get; set; }
        public DataSubstitutions DataSubstitutions { get; set; }
    }
    public class DataPatterns
    {
        public Pattern[] Pattern { get; set; }
    }
    public class Pattern
    {
        public string Index { get; set; }
        public string RangeStart { get; set; }
        public string RangeEnd { get; set; }
    }
    public class DataSubstitutions
    {
        public Substitution[] Substitution { get; set; }
    }
    public class Substitution
    {
        public string SourceDataIndexFromRequest { get; set; }
        public string DestinationDataIndexToResponse { get; set; }
    }
}

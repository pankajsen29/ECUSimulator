using System;
using System.Collections.Generic;

namespace MessageDesignerLib
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    internal class Root
    {
        public List<Message> Messages { get; set; }
    }

    internal class Message
    {
        public string Name { get; set; }
        public Request Request { get; set; }
        public Response Response { get; set; }
    }
    internal class Request
    {
        public string Id { get; set; }
        public int Payload { get; set; }
        public bool IsCanFdFrame { get; set; }
        public RequestHexData RequestHexData { get; set; }
    }
    internal class RequestHexData
    {
        public bool IsRuleBased { get; set; }
        public string DataString { get; set; }
        public List<Pattern> DataPatterns { get; set; }
    }
    internal class Pattern
    {
        public int Index { get; set; }
        public byte RangeStart { get; set; }
        public byte RangeEnd { get; set; }
    }
    internal class Response
    {
        public string Id { get; set; }
        public int Payload { get; set; }
        public bool IsCanFdFrame { get; set; }
        public ResponseHexData ResponseHexData { get; set; }
    }
    internal class ResponseHexData
    {
        public bool IsRuleBased { get; set; }
        public string DataString { get; set; }
        public List<Substitution> DataSubstitutions { get; set; }
    }
    internal class Substitution
    {
        public int SourceDataIndexFromRequest { get; set; }
        public int DestinationDataIndexToResponse { get; set; }
    }
}

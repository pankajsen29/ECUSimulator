using System;
using System.Collections.Generic;

namespace MessageDesignerLib
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

    public class Root
    {
        public List<Message> Messages { get; set; }
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
        public byte Payload { get; set; }
        public bool IsCanFdFrame { get; set; }
        public RequestHexData RequestHexData { get; set; }
    }
    public class RequestHexData
    {
        public bool IsRuleBased { get; set; }
        public string DataString { get; set; }
        public List<Pattern> DataPatterns { get; set; }
    }
    public class Pattern
    {
        public int Index { get; set; }
        public byte RangeStart { get; set; }
        public byte RangeEnd { get; set; }
    }
    public class Response
    {
        public string Id { get; set; }
        public byte Payload { get; set; }
        public bool IsCanFdFrame { get; set; } // possibility to overwrite the frame type of the response (i.e., tx message)
        public ResponseHexData ResponseHexData { get; set; }
    }
    public class ResponseHexData
    {
        public bool IsRuleBased { get; set; }
        public string DataString { get; set; }
        public List<Substitution> DataSubstitutions { get; set; }
    }
    public class Substitution
    {
        public int SourceDataIndexFromRequest { get; set; }
        public int DestinationDataIndexToResponse { get; set; }
    }
}

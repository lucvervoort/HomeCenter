// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: conformance_tests.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace CloudNative.CloudEvents.Protobuf.UnitTests {

  /// <summary>Holder for reflection information generated from conformance_tests.proto</summary>
  public static partial class ConformanceTestsReflection {

    #region Descriptor
    /// <summary>File descriptor for conformance_tests.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static ConformanceTestsReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "Chdjb25mb3JtYW5jZV90ZXN0cy5wcm90bxIRaW8uY2xvdWRldmVudHMudjEa",
            "EWNsb3VkZXZlbnRzLnByb3RvIkgKE0NvbmZvcm1hbmNlVGVzdEZpbGUSMQoF",
            "dGVzdHMYASADKAsyIi5pby5jbG91ZGV2ZW50cy52MS5Db25mb3JtYW5jZVRl",
            "c3QitgIKD0NvbmZvcm1hbmNlVGVzdBIKCgJpZBgBIAEoCRITCgtkZXNjcmlw",
            "dGlvbhgCIAEoCRIRCglzYW1wbGVfaWQYAyABKAkSNQoMdmFsaWRfc2luZ2xl",
            "GAQgASgLMh0uaW8uY2xvdWRldmVudHMudjEuQ2xvdWRFdmVudEgAEjkKC3Zh",
            "bGlkX2JhdGNoGAUgASgLMiIuaW8uY2xvdWRldmVudHMudjEuQ2xvdWRFdmVu",
            "dEJhdGNoSAASNwoOaW52YWxpZF9zaW5nbGUYBiABKAsyHS5pby5jbG91ZGV2",
            "ZW50cy52MS5DbG91ZEV2ZW50SAASOwoNaW52YWxpZF9iYXRjaBgHIAEoCzIi",
            "LmlvLmNsb3VkZXZlbnRzLnYxLkNsb3VkRXZlbnRCYXRjaEgAQgcKBWV2ZW50",
            "IioKGkNvbmZvcm1hbmNlVGVzdE1lc3NhZ2VEYXRhEgwKBHRleHQYASABKAlC",
            "LaoCKkNsb3VkTmF0aXZlLkNsb3VkRXZlbnRzLlByb3RvYnVmLlVuaXRUZXN0",
            "c2IGcHJvdG8z"));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { global::CloudNative.CloudEvents.V1.CloudeventsReflection.Descriptor, },
          new pbr::GeneratedClrTypeInfo(null, null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestFile), global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestFile.Parser, new[]{ "Tests" }, null, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest), global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest.Parser, new[]{ "Id", "Description", "SampleId", "ValidSingle", "ValidBatch", "InvalidSingle", "InvalidBatch" }, new[]{ "Event" }, null, null, null),
            new pbr::GeneratedClrTypeInfo(typeof(global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestMessageData), global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestMessageData.Parser, new[]{ "Text" }, null, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  /// <summary>
  /// A simple container for conformance tests.
  /// </summary>
  public sealed partial class ConformanceTestFile : pb::IMessage<ConformanceTestFile>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ConformanceTestFile> _parser = new pb::MessageParser<ConformanceTestFile>(() => new ConformanceTestFile());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ConformanceTestFile> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestsReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestFile() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestFile(ConformanceTestFile other) : this() {
      tests_ = other.tests_.Clone();
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestFile Clone() {
      return new ConformanceTestFile(this);
    }

    /// <summary>Field number for the "tests" field.</summary>
    public const int TestsFieldNumber = 1;
    private static readonly pb::FieldCodec<global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest> _repeated_tests_codec
        = pb::FieldCodec.ForMessage(10, global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest.Parser);
    private readonly pbc::RepeatedField<global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest> tests_ = new pbc::RepeatedField<global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest>();
    /// <summary>
    /// The tests within this file.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public pbc::RepeatedField<global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTest> Tests {
      get { return tests_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ConformanceTestFile);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ConformanceTestFile other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if(!tests_.Equals(other.tests_)) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      hash ^= tests_.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      tests_.WriteTo(output, _repeated_tests_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      tests_.WriteTo(ref output, _repeated_tests_codec);
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      size += tests_.CalculateSize(_repeated_tests_codec);
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ConformanceTestFile other) {
      if (other == null) {
        return;
      }
      tests_.Add(other.tests_);
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            tests_.AddEntriesFrom(input, _repeated_tests_codec);
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            tests_.AddEntriesFrom(ref input, _repeated_tests_codec);
            break;
          }
        }
      }
    }
    #endif

  }

  /// <summary>
  /// A single test in the conformance test suite.
  /// </summary>
  public sealed partial class ConformanceTest : pb::IMessage<ConformanceTest>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ConformanceTest> _parser = new pb::MessageParser<ConformanceTest>(() => new ConformanceTest());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ConformanceTest> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestsReflection.Descriptor.MessageTypes[1]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTest() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTest(ConformanceTest other) : this() {
      id_ = other.id_;
      description_ = other.description_;
      sampleId_ = other.sampleId_;
      switch (other.EventCase) {
        case EventOneofCase.ValidSingle:
          ValidSingle = other.ValidSingle.Clone();
          break;
        case EventOneofCase.ValidBatch:
          ValidBatch = other.ValidBatch.Clone();
          break;
        case EventOneofCase.InvalidSingle:
          InvalidSingle = other.InvalidSingle.Clone();
          break;
        case EventOneofCase.InvalidBatch:
          InvalidBatch = other.InvalidBatch.Clone();
          break;
      }

      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTest Clone() {
      return new ConformanceTest(this);
    }

    /// <summary>Field number for the "id" field.</summary>
    public const int IdFieldNumber = 1;
    private string id_ = "";
    /// <summary>
    /// The ID of the test; must be unique across all protobuf conformance tests.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Id {
      get { return id_; }
      set {
        id_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "description" field.</summary>
    public const int DescriptionFieldNumber = 2;
    private string description_ = "";
    /// <summary>
    /// The description of the test.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Description {
      get { return description_; }
      set {
        description_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "sample_id" field.</summary>
    public const int SampleIdFieldNumber = 3;
    private string sampleId_ = "";
    /// <summary>
    /// For valid tests, the ID of the well-known sample event/batch that
    /// this test data should be equivalent to.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string SampleId {
      get { return sampleId_; }
      set {
        sampleId_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "valid_single" field.</summary>
    public const int ValidSingleFieldNumber = 4;
    /// <summary>
    /// A single event that should be converted to an in-memory representation without error.
    /// sample_id indicates the sample event that the result should be equivalent to.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::CloudNative.CloudEvents.V1.CloudEvent ValidSingle {
      get { return eventCase_ == EventOneofCase.ValidSingle ? (global::CloudNative.CloudEvents.V1.CloudEvent) event_ : null; }
      set {
        event_ = value;
        eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.ValidSingle;
      }
    }

    /// <summary>Field number for the "valid_batch" field.</summary>
    public const int ValidBatchFieldNumber = 5;
    /// <summary>
    /// A batch of events that should be converted to an in-memory representation without error.
    /// sample_id indicates the sample batch that the result should be equivalent to.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::CloudNative.CloudEvents.V1.CloudEventBatch ValidBatch {
      get { return eventCase_ == EventOneofCase.ValidBatch ? (global::CloudNative.CloudEvents.V1.CloudEventBatch) event_ : null; }
      set {
        event_ = value;
        eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.ValidBatch;
      }
    }

    /// <summary>Field number for the "invalid_single" field.</summary>
    public const int InvalidSingleFieldNumber = 6;
    /// <summary>
    /// A single event that should be rejected when converted to an in-memory representation.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::CloudNative.CloudEvents.V1.CloudEvent InvalidSingle {
      get { return eventCase_ == EventOneofCase.InvalidSingle ? (global::CloudNative.CloudEvents.V1.CloudEvent) event_ : null; }
      set {
        event_ = value;
        eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.InvalidSingle;
      }
    }

    /// <summary>Field number for the "invalid_batch" field.</summary>
    public const int InvalidBatchFieldNumber = 7;
    /// <summary>
    /// A batch of events that should be rejected when converted to an in-memory representation.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public global::CloudNative.CloudEvents.V1.CloudEventBatch InvalidBatch {
      get { return eventCase_ == EventOneofCase.InvalidBatch ? (global::CloudNative.CloudEvents.V1.CloudEventBatch) event_ : null; }
      set {
        event_ = value;
        eventCase_ = value == null ? EventOneofCase.None : EventOneofCase.InvalidBatch;
      }
    }

    private object event_;
    /// <summary>Enum of possible cases for the "event" oneof.</summary>
    public enum EventOneofCase {
      None = 0,
      ValidSingle = 4,
      ValidBatch = 5,
      InvalidSingle = 6,
      InvalidBatch = 7,
    }
    private EventOneofCase eventCase_ = EventOneofCase.None;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public EventOneofCase EventCase {
      get { return eventCase_; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void ClearEvent() {
      eventCase_ = EventOneofCase.None;
      event_ = null;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ConformanceTest);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ConformanceTest other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Id != other.Id) return false;
      if (Description != other.Description) return false;
      if (SampleId != other.SampleId) return false;
      if (!object.Equals(ValidSingle, other.ValidSingle)) return false;
      if (!object.Equals(ValidBatch, other.ValidBatch)) return false;
      if (!object.Equals(InvalidSingle, other.InvalidSingle)) return false;
      if (!object.Equals(InvalidBatch, other.InvalidBatch)) return false;
      if (EventCase != other.EventCase) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Id.Length != 0) hash ^= Id.GetHashCode();
      if (Description.Length != 0) hash ^= Description.GetHashCode();
      if (SampleId.Length != 0) hash ^= SampleId.GetHashCode();
      if (eventCase_ == EventOneofCase.ValidSingle) hash ^= ValidSingle.GetHashCode();
      if (eventCase_ == EventOneofCase.ValidBatch) hash ^= ValidBatch.GetHashCode();
      if (eventCase_ == EventOneofCase.InvalidSingle) hash ^= InvalidSingle.GetHashCode();
      if (eventCase_ == EventOneofCase.InvalidBatch) hash ^= InvalidBatch.GetHashCode();
      hash ^= (int) eventCase_;
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (Description.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Description);
      }
      if (SampleId.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(SampleId);
      }
      if (eventCase_ == EventOneofCase.ValidSingle) {
        output.WriteRawTag(34);
        output.WriteMessage(ValidSingle);
      }
      if (eventCase_ == EventOneofCase.ValidBatch) {
        output.WriteRawTag(42);
        output.WriteMessage(ValidBatch);
      }
      if (eventCase_ == EventOneofCase.InvalidSingle) {
        output.WriteRawTag(50);
        output.WriteMessage(InvalidSingle);
      }
      if (eventCase_ == EventOneofCase.InvalidBatch) {
        output.WriteRawTag(58);
        output.WriteMessage(InvalidBatch);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Id.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Id);
      }
      if (Description.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Description);
      }
      if (SampleId.Length != 0) {
        output.WriteRawTag(26);
        output.WriteString(SampleId);
      }
      if (eventCase_ == EventOneofCase.ValidSingle) {
        output.WriteRawTag(34);
        output.WriteMessage(ValidSingle);
      }
      if (eventCase_ == EventOneofCase.ValidBatch) {
        output.WriteRawTag(42);
        output.WriteMessage(ValidBatch);
      }
      if (eventCase_ == EventOneofCase.InvalidSingle) {
        output.WriteRawTag(50);
        output.WriteMessage(InvalidSingle);
      }
      if (eventCase_ == EventOneofCase.InvalidBatch) {
        output.WriteRawTag(58);
        output.WriteMessage(InvalidBatch);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (Id.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Id);
      }
      if (Description.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Description);
      }
      if (SampleId.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(SampleId);
      }
      if (eventCase_ == EventOneofCase.ValidSingle) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ValidSingle);
      }
      if (eventCase_ == EventOneofCase.ValidBatch) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(ValidBatch);
      }
      if (eventCase_ == EventOneofCase.InvalidSingle) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(InvalidSingle);
      }
      if (eventCase_ == EventOneofCase.InvalidBatch) {
        size += 1 + pb::CodedOutputStream.ComputeMessageSize(InvalidBatch);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ConformanceTest other) {
      if (other == null) {
        return;
      }
      if (other.Id.Length != 0) {
        Id = other.Id;
      }
      if (other.Description.Length != 0) {
        Description = other.Description;
      }
      if (other.SampleId.Length != 0) {
        SampleId = other.SampleId;
      }
      switch (other.EventCase) {
        case EventOneofCase.ValidSingle:
          if (ValidSingle == null) {
            ValidSingle = new global::CloudNative.CloudEvents.V1.CloudEvent();
          }
          ValidSingle.MergeFrom(other.ValidSingle);
          break;
        case EventOneofCase.ValidBatch:
          if (ValidBatch == null) {
            ValidBatch = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
          }
          ValidBatch.MergeFrom(other.ValidBatch);
          break;
        case EventOneofCase.InvalidSingle:
          if (InvalidSingle == null) {
            InvalidSingle = new global::CloudNative.CloudEvents.V1.CloudEvent();
          }
          InvalidSingle.MergeFrom(other.InvalidSingle);
          break;
        case EventOneofCase.InvalidBatch:
          if (InvalidBatch == null) {
            InvalidBatch = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
          }
          InvalidBatch.MergeFrom(other.InvalidBatch);
          break;
      }

      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Id = input.ReadString();
            break;
          }
          case 18: {
            Description = input.ReadString();
            break;
          }
          case 26: {
            SampleId = input.ReadString();
            break;
          }
          case 34: {
            global::CloudNative.CloudEvents.V1.CloudEvent subBuilder = new global::CloudNative.CloudEvents.V1.CloudEvent();
            if (eventCase_ == EventOneofCase.ValidSingle) {
              subBuilder.MergeFrom(ValidSingle);
            }
            input.ReadMessage(subBuilder);
            ValidSingle = subBuilder;
            break;
          }
          case 42: {
            global::CloudNative.CloudEvents.V1.CloudEventBatch subBuilder = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
            if (eventCase_ == EventOneofCase.ValidBatch) {
              subBuilder.MergeFrom(ValidBatch);
            }
            input.ReadMessage(subBuilder);
            ValidBatch = subBuilder;
            break;
          }
          case 50: {
            global::CloudNative.CloudEvents.V1.CloudEvent subBuilder = new global::CloudNative.CloudEvents.V1.CloudEvent();
            if (eventCase_ == EventOneofCase.InvalidSingle) {
              subBuilder.MergeFrom(InvalidSingle);
            }
            input.ReadMessage(subBuilder);
            InvalidSingle = subBuilder;
            break;
          }
          case 58: {
            global::CloudNative.CloudEvents.V1.CloudEventBatch subBuilder = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
            if (eventCase_ == EventOneofCase.InvalidBatch) {
              subBuilder.MergeFrom(InvalidBatch);
            }
            input.ReadMessage(subBuilder);
            InvalidBatch = subBuilder;
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            Id = input.ReadString();
            break;
          }
          case 18: {
            Description = input.ReadString();
            break;
          }
          case 26: {
            SampleId = input.ReadString();
            break;
          }
          case 34: {
            global::CloudNative.CloudEvents.V1.CloudEvent subBuilder = new global::CloudNative.CloudEvents.V1.CloudEvent();
            if (eventCase_ == EventOneofCase.ValidSingle) {
              subBuilder.MergeFrom(ValidSingle);
            }
            input.ReadMessage(subBuilder);
            ValidSingle = subBuilder;
            break;
          }
          case 42: {
            global::CloudNative.CloudEvents.V1.CloudEventBatch subBuilder = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
            if (eventCase_ == EventOneofCase.ValidBatch) {
              subBuilder.MergeFrom(ValidBatch);
            }
            input.ReadMessage(subBuilder);
            ValidBatch = subBuilder;
            break;
          }
          case 50: {
            global::CloudNative.CloudEvents.V1.CloudEvent subBuilder = new global::CloudNative.CloudEvents.V1.CloudEvent();
            if (eventCase_ == EventOneofCase.InvalidSingle) {
              subBuilder.MergeFrom(InvalidSingle);
            }
            input.ReadMessage(subBuilder);
            InvalidSingle = subBuilder;
            break;
          }
          case 58: {
            global::CloudNative.CloudEvents.V1.CloudEventBatch subBuilder = new global::CloudNative.CloudEvents.V1.CloudEventBatch();
            if (eventCase_ == EventOneofCase.InvalidBatch) {
              subBuilder.MergeFrom(InvalidBatch);
            }
            input.ReadMessage(subBuilder);
            InvalidBatch = subBuilder;
            break;
          }
        }
      }
    }
    #endif

  }

  /// <summary>
  /// A sample message for tests using CloudEvent.proto_data.
  /// </summary>
  public sealed partial class ConformanceTestMessageData : pb::IMessage<ConformanceTestMessageData>
  #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      , pb::IBufferMessage
  #endif
  {
    private static readonly pb::MessageParser<ConformanceTestMessageData> _parser = new pb::MessageParser<ConformanceTestMessageData>(() => new ConformanceTestMessageData());
    private pb::UnknownFieldSet _unknownFields;
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pb::MessageParser<ConformanceTestMessageData> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::CloudNative.CloudEvents.Protobuf.UnitTests.ConformanceTestsReflection.Descriptor.MessageTypes[2]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestMessageData() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestMessageData(ConformanceTestMessageData other) : this() {
      text_ = other.text_;
      _unknownFields = pb::UnknownFieldSet.Clone(other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public ConformanceTestMessageData Clone() {
      return new ConformanceTestMessageData(this);
    }

    /// <summary>Field number for the "text" field.</summary>
    public const int TextFieldNumber = 1;
    private string text_ = "";
    /// <summary>
    /// Just some text data.
    /// </summary>
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public string Text {
      get { return text_; }
      set {
        text_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override bool Equals(object other) {
      return Equals(other as ConformanceTestMessageData);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public bool Equals(ConformanceTestMessageData other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (Text != other.Text) return false;
      return Equals(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override int GetHashCode() {
      int hash = 1;
      if (Text.Length != 0) hash ^= Text.GetHashCode();
      if (_unknownFields != null) {
        hash ^= _unknownFields.GetHashCode();
      }
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void WriteTo(pb::CodedOutputStream output) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      output.WriteRawMessage(this);
    #else
      if (Text.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Text);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(output);
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalWriteTo(ref pb::WriteContext output) {
      if (Text.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(Text);
      }
      if (_unknownFields != null) {
        _unknownFields.WriteTo(ref output);
      }
    }
    #endif

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public int CalculateSize() {
      int size = 0;
      if (Text.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Text);
      }
      if (_unknownFields != null) {
        size += _unknownFields.CalculateSize();
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(ConformanceTestMessageData other) {
      if (other == null) {
        return;
      }
      if (other.Text.Length != 0) {
        Text = other.Text;
      }
      _unknownFields = pb::UnknownFieldSet.MergeFrom(_unknownFields, other._unknownFields);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    public void MergeFrom(pb::CodedInputStream input) {
    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
      input.ReadRawMessage(this);
    #else
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, input);
            break;
          case 10: {
            Text = input.ReadString();
            break;
          }
        }
      }
    #endif
    }

    #if !GOOGLE_PROTOBUF_REFSTRUCT_COMPATIBILITY_MODE
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    [global::System.CodeDom.Compiler.GeneratedCode("protoc", null)]
    void pb::IBufferMessage.InternalMergeFrom(ref pb::ParseContext input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            _unknownFields = pb::UnknownFieldSet.MergeFieldFrom(_unknownFields, ref input);
            break;
          case 10: {
            Text = input.ReadString();
            break;
          }
        }
      }
    }
    #endif

  }

  #endregion

}

#endregion Designer generated code

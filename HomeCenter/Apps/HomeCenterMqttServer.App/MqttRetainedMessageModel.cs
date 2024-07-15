﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace HomeCenter.Mqtt.Server;

internal partial class Program
{
    sealed class MqttRetainedMessageModel
    {
        public string? ContentType { get; set; }
        public byte[]? CorrelationData { get; set; }
        public byte[]? Payload { get; set; }
        public MqttPayloadFormatIndicator PayloadFormatIndicator { get; set; }
        public MqttQualityOfServiceLevel QualityOfServiceLevel { get; set; }
        public string? ResponseTopic { get; set; }
        public string? Topic { get; set; }
        public List<MqttUserProperty>? UserProperties { get; set; }

        public static MqttRetainedMessageModel Create(MqttApplicationMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return new MqttRetainedMessageModel
            {
                Topic = message.Topic,

                // Create a copy of the buffer from the payload segment because 
                // it cannot be serialized and deserialized with the JSON serializer.
                Payload = message.PayloadSegment.ToArray(),
                UserProperties = message.UserProperties,
                ResponseTopic = message.ResponseTopic,
                CorrelationData = message.CorrelationData,
                ContentType = message.ContentType,
                PayloadFormatIndicator = message.PayloadFormatIndicator,
                QualityOfServiceLevel = message.QualityOfServiceLevel

                // Other properties like "Retain" are not if interest in the storage.
                // That's why a custom model makes sense.
            };
        }

        public MqttApplicationMessage ToApplicationMessage()
        {
            return new MqttApplicationMessage
            {
                Topic = Topic,
                PayloadSegment = new ArraySegment<byte>(Payload ?? Array.Empty<byte>()),
                PayloadFormatIndicator = PayloadFormatIndicator,
                ResponseTopic = ResponseTopic,
                CorrelationData = CorrelationData,
                ContentType = ContentType,
                UserProperties = UserProperties,
                QualityOfServiceLevel = QualityOfServiceLevel,
                Dup = false,
                Retain = true
            };
        }
    }
}
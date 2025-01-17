using System;
using System.Text.Json;
using Xunit;
using EventPi.Abstractions;

namespace EventPi.Tests;

public class FrameIdSerializationTests
{
    private static readonly FrameId TestFrameId = FrameId.Parse("frame://device1:1/2024-01-16T12:00:00Z/123");



    [Fact]
    public void SerializeAndDeserialize_ShouldMaintainEquality2()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Act
        var sut=FrameId.Parse(TestFrameId.ToString());
        // Assert
        Assert.Equal(TestFrameId, sut);
    }

    [Fact]
    public void SerializeAndDeserialize_ShouldMaintainEquality()
    {
        // Arrange
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        // Act
        string jsonString = JsonSerializer.Serialize(TestFrameId, options);
        var deserializedFrameId = JsonSerializer.Deserialize<FrameId>(jsonString);

        // Assert
        Assert.Equal(TestFrameId, deserializedFrameId);
    }



  
    [Fact]
    public void SerializeMultipleFrameIds_ShouldMaintainDistinctValues()
    {
        // Arrange
        var frameId1 = FrameId.Parse("frame://device1:1/2024-01-16T12:00:00Z/123");
        var frameId2 = FrameId.Parse("frame://device2:2/2024-01-16T12:00:00Z/456");
        var frameIds = new[] { frameId1, frameId2 };

        // Act
        string jsonString = JsonSerializer.Serialize(frameIds);
        var deserializedFrameIds = JsonSerializer.Deserialize<FrameId[]>(jsonString);

        // Assert
        Assert.Equal(2, deserializedFrameIds.Length);
        Assert.Equal(frameId1, deserializedFrameIds[0]);
        Assert.Equal(frameId2, deserializedFrameIds[1]);
    }
}
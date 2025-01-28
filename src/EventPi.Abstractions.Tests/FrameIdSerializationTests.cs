using System;
using System.Text.Json;
using Xunit;
using EventPi.Abstractions;
using Xunit.Abstractions;

namespace EventPi.Tests;

public class FrameIdSerializationTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public FrameIdSerializationTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private static readonly FrameId TestFrameId =
        new FrameId(new VideoRecordingIdentifier(HostName.Localhost, 1, DateTimeOffset.Now),69);



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
        _testOutputHelper.WriteLine(TestFrameId);
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



  
   
}
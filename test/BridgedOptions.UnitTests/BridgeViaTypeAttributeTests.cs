using System;
using Shouldly;
using Xunit;

namespace BridgedOptions.UnitTests
{
    public class BridgeViaTypeAttributeTests
    {
        [Fact]
        public void NullBridgeType_Constructor_ThrowsArgumentNullException()
        {
            var exception = Should.Throw<ArgumentNullException>(() => new BridgeViaTypeAttribute(null));
            
            exception.Message.ShouldBe("Value cannot be null. (Parameter 'bridgeType')");
        }
        
        [Fact]
        public void IncorrectBridgeType_Constructor_ThrowsArgumentException()
        {
            var exception = Should.Throw<ArgumentException>(() => new BridgeViaTypeAttribute(typeof(object)));
            
            exception.Message.ShouldBe(
                "The bridge type does not implement the IBridgeOptions interface. (Parameter 'bridgeType')");
        }
    }
}
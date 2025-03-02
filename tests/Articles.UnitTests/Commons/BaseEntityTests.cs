using Articles.Domain.Commons;
using FluentAssertions;

namespace Articles.UnitTests.Commons;

public class BaseEntityTests
{
    private record TestId : EntityId
    {
        private TestId(Guid value) : base(value) { }
        
        public static TestId Create(Guid value) => new(value);
    }
    
    private record AnotherTestId : EntityId
    {
        private AnotherTestId(Guid value) : base(value) { }
        
        public static AnotherTestId Create(Guid value) => new(value);
    }
    
    private class TestEntity : BaseEntity<TestId>
    {
        public TestEntity(TestId id) : base(id) { }
    }

    private class AnotherTestEntity : BaseEntity<AnotherTestId>
    {
        public AnotherTestEntity(AnotherTestId id) : base(id) { }
    }

    [Fact]
    public void Equals_SameTypeAndId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(TestId.Create(id));
        var entity2 = new TestEntity(TestId.Create(id));

        entity1.Equals(entity2).Should().BeTrue();
    }

    [Fact]
    public void Equals_DifferentTypesSameId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(TestId.Create(id));
        var entity2 = new AnotherTestEntity(AnotherTestId.Create(id));

        entity1.GetType().Should().NotBe(entity2.GetType());
    }

    [Fact]
    public void Equals_SameTypeDifferentId_ReturnsFalse()
    {
        var entity1 = new TestEntity(TestId.Create(Guid.NewGuid()));
        var entity2 = new TestEntity(TestId.Create(Guid.NewGuid()));

        entity1.Equals(entity2).Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_SameEntities_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(TestId.Create(id));
        var entity2 = new TestEntity(TestId.Create(id));

        (entity1 == entity2).Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_DifferentEntities_ReturnsTrue()
    {
        var entity1 = new TestEntity(TestId.Create(Guid.NewGuid()));
        var entity2 = new TestEntity(TestId.Create(Guid.NewGuid()));

        (entity1 != entity2).Should().BeTrue();
    }
}
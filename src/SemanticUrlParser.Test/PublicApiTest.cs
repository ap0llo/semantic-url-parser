using System;
using Grynwald.SemanticUrlParser.GitHub;
using NetArchTest.Rules;
using Xunit;

namespace Grynwald.SemanticUrlParser.Test
{
    public class PublicApiTest
    {
        [Fact]
        public void Public_classes_are_sealed()
        {
            var result = Types.InAssembly(typeof(GitHubUrlParser<>).Assembly)
                .That().AreClasses().And().ArePublic().And().AreNotAbstract()
                .Should().BeSealed()
                .GetResult();

            Assert.True(result.IsSuccessful, $"Types  {String.Join(", ", result.FailingTypeNames ?? Array.Empty<string>())} should be public.");
        }
    }
}

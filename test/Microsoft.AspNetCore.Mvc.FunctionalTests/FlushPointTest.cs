// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class FlushPointTest : IClassFixture<MvcTestFixture<RazorWebSite.Startup>>
    {
        public FlushPointTest(MvcTestFixture<RazorWebSite.Startup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Fact]
        public async Task FlushPointsAreExecutedForPagesWithLayouts()
        {
            var expected = @"<title>Page With Layout</title>

RenderBody content


    <span>Content that takes time to produce</span>

";

            // Act
            var body = await Client.GetStringAsync("http://localhost/FlushPoint/PageWithLayout");

            // Assert
            Assert.Equal(expected, body, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public async Task FlushPointsAreExecutedForPagesWithoutLayouts()
        {
            var expected = @"Initial content

Secondary content

Inside partial

After flush inside partial<form action=""/FlushPoint/PageWithoutLayout"" method=""post"">" +
                @"<input id=""Name1"" name=""Name1"" type=""text"" value="""" />" +
                @"<input id=""Name2"" name=""Name2"" type=""text"" value="""" /></form>";

            // Act
            var body = await Client.GetStringAsync("http://localhost/FlushPoint/PageWithoutLayout");

            // Assert
            Assert.Equal(expected, body, ignoreLineEndingDifferences: true);
        }

        [Theory]
        [InlineData("PageWithPartialsAndViewComponents", "FlushAsync invoked inside RenderSection")]
        [InlineData("PageWithRenderSectionAsync", "FlushAsync invoked inside RenderSectionAsync")]
        public async Task FlushPointsAreExecutedForPagesWithComponentsPartialsAndSections(string action, string title)
        {
            var expected = $@"<title>{ title }</title>
RenderBody content


partial-content

Value from TaskReturningString
<p>section-content</p>
    component-content
    <span>Content that takes time to produce</span>

More content from layout
";

            // Act
            var body = await Client.GetStringAsync("http://localhost/FlushPoint/" + action);

            // Assert
            Assert.Equal(expected, body, ignoreLineEndingDifferences: true);
        }

        [Fact]
        public async Task FlushPointsNestedLayout()
        {
            // Arrange
            var expected = @"Inside Nested Layout
<title>Nested Page With Layout</title>



    <span>Nested content that takes time to produce</span>
";

            // Act
            var body = await Client.GetStringAsync("http://localhost/FlushPoint/PageWithNestedLayout");

            // Assert
            Assert.Equal(expected, body, ignoreLineEndingDifferences: true);
        }
    }
}
